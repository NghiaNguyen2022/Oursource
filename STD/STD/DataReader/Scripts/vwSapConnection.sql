CREATE VIEW "vw_bank_SapConnection"
AS

SELECT 'B1i' AS "SapUser",
	   'Pd8DsFYMHA/hDgMrbaxjIw==' AS "SapPassword",
	   '172.31.126.11:30015' AS "ServerName",
	   'PMP_QAS_SP2' AS "CompanyDB",
	   'azstvntstsapb1db:40000' AS "SLDServer",
	   'SYSTEM' AS "DBUser",
	   '9i+rFQcelZKpJ5Ex3uy5sQ==' AS "DBPass"
FROM DUMMY;