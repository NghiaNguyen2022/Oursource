using System.Configuration;

namespace APIHandler
{
    public class Constant
    {
        public static string Schema = ConfigurationManager.AppSettings["Schema"];
        public class APIVietinBankConstrant
        {
            public static string APIVTB = "https://api-uat.vietinbank.vn";
            public static string UAT_VTB = "/vtb-api-uat/development/erp/v1";
            public static string InquiryVTB = $"{UAT_VTB}/statement/inquiry";
            public static string RateVTB = $"vtb-api-uat/development/v1/fxrate/ForeignExchangeRate_Inq";
            public static string TransferVTB = $"{UAT_VTB}//payment/transfer";
            public static string TransferInqVTB = $"{UAT_VTB}//payment/transferInq";

            public static string ClientID = $"fbbf1989a3ad0de68446317f5f104df0";
            public static string ClientSecret = $"2cd7b943f4d7c5115d44b81487497ae3";
            public static string AccountRecv = $"108004261279";
            public static string Account = $"112000002609";
            public static string MerchantId = "";
            public static string ProviderId = "PYMEPHARCO";
        }
    }
}
