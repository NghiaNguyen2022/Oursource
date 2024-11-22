using SAPbobsCOM;
using System;

namespace SAPCore.SAP.DIAPI
{
    public class DraftToDocument
    {
        private static void InitDocumentType(string objType, ref SAPbobsCOM.Documents draft, ref SAPbobsCOM.Documents documents)
        {
            switch (objType)
            {
                case "23":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oQuotations);
                    break;
                case "17":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oOrders);
                    break;
                case "15":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
                    break;
                case "234000031":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oReturnRequest);
                    break;
                case "16":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oReturns);
                    break;
                case "203":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDownPayments);
                    break;
                case "13":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oInvoices);
                    break;
                case "14":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oCreditNotes);
                    break;

                case "1470000113":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPurchaseRequest);
                    break;
                case "540000006":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPurchaseQuotations);
                    break;
                case "22":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPurchaseOrders);
                    break;
                case "20":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
                    break;
                case "234000032":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oGoodsReturnRequest);
                    break;
                case "21":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPurchaseReturns);
                    break;
                case "204":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPurchaseDownPayments);
                    break;
                case "18":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                    break;
                case "19":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPurchaseCreditNotes);
                    break;

                case "59":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oInventoryGenEntry);
                    break;
                case "60":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oInventoryGenExit);
                    break;
                case "1250000001":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);
                    break;
                case "67":
                    draft = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                    documents = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                    break;

            }
        }
        public static bool ConvertPayment(int draftEntry, string objType, ref string message)
        {
            try
            {
                var oPaymntDraf = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                SAPbobsCOM.Payments oPayment = null;
                if (objType == "46")
                {
                    oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oVendorPayments);
                }
                else if (objType == "24")
                {
                    oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oIncomingPayments);
                }

                if (oPayment == null)
                {
                    message = StringConstrants.Error;
                    return false;
                }

                if (oPaymntDraf.GetByKey(draftEntry))
                {
                    oPayment.CardCode = oPaymntDraf.CardCode;
                    oPayment.DocCurrency = oPaymntDraf.DocCurrency;
                    oPayment.DocRate = oPaymntDraf.DocRate;
                    oPayment.DocDate = oPaymntDraf.DocDate;
                    oPayment.BPLID = oPaymntDraf.BPLID;

                    for (int i = 0; i < oPaymntDraf.Invoices.Count; i++)
                    {
                        oPaymntDraf.Invoices.SetCurrentLine(i);

                        if (i != 0)
                        {
                            oPayment.Invoices.Add();
                            oPayment.Invoices.SetCurrentLine(i);
                        }
                        oPayment.Invoices.DocEntry = oPaymntDraf.Invoices.DocEntry;
                        oPayment.Invoices.InvoiceType = oPaymntDraf.Invoices.InvoiceType;
                        oPayment.Invoices.SumApplied = oPaymntDraf.Invoices.SumApplied;
                        oPayment.Invoices.AppliedFC = oPaymntDraf.Invoices.AppliedFC;
                    }
                    oPayment.CashSum = oPaymntDraf.CashSum;
                    oPayment.CashAccount = oPaymntDraf.CashAccount;
                    oPayment.TransferSum = oPaymntDraf.TransferSum;
                    oPayment.TransferAccount = oPaymntDraf.TransferAccount;

                    if (oPayment.PrimaryFormItems.Count > 0)
                    {
                        for (int i = 0; i < oPaymntDraf.PrimaryFormItems.Count; i++)
                        {
                            oPaymntDraf.PrimaryFormItems.SetCurrentLine(i);
                            if (i != 0)
                            {
                                oPayment.PrimaryFormItems.Add();
                            }
                            oPayment.PrimaryFormItems.SetCurrentLine(i);
                            oPayment.PrimaryFormItems.CashFlowLineItemID = oPaymntDraf.PrimaryFormItems.CashFlowLineItemID;
                            oPayment.PrimaryFormItems.AmountFC = oPaymntDraf.PrimaryFormItems.AmountFC;
                            oPayment.PrimaryFormItems.AmountLC = oPaymntDraf.PrimaryFormItems.AmountLC;
                            oPayment.PrimaryFormItems.PaymentMeans = oPaymntDraf.PrimaryFormItems.PaymentMeans;
                        }
                    }
                    oPayment.Remarks = StringConstrants.DocumentByAPI;
                    var ret = oPayment.Add();
                    if (ret == 0)
                    {
                        oPaymntDraf.SaveDraftToDocument();
                        message = "Add success";
                        return true;
                    }
                    else
                    {
                        message = DIServiceConnection.Instance.Company.GetLastErrorDescription();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            return false;
        }
        public static bool ConvertStockTransfer(int draftEntry, ref string message)
        {
            try
            {
                SAPbobsCOM.StockTransfer documentDraft = (SAPbobsCOM.StockTransfer)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oStockTransferDraft);
                SAPbobsCOM.StockTransfer document = (SAPbobsCOM.StockTransfer)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
                if (documentDraft.GetByKey(draftEntry))
                {
                    document.DocDate = documentDraft.DocDate;
                    document.DueDate = documentDraft.DueDate;
                    document.FromWarehouse = documentDraft.FromWarehouse;
                    document.ToWarehouse = documentDraft.ToWarehouse;

                    for (int i = 0; i < documentDraft.Lines.Count; i++)
                    {
                        documentDraft.Lines.SetCurrentLine(i);

                        if (i != 0)
                        {
                            document.Lines.Add();
                            document.Lines.SetCurrentLine(i);
                        }
                        document.Lines.ItemCode = documentDraft.Lines.ItemCode;
                        document.Lines.Quantity = documentDraft.Lines.Quantity;
                        document.Lines.Price = documentDraft.Lines.Price;
                        document.Lines.Currency = documentDraft.Lines.Currency;
                        document.Lines.FromWarehouseCode = documentDraft.Lines.FromWarehouseCode;
                        document.Lines.WarehouseCode = documentDraft.Lines.WarehouseCode;
                    }
                    document.Comments = StringConstrants.DocumentByAPI;
                    var ret = document.Add();
                    if (ret == 0)
                    {
                        documentDraft.SaveDraftToDocument();
                        message = "Add success";
                        return true;
                    }
                    else
                    {
                        message = DIServiceConnection.Instance.Company.GetLastErrorDescription();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            return false;
        }
        public static bool ConvertDocument(int draftEntry, string objType, ref string message)
        {
            try
            {
                SAPbobsCOM.Documents documentDraft = null;// = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oDrafts);
                SAPbobsCOM.Documents document = null;// = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oOrders);
                InitDocumentType(objType, ref documentDraft, ref document);
                if (documentDraft == null || document == null)
                {
                    message = StringConstrants.Error;
                    return false;
                }

                if (documentDraft.GetByKey(draftEntry))
                {
                    document.CardCode = documentDraft.CardCode;
                    document.DocDate = documentDraft.DocDate;
                    document.DocDueDate = documentDraft.DocDueDate;
                    document.DocCurrency = documentDraft.DocCurrency;
                    document.DocRate = documentDraft.DocRate;
                    document.BPL_IDAssignedToInvoice = documentDraft.BPL_IDAssignedToInvoice;
                    document.Comments = StringConstrants.DocumentByAPI;

                    if (objType == "540000006" || objType == "23")
                    {
                        document.RequriedDate = documentDraft.RequriedDate;
                    }

                    for (int i = 0; i < documentDraft.Lines.Count; i++)
                    {
                        documentDraft.Lines.SetCurrentLine(i);

                        if (i != 0)
                        {
                            document.Lines.Add();
                            document.Lines.SetCurrentLine(i);
                        }
                        document.Lines.ItemCode = documentDraft.Lines.ItemCode;
                        document.Lines.Quantity = documentDraft.Lines.Quantity;
                        document.Lines.Price = documentDraft.Lines.Price;
                        if (documentDraft.Lines.BaseType != -1)
                        {
                            document.Lines.BaseLine = documentDraft.Lines.BaseLine;
                            document.Lines.BaseType = documentDraft.Lines.BaseType;
                            document.Lines.BaseEntry = documentDraft.Lines.BaseEntry;
                        }

                        if (objType == "59" || objType == "60")
                        {
                            document.Lines.WarehouseCode = documentDraft.Lines.WarehouseCode;
                            //document.Lines.
                        }
                    }
                    var ret = document.Add();
                    if (ret == 0)
                    {
                        documentDraft.SaveDraftToDocument();
                        message = "Add success";
                        return true;
                    }
                    else
                    {
                        message = DIServiceConnection.Instance.Company.GetLastErrorDescription();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            return false;
        }
    }
}
