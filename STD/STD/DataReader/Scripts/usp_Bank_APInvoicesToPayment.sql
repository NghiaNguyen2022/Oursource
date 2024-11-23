-- Call "usp_InvoicesToPayment" ('20240101', '20240415', 'C', '1')
CREATE PROCEDURE "usp_Bank_APInvoicesToPayment"
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
	IF :v_Type = 'PT'
	THEN
		BEGIN 
			SELECT T."CardCode",
				    T."CardName",
				    T."Check",
				    T."DocEntry" AS "DocNum",
				    T."DocDate", 
				    T."DueDate",
				    t."JrnlMemo",
				    T."DocCur",
				    T."InsTotal" AS "InsTotal",	
					T."InsTotalFC",	    
				    CAST(T."MustPay" AS DECIMAL) AS "MustPay",  
				    T."DocEntry",
				    'Data' AS "Manual"
			   FROM (
					SELECT T."CardCode",
						    T."CardName",
						    T."Check",
						    T."DocNum",
						    T."DocDate", 
						    T."DueDate",
						    T."DocCur",
						    T."InsTotal" AS "InsTotal",		  
						    T."InsTotalFC",	      
						    CASE WHEN T."MustPay" > 0 THEN T."MustPay"
						   	 	 ELSE 0 
						   	 END AS "MustPay",  
						    T."DocEntry",
					        t."JrnlMemo",
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
							      t0."JrnlMemo"
							  FROM OPCH T0
							  LEFT JOIN PCH6 T1 ON T0."DocEntry" = T1."DocEntry"
							  LEFT JOIN OCRD T2 ON T0."CardCode" = T2."CardCode"	
							 WHERE T1."DueDate" BETWEEN :v_FromDate AND :v_ToDate
							   AND T1."InsTotal" - T1."PaidToDate" > 0		
							   AND (:v_Account = 'All' OR t2."HousBnkAct" = :v_Account)
						     ORDER BY T0."CardCode"
							) T
				 )T;
			
			END;
		END IF;
END;
