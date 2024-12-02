DROP TABLE "tb_Bank_BIDV_AccesstokenINT"

CREATE TABLE "tb_Bank_BIDV_AccesstokenINT"
(
	"PaymentMethod"	INT,
	"PaymentMethodName"	NVARCHAR(50),
	"PurDate" NVARCHAR(20),
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