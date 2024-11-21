drop TABLE "tbPaymentToolLogging"
CREATE TABLE "tbPaymentToolLogging"
(
	"CardCode" VARCHAR(50),
	"DocEntry" INT,
	"PaymentType" CHAR(1),
	"PaymentKey" VARCHAR(50),
	"Status"  VARCHAR(10),
	"StatusName" VARCHAR(500),
	"DateAction" Date,
	"Key" VARCHAR(30)
)

-- Call "SP_LogPaymentTool" ('NC0100051', '1283', 'C', 'F', 'Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Dòng tiền.') 

create PROCEDURE "SP_LogPaymentTool"
(
	IN v_CardCode NVARCHAR(100),
	IN v_DocEntry INT,
	IN v_DocType CHAR(1),
	IN v_PaymentKey VARCHAR(50),
	IN v_Status VARCHAR(1),
	IN v_StatusName VARCHAR(500),
	IN v_key VARCHAR(30)
	
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	IF EXISTS (SELECT 1	
				 FROM "tbPaymentToolLogging"
				WHERE "CardCode" = :v_CardCode
				  AND "DocEntry" = :v_DocEntry
				  AND "PaymentType" = :v_DocType
				)
	THEN
		UPDATE "tbPaymentToolLogging"
 		   SET "PaymentKey" = :v_PaymentKey,
 		   	   "Status" = :v_Status,
 		   	   "StatusName" = :v_StatusName,
 		   	   "DateAction" = CURRENT_DATE,
 		   	   "Key" = :v_key
 		 WHERE "CardCode" = :v_CardCode
		   AND "DocEntry" = :v_DocEntry
		   AND "PaymentType" = :v_DocType;
	ELSE
		INSERT INTO "tbPaymentToolLogging"
		VALUES (:v_CardCode, :v_DocEntry, :v_DocType, :v_PaymentKey, :v_Status, :v_StatusName, CURRENT_DATE, :v_key);
	END IF;
END;