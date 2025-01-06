using APIHandler.Models;
using PN.SmartLib.Helper;
using RestSharp;
using STD.DataReader;
using System;
using System.Text.Json;
using System.Web.Script.Serialization;
using static APIHandler.Constant;

namespace APIHandler
{
    public class PayooHandler
    {
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
    }
}
