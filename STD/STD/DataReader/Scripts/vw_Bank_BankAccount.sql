alter VIEW "vw_Bank_BankAccount"
AS
	SELECT ROW_NUMBER() OVER (ORDER BY T1."BankCode") AS "RowNum", 
		   T1."BankCode" AS "Code", 
		   T1."BankName" AS "Name", 
		   T0."Account" AS "Account" ,
		   '10698' AS "SenderBankCode"
	  FROM DSC1 t0 
	  JOIN ODSC T1  ON T0."BankCode" = T1."BankCode"
	  WHERE  T0."AcctName" = 'PYMEPHARCO'


