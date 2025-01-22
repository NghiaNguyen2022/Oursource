CREATE TABLE "tb_Bank_TransferRecord"
(   "CardCode" NVARCHAR(30),
    "DocEntry" NVARCHAR(30),
    "requestId" NVARCHAR(30),
    "transId" NVARCHAR(30),
    "amount" DECIMAL(19,2),
    "status" NVARCHAR(30),
    "bankStatus" NVARCHAR(30),
    "Message" NVARCHAR(300),
    "bank" NVARCHAR(30)
)


ALTER TABLE "tb_Bank_TransferRecord"
ADD ("ApprStatus" NVARCHAR(30));
ALTER TABLE "tb_Bank_TransferRecord"
ADD ("UserRequest" NVARCHAR(30))