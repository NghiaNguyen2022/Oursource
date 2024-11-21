ALTER PROCEDURE "usp_PaymentListReportDetail"
(
	IN v_PaymentNumber INTEGER,
	IN v_DocType CHAR(2),
	IN v_BrId NVARCHAR(10)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	IF :v_DocType = 'PT'
	THEN
		SELECT T."CardCode",
			    T."CardName",
			    T."Check",
			    T."DocNum",
			    T."InvCode",
			    T."DocDate", 
			    T."DueDate",
			    T."DocCur",
			    T."InsTotal" AS "InsTotal",
			    CAST(CAST(
				    CASE WHEN T."MustPay" > 0 THEN T."MustPay"
				   	 	 ELSE 0 
				   	 END 
				 AS DECIMAL(19,2)) AS NVARCHAR(20)) AS "MustPay",  
	            CASE WHEN COALESCE(T."DocCur", 'VND') = 'VND' THEN T."CashSum"
			   	 	 ELSE T."CashSumFC" 
			   	 END AS "MustPayAsCash",
	            CASE WHEN COALESCE(T."DocCur", 'VND') = 'VND' THEN T."TrsfrSum"
			   	 	 ELSE "TrsfrSumFC" 
			   	 END AS "MustPayAsBank",
			    T."DocEntry",
			    'Data' AS "Manual"
		  FROM (SELECT T4."CardCode",
					   T2."CardName",
					   'N' AS "Check",
					   T0."DocNum",
					   T0."U_InvCode" 
						AS "InvCode",
					   T0."DocDate", 
					   T1."DueDate",
					   CASE WHEN  T4."DocCurr" = 'VND' THEN T1."InsTotal"
					   		ELSE T1."InsTotalFC"
					   	END AS "InsTotal",
					   CASE WHEN T4."DocCurr" = 'VND' THEN T1."InsTotal" - T1."PaidToDate"
					   		ELSE T1."InsTotalFC" - T1."PaidFC" 
					   	END AS "MustPay",
					   T0."DocEntry",
					   T4."DocCurr" AS "DocCur",
					   T3."SumApplied" AS "CashSum",
					   T3."AppliedFC" AS "CashSumFC" ,
					   T3."SumApplied" AS "TrsfrSum",
					   T3."AppliedFC" AS"TrsfrSumFC"
				  FROM ORCT T4
				  LEFT JOIN RCT2 T3 ON T3."DocNum" = T4."DocEntry"
				   				   AND T3."InvType" = 13
				  LEFT JOIN OINV T0 ON T3."DocEntry" = T0."DocEntry"
 				  LEFT JOIN INV6 T1 ON T0."DocEntry" = T1."DocEntry"
				  LEFT JOIN OCRD T2 ON T4."CardCode" = T2."CardCode"
				 WHERE T4."DocNum" = :v_PaymentNumber 
				   AND T4."BPLId" = :v_BrId
		   		   AND T4."Canceled" = 'N'
				 UNION ALL
				SELECT T4."CardCode",
					   T2."CardName",
					   'N' AS "Check",
					   T0."DocNum",
					   T0."U_InvCode" 
						AS "InvCode",
					   T0."DocDate", 
					   T1."DueDate",
					   CASE WHEN  T4."DocCurr" = 'VND' THEN T1."InsTotal"
					   		ELSE T1."InsTotalFC"
					   	END AS "InsTotal",
					   CASE WHEN T4."DocCurr" = 'VND' THEN T1."InsTotal" - T1."PaidToDate"
					   		ELSE T1."InsTotalFC" - T1."PaidFC" 
					   	END AS "MustPay",
					   T0."DocEntry", 
					   T4."DocCurr" AS "DocCur",
					   T3."SumApplied" AS "CashSum",
					   T3."AppliedFC" AS "CashSumFC" ,
					   T3."SumApplied" AS "TrsfrSum",
					   T3."AppliedFC" AS"TrsfrSumFC"
				  FROM OPDF T4
				  LEFT JOIN PDF2 T3 ON T3."DocNum" = T4."DocEntry"
				   			   	   AND T3."InvType" = 13
				  LEFT JOIN OINV T0 ON T3."DocEntry" = T0."DocEntry"
 				  LEFT JOIN INV6 T1 ON T0."DocEntry" = T1."DocEntry"
				  LEFT JOIN OCRD T2 ON T4."CardCode" = T2."CardCode"
				 WHERE T4."DocEntry" = :v_PaymentNumber 
				   AND T4."BPLId" = :v_BrId 
				   AND T4."U_PayType" = 'PT'
		   		   AND T4."Canceled" = 'N') T;
	ELSE 
		SELECT T."CardCode",
			    T."CardName",
			    T."Check",
			    T."DocNum",
			    T."InvCode",
			    T."DocDate", 
			    T."DueDate",
			    T."DocCur",
			    T."InsTotal" AS "InsTotal",
			    CAST(CAST(
				    CASE WHEN T."MustPay" > 0 THEN T."MustPay"
				   	 	 ELSE 0 
				   	 END 
				 AS DECIMAL(19,2)) AS NVARCHAR(20)) AS "MustPay",  
	            CASE WHEN COALESCE(T."DocCur", 'VND') = 'VND' THEN T."CashSum"
			   	 	 ELSE T."CashSumFC" 
			   	 END AS "MustPayAsCash",
	            CASE WHEN COALESCE(T."DocCur", 'VND') = 'VND' THEN T."TrsfrSum"
			   	 	 ELSE "TrsfrSumFC" 
			   	 END AS "MustPayAsBank",
			    T."DocEntry",
			    'Data' AS "Manual"
		  FROM (SELECT T4."CardCode",
					   T2."CardName",
					   'N' AS "Check",
					   T0."DocNum",
					   T0."U_InvCode" 
						AS "InvCode",
					   T0."DocDate", 
					   T1."DueDate",
					   CASE WHEN  T4."DocCurr" = 'VND' THEN T1."InsTotal"
					   		ELSE T1."InsTotalFC"
					   	END AS "InsTotal",
					   CASE WHEN T4."DocCurr" = 'VND' THEN T1."InsTotal" - T1."PaidToDate"
					   		ELSE T1."InsTotalFC" - T1."PaidFC" 
					   	END AS "MustPay",
					   T0."DocEntry", 
					   T4."DocCurr" AS "DocCur",
					   T3."SumApplied" AS "CashSum",
					   T3."AppliedFC" AS "CashSumFC" ,
					   T3."SumApplied" AS "TrsfrSum",
					   T3."AppliedFC" AS"TrsfrSumFC"
				  FROM OVPM T4
				  LEFT JOIN VPM2 T3 ON T3."DocNum" = T4."DocEntry"
				  				   AND T3."InvType" = 18
				  LEFT JOIN OPCH T0 ON T3."DocEntry" = T0."DocEntry"				  
				  LEFT JOIN PCH6 T1 ON T0."DocEntry" = T1."DocEntry"
				  LEFT JOIN OCRD T2 ON T4."CardCode" = T2."CardCode"
				 WHERE T4."DocNum" = :v_PaymentNumber 
				   AND T4."BPLId" = :v_BrId
		   		   AND T4."Canceled" = 'N'
				 UNION ALL
				SELECT T4."CardCode",
					   T2."CardName",
					   'N' AS "Check",
					   T0."DocNum",
					 T0."U_InvCode" 
						AS "InvCode",
					   T0."DocDate", 
					   T1."DueDate",
					   CASE WHEN  T4."DocCurr" = 'VND' THEN T1."InsTotal"
					   		ELSE T1."InsTotalFC"
					   	END AS "InsTotal",
					   CASE WHEN T4."DocCurr" = 'VND' THEN T1."InsTotal" - T1."PaidToDate"
					   		ELSE T1."InsTotalFC" - T1."PaidFC" 
					   	END AS "MustPay",
					   T0."DocEntry", 
					   T4."DocCurr" AS "DocCur",
					   T3."SumApplied" AS "CashSum",
					   T3."AppliedFC" AS "CashSumFC" ,
					   T3."SumApplied" AS "TrsfrSum",
					   T3."AppliedFC" AS"TrsfrSumFC"
				  FROM OPDF T4
				  LEFT JOIN PDF2 T3 ON T3."DocNum" = T4."DocEntry"
				   					AND T3."InvType" = 18
				  LEFT JOIN OPCH T0 ON T3."DocEntry" = T0."DocEntry"				  
				  LEFT JOIN PCH6 T1 ON T0."DocEntry" = T1."DocEntry"
				  LEFT JOIN OCRD T2 ON T4."CardCode" = T2."CardCode"
				 WHERE T4."DocEntry" = :v_PaymentNumber 
				   AND T4."BPLId" = :v_BrId
				   AND T4."U_PayType" = :v_DocType
		   		   AND T4."Canceled" = 'N') T;
	END IF;
END;
