CREATE VIEW "vw_bank_SapConnection" ( "SapUser",
	 "SapPassword",
	 "ServerName",
	 "CompanyDB",
	 "SLDServer",
	 "DBUser",
	 "DBPass" ) AS SELECT
	 'fpt.phinh17' AS "SapUser",
	 'Pd8DsFYMHA/hDgMrbaxjIw==' AS "SapPassword",
	 'HSQ@AZSTVNTSTSAPB1DB:30013' AS "ServerName",
	 'PMP_QAS_SP2' AS "CompanyDB",
	 'azstvntstsapb1db:40000' AS "SLDServer",
	 'SYSTEM' AS "DBUser",
	 '9i+rFQcelZKpJ5Ex3uy5sQ==' AS "DBPass" 
FROM DUMMY