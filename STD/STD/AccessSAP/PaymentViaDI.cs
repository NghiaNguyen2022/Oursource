using System;
using System.Collections.Generic;
using System.Linq;
using STDApp.Models;
using PN.SmartLib.Helper;
using SAPbobsCOM;
using SAPCore.SAP.DIAPI;

namespace STDApp.AccessSAP
{
    public class PaymentViaDI
    {
        private static bool ConnectDI(ref string message)
        {
            try
            {
                var retConnect = DIConnection.Instance.Connect(ref message, DIConnection.Instance.CookieConnection);
                if (!retConnect)
                {
                    var mess = DIConnection.Instance.Company.GetLastErrorDescription();
                    message = STRING_CONTRANTS.CanNotConnectDIAPI + " - " + mess;
                }
                return retConnect;
            }
            catch (Exception ex)
            {
                message = STRING_CONTRANTS.CanNotConnectDIAPI + " - " + ex.Message;
                return false;
            }
        }

        private static void DisConnectDI()
        {
            DIConnection.Instance.DIDisconnect();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docentry"></param>
        /// <param name="newVal"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool UpdateAccountPayment(ChangeOfApprove data, ref string message)
        {
            if (!ConnectDI(ref message))
                return false;

            var ret = false;

            try
            {
                var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                if (oPayment.GetByKey(data.DocEntry))
                {
                    if (!string.IsNullOrEmpty(data.OldAccount) && data.OldAccount != data.ChangeAccount)
                    {
                        if (!string.IsNullOrEmpty(data.ChangeAccount))
                        {
                            oPayment.CashAccount = data.ChangeAccount;
                        }
                    }
                    if (!string.IsNullOrEmpty(data.OldBank) && data.OldBank != data.ChangeBank)
                    {
                        if (!string.IsNullOrEmpty(data.ChangeBank))
                        {
                            oPayment.UserFields.Fields.Item("U_BANK").Value = data.ChangeBank;
                            var bankAccount = ViewHelper.Banks.Where(x => x.Code == data.ChangeBank).Select(x => x.Account).FirstOrDefault();
                            oPayment.TransferAccount = bankAccount;
                        }
                    }
                    oPayment.UserFields.Fields.Item("U_Review").Value = "Y";
                    var saveret = oPayment.Update();
                    if (saveret != 0)
                    {
                        message = DIConnection.Instance.Company.GetLastErrorDescription();
                        ret = false;
                    }
                    else
                    {
                        message = STRING_CONTRANTS.Notice_UpdatePaymentSuccess;
                        ret = true;
                    }
                }
                else
                {
                    ret = false;
                    message = STRING_CONTRANTS.Notice_CanNotloadDraft;
                }
            }
            catch (Exception ex)
            {
                ret = false;
                message = ex.Message;
            }
            DisConnectDI();
            return ret;
        }
        public static List<PaymentActionResult> CreatePaymnents(PaymentType type, List<PaymentDetail> datas, string key, PaymentDocumentType paymentType, PaymentMethod paymentmethod, string branch, ref string message)
        {
            if (!ConnectDI(ref message))
                return null;
            var results = new List<PaymentActionResult>();
            try
            {

                var bpList = datas.Select(x => x.CardCode).Distinct().ToList();
                foreach (var pb in bpList)
                {
                    var invoices = datas.Where(x => x.CardCode == pb).ToList();

                    var result = new PaymentActionResult();
                    result.CardCode = pb;
                    result.Count = invoices.Count;

                    int lRetCode = -1, lErrCode;
                    var currencies = datas.Where(x => x.CardCode == pb).Select(x => x.Currency).Distinct().ToList();
                    if (currencies.Count > 1 || currencies[0] == "")
                    {
                        result.Message = STRING_CONTRANTS.DifferenceCurrency;
                        result.Flag = false;
                        results.Add(result);
                        continue;
                    }
                    var banks = datas.Where(x => x.CardCode == pb).Select(x => x.BankInfo).Distinct().ToList();
                    if (banks.Count > 1 || banks[0] == "")
                    {
                        result.Message = STRING_CONTRANTS.DifferenceBank;
                        result.Flag = false;
                        results.Add(result);
                        continue;
                    }

                    var cfs = datas.Where(x => x.CardCode == pb).Select(x => x.Cashflow).Distinct().ToList();
                    if (cfs.Count > 1 || cfs[0] == "")
                    {
                        result.Message = STRING_CONTRANTS.DifferenceCashFlow;
                        result.Flag = false;
                        results.Add(result);
                        continue;
                    }
                    var bankAccounts = datas.Where(x => x.CardCode == pb).Select(x => x.BankAccount).Distinct().ToList();
                    SAPbobsCOM.Payments oPayment;
                    if (type == PaymentType.T)
                    {
                        oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oIncomingPayments);
                    }
                    else
                    {
                        oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                        oPayment.DocObjectCode = BoPaymentsObjectType.bopot_OutgoingPayments;
                        oPayment.DocTypte = BoRcptTypes.rSupplier;
                        // oPayment.AuthorizationStatus = PaymentsAuthorizationStatusEnum.pasPending;
                    }
                    oPayment.CardCode = pb;
                    oPayment.DocCurrency = currencies[0];
                    var rate = 1.0;
                    if (currencies[0] != GlobalsConfig.Instance.LocalCurrencyDefault)
                    {
                        var rate2 = datas.Where(x => x.CardCode == pb).Select(x => x.Rate).Distinct().ToList();
                        rate = (double)rate2[0];
                    }
                    oPayment.DocRate = rate;
                    // oPayment.DocDate = DateTime.Now;
                    oPayment.BPLID = int.Parse(branch);
                    oPayment.DocDate = datas.Where(x => x.CardCode == pb).Select(x => x.PaymentDate).Distinct().FirstOrDefault();
                    oPayment.Remarks = datas.Where(x => x.CardCode == pb).Select(x => x.Remark).Distinct().FirstOrDefault();
                    var objectType = type == PaymentType.T ? BoRcptInvTypes.it_Invoice : BoRcptInvTypes.it_PurchaseInvoice;
                    double cash = 0.0;
                    double bank = 0.0;
                    for (var i = 0; i < invoices.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(invoices[i].DocEntry))
                        {
                            if (i != 0)
                            {
                                oPayment.Invoices.Add();
                                oPayment.Invoices.SetCurrentLine(i);
                            }
                            oPayment.Invoices.DocEntry = int.Parse(invoices[i].DocEntry);
                            oPayment.Invoices.InvoiceType = objectType;
                            //oPayment.Invoices.
                            if (currencies[0] == GlobalsConfig.Instance.LocalCurrencyDefault)
                                oPayment.Invoices.SumApplied = (double)invoices[i].AmountCash + (double)invoices[i].AmountBank;
                            else
                                oPayment.Invoices.AppliedFC = (double)invoices[i].AmountCash + (double)invoices[i].AmountBank;
                        }
                        cash = cash + (double)invoices[i].AmountCash;
                        bank = bank + (double)invoices[i].AmountBank;
                    }
                    //oPayment.ac
                    var accountlist = datas.Where(x => x.CardCode == pb).Select(x => x.Account).Distinct().ToList();
                    var account = string.Empty;
                    if (accountlist.Count > 0)
                    {
                        account = accountlist[0];
                    }
                    if (paymentmethod == PaymentMethod.Cash)
                    {
                        oPayment.CashSum = cash;
                        if (!string.IsNullOrEmpty(account))
                        {
                            oPayment.CashAccount = account;
                        }
                    }
                    else if (paymentmethod == PaymentMethod.Bank)
                    {
                        oPayment.TransferSum = bank;
                        oPayment.UserFields.Fields.Item("U_BANK").Value = banks[0];
                        oPayment.TransferAccount = bankAccounts[0];
                    }
                    else
                    {
                        oPayment.CashSum = cash;
                        oPayment.TransferSum = bank;
                        oPayment.UserFields.Fields.Item("U_BANK").Value = banks[0];
                        oPayment.TransferAccount = bankAccounts[0];
                        if (!string.IsNullOrEmpty(account))
                        {
                            oPayment.CashAccount = account;
                        }
                    }
                    if (cfs[0] != "-")
                    {
                        //var oCashFlowAssignments = oPayment.PrimaryFormItems;
                        // if (oCashFlowAssignments != null)
                        // {

                        //oCashFlowAssignments.SetCurrentLine(0);
                        if (bank != 0 && cash != 0)
                        {
                            oPayment.PrimaryFormItems.Add();
                            oPayment.PrimaryFormItems.SetCurrentLine(0);
                            oPayment.PrimaryFormItems.CashFlowLineItemID = int.Parse(cfs[0]);
                            if (currencies[0] != GlobalsConfig.Instance.LocalCurrencyDefault)
                                oPayment.PrimaryFormItems.AmountFC = cash;
                            else
                                oPayment.PrimaryFormItems.AmountLC = cash;
                            oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtCash;
                            oPayment.PrimaryFormItems.Add();

                            oPayment.PrimaryFormItems.SetCurrentLine(1);
                            oPayment.PrimaryFormItems.CashFlowLineItemID = int.Parse(cfs[0]);
                            if (currencies[0] != GlobalsConfig.Instance.LocalCurrencyDefault)
                                oPayment.PrimaryFormItems.AmountFC = bank;
                            else
                                oPayment.PrimaryFormItems.AmountLC = bank;
                            oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                            // oPayment.PrimaryFormItems.Add();
                        }
                        else
                        {
                            oPayment.PrimaryFormItems.Add();
                            oPayment.PrimaryFormItems.SetCurrentLine(0);
                            oPayment.PrimaryFormItems.CashFlowLineItemID = int.Parse(cfs[0]);
                            if (currencies[0] != GlobalsConfig.Instance.LocalCurrencyDefault)
                                oPayment.PrimaryFormItems.AmountFC = cash + bank;
                            else
                                oPayment.PrimaryFormItems.AmountLC = cash + bank;
                            if (cash != 0)
                                oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtCash;
                            else
                                oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                            // oPayment.PrimaryFormItems.Add();
                        }
                        //}                              

                    }
                    oPayment.UserFields.Fields.Item("U_PayType").Value = paymentType.GetDescription();
                    oPayment.UserFields.Fields.Item("U_Review").Value = "N";
                    oPayment.UserFields.Fields.Item("U_PaymentKey").Value = key;
                    oPayment.UserFields.Fields.Item("U_Status").Value = "N";
                    var ret1 = -1;
                    ret1 = oPayment.Add();
                    if (ret1 != 0)
                    {
                        DIConnection.Instance.Company.GetLastError(out lErrCode, out message);
                        result.Flag = false;
                        result.Message = message;// + " " + DIConnection.Instance.Company.GetLastErrorDescription(); ;
                        results.Add(result);
                    }
                    else
                    {
                        string newId = string.Empty;
                        DIConnection.Instance.Company.GetNewObjectCode(out newId);
                        result.Flag = true;
                        result.Message = $"Tạo thành đơn id {newId}";
                        results.Add(result);
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("Error {0}", ex.Message);
                results = null;
            }
            DisConnectDI();
            return results;
        }

        public static List<PaymentActionResult> CreatePayments(PaymentType type, List<PaymentDocument> dataHeader, string key, PaymentDocumentType paymentType, PaymentMethod paymentmethod, string branch, ref string message)
        {
            if (!ConnectDI(ref message))
                return null;
            var results = new List<PaymentActionResult>();
            DIConnection.Instance.Company.StartTransaction();
            try
            {
                foreach (var data in dataHeader)
                {
                    var invoices = data.Details;
                    var result = new PaymentActionResult();
                    result.CardCode = data.CardCode;
                    result.Count = invoices.Count;

                    //result.Flag = true;
                    int lRetCode = -1, lErrCode;

                    SAPbobsCOM.Payments oPayment;
                    if (type == PaymentType.T)
                    {
                        oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oIncomingPayments);
                    }
                    else
                    {
                        oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                        oPayment.DocObjectCode = BoPaymentsObjectType.bopot_OutgoingPayments;
                        oPayment.DocTypte = BoRcptTypes.rSupplier;
                    }
                    oPayment.CardCode = data.CardCode;
                    oPayment.DocCurrency = data.Currency;
                    var rate = 1.0;
                    if (data.Currency != GlobalsConfig.Instance.LocalCurrencyDefault)
                    {
                        rate = (double)data.Rate;
                    }
                    oPayment.DocRate = rate;
                    oPayment.BPLID = int.Parse(branch);
                    oPayment.DocDate = data.PaymentDate;
                    oPayment.Remarks = data.Remark;
                    var objectType = type == PaymentType.T ? BoRcptInvTypes.it_Invoice : BoRcptInvTypes.it_PurchaseInvoice;
                    double cash = 0.0;
                    double bank = 0.0;
                    for (var i = 0; i < invoices.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(invoices[i].DocEntry))
                        {
                            if (i != 0)
                            {
                                oPayment.Invoices.Add();
                                oPayment.Invoices.SetCurrentLine(i);
                            }
                            oPayment.Invoices.DocEntry = int.Parse(invoices[i].DocEntry);
                            oPayment.Invoices.InvoiceType = objectType;
                            //oPayment.Invoices.
                            if (data.Currency == GlobalsConfig.Instance.LocalCurrencyDefault)
                                oPayment.Invoices.SumApplied = (double)invoices[i].AmountCash + (double)invoices[i].AmountBank;
                            else
                                oPayment.Invoices.AppliedFC = (double)invoices[i].AmountCash + (double)invoices[i].AmountBank;
                        }
                        cash = cash + (double)invoices[i].AmountCash;
                        bank = bank + (double)invoices[i].AmountBank;
                    }
                    var account = data.Account;

                    if (paymentmethod == PaymentMethod.Cash)
                    {
                        oPayment.CashSum = cash;
                        oPayment.CashAccount = data.Account;
                    }
                    else if (paymentmethod == PaymentMethod.Bank)
                    {
                        oPayment.TransferSum = bank;
                        oPayment.UserFields.Fields.Item("U_BANK").Value = data.Bank;
                        oPayment.TransferAccount = data.BankAccount;
                    }
                    else
                    {
                        oPayment.CashSum = cash;
                        oPayment.TransferSum = bank;
                        oPayment.UserFields.Fields.Item("U_BANK").Value = data.Bank;
                        oPayment.TransferAccount = data.BankAccount;
                        oPayment.CashAccount = data.Account;
                    }

                    if (bank != 0 && cash != 0)
                    {
                        oPayment.PrimaryFormItems.Add();
                        oPayment.PrimaryFormItems.SetCurrentLine(0);
                        oPayment.PrimaryFormItems.CashFlowLineItemID = int.Parse(data.Cashflow);
                        if (data.Currency != GlobalsConfig.Instance.LocalCurrencyDefault)
                            oPayment.PrimaryFormItems.AmountFC = cash;
                        else
                            oPayment.PrimaryFormItems.AmountLC = cash;
                        oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtCash;
                        oPayment.PrimaryFormItems.Add();

                        oPayment.PrimaryFormItems.SetCurrentLine(1);
                        oPayment.PrimaryFormItems.CashFlowLineItemID = int.Parse(data.Cashflow);
                        if (data.Currency != GlobalsConfig.Instance.LocalCurrencyDefault)
                            oPayment.PrimaryFormItems.AmountFC = bank;
                        else
                            oPayment.PrimaryFormItems.AmountLC = bank;
                        oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                        // oPayment.PrimaryFormItems.Add();
                    }
                    else
                    {
                        oPayment.PrimaryFormItems.Add();
                        oPayment.PrimaryFormItems.SetCurrentLine(0);
                        oPayment.PrimaryFormItems.CashFlowLineItemID = int.Parse(data.Cashflow);
                        if (data.Currency != GlobalsConfig.Instance.LocalCurrencyDefault)
                            oPayment.PrimaryFormItems.AmountFC = cash + bank;
                        else
                            oPayment.PrimaryFormItems.AmountLC = cash + bank;
                        if (cash != 0)
                            oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtCash;
                        else
                            oPayment.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                    }

                    oPayment.UserFields.Fields.Item("U_PayType").Value = paymentType.GetDescription();
                    oPayment.UserFields.Fields.Item("U_Review").Value = "N";
                    oPayment.UserFields.Fields.Item("U_PaymentKey").Value = key;
                    oPayment.UserFields.Fields.Item("U_Status").Value = "N";
                    var ret1 = -1;
                    ret1 = oPayment.Add();
                    if (ret1 != 0)
                    {
                        DIConnection.Instance.Company.GetLastError(out lErrCode, out message);
                        result.Flag = false;
                        result.Message = message;// + " " + DIConnection.Instance.Company.GetLastErrorDescription(); ;
                        results.Add(result);

                        throw new Exception(string.Format("{0} {1}", "Tạo payment có lỗi", message));
                    }
                    else
                    {
                        string newId = string.Empty;
                        DIConnection.Instance.Company.GetNewObjectCode(out newId);
                        result.Flag = true;
                        result.Message = $"Tạo thành đơn id {newId}";
                        results.Add(result);
                    }
                }
                DIConnection.Instance.Company.EndTransaction(BoWfTransOpt.wf_Commit);
            }
            catch (Exception ex)
            {
                message = string.Format("Error {0}", ex.Message);
                try
                {
                    DIConnection.Instance.Company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }
                catch (Exception)
                { }
                //results = null;
            }
            finally
            {
                DisConnectDI();
            }
            return results;
        }
        //public static List<PaymentApproveResult> ApprovePayment(List<int> datas, ApprovalStatus status, string remark, ref string message)
        //{
        //    if (!ConnectDI(ref message))
        //        return null;

        //    var results = new List<PaymentApproveResult>();
        //    var notice = string.Empty;
        //    if (status == ApprovalStatus.Generate)
        //        notice = "Tạo";
        //    else if (status == ApprovalStatus.Approve)
        //        notice = "Duyệt";
        //    else if (status == ApprovalStatus.Reject)
        //        notice = "Từ chối";
        //    else
        //        notice = "Chuyển về chờ duyệt";

        //    try
        //    {
        //        foreach (var docentry in datas)
        //        {
        //            PaymentApproveResult result = new PaymentApproveResult();

        //            result.DocNum = docentry.ToString();
        //            ApprovalRequestsService oApprovalRequestsService = (ApprovalRequestsService)DIConnection.Instance.Company.GetCompanyService().GetBusinessService(ServiceTypes.ApprovalRequestsService);

        //            var approvalRequestParams = (SAPbobsCOM.ApprovalRequestParams)oApprovalRequestsService.GetDataInterface(SAPbobsCOM.ApprovalRequestsServiceDataInterfaces.arsApprovalRequestParams);

        //            var code = string.Empty;
        //            Hashtable data;
        //            using (var connection = CoreSetting.DataConnection)
        //            {
        //                data = connection.ExecQueryToHashtable(string.Format(QueryString.GetApproveRequestCodeQuery, docentry.ToString()));
        //                connection.Dispose();
        //            }

