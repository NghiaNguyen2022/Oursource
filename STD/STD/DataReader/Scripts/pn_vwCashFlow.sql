ALTER VIEW "vw_Bank_vwCashFlow"
AS
SELECT "CFWId", "CFWName" FROM "OCFW" 
 WHERE "Postable" = 'Y'
   AND "CFWId" = 7



