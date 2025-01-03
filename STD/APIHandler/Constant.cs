using System.Configuration;

namespace APIHandler
{
    public class Constant
    {
        public static string Schema = ConfigurationManager.AppSettings["Schema"];
        public class APIVietinBankConstrant
        {
            //public static string APIVTB = "https://api-uat.vietinbank.vn";
            //public static string UAT_VTB = "/vtb-api-uat/development/erp/v1";
            //public static string InquiryVTB = $"{UAT_VTB}/statement/inquiry";
            //public static string RateVTB = $"vtb-api-uat/development/v1/fxrate/ForeignExchangeRate_Inq";
            //public static string TransferVTB = $"{UAT_VTB}//payment/transfer";
            //public static string TransferInqVTB = $"{UAT_VTB}//payment/transferInq";

            //public static string ClientID = $"fbbf1989a3ad0de68446317f5f104df0";
            //public static string ClientSecret = $"2cd7b943f4d7c5115d44b81487497ae3";
            //public static string AccountRecv = $"108004261279";
            //public static string Account = $"112000002609";
            //public static string MerchantId = "";
            //public static string ProviderId = "PYMEPHARCO";

            public static string APIVTB
            {
                get
                {
                    return Configs.AddonConfigurations["APIVTB"];
                }
            } // = "https://api-uat.vietinbank.vn";
              //public static string UAT_VTB
              //{
              //} = "/vtb-api-uat/development/erp/v1";
            public static string InquiryVTB
            {
                get
                {
                    return Configs.AddonConfigurations["InquiryVTB"];
                }
            }// = $"{UAT_VTB}/statement/inquiry";
             //public static string RateVTB = $"vtb-api-uat/development/v1/fxrate/ForeignExchangeRate_Inq";
             //public static string TransferVTB = $"{UAT_VTB}//payment/transfer";
             //public static string TransferInqVTB = $"{UAT_VTB}//payment/transferInq";

            //public static string ClientID = $"fbbf1989a3ad0de68446317f5f104df0";
            //public static string ClientSecret = $"2cd7b943f4d7c5115d44b81487497ae3";
            //public static string AccountRecv = $"108004261279";
            //public static string Account = $"112000002609";

            public static string RateVTB
            {
                get
                {
                    return Configs.AddonConfigurations["RateVTB"];
                }
            }// = $"vtb-api-uat/development/v1/fxrate/ForeignExchangeRate_Inq";
            public static string TransferVTB
            {
                get
                {
                    return Configs.AddonConfigurations["TransferVTB"];
                }
            }// = $"{UAT_VTB}//payment/transfer";
            public static string TransferInqVTB
            {
                get
                {
                    return Configs.AddonConfigurations["TransferInqVTB"];
                }
            }// = $"{UAT_VTB}//payment/transferInq";

            public static string ClientID
            {
                get
                {
                    return Configs.AddonConfigurations["ClientIDVTB"];
                }
            }//            = $"fbbf1989a3ad0de68446317f5f104df0";
            public static string ClientSecret
            {
                get
                {
                    return Configs.AddonConfigurations["ClientSecretVTB"];
                }
            }// = $"2cd7b943f4d7c5115d44b81487497ae3";

            public static string USER_APPROVE
            {
                get
                {
                    return Configs.AddonConfigurations["USER_APPROVE"];
                }
            }
            public static string USER_CREATE
            {
                get
                {
                    return Configs.AddonConfigurations["USER_CREATE"];
                }
            }
            public static string ProviderId
            {
                get
                {
                    return Configs.AddonConfigurations["ProviderId"];
                }
            }
            public static string MerchantId
            {
                get
                {
                    return Configs.AddonConfigurations["MerchantId"];
                }
            }
            public static string ClientIP
            {
                get
                {
                    return Configs.AddonConfigurations["ClientIP"];
                }
            }
        }

        public class APIPayooConstant
        {
            public static string APILink
            {
                get
                {
                    return Configs.AddonConfigurations["APILinkPY"];
                }
            }// = "https://bizsandbox.payoo.com.vn";
            public static string SettlementTransactionsLink
            {
                get
                {
                    return Configs.AddonConfigurations["SettlementTransactionsLink"];
                }
            }// = "/BusinessRestAPI.svc/GetSettlementTransactions";
            public static string APIUsername
            {
                get
                {
                    return Configs.AddonConfigurations["APIUsernamePY"];
                }
            }// = "SB_Stada_BizAPI";
            public static string APIPassword
            {
                get
                {
                    return Configs.AddonConfigurations["APIPasswordPY"];
                }
            }// = "9qW0k9/5khmSqdOE";
            public static string APISignature
            {
                get
                {
                    return Configs.AddonConfigurations["APISignaturePY"];
                }
            }// = "U5E0ykWrsQSqkjS6xR+Qnj3cbuNUi8YgSC2r6/BcXrpdcjOx6XYh7XNjQf806Yay";

            public static string ChecksumKey
            {
                get
                {
                    return Configs.AddonConfigurations["ChecksumKeyPY"];
                }
            }// = "NzlkNTJmZDBhMjcyNDM0MjBkZWQ4NDI0ODNjYjY2YTI=";
        }
    }
}
