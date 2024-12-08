ALTER VIEW "vw_Bank_BankAccount"
AS
	SELECT * FROM (
	SELECT ROW_NUMBER() OVER (ORDER BY T1."BankCode") AS "RowNum", 
		   'VT' AS "Key",
		   T1."BankCode" AS "Code", 
		   T1."BankName" AS "Name", 
		   T0."Account" AS "Account" ,
		   '10698' AS "SenderBankCode"
	  FROM DSC1 t0 
	  JOIN ODSC T1  ON T0."BankCode" = T1."BankCode"
	 WHERE T0."AcctName" = 'PYMEPHARCO'
	   AND T1."BankCode" = 'ICB'
     UNION ALL
	SELECT ROW_NUMBER() OVER (ORDER BY T1."BankCode") AS "RowNum", 
		   'BI' AS "Key",
		   T1."BankCode" AS "Code", 
		   T1."BankName" AS "Name", 
		   T0."Account" AS "Account" ,
		   '10698' AS "SenderBankCode"
	  FROM DSC1 t0 
	  JOIN ODSC T1  ON T0."BankCode" = T1."BankCode"
	 WHERE T1."BankCode" = 'BIDV') T


