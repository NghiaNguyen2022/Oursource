-- Call "usp_InvoicesToPayment" ('20240101', '20240415', 'C', '1')
ALTER PROCEDURE "usp_Bank_APInvoicesToPayment"
(
	IN v_FromDate DATE,
	IN v_ToDate DATE,
	IN v_Type CHAR(2),
	IN v_Account NVARCHAR(50) DEFAULT 'All',
	IN v_FeeType NVARCHAR(50) DEFAULT '', 	
	IN v_Key VARCHAR(100)  DEFAULT ''
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	IF :v_Type = 'PC'
	THEN
		BEGIN 			
			SELECT T."CardCode",
				   T."CardName",
				   T."Check",
				   --t."HouseBank",
				   T."DocEntry" AS "DocNum",
				   T."DocDate", 
				   T."DueDate",
				   T."DocCur",
				   T."DflSwift" AS "ReceiveBankCode",
				   t."DflAccount" AS "ReceiveAccount",
				   t."BankName" AS "ReceiveBankName",
				   T."AcctName" AS "ReceiveAccountName",
				   T."InsTotal" AS "InsTotal",		  
				   T."InsTotalFC",	      
				   CAST(CASE WHEN T."MustPay" > 0 THEN T."MustPay"
				   	   	ELSE 0 
				   	END AS DECIMAL) AS "MustPay",  
				   T."DocEntry",
				   t."JrnlMemo",
				   t."Content",
				   '02' AS "SAPStatus",
				   '' AS "BankStatus",
				   '' AS "Message",
				   'Data' AS "Manual"
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
							'Thanh toan' || t2."CardName" ||T1."DueDate" AS "Content"
						FROM OPCH T0
						LEFT JOIN PCH6 T1 ON T0."DocEntry" = T1."DocEntry"
						LEFT JOIN OCRD T2 ON T0."CardCode" = T2."CardCode"	
						LEFT JOIN ODSC T3 ON T2."BankCode" = T3."BankCode"
						LEFT JOIN OCRB T4 ON T4."CardCode" = t2."CardCode"
										 AND T4."BankCode" = T2."BankCode"
										 AND T4."Account" = T2."DflAccount"
						WHERE T1."DueDate" BETWEEN :v_FromDate AND :v_ToDate
						  AND T1."InsTotal" - T1."PaidToDate" > 0		
						  AND (:v_Account = 'All' OR t2."HousBnkAct" = :v_Account)
						ORDER BY T0."CardCode"
					) T
				 ;			
			END;
		END IF;
END;
