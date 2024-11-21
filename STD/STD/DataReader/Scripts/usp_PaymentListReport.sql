CREATE PROCEDURE "usp_PaymentListReport"
(
	IN v_FromDate DATE,
	IN v_ToDate DATE,
	IN v_DocType CHAR(2),
	IN v_BrId NVARCHAR(10),
	IN v_Status CHAR(1) DEFAULT '-',
	IN v_CardCode NVARCHAR(5000) DEFAULT ''
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	IF :v_DocType = 'PT'
	THEN
		SELECT * FROM 
		(
			SELECT COALESCE("U_PaymentKey", '') AS "PaymentKey",
				   'N' AS "Check",
			       'Phiếu thu' AS "PaymentType", 
			       t0."CardCode", 
			       T0."DocNum",
			       T0."DocEntry",
			       T0."DocDate",
			       T0."DocCurr",
		           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."CashSum"
				   	 	ELSE T0."CashSumFC" 
				   	END AS "Cash",
		           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."TrsfrSum"
				   	 	ELSE "TrsfrSumFC" 
				   	END AS "Bank", 
				   	T1."U_NAME" AS "CreateName",
				   	'Generated' AS "Status",
				   	'Đã tạo' AS "StatusName"
			  FROM ORCT T0
			  JOIN OUSR T1 ON T0."UserSign" = T1."USERID"
			 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
			   AND COALESCE("U_PaymentKey", '') <> '' 
			   AND "BPLId" = :v_BrId 
			   AND (:v_CardCode = '' OR T0."CardCode" IN (SELECT * FROM "SPLIT_STRING"(:v_CardCode)))
			   AND "Canceled" = 'N'
			 UNION ALL 
			SELECT COALESCE("U_PaymentKey", '') AS "PaymentKey",
				   'N' AS "Check",
			       'Phiếu thu'  AS "PaymentType",
			       t0."CardCode", 
			       T0."DocEntry" AS "DocNum",
			       T0."DocEntry",
			       T0."DocDate",
			       T0."DocCurr",
		           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."CashSum"
				   	 	ELSE T0."CashSumFC" 
				   	END AS "Cash",
		           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."TrsfrSum"
				   	 	ELSE "TrsfrSumFC" 
				   	END AS "Bank",
				   	T1."U_NAME" AS "CreateName" ,
				   	'Generated' AS "Status",
				   	'Đã tạo' AS "StatusName"
			  FROM OPDF T0
			  JOIN OUSR T1 ON T0."UserSign" = T1."USERID"
			 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
			   AND T0."U_PayType" = 'PT'
			   AND COALESCE("U_PaymentKey", '') <> ''
			   AND (:v_CardCode = '' OR T0."CardCode" IN (SELECT * FROM "SPLIT_STRING"(:v_CardCode)))
			   AND "BPLId" = :v_BrId
			   AND "Canceled" = 'N'
		   )T 
		   ORDER BY T."PaymentKey";
	ELSE 
		SELECT * 
		  FROM (
				SELECT COALESCE("U_PaymentKey", '') AS "PaymentKey",
					   'N' AS "Check",
				        CASE WHEN COALESCE(T0."U_PayType", 'PC') = 'PC' THEN 'Phiếu chi'
					   	 	ELSE 'Ủy nhiệm chi' 
					   	END AS "PaymentType",
				       t0."CardCode", 
				       T0."DocNum",		
				       T0."DocEntry" AS "PaymentEntry",
			       	   NULL AS "DocEntry",
				       T0."DocDate",   
				       T0."DocCurr",    
			           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."CashSum"
					   	 	ELSE T0."CashSumFC" 
					   	END AS "Cash",
					   T0."CashAcct",
			           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."TrsfrSum"
					   	 	ELSE "TrsfrSumFC" 
					   	END AS "Bank",
					   T0."U_Bank" AS "BankCode",
					   CASE WHEN COALESCE(T0."U_Bank", '') = '' THEN ''
					   	    ELSE T0."TrsfrAcct"
					   	END "TrsfrAcct",
					   T1."U_NAME" AS "CreateName" ,
					   'Generated' AS "Status",
					   'Đã tạo' AS "StatusName"
				  FROM OVPM T0
				  JOIN OUSR T1 ON T0."UserSign" = T1."USERID"
				 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
				   AND T0."U_PayType" = :v_DocType
				   AND COALESCE("U_PaymentKey", '') <> ''
				   AND "BPLId" = :v_BrId
			       AND (:v_CardCode = '' OR T0."CardCode" IN (SELECT * FROM "SPLIT_STRING"(:v_CardCode)))
				   AND "Canceled" = 'N'
				   AND (v_Status = '-'
				   		OR v_Status = 'G')
				 UNION ALL 
				SELECT DISTINCT COALESCE("U_PaymentKey", '') AS "PaymentKey",
					   'N' AS "Check",
				        CASE WHEN COALESCE(T0."U_PayType", 'PC') = 'PC' THEN 'Phiếu chi'
					   	 	ELSE 'Ủy nhiệm chi' 
					   	END AS "PaymentType",
				       t0."CardCode", 
				       T0."DocEntry" AS "DocNum",
				       NULL AS "PaymentEntry", -- DocEntry
				       T0."DocEntry"  AS "DocEntry", -- DrafEntry
			       	   --T0."DocEntry" AS "DrafEntry",
				       T0."DocDate",
				       T0."DocCurr",
			           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."CashSum"
					   	 	ELSE T0."CashSumFC" 
					   	END AS "Cash",
					   T0."CashAcct",
			           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."TrsfrSum"
					   	 	ELSE "TrsfrSumFC" 
					   	END AS "Bank",
					   T0."U_Bank" AS "BankCode",
					   CASE WHEN COALESCE(T0."U_Bank", '') = '' THEN ''
					   	    ELSE T0."TrsfrAcct"
					   	END "TrsfrAcct",
					   	T1."U_NAME" AS "CreateName" ,
					   	CASE WHEN "U_Status" = 'N' THEN 'Pending'
					   	     WHEN "U_Status" = 'R' THEN 'Reviewed'
					   		 WHEN "U_Status" = 'A'  THEN 'Approved'
					   		 WHEN "U_Status" = 'J' THEN 'Rejected'
					   		 ELSE 'Generated'
			             END AS "Status",
					   	CASE WHEN "U_Status" = 'N' THEN 'Đề xuất thanh toán' 
					   		 WHEN "U_Status" = 'R' THEN 'Đề nghị thanh toán'
					   		 WHEN "U_Status" = 'A' THEN 'Đã duyệt'
					   		 WHEN "U_Status" = 'J' THEN 'Từ chối'
					   		 ELSE 'Đã tạo'
			             END AS "StatusName"
				  FROM OPDF T0
				  LEFT JOIN PDF2 T3 ON T3."DocNum" = T0."DocEntry"
				   				   AND T3."InvType" = 18
				  LEFT JOIN OPCH T4 ON T3."DocEntry" = T4."DocEntry"	
				  JOIN OUSR T1 ON T0."UserSign" = T1."USERID"
				 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
				   AND T0."U_PayType" = :v_DocType
				   AND COALESCE("U_PaymentKey", '') <> ''
				   AND T0."BPLId" = :v_BrId
			       AND (:v_CardCode = '' OR T0."CardCode" IN (SELECT * FROM "SPLIT_STRING"(:v_CardCode)))
				   AND T0."Canceled" = 'N'					
				   AND COALESCE(T4."DocStatus", 'O') = 'O'
			       AND ((v_Status = '-'  AND "U_Status" <> 'S')
			   		    OR (v_Status = 'P' AND "U_Status" = 'N')
			   		    OR (v_Status = 'R' AND "U_Status" = 'J')
			   		    OR (v_Status = 'V' AND "U_Status" = 'R' )
			   		    OR (v_Status = 'A' AND "U_Status" = 'A')
			   		   )
	  		 ) T
	  ORDER BY "PaymentKey";
		   --AND v_Status IN ('P', '-', 'R', 'V', 'A'); -- pending, all, reject, viewed, approve"
	END IF;
END;
 
