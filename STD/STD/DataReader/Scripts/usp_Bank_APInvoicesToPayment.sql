-- Call "usp_InvoicesToPayment" ('20240101', '20240415', 'C', '1')
ALTER PROCEDURE "usp_Bank_APInvoicesToPayment1"
(
	IN v_FromDate DATE,
	IN v_ToDate DATE,
	IN v_Type CHAR(2),
	IN v_Account NVARCHAR(50) DEFAULT 'All',
	IN v_FeeType NVARCHAR(50) DEFAULT '', 	
	IN v_RequestID VARCHAR(100) DEFAULT '',
	IN v_UserID VARCHAR(4) DEFAULT '1'
)
LANGUAGE SQLSCRIPT
AS
BEGIN	
	DECLARE _BankCode NVARCHAR(50);
	DECLARE _BankName NVARCHAR(100);
	DECLARE _Account NVARCHAR(50);
	
	SELECT "SenderBankCode", "Name", "Account"
	INTO _BankCode, _BankName, _Account
	  FROM "vw_Bank_BankAccount" 
	 WHERE "Account" = :v_Account
	   AND "Key" = :v_Type ; 
	
	  			
	SELECT T."CardCode",
		   T."CardName",
		   T."Check",
		   T."DocEntry" AS "DocNum",
		   T."SAPStatus",
		   T."BankStatus",
		   T."Message",
		   T."DocDate", 
		   T."DueDate",
		   T."DocCur",
		   T."DflSwift" AS "ReceiveBankCode",
		   t."DflAccount" AS "ReceiveAccount",
		   t."BankName" AS "ReceiveBankName",
		   T."AcctName" AS "ReceiveAccountName",
		   :_BankCode AS "SenderBankCode",
		   :_Account AS "SenderAccount",
		   :_BankName AS "SenderAccountName",
		   T."InsTotal" AS "InsTotal",		  
		   T."InsTotalFC",	      
		   CAST(CASE WHEN T."MustPay" > 0 THEN T."MustPay"
		   	   	ELSE 0 
		   	END AS DECIMAL (19,2)) AS "MustPay",  
		   T."DocEntry",
		   t."JrnlMemo",
		   t."Content",
		   T."Manual",
		   t."requestId",
		   t."transId",
		   T."UserRequest",
		   T1."AuthorName",
		   T1."Approval1",
		   T1."Approval1Name",
		   T1."Approval2",
		   T1."Approval2Name",
		   COALESCE(T."ApprStatus", 'N') "ApprStatus",
		   CASE WHEN T."ApprStatus" = 'R' THEN 'Gửi yêu cầu'
				WHEN T."ApprStatus" = 'A1' THEN 'Cấp 1 đã duyệt'			
				WHEN T."ApprStatus" = 'A2' THEN 'Cấp 2 đã duyệt'
				ELSE 'Chưa tạo TT'
			END AS "ApprNameStatus"
	  FROM (SELECT T0."CardCode",
					T2."CardName",
					'N' AS "Check",
					T0."DocNum",
					T0."DocDate", 
					T1."DueDate",
					T1."InsTotal",
					T1."InsTotalFC",
					CASE WHEN T0."DocCur" = 'VND' THEN T1."InsTotal" - T1."PaidToDate" 
						ELSE T1."InsTotalFC" - T1."PaidFC" 
					END AS "MustPay",
					T0."DocCur",
					T0."DocEntry",
					t0."JrnlMemo",
					T2."BankCode", 
					T2."DflSwift", 
					T3."BankName",
					T2."DflAccount",
					T4."AcctName",
					t2."HouseBank",
					'Thanh toan ' || t2."CardName" ||T1."DueDate" AS "Content", 
		   			CASE WHEN t5."bank" = 'VT' AND COALESCE(T5."status", 'N') = '1' THEN '02'
		   				 WHEN t5."bank" = 'BI' AND COALESCE(T5."status", 'N') = '11' THEN '02'
		   				 ELSE '01'
					 END AS "SAPStatus",
					COALESCE(T5."bankStatus", '') AS "BankStatus",
					COALESCE(T5."Message", '') AS "Message",
					'Data' AS "Manual", 
					COALESCE(t5."requestId", '') AS "requestId",
					COALESCE(t5."transId", '')AS "transId",
				    COALESCE(t5."UserRequest", '-1') "UserRequest",
					COALESCE(t5."ApprStatus", 'N') "ApprStatus"
			   FROM OPCH T0
				LEFT JOIN PCH6 T1 ON T0."DocEntry" = T1."DocEntry"
				LEFT JOIN OCRD T2 ON T0."CardCode" = T2."CardCode"	
				LEFT JOIN ODSC T3 ON T2."BankCode" = T3."BankCode"
				LEFT JOIN OCRB T4 ON T4."CardCode" = t2."CardCode"
								 AND T4."BankCode" = T2."BankCode"
								 AND T4."Account" = T2."DflAccount"								
				LEFT JOIN (					  	
							SELECT T0.* 
							  FROM "tb_Bank_TransferRecord" T0
							  JOIN (SELECT "CardCode",
										   "DocEntry","bank",
										   MAX("transId") AS "transId"
									  FROM "tb_Bank_TransferRecord"
									 WHERE COALESCE("DocEntry", '') != ''
									   --AND "bank" = 'VT'
									 GROUP BY "CardCode",
										   "DocEntry", "bank") t1 ON T0."CardCode" = T1."CardCode"
										   				 AND T0."DocEntry" = T1."DocEntry"
										   				 AND T0."transId" = T1."transId"
										   				 AND T0."bank" = T1."bank"
						  ) T5 ON T2."CardCode" = T5."CardCode"
							  AND T0."DocEntry" = t5."DocEntry"
				WHERE T1."DueDate" BETWEEN :v_FromDate AND :v_ToDate
				  AND T1."InsTotal" - T1."PaidToDate" > 0		
				  AND t2."HousBnkAct" = :v_Account
				  AND T2."DflSwift" = :_BankCode
			UNION ALL
			SELECT T1."CardCode",
					T2."CardName",
					'N' AS "Check",
					-1 AS "DocNum",
					'' "DocDate", 
					'' "DueDate",
					0 "InsTotal",
					0 "InsTotalFC",
					t1."amount" AS "MustPay",
					T0."Currency" AS "DocCur",
					-1 "DocEntry",
					'' "JrnlMemo",
					T2."BankCode", 
					T2."DflSwift", 
					T3."BankName",
					T2."DflAccount",
					T4."AcctName",
					t2."HouseBank",
					'Thanh toan ' || t2."CardName" --||T1."DueDate"
						 AS "Content",
				    CASE WHEN t1."bank" = 'VT' AND COALESCE(T1."status", 'N') = '1' THEN '02'
		   				 WHEN t1."bank" = 'BI' AND COALESCE(T1."status", 'N') = '11' THEN '02'
		   				 ELSE '01'
					 END AS "SAPStatus",
					COALESCE(T1."bankStatus", '') AS "BankStatus",
					COALESCE(T1."Message", '') AS "Message",
					t0."SourceID" AS "Manual",
					COALESCE(t1."requestId", '') AS "requestId",
					COALESCE(t1."transId", '')AS "transId",
				    COALESCE(t1."UserRequest", '-1') "UserRequest",
					COALESCE(t1."ApprStatus", 'N') "ApprStatus"
			    FROM "tb_Bank_TransferRecord" T1 
				LEFT JOIN "tb_Bank_PaymentOnAccount" T0 ON T1."CardCode" = T0."CardCode"
													 AND T1."requestId" = t0."RequestID"
				LEFT JOIN OCRD T2 ON T1."CardCode" = T2."CardCode"	
				LEFT JOIN ODSC T3 ON T2."BankCode" = T3."BankCode"
				LEFT JOIN OCRB T4 ON T4."CardCode" = t2."CardCode"
								 AND T4."BankCode" = T2."BankCode"
								 AND T4."Account" = T2."DflAccount"
				WHERE  COALESCE(T1."DocEntry", '') = ''
				 AND  t2."HousBnkAct" = :v_Account 
				 AND T2."DflSwift" = :_BankCode
			) T
		LEFT JOIN (
					SELECT TOP 1 T0."U_Author" AS "Author",
						   T1.U_NAME "AuthorName",
						   COALESCE(T0."U_Appr1", '1') AS "Approval1",
						   T2.U_NAME "Approval1Name",
						   COALESCE(T0."U_Appr2", '-1') AS "Approval2",
						   COALESCE(T3.U_NAME, '' )  "Approval2Name"
					  FROM "@PAYMENT_AUTHEN" T0
					  JOIN OUSR T1 ON T1.USERID = T0."U_Author" 
					  JOIN OUSR T2 ON T2.USERID = COALESCE(T0."U_Appr1", '1')
					  LEFT JOIN OUSR T3 ON T3.USERID = T0."U_Appr2"
					 WHERE CAST(T0."U_Author" AS NVARCHAR(4)) = :v_UserID
					 ORDER BY T0."CreateDate" DESC, T0."CreateTime" DESC
					) T1  ON T."UserRequest" = T1."Author"
		 ;			

END;
