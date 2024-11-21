-- Call "usp_InvoicesToPayment" ('20240101', '20240415', 'C', '1')
ALTER PROCEDURE "usp_InvoicesToPayment1"
(
	IN v_FromDate DATE,
	IN v_ToDate DATE,
	IN v_Type CHAR(1),
	IN v_BrId NVARCHAR(10),
	IN v_CardCode NVARCHAR(5000) DEFAULT '',
	IN v_IsAfter CHAR(1) DEFAULT 'N',	
	IN v_Key VARCHAR(100)  DEFAULT '',
	IN v_IsHistory CHAR(1) DEFAULT 'N'
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	IF :v_IsAfter = 'N'
	THEN
		IF :v_Type = 'T' -- thu 
		THEN
		BEGIN
			 SELECT T."CardCode",
				    T."CardName",
				    T."Check",
				    T."DocEntry" AS "DocNum",
				    T."InvCode",
				    T."HasRequest",
					CASE WHEN T."HasRequest"  = 'Y' THEN 'Đã tạo'
						 ELSE ''
					 END AS "Message",
				    T."DocDate", 
				    T."DueDate",
				    CURRENT_DATE AS "PostingDate",
				    t."JrnlMemo",
				    T."DocCur",
				    T."InsTotal" AS "InsTotal",	
					T."InsTotalFC",	
	  				T."DocRate",		    
				    CAST(T."MustPay" AS DECIMAL) AS "MustPay",  
				    CAST(T."MustPayAsCash" AS DECIMAL) AS "MustPayAsCash",   
				    CAST('' AS NVARCHAR(200)) AS "Account",
				    CAST(T."MustPayAsBank" AS DECIMAL) AS "MustPayAsBank",
				    CAST('' AS NVARCHAR(200)) AS "Bank",
				    T."DocEntry",
				    CAST('' AS NVARCHAR(200))  AS "CFlow",
				    'Data' AS "Manual"
			   FROM (
				 SELECT T."CardCode",
					    T."CardName",
					    T."Check",
					    T."DocNum",
					    T."InvCode",
					    T."HasRequest",
					    T."DocDate", 
					    T."DueDate",
					    T."DocCur",
					    T."InsTotal" AS "InsTotal",	
						T."InsTotalFC",
	  				    T."DocRate",		    
					    CASE WHEN T."MustPay" > 0 THEN T."MustPay"
					   	 	 ELSE 0 
					   	 END AS "MustPay",  
					    0 AS "MustPayAsCash",  
					    0 AS "MustPayAsBank",
					    T."DocEntry",
					    t."JrnlMemo",
					    'Data' AS "Manual"
				  FROM (SELECT T0."CardCode",
							   T2."CardName",
							   'N' AS "Check",
							   T0."DocNum",
							   T0."U_InvCode" 
							   AS "InvCode",
							   CASE WHEN COALESCE(T4."Status", '') <> 'S' THEN 'N'
							   	    ELSE 'Y'
							   	END AS "HasRequest",
							   T0."DocDate", 
							   T1."DueDate",
							   T1."InsTotal",
							   T1."InsTotalFC",
		  				       CASE WHEN T0."DocCur" = 'VND' THEN 1.0 
		  				       	    ELSE COALESCE(T3."Rate", 0.0)
		  				       END "DocRate",
		  				       -- T0."DocRate",
							   CASE WHEN T0."DocCur" = 'VND' THEN T1."InsTotal" - T1."PaidToDate" 
							   		ELSE T1."InsTotalFC" - T1."PaidFC" 
							   	END AS "MustPay",
							   T0."DocCur",
							   T0."DocEntry",
							   t0."JrnlMemo"
						  FROM OINV T0
						  LEFT JOIN INV6 T1 ON T0."DocEntry" = T1."DocEntry"
						  LEFT JOIN OCRD T2 ON T0."CardCode" = T2."CardCode"
						  LEFT JOIN ORTT T3 ON T3."Currency" = T0."DocCur"
						  				   AND T3."RateDate" = CURRENT_DATE	
						  LEFT JOIN "tbPaymentToolLogging" T4 ON T4."CardCode" = T0."CardCode" 
						  				   		 		     AND T4."DocEntry" = T0."DocEntry"
						  				   		 		     AND T4."PaymentType" = 'T'
						  				   		 		     AND T4."Status" = 'S'					  				   
						 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
						   AND T0."BPLId" = :v_BrId
						   AND T1."InsTotal" - T1."PaidToDate" > 0
						   --AND COALESCE(T4."CardCode", '') = ''
						   AND (:v_CardCode = '' OR T0."CardCode" IN (SELECT * FROM "SPLIT_STRING"(:v_CardCode)))
						 ORDER BY T0."CardCode") T ) T;
		END;
		ELSE -- chi
		BEGIN 
			SELECT T."CardCode",
				    T."CardName",
				    T."Check",
				    T."DocEntry" AS "DocNum",
				    T."InvCode",
				    T."HasRequest",
					CASE WHEN T."HasRequest"  = 'Y' THEN 'Đã tạo đề xuất'
						 ELSE ''
					 END AS "Message",
				    T."DocDate", 
				    T."DueDate",
				    CURRENT_DATE AS "PostingDate",
				    t."JrnlMemo",
				    T."DocCur",
				    T."InsTotal" AS "InsTotal",	
					T."InsTotalFC",	
	  				T."DocRate",			    
				    CAST(T."MustPay" AS DECIMAL) AS "MustPay",  
				    CAST(T."MustPayAsCash" AS DECIMAL) AS "MustPayAsCash", 
				    CAST('' AS NVARCHAR(200)) AS "Account",  
				    CAST(T."MustPayAsBank" AS DECIMAL) AS "MustPayAsBank",
				    CAST('' AS NVARCHAR(200)) AS "Bank",
				    T."DocEntry",
				    CAST('' AS NVARCHAR(200))  AS "CFlow",
				    'Data' AS "Manual"
			   FROM (
					SELECT T."CardCode",
						    T."CardName",
						    T."Check",
						    T."DocNum",
						    T."InvCode",
						    T."HasRequest",
						    T."DocDate", 
						    T."DueDate",
						    T."DocCur",
						    T."InsTotal" AS "InsTotal",		  
						    T."InsTotalFC",
	  				        T."DocRate",		      
						    CASE WHEN T."MustPay" > 0 THEN T."MustPay"
						   	 	 ELSE 0 
						   	 END AS "MustPay",  
						    0 AS "MustPayAsCash",  
						    0 AS "MustPayAsBank",
						    T."DocEntry",
					        t."JrnlMemo",
						    'Data' AS "Manual"
					  FROM (SELECT T0."CardCode",
								   T2."CardName",
								   'N' AS "Check",
								   T0."DocNum",
							       T0."U_InvCode" 
							   AS "InvCode",
							       CASE WHEN COALESCE(T4."Status", '') <> 'S' THEN 'N'
							   	        ELSE 'Y'
							   	    END AS "HasRequest",
								   T0."DocDate", 
								   T1."DueDate",
								   T1."InsTotal",
								   T1."InsTotalFC",
		  				       	    CASE WHEN T0."DocCur" = 'VND' THEN 1.0 
		  				       	    ELSE COALESCE(T3."Rate", 0.0)
		  				       END "DocRate",
		  				       --T0."DocRate",
								   CASE WHEN T0."DocCur" = 'VND' THEN T1."InsTotal" - T1."PaidToDate" 
								   		ELSE T1."InsTotalFC" - T1."PaidFC" 
								   	END AS "MustPay",
								   T0."DocCur",
								   T0."DocEntry",
							      t0."JrnlMemo"
							  FROM OPCH T0
							  LEFT JOIN PCH6 T1 ON T0."DocEntry" = T1."DocEntry"
							  LEFT JOIN OCRD T2 ON T0."CardCode" = T2."CardCode"
						      LEFT JOIN ORTT T3 ON T3."Currency" = T0."DocCur"
						  				       AND T3."RateDate" = CURRENT_DATE
						      LEFT JOIN "tbPaymentToolLogging" T4 ON T4."CardCode" = T0."CardCode" 
						  				   		 		         AND T4."DocEntry" = T0."DocEntry"
						  				   		 		         AND T4."PaymentType" = 'C'
						  				   		 		         AND T4."Status" = 'S'	
							 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
							   AND T0."BPLId" = :v_BrId
							   AND T1."InsTotal" - T1."PaidToDate" > 0	
						   	   AND COALESCE(T4."CardCode", '') = ''	
						       AND (:v_CardCode = '' OR T0."CardCode" IN (SELECT * FROM "SPLIT_STRING"(:v_CardCode)))				   
						     ORDER BY T0."CardCode") T
							 )T;
			
			END;
		END IF;
	ELSE -- view history and after action
		IF :v_Type = 'T' -- thu 
		THEN
		BEGIN
			 SELECT T."CardCode",
				    T."CardName",
				    T."Check",
				    T."DocEntry" AS "DocNum",
				    T."InvCode",
				    T."HasRequest",					
					CASE WHEN T."HasRequest"  = 'Y' THEN 'Đã tạo đề xuất'
						 ELSE T."Message"
					 END AS "Message",
				    T."DocDate", 
				    T."DueDate",
				    CURRENT_DATE AS "PostingDate",
				    t."JrnlMemo",
				    T."DocCur",
				    T."InsTotal" AS "InsTotal",	
					T."InsTotalFC",	
	  				T."DocRate",		    
				    CAST(T."MustPay" AS DECIMAL) AS "MustPay",  
				    CAST(T."MustPayAsCash" AS DECIMAL) AS "MustPayAsCash",   
				    CAST('' AS NVARCHAR(200)) AS "Account",
				    CAST(T."MustPayAsBank" AS DECIMAL) AS "MustPayAsBank",
				    CAST('' AS NVARCHAR(200)) AS "Bank",
				    T."DocEntry",
				    CAST('' AS NVARCHAR(200))  AS "CFlow",
				    'Data' AS "Manual"
			   FROM (
				 SELECT T."CardCode",
					    T."CardName",
					    T."Check",
					    T."DocNum",
					    T."InvCode",
						T."HasRequest",
						T."Message",
					    T."DocDate", 
					    T."DueDate",
					    T."DocCur",
					    T."InsTotal" AS "InsTotal",	
						T."InsTotalFC",
	  				    T."DocRate",		    
					    CASE WHEN T."MustPay" > 0 THEN T."MustPay"
					   	 	 ELSE 0 
					   	 END AS "MustPay",  
					    0 AS "MustPayAsCash",  
					    0 AS "MustPayAsBank",
					    T."DocEntry",
					    t."JrnlMemo",
					    'Data' AS "Manual"
				  FROM (
				  	/*
				  	SELECT T0."CardCode",
							   T2."CardName",
							   'N' AS "Check",
							   T0."DocNum",
							   T0."U_InvCode" AS "InvCode",
							   CASE WHEN COALESCE(T4."Status", '') <> 'S' THEN 'N'
							   	    ELSE 'Y'
							   	END AS "HasRequest",
							   T4."StatusName" AS "Message",
							   T0."DocDate", 
							   T1."DueDate",
							   T1."InsTotal",
							   T1."InsTotalFC",
		  				       CASE WHEN T0."DocCur" = 'VND' THEN 1.0 
		  				       	    ELSE COALESCE(T3."Rate", 0.0)
		  				       END "DocRate",
		  				       -- T0."DocRate",
							   CASE WHEN T0."DocCur" = 'VND' THEN T1."InsTotal" - T1."PaidToDate" 
							   		ELSE T1."InsTotalFC" - T1."PaidFC" 
							   	END AS "MustPay",
							   T0."DocCur",
							   T0."DocEntry",
							   t0."JrnlMemo"
						  FROM OINV T0
						  LEFT JOIN INV6 T1 ON T0."DocEntry" = T1."DocEntry"
						  LEFT JOIN OCRD T2 ON T0."CardCode" = T2."CardCode"
						  LEFT JOIN ORTT T3 ON T3."Currency" = T0."DocCur"
						  				   AND T3."RateDate" = CURRENT_DATE	
						  JOIN "tbPaymentToolLogging" T4 ON T4."CardCode" = T0."CardCode" 
						  				   		 		     AND T4."DocEntry" = T0."DocEntry"
						  				   		 		     AND T4."PaymentType" = 'T'
						  				   		 		  --   AND T4."Status" = 'S'
						*/
				SELECT DISTINCT * 
				  FROM (
						SELECT T0."CardCode",
						       T2."CardName",
						       'N' AS "Check",
							   T0."DocNum",
							    T0."U_InvCode" 
							   AS "InvCode",
							   CASE WHEN COALESCE(T4."Status", '') <> 'S' THEN 'N'
									ELSE 'Y'
							    END AS "HasRequest",
							  T4."StatusName" AS "Message",T0."BPLId",
							  T0."DocDate", 
							  T1."DueDate",
						      T1."InsTotal",
											   T1."InsTotalFC",
						  				       CASE WHEN T0."DocCur" = 'VND' THEN 1.0 
						  				       	    ELSE COALESCE(T3."Rate", 0.0)
						  				       END "DocRate",
						  				       -- T0."DocRate",
							   CASE WHEN T0."DocCur" = 'VND' THEN T1."InsTotal" - T1."PaidToDate" 
							   		ELSE T1."InsTotalFC" - T1."PaidFC" 
							   	END AS "MustPay",
							   T0."DocCur",
							   T0."DocEntry",
							   t0."JrnlMemo",
							   T4."Key",
							   T4."Status"
						  FROM OINV T0
						  LEFT JOIN INV6 T1 ON T0."DocEntry" = T1."DocEntry"
						  LEFT JOIN OCRD T2 ON T0."CardCode" = T2."CardCode"
						  LEFT JOIN ORTT T3 ON T3."Currency" = T0."DocCur"
						  				   AND T3."RateDate" = CURRENT_DATE	
						  JOIN "tbPaymentToolLogging" T4 ON T4."CardCode" = T0."CardCode" 
						  				   		 		     AND T4."DocEntry" = T0."DocEntry"
						  				   		 		     AND T4."PaymentType" = 'T'
						  				   		 		     AND T4."Status" <> 'S'
						 
						 UNION ALL
						SELECT DISTINCT T4."CardCode",
											   T2."CardName",
											   'N' AS "Check",
											   T0."DocNum",
							   '' -- T0."U_InvCode" 
							   AS "InvCode",
							   CASE WHEN COALESCE(T4."Status", '') <> 'S' THEN 'N'
									ELSE 'Y'
							    END AS "HasRequest",
							  'Đã tạo' AS "Message",T0."BPLId",
											   T0."DocDate", 
											   T1."DueDate",
											   T1."InsTotal",
											   T1."InsTotalFC",
						  				       t4."DocRate",
											   CASE WHEN T4."DocCurr" = 'VND' THEN T1."InsTotal" - T1."PaidToDate"
											   		ELSE T1."InsTotalFC" - T1."PaidFC" 
											   	END AS "MustPay",
											   	T0."DocCur",
											   T0."DocEntry",
							   t0."JrnlMemo",
							   T5."Key",
							   T5."Status"
										  FROM ORCT T4
										  LEFT JOIN RCT2 T3 ON T3."DocNum" = T4."DocEntry"
										   				   AND T3."InvType" = 13
										  LEFT JOIN OINV T0 ON T3."DocEntry" = T0."DocEntry"
										  				   AND T4."CardCode" = T0."CardCode"
						 				  LEFT JOIN INV6 T1 ON T0."DocEntry" = T1."DocEntry"
										  LEFT JOIN OCRD T2 ON T4."CardCode" = T2."CardCode"
						  JOIN "tbPaymentToolLogging" T5 ON T5."CardCode" = T0."CardCode" 
						  				   		 		     AND T5."DocEntry" = T0."DocEntry"
						  				   		 		     AND T5."PaymentType" = 'T'
						  				   		 		     AND T5."Status" = 'S'
						  				   		 		     AND T5."PaymentKey" = t4."U_PaymentKey"
								) T0			  				   
						 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
						   AND T0."BPLId" = :v_BrId
						   --AND T1."InsTotal" - T1."PaidToDate" > 0
						   AND (:v_CardCode = '' OR T0."CardCode" IN (SELECT * FROM "SPLIT_STRING"(:v_CardCode)))
						   AND ((:v_IsHistory = 'N' AND T0."Key" = :v_Key) OR (:v_IsHistory = 'Y' AND T0."Status" = 'S' ))
						 ORDER BY T0."CardCode") T ) T;
		END;
		ELSE -- chi
		BEGIN 
			SELECT T."CardCode",
				    T."CardName",
				    T."Check",
				    T."DocEntry" AS "DocNum",
				    T."InvCode",
				    T."HasRequest",			
					CASE WHEN T."HasRequest"  = 'Y' THEN 'Đã tạo đề xuất'
						 ELSE T."Message"
					 END AS "Message",
				    T."DocDate", 
				    T."DueDate",
				    CURRENT_DATE AS "PostingDate",
				    t."JrnlMemo",
				    T."DocCur",
				    T."InsTotal" AS "InsTotal",	
					T."InsTotalFC",	
	  				T."DocRate",			    
				    CAST(T."MustPay" AS DECIMAL) AS "MustPay",  
				    CAST(T."MustPayAsCash" AS DECIMAL) AS "MustPayAsCash", 
				    CAST('' AS NVARCHAR(200)) AS "Account",  
				    CAST(T."MustPayAsBank" AS DECIMAL) AS "MustPayAsBank",
				    CAST('' AS NVARCHAR(200)) AS "Bank",
				    T."DocEntry",
				    CAST('' AS NVARCHAR(200))  AS "CFlow",
				    'Data' AS "Manual"
			   FROM (
					SELECT T."CardCode",
						    T."CardName",
						    T."Check",
						    T."DocNum",
						    T."InvCode",
						    T."HasRequest",
						    T."Message",
						    T."DocDate", 
						    T."DueDate",
						    T."DocCur",
						    T."InsTotal" AS "InsTotal",		  
						    T."InsTotalFC",
	  				        T."DocRate",		      
						    CASE WHEN T."MustPay" > 0 THEN T."MustPay"
						   	 	 ELSE 0 
						   	 END AS "MustPay",  
						    0 AS "MustPayAsCash",  
						    0 AS "MustPayAsBank",
						    T."DocEntry",
					        t."JrnlMemo",
						    'Data' AS "Manual"
					  FROM (SELECT T0."CardCode",
								   T2."CardName",
								   'N' AS "Check",
								   T0."DocNum",
							   T0."U_InvCode" 
							   AS "InvCode",
							       CASE WHEN COALESCE(T4."Status", '') <> 'S' THEN 'N'
							   	        ELSE 'Y'
							   	    END AS "HasRequest",
							   	   T4."StatusName" AS "Message",
								   T0."DocDate", 
								   T1."DueDate",
								   T1."InsTotal",
								   T1."InsTotalFC",
		  				       	    CASE WHEN T0."DocCur" = 'VND' THEN 1.0 
		  				       	    ELSE COALESCE(T3."Rate", 0.0)
		  				       END "DocRate",
		  				       --T0."DocRate",
								   CASE WHEN T0."DocCur" = 'VND' THEN T1."InsTotal" - T1."PaidToDate" 
								   		ELSE T1."InsTotalFC" - T1."PaidFC" 
								   	END AS "MustPay",
								   T0."DocCur",
								   T0."DocEntry",
							      t0."JrnlMemo"
							  FROM OPCH T0
							  LEFT JOIN PCH6 T1 ON T0."DocEntry" = T1."DocEntry"
							  LEFT JOIN OCRD T2 ON T0."CardCode" = T2."CardCode"
						      LEFT JOIN ORTT T3 ON T3."Currency" = T0."DocCur"
						  				       AND T3."RateDate" = CURRENT_DATE
						      JOIN "tbPaymentToolLogging" T4 ON T4."CardCode" = T0."CardCode" 
						  				   		 		         AND T4."DocEntry" = T0."DocEntry"
						  				   		 		         AND T4."PaymentType" = 'C'
  				   		 		    							 --AND T4."PaymentKey" = t0."U_PaymentKey"
						  				   		 		       --  AND T4."Status" = 'S'	
							 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
							   AND T0."BPLId" = :v_BrId
							   AND T1."InsTotal" - T1."PaidToDate" > 0		
						       AND (:v_CardCode = '' OR T0."CardCode" IN (SELECT * FROM "SPLIT_STRING"(:v_CardCode)))	
						   AND ((:v_IsHistory = 'N' AND T4."Key" = :v_Key) OR (:v_IsHistory = 'Y' AND T4."Status" = 'S' ))		   
						     ORDER BY T0."CardCode") T
							 )T;
			
			END;
		END IF;
	END IF;
END;