        //            if (data != null)
        //            {
        //                code = data["WddCode"].ToString();
        //            }

        //            if (string.IsNullOrEmpty(code))
        //            {
        //                result.Flag = false;
        //                result.Message = $"Không tìm thấy mã chứng từ nháp để {notice}";
        //                results.Add(result);
        //                continue;
        //            }
        //            var intCode = int.Parse(code);
        //            approvalRequestParams.Code = intCode;
        //            var approvalRequest = oApprovalRequestsService.GetApprovalRequest(approvalRequestParams);

        //            if (status != ApprovalStatus.Generate && status != ApprovalStatus.Pending)
        //            {
        //                try
        //                {
        //                    approvalRequest.ApprovalRequestDecisions.Add();
        //                    approvalRequest.ApprovalRequestDecisions.Item(0).Remarks = remark;
        //                    var statusApp = BoApprovalRequestDecisionEnum.ardPending;
        //                    if (status == ApprovalStatus.Approve)
        //                        statusApp = BoApprovalRequestDecisionEnum.ardApproved;
        //                    else if (status == ApprovalStatus.Reject)
        //                        statusApp = BoApprovalRequestDecisionEnum.ardNotApproved;
        //                    approvalRequest.ApprovalRequestDecisions.Item(0).Status = statusApp;
        //                    oApprovalRequestsService.UpdateRequest(approvalRequest);
        //                    result.Flag = true;
        //                    result.Message = $"{notice} thành công";
        //                    results.Add(result);
        //                }
        //                catch (Exception ex)
        //                {
        //                    result.Flag = false;
        //                    result.Message = $"{notice} lỗi {ex.Message}";
        //                    results.Add(result);
        //                }
        //            }
        //            else if (status == ApprovalStatus.Generate)
        //            {
        //                try
        //                {
        //                    var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
        //                    if (oPayment.GetByKey(approvalRequest.DraftEntry))
        //                    {
        //                        var saveret = oPayment.SaveDraftToDocument();
        //                        if (saveret != 0)
        //                        {
        //                            var mess = DIConnection.Instance.Company.GetLastErrorDescription();
        //                            result.Flag = false;
        //                            result.Message = $"{notice} lỗi {mess}";
        //                            results.Add(result);
        //                        }
        //                        else
        //                        {
        //                            result.Flag = true;
        //                            result.Message = $"{notice} thành công";
        //                            results.Add(result);
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    result.Flag = false;
        //                    result.Message = $"{notice} lỗi {ex.Message}";
        //                    results.Add(result);
        //                }
        //            }
        //            else if (status == ApprovalStatus.Pending)
        //            {
        //                try
        //                {
        //                    approvalRequest.ApprovalRequestDecisions.Add();
        //                    approvalRequest.ApprovalRequestDecisions.Item(0).Remarks = remark;
        //                    var statusApp = BoApprovalRequestDecisionEnum.ardPending;

