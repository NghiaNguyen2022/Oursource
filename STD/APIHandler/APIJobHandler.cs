using APIHandler.Models;
using PN.SmartLib.Helper;
using RestSharp;
using SAPbobsCOM;
using SAPCore.SAP.DIAPI;
using SAPCore.SAP.DIAPI.Models;
using STD.DataReader;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json;
using System.Web.Script.Serialization;
using static APIHandler.Constant;

namespace APIHandler
{
    public class APIJobHandler
    {
        private static bool ReadConfigAndConnect(ref string message)
        {
            var query = ServiceQueryString.SAPConnection;
            var data = dbProvider.QuerySingle(query);
            if (data == null)
            {
                message = "Chưa cấu hình connection cho ERP sevice";
                return false;
            }

            var config = new ServiceConnection(data);
            var cont = DIServiceConnection.Instance.ConnectDI(config, ref message);

            if (!cont)
            {
                //message = "Không thể kết nối";
                return false;
            }

            return true;

        }

        public static bool CreateIncommingPayment(string BatchNo, ref string message)
        {
            var query = string.Format(QueryString.PaymentData, BatchNo);

            var datas = dbProvider.QueryList(query);
            var invoices = new List<InvoicePayment>();
            if (datas != null && datas.Count() > 0)
            {
                foreach (var itm in datas)
                {
                    var inv = new InvoicePayment();
                    inv.CardCode = itm["CardCode"].ToString();
                    inv.DocEntry = itm["DocEntry"].ToString();
                    inv.DocCur = itm["DocCur"].ToString();
                    inv.OrderNo = itm["OrderNo"].ToString();
                    inv.TransferAmount = itm["TransferAmount"].ToString();
                    invoices.Add(inv);
                }
            }
            if (invoices.Count <= 0)
            {
                message = "Không có dữ liệu";
            }

            if (!ReadConfigAndConnect(ref message))
                return false;
            int lErrCode = -1;
            var ret = false;
            foreach (var cardcode in invoices.Select(x => x.CardCode).Distinct())
            {
                lErrCode = -1;

                var invs = invoices.Where(x => x.CardCode == cardcode);
                var cardCode = cardcode;
                var currency = invs.Select(x => x.DocCur).Distinct().FirstOrDefault();

                SAPbobsCOM.Payments oPayment;
                oPayment = (SAPbobsCOM.Payments)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oIncomingPayments);

                oPayment.CardCode = cardCode;
                oPayment.DocCurrency = currency;
                oPayment.DocDate = DateTime.Now;
                var objectType = BoRcptInvTypes.it_Invoice;
                double total = 0;
                var docEntry = string.Empty;
                var i = 0;
                foreach (var item in invs)
                {
                    oPayment.Invoices.Add();
                    oPayment.Invoices.SetCurrentLine(i);
                    oPayment.Invoices.DocEntry = int.Parse(item.DocEntry);
                    oPayment.Invoices.InvoiceType = objectType;
                    oPayment.Invoices.SumApplied = double.Parse(item.TransferAmount);
                    total += oPayment.Invoices.SumApplied;
                    docEntry += item + ";";
                    i++;
                }
                oPayment.TransferSum = total;
                oPayment.TransferAccount = "112101";

                var name = cardCode;
                var wuer = "SELECT \"CardName\" FROM \"" + ConfigurationManager.AppSettings["Schema"].ToString() + "\".OCRD WHERE \"CardCode\" = '" + cardCode + "'";
                var hash = dbProvider.QuerySingle(wuer);
                if (hash != null)
                    name = hash["CardName"].ToString();
                oPayment.Remarks = "Khách hàng " + name + " chuyển tiền hóa đơn " + docEntry;

                var ret1 = -1;
                ret1 = oPayment.Add();

                var newId = string.Empty;
                if (ret1 != 0)
                {
                    DIServiceConnection.Instance.Company.GetLastError(out lErrCode, out message);
                    ret = false;
                }
                else
                {
                    DIServiceConnection.Instance.Company.GetNewObjectCode(out newId);
                    message = $"Tạo thành đơn id {newId}";
                    ret = true;
                }
                //message, UIHelper.MsgType.StatusBar, ret1 != 0);

                foreach (var item in invs)
                {
                    query = string.Format(QueryString.UpdateAfterClear,
                                            ret1 == 0 ? "Y" : "N",
                                            "",
                                            message,
                                            newId,
                                            BatchNo,
                                            item.OrderNo);
                    var retupdate = dbProvider.ExecuteNonQuery(query);
                }
            }
            DIServiceConnection.Instance.DIDisconnect();
            return ret;
        }
        public static bool Clear(string BatchNo, ref string message)
        {
            try
            {
                var query = string.Format(ServiceQueryString.PaymentByBatchClear, BatchNo);
                var hash = dbProvider.QuerySingle(query);
                if (hash == null)
                {
                    message = "Không có data để đối soát";
                    return false;
                }

                var diffStr = hash["Diff"].ToString();
                decimal diff = 0;
                if (decimal.TryParse(diffStr, out diff))
                {
                    if (diff == 0)
                    {
                        var ret = CreateIncommingPayment(BatchNo, ref message);

                    }
                    else
                    {
                        message = $"{hash["BatchNo"].ToString()} lệch {diff}";//, UIHelper.MsgType.StatusBar, true);

                        return false;
                    }
                }
            }
            catch (Exception exx)
            {
                message = exx.Message;
                return false;
            }
            return false;
        }
        public static InquiryHeader CallVTBAPI(string account, string fromDate, string toDate, string fromTime, string toTime, ref string message)
        {
            try
            {

                var options = new RestClientOptions(APIVietinBankConstrant.APIVTB)
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request = new RestRequest(APIVietinBankConstrant.InquiryVTB, Method.Post);
                request.AddHeader("x-ibm-client-id", APIVietinBankConstrant.ClientID);
                request.AddHeader("x-ibm-client-secret", APIVietinBankConstrant.ClientSecret);
                request.AddHeader("Content-Type", "application/json");

                var data = new InquiryRequest()
                {
                    requestId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    merchantId = APIVietinBankConstrant.MerchantId,
                    providerId = APIVietinBankConstrant.ProviderId,
                    model = "2",
                    account = account,
                    fromDate = DateTime.ParseExact(fromDate, "yyyyMMdd", null).ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
                    toDate = DateTime.ParseExact(toDate, "yyyyMMdd", null).ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
                    accountType = "D",
                    collectionType = "c,d",
                    agencyType = "a",
                    transTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    channel = "ERP",
                    version = "1",
                    clientIP = "",
                    language = "vi",
                    signature = "", // Giá trị rỗng
                    fromTime = fromTime,
                    toTime = toTime
                };

                data.signature = (
                  data.requestId +
                  data.providerId +
                  data.merchantId +
                  data.account
                  );

                var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

                data.signature = FPT.SHA256_RSA2048.Encrypt(data.signature, path);
                var json = JsonSerializer.Serialize(data);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    message = $"Lỗi {response.ErrorMessage}";
                    return null;
                }
                var result = response.Content;

                var rps = JsonSerializer.Deserialize<VTResponse>(result);
                if (rps == null)
                {
                    message = $"Lỗi không có phản hồi, vui lòng check lại";
                    return null;
                }
                if (rps.status.code == "0")
                {
                    message = $"Lỗi {rps.status.message}";
                    return null;
                }

                var inquiry = JsonSerializer.Deserialize<InquiryHeader>(result);

                if (inquiry.transactions.Count > 0)
                {
                    foreach (var item in inquiry.transactions)
                    {
                        var sqlCheckExist = string.Format(QueryString.CheckInquiryExist, item.transactionNumber);
                        var data1 = dbProvider.QuerySingle(sqlCheckExist);
                        if (data1 != null && data1["Existed"].ToString() != "Existed")
                        {
                            item.InsertData(data.requestId, data.providerId, data.merchantId.ToString());
                        }

                    }
                    message = "Hoàn tất gửi thông tin";
                }
                else
                {
                    message = "Không có dữ liệu";
                }
                return inquiry;
            }
            catch (Exception ex)
            {
                message = $"Lỗi {ex.Message}";
                return null;
            }
        }

        public static PayooResponseDataExt CallPayooAPI(string TransDate, int BatchNo, ref string message)
        {

            try
            {
                var options = new RestClientOptions(APIPayooConstant.APILink)
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request = new RestRequest(APIPayooConstant.SettlementTransactionsLink, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("APIUsername", APIPayooConstant.APIUsername);
                request.AddHeader("APIPassword", APIPayooConstant.APIPassword);
                request.AddHeader("APISignature", APIPayooConstant.APISignature);
                request.AddHeader("Accept", "application/json");

                var transDate = DateTime.ParseExact(TransDate, "yyyyMMdd", null).ToString("yyyyMMdd");

                var obj = new PayooGetSettlement
                {
                    BatchNumber = BatchNo,
                    PageNumber = 1,
                    SettlementDate = transDate
                };
                var objJson = new JavaScriptSerializer();
                var objBody = objJson.Serialize(obj);

                var oRequest = new PayoRequest();
                oRequest.RequestData = objBody;
                // sign data
                oRequest.SecureHash = APIUtil.EncryptSHA512(APIPayooConstant.ChecksumKey + oRequest.RequestData);
                var objJsonFinal = new JavaScriptSerializer();
                string temp = objJsonFinal.Serialize(oRequest);

                request.AddParameter("application/json", temp, ParameterType.RequestBody);


                var response = client.Execute(request);

                var result = response.Content;

                var resp = JsonSerializer.Deserialize<PayooResponse>(result);
                if (resp == null)
                {
                    message = $"Lỗi không có phản hồi, vui lòng check lại";
                    return null;
                }
                var respDataStr = resp.ResponseData.Replace(@"\", "");
                var respDataObj = JsonSerializer.Deserialize<PayooResponseData>(respDataStr);
                if (respDataObj == null)
                {
                    message = $"Lỗi không có phản hồi, vui lòng check lại";
                    return null;
                }

                PayooResponseCode rpcode = (PayooResponseCode)CoreExtensions.GetEnumValue<PayooResponseCode>(respDataObj.ResponseCode);
                if (rpcode != PayooResponseCode.Res0)
                {
                    message = rpcode.GetDescription();
                    return null;
                }
                var rpDataTransaction = JsonSerializer.Deserialize<PayooResponseDataExt>(respDataStr);

                if (rpDataTransaction != null)
                {
                    var sqlCheckExist = string.Format(QueryString.CheckBatchExists, rpDataTransaction.BatchNo);
                    var data1 = dbProvider.QuerySingle(sqlCheckExist);
                    if (data1 != null && data1["Existed"].ToString() != "Existed")
                    {


                        var insertHd = "INSERT INTO \"" + Constant.Schema + "\".\"tb_Payoo_BatchHeader\" VALUES ( ";
                        insertHd += $"'{rpDataTransaction.BatchNo}',";
                        insertHd += $"{rpDataTransaction.TotalSettlementAmount},";
                        insertHd += $"{rpDataTransaction.TotalSettlementRowCount},";
                        insertHd += $"{rpDataTransaction.PageSize}";
                        insertHd += ")";
                        var retHeader = dbProvider.ExecuteNonQuery(insertHd);

                        foreach (var data in rpDataTransaction.TransactionList)
                        {
                            var insertdt = "INSERT INTO \"" + Constant.Schema + "\".\"tb_Payoo_BatchDetail\" VALUES ( ";
                            insertdt += $"'{rpDataTransaction.BatchNo}',";
                            insertdt += $"'{data.OrderNo}',";
                            insertdt += $"'{data.ShopId}',";
                            insertdt += $"'{data.SellerName}',";
                            insertdt += $"{data.MoneyAmount},";
                            insertdt += $"'{data.PurchaseDate}',";
                            insertdt += $"'{data.Status}',";
                            insertdt += $"'{DateTime.Now.ToString("yyyyMMdd")}',";
                            insertdt += $"'{DateTime.Now.ToString("HHmmss")}',";
                            insertdt += $"'N',";
                            insertdt += $"'', ";
                            insertdt += $"'', '', ''";
                            insertdt += ")";
                            var retDetail = dbProvider.ExecuteNonQuery(insertdt);
                        }
                        message = "Hoàn tất gửi thông tin";
                    }
                }
                else
                {

                }
                return rpDataTransaction;


            }
            catch (Exception ex)
            { }
            return null;
        }


        public static bool Test(int DocEntry, ref string message)
        {
            if (!ReadConfigAndConnect(ref message))
                return false;
            int lErrCode = -1;
            var ret1 = false;
            try
            {
                SAPbobsCOM.Documents oInvoice = (SAPbobsCOM.Documents)DIServiceConnection.Instance.Company.GetBusinessObject(BoObjectTypes.oInvoices);

                if (oInvoice.GetByKey(DocEntry))
                {
                    //Create an object which represent to a new cancellation document based on doc
                    Documents cancelDoc = oInvoice.CreateCancellationDocument();

                    //We can modify some values in the cancellation document
                    cancelDoc.DocDate = new DateTime(2012, 4, 8);

                    //Then we can add this cancellation document, and at the same time the status of the base document will be changed into ‘canceled’
                    cancelDoc.Add();

                    var ret = cancelDoc.Add();
                    if (ret == 0)
                    {
                        message = "Cancel success";
                        ret1 = true;
                    }
                    else
                    {
                        message = DIServiceConnection.Instance.Company.GetLastErrorDescription();
                        ret1 = false;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                ret1 = false;
            }
            DIServiceConnection.Instance.DIDisconnect();
            return ret1;
        }
    }
}
