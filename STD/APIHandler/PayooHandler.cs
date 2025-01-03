using APIHandler.Models;
using PN.SmartLib.Helper;
using RestSharp;
using System;
using System.Linq;
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

                return rpDataTransaction;


            }
            catch (Exception ex)
            { }
            return null;
        }
    }
}