        //                    approvalRequest.ApprovalRequestDecisions.Item(0).Status = statusApp;
        //                    oApprovalRequestsService.UpdateRequest(approvalRequest);
        //                    result.Flag = true;
        //                    result.Message = $"{notice} thành công";
        //                    results.Add(result);

        //                    var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
        //                    if (oPayment.GetByKey(approvalRequest.DraftEntry))
        //                    {
        //                        oPayment.UserFields.Fields.Item("U_Review").Value = "Y";
        //                        var saveret = oPayment.Update();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    result.Flag = false;
        //                    result.Message = $"{notice} lỗi {ex.Message}";
        //                    results.Add(result);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        message = string.Format("Error {0}", ex.Message);
        //        results = null;
        //    }
        //    DisConnectDI();
        //    return results;
        //}
        public static List<PaymentApproveResult> ApprovePayment1(List<int> datas, ApprovalStatus status, string remark, ref string message)
        {
            if (!ConnectDI(ref message))
                return null;

            var results = new List<PaymentApproveResult>();
            var notice = string.Empty;
            if (status == ApprovalStatus.Generate)
                notice = "Tạo";
            else if (status == ApprovalStatus.Approve)
                notice = "Duyệt";
            else if (status == ApprovalStatus.Reject)
                notice = "Từ chối";
            else
                notice = "Chuyển về chờ duyệt";

            try
            {
                foreach (var docentry in datas)
                {
                    PaymentApproveResult result = new PaymentApproveResult();

                    result.DocNum = docentry.ToString();
                    if (status != ApprovalStatus.Generate && status != ApprovalStatus.Pending)
                    {
                        try
                        {
                            var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                            if (oPayment.GetByKey(docentry))
                            {
                                oPayment.UserFields.Fields.Item("U_Status").Value = status == ApprovalStatus.Approve ? "A" : "J";
                                var saveret = oPayment.Update();

                                result.Flag = true;
                                result.Message = $"{notice} thành công";
                                results.Add(result);

                            }
                        }
                        catch (Exception ex)
                        {
                            result.Flag = false;
                            result.Message = $"{notice} lỗi {ex.Message}";
                            results.Add(result);
                        }
                    }
                    else if (status == ApprovalStatus.Generate)
                    {
                        try
                        {
                            var oPaymntDraf = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                            var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oVendorPayments);
                            if (oPaymntDraf.GetByKey(docentry))
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
                                oPayment.UserFields.Fields.Item("U_BANK").Value = oPaymntDraf.UserFields.Fields.Item("U_BANK").Value;

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

                                oPayment.UserFields.Fields.Item("U_PayType").Value = oPaymntDraf.UserFields.Fields.Item("U_PayType").Value;
                                oPayment.UserFields.Fields.Item("U_Review").Value = "Y";
                                oPayment.UserFields.Fields.Item("U_PaymentKey").Value = oPaymntDraf.UserFields.Fields.Item("U_PaymentKey").Value;

                                oPayment.UserFields.Fields.Item("U_Status").Value = "S";

                                var befStatus = oPaymntDraf.UserFields.Fields.Item("U_Status").Value;
                                oPaymntDraf.UserFields.Fields.Item("U_Status").Value = "S";
                                var ret1 = oPaymntDraf.Update();
                                //var ret1 = -1;
                                if (ret1 == 0)
                                {
                                    var saveret = oPayment.Add();

                                    // var saveret = oPaymntDraf.SaveDraftToDocument();
                                    if (saveret != 0)
                                    {
                                        var mess = DIConnection.Instance.Company.GetLastErrorDescription();
                                        result.Flag = false;
                                        result.Message = $"{notice} lỗi {mess}";
                                        results.Add(result);

                                        oPaymntDraf.UserFields.Fields.Item("U_Status").Value = befStatus;
                                        ret1 = oPaymntDraf.Update();
                                    }
                                    else
                                    {
                                        var code = string.Empty;
                                        DIConnection.Instance.Company.GetNewObjectCode(out code);
                                        result.Flag = true;
                                        result.Message = $"{notice} thành công với mã {code}";
                                        results.Add(result);

                                        //if (oPaymntDraf.GetByKey(docentry))
                                        //{
                                        //    oPaymntDraf.UserFields.Fields.Item("U_Status").Value = "S";
                                        //   // saveret = oPaymntDraf.Cancel();
                                        //    saveret = oPaymntDraf.Update();
                                        //}
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result.Flag = false;
                            result.Message = $"{notice} lỗi {ex.Message}";
                            results.Add(result);
                        }
                    }
                    else if (status == ApprovalStatus.Pending)
                    {
                        try
                        {
                            var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                            if (oPayment.GetByKey(docentry))
                            {
                                oPayment.UserFields.Fields.Item("U_Status").Value = "N";
                                var saveret = oPayment.Update();
                                result.Flag = true;
                                result.Message = $"{notice} thành công với ";
                                results.Add(result);

                            }
                        }
                        catch (Exception ex)
                        {
                            result.Flag = false;
                            result.Message = $"{notice} lỗi {ex.Message}";
                            results.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = string.Format("Error {0}", ex.Message);
                results = null;
            }
            DisConnectDI();
            return results;
        }
        public static bool ReviewPayment(int data, ref string message)
        {
            if (!ConnectDI(ref message))
                return false;

            var ret = false;

            try
            {
                var oPayment = (SAPbobsCOM.Payments)DIConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                if (oPayment.GetByKey(data))
                {
                    //oPayment.UserFields.Fields.Item("U_Review").Value = "Y";
                    oPayment.UserFields.Fields.Item("U_Status").Value = "R";

                    var saveret = oPayment.Update();
                    if (saveret != 0)
                    {
                        message = DIConnection.Instance.Company.GetLastErrorDescription();
                        ret = false;
                    }
                    else
                    {
                        message = STRING_CONTRANTS.Notice_UpdatePaymentSuccess;
                        ret = true;
                    }
                }
                else
                {
                    ret = false;
                    message = STRING_CONTRANTS.Notice_CanNotloadDraft;
                }

            }
            catch (Exception ex)
            {
                ret = false;
                message = ex.Message;
            }
            DisConnectDI();
            return ret;
        }
    }
}
