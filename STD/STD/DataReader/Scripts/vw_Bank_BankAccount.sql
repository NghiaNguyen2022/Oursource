CREATE VIEW "vw_Bank_BankAccount"
AS
	SELECT ROW_NUMBER() OVER (ORDER BY T1."BankCode") AS "RowNum", 
		   T1."BankCode" AS "Code", 
		   T1."BankName" AS "Name", 
		   T0."Account" AS "Account" 
	  FROM DSC1 t0 
	  JOIN ODSC T1  ON T0."BankCode" = T1."BankCode"