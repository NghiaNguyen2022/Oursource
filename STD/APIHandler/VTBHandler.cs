using APIHandler.Models;
using RestSharp;
using System;
using System.Linq;
using System.Text.Json;
using static APIHandler.Constant;

namespace APIHandler
{
    public class VTBHandler
    {
        public static InquiryHeader CallVTBAPI(string account, string fromDate, string toDate, ref string message)
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
                    fromTime = "00:00:00",
                    toTime = DateTime.Now.ToString("HH:mm:ss")
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

                message = "Hoàn tất gửi thông tin";
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
