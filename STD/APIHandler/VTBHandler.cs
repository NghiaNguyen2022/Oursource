using APIHandler.Models;
using RestSharp;
using STD.DataReader;
using System;
using System.Text.Json;
using static APIHandler.Constant;

namespace APIHandler
{
    public class VTBHandler
    {

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
    }
}
