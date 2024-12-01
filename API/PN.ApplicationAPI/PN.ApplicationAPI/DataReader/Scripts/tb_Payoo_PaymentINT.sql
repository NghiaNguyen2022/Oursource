DROP TABLE "tb_Payoo_PaymentINT"

CREATE TABLE "tb_Payoo_PaymentINT"
(
	"PaymentMethod"	INT,
	"PaymentMethodName"	NVARCHAR(50),
	"PurDate" NVARCHAR(13),
	"MerchantUserName" NVARCHAR(50),
	"ShopId" NUMERIC,
	"MasterShopID" NUMERIC,
	"OrderNo" NVARCHAR(50),
	"OrderCash" DECIMAL(19,2),
	"BankName" NVARCHAR(50),
	"CardNumber" NVARCHAR(50),
	"CardIssuanceType" INT,
	"PaymentStatus" INT,
	"MDD1" NVARCHAR(50),
	"MDD2" NVARCHAR(50),
	"PaymentSource" NVARCHAR(50),
	"VoucherTotalAmount" DECIMAL(19,2),
	"VoucherID" NVARCHAR(50),
	"IntDate" DATE,
	"IntTime" NVARCHAR(6) -- 'HHmmss'
)
CREATE PROCEDURE "usp_Payoo_CheckOrderExists"
(
	IN v_OrderNo NVARCHAR(50)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	IF EXISTS (SELECT 1
				 FROM "tb_Payoo_PaymentINT" 
			    WHERE "OrderNo" = :v_OrderNo)
	THEN
		SELECT 'Existed' AS "Existed" FROM DUMMY;
	ELSE	
		SELECT 'No' AS "Existed" FROM DUMMY;
	END IF;
END