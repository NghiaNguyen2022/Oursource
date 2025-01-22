ALTER PROCEDURE "usp_Bank_ApprovalInfor"
(
	IN v_UserID NVARCHAR(4)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
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
	 ORDER BY T0."CreateDate" DESC, T0."CreateTime" DESC;
END

