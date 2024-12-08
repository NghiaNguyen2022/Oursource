using Jose;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using PN.SmartLib.Helper;
using RestSharp;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.Config;
using SAPCore.Helper;
using STD.DataReader;
using STDApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace STDApp.Bank
{
    [FormAttribute("STDApp.Bank.frmInquiry", "Bank/frmInquiry.b1f")]
    class frmInquiry : UserFormBase
    {
        public frmInquiry()
        {
        }

        private SAPbouiCOM.DataTable DT_Header_VT;
        private SAPbouiCOM.DataTable DT_Header_BI;
        private SAPbouiCOM.DataTable DT_Detail_VT;
        private SAPbouiCOM.DataTable DT_Detail_BI;

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblBank = ((SAPbouiCOM.StaticText)(this.GetItem("lblBank").Specific));
            this.cbbBank = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBank").Specific));
            this.cbbBank.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbBank_ComboSelectAfter);
            this.lblAcc = ((SAPbouiCOM.StaticText)(this.GetItem("lblAcc").Specific));
            this.cbbAcc = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbAcc").Specific));
            this.lblFDa = ((SAPbouiCOM.StaticText)(this.GetItem("lblFDa").Specific));
            this.lblTDa = ((SAPbouiCOM.StaticText)(this.GetItem("lblTDa").Specific));
            this.txtFDate = ((SAPbouiCOM.EditText)(this.GetItem("txtFDa").Specific));
            this.txtTDate = ((SAPbouiCOM.EditText)(this.GetItem("txtTDa").Specific));
            this.btnLoad = ((SAPbouiCOM.Button)(this.GetItem("btnLoad").Specific));
            this.btnLoad.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnLoad_ClickBefore);
            this.grHdr = ((SAPbouiCOM.Grid)(this.GetItem("grHdr").Specific));
            this.grDt = ((SAPbouiCOM.Grid)(this.GetItem("grDt").Specific));
            this.btnClear = ((SAPbouiCOM.Button)(this.GetItem("btnClear").Specific));
            this.btnClear.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnClear_ClickBefore);
            this.DT_Header_VT = this.UIAPIRawForm.DataSources.DataTables.Item("DT_Hd");
            this.DT_Header_BI = this.UIAPIRawForm.DataSources.DataTables.Item("DT_hd1");
            this.DT_Detail_VT = this.UIAPIRawForm.DataSources.DataTables.Item("DT_Dt");
            this.DT_Detail_BI = this.UIAPIRawForm.DataSources.DataTables.Item("DT_dt1");
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(this.Form_CloseAfter);
            this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);

        }

        private SAPbouiCOM.StaticText lblBank;

        private void OnCustomInitialize()
        {

        }

        private SAPbouiCOM.ComboBox cbbBank;
        private SAPbouiCOM.StaticText lblAcc;
        private SAPbouiCOM.ComboBox cbbAcc;
        private SAPbouiCOM.StaticText lblFDa;
        private SAPbouiCOM.StaticText lblTDa;
        private SAPbouiCOM.EditText txtFDate;
        private SAPbouiCOM.EditText txtTDate;
        private SAPbouiCOM.Button btnLoad;
        private SAPbouiCOM.Grid grHdr;

        public static bool IsFormOpen = false;
        private static frmInquiry instance;

        private string FromDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtFDate);
            }
        }

        private string ToDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtTDate);
            }
        }

        private string Bank
        {
            get
            {
                return UIHelper.GetComboValue(cbbBank);
            }
        }
        private string Account
        {
            get
            {
                return UIHelper.GetComboValue(cbbAcc);
            }
        }
        public static void ShowForm()
        {
            if (instance == null)
            {
                try
                {
                    instance = new frmInquiry();
                    instance.InitControl();
                    instance.Show();
                    IsFormOpen = true;
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void InitControl()
        {
            SetLocation();

            var account = ConfigurationManager.AppSettings["Account"];
            UIHelper.ComboboxSelectDefault(cbbBank);

            var data = DataProvider.QuerySingle(CoreSetting.DataConnection, string.Format(QueryString.BankLoad, Bank));
            if (data != null)
            {
                UIHelper.ClearSelectValidValues(cbbAcc);
                this.cbbAcc.ValidValues.Add(data["Account"].ToString(), data["Account"].ToString());
                UIHelper.ComboboxSelectDefault(cbbAcc);
            }

        }

        private void SetLocation()
        {
            var max = this.UIAPIRawForm.ClientHeight;
            var maxw = this.UIAPIRawForm.ClientWidth;

            this.lblBank.Item.Top = 15;
            this.lblBank.Item.Left = CoreSetting.UF_HorMargin;

            this.cbbBank.Item.Top = this.lblBank.Item.Top;
            this.cbbBank.Item.Left = this.lblBank.Item.Left + this.lblBank.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblAcc.Item.Top = this.lblBank.Item.Top;
            this.lblAcc.Item.Left = this.cbbBank.Item.Left + this.cbbBank.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.cbbAcc.Item.Top = this.lblBank.Item.Top;
            this.cbbAcc.Item.Left = this.lblAcc.Item.Left + this.lblAcc.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblFDa.Item.Top = this.lblBank.Item.Top;
            this.lblFDa.Item.Left = this.cbbAcc.Item.Left + this.cbbAcc.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.txtFDate.Item.Top = this.lblBank.Item.Top;
            this.txtFDate.Item.Left = this.lblFDa.Item.Left + this.lblFDa.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblTDa.Item.Top = this.lblBank.Item.Top;
            this.lblTDa.Item.Left = this.txtFDate.Item.Left + this.txtFDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.txtTDate.Item.Top = this.lblBank.Item.Top;
            this.txtTDate.Item.Left = this.lblTDa.Item.Left + this.lblTDa.Item.Width + CoreSetting.UF_HorizontallySpaced;

            var labBottom = this.lblBank.Item.Top + this.lblBank.Item.Height;
            var bttTop = labBottom - this.btnLoad.Item.Height;
            this.btnLoad.Item.Top = bttTop;
            this.btnLoad.Item.Left = this.txtTDate.Item.Left + this.txtTDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.btnClear.Item.Top = bttTop;
            this.btnClear.Item.Left = this.btnLoad.Item.Left + this.btnLoad.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.grHdr.Item.Left = this.lblBank.Item.Left;
            this.grHdr.Item.Width = maxw - grHdr.Item.Left - 40;
            this.grHdr.Item.Top = lblBank.Item.Top + lblBank.Item.Height + 20;
            var bodyHeight = max - grHdr.Item.Top - 20;
            var headeHeight = (bodyHeight - 10) / 2;
            this.grHdr.Item.Height = headeHeight;

            this.grDt.Item.Left = this.lblBank.Item.Left;
            this.grDt.Item.Width = maxw - grDt.Item.Left - 40;
            this.grDt.Item.Top = this.grHdr.Item.Top + this.grHdr.Item.Height + 10;
            this.grDt.Item.Height = max - grDt.Item.Top - 20;
        }

        private SAPbouiCOM.Grid grDt;

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            instance = null;
            IsFormOpen = false;
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            SetLocation();
            this.Freeze(false);
        }

        private void btnLoad_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            if (string.IsNullOrEmpty(FromDate) || string.IsNullOrEmpty(ToDate))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_DateSelectNull, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            if (!StringUtils.CheckFromDateEarlyToDate(FromDate, ToDate))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_FromDateEarlyToDate, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            this.Clear();
            if (Bank == Banks.ViettinBank.GetDescription())
            {
                this.CallVTBAPI();
            }
            else
            {
                this.CallBIDVAPI();
            }
            this.Freeze(false);
        }

        private string GetToken()
        {
            try
            {
                var query = string.Format(QueryString.GetAPICode);
                var data = DataProvider.QuerySingle(CoreSetting.DataConnection, query);
                if (data == null)
                {
                    return string.Empty;
                }
                var code = data["Code"].ToString();
                if (string.IsNullOrEmpty(code))
                {
                    return string.Empty;
                }

                var options = new RestClientOptions(ConfigurationManager.AppSettings["LinkBIDVAPI"])
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest(ConfigurationManager.AppSettings["AuthenBIDV"], Method.Post);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("client_id", ConfigurationManager.AppSettings["ClientIDBIDV"]);
                request.AddParameter("client_secret", ConfigurationManager.AppSettings["ClientSecretBIDV"]);
                request.AddParameter("code", code);
                request.AddParameter("redirect_uri", ConfigurationManager.AppSettings["redirect_uri"]);

                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var error = JsonSerializer.Deserialize<BIDVAccesstokenResponseErrro>(response.Content);
                    if (error.error_description.Contains("code expired"))
                    {
                        UIHelper.LogMessage($"Code để lấy token đã bị hết hạn, vui lòng lấy lại code mới và thử lại", UIHelper.MsgType.StatusBar, true);
                    }
                    else
                    {
                        UIHelper.LogMessage($"Lỗi {error.error_description}", UIHelper.MsgType.StatusBar, true);
                    }
                }
                else
                {
                    var result = JsonSerializer.Deserialize<BIDVAccesstokenResponse>(response.Content);
                    if (result != null)
                        return result.access_token;
                }
            }
            catch (Exception ex)
            { }
            return string.Empty;
        }
        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
        private static RSAParameters ConvertToRSAParameters(RsaPrivateCrtKeyParameters rsaKey)
        {
            RSAParameters result = default(RSAParameters);
            result.Modulus = rsaKey.Modulus.ToByteArrayUnsigned();
            result.Exponent = rsaKey.PublicExponent.ToByteArrayUnsigned();
            result.D = rsaKey.Exponent.ToByteArrayUnsigned();
            result.P = rsaKey.P.ToByteArrayUnsigned();
            result.Q = rsaKey.Q.ToByteArrayUnsigned();
            result.DP = rsaKey.DP.ToByteArrayUnsigned();
            result.DQ = rsaKey.DQ.ToByteArrayUnsigned();
            result.InverseQ = rsaKey.QInv.ToByteArrayUnsigned();
            return result;
        }
        private RSA LoadPrivateKey(string filePath)
        {
            try
            {
                string text = File.ReadAllText(filePath);
                text = text.Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "").Replace("\n", "")
                    .Replace("\r", "")
                    .Trim();
                var obj = Convert.FromBase64String(text);
                var instance = PrivateKeyInfo.GetInstance(obj);
                var asymmetricKeyParameter = PrivateKeyFactory.CreateKey(instance);
                if (asymmetricKeyParameter is RsaPrivateCrtKeyParameters rsaKey)
                {
                    RSA rSA = RSA.Create();
                    rSA.ImportParameters(ConvertToRSAParameters(rsaKey));
                    return rSA;
                }

                throw new ArgumentException("Key is not an RSA private key.");
            }
            catch (Exception innerException)
            {
                throw new Exception("Error loading private key", innerException);
            }
        }
        static byte[] SignData(RSA rsa, string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        private string GenSignature()
        {

            // Header (base64 encoded JSON)
            var header = new
            {
                alg = "RS256", // RSA with SHA-256
                typ = "JWT"
            };
            var headerJson = JsonSerializer.Serialize(header);
            var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var detachedPayload = string.Empty;

            var signingInput = $"{headerBase64}.{detachedPayload}";
            var certPath = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

            var rsa = LoadPrivateKey(certPath);
            var signature = SignData(rsa, signingInput);

            var signatureBase64 = Base64UrlEncode(signature);
            var jws = $"{headerBase64}.{detachedPayload}.{signatureBase64}";
            return jws;
            // Console.WriteLine("Detached JWS:");
            //Console.WriteLine(jws);

            // Example: Verification
            //bool isValid = VerifySignature(rsa, signingInput, signature);
            //Console.WriteLine("Signature valid: " + isValid);
        }
        private void CallBIDVAPI()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token))
            {
                UIHelper.LogMessage($"Không lấy được token từ phía ngân hàng, vui lòng thử lại",
                    UIHelper.MsgType.StatusBar, true);
                return;
            }
            var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";
            var certificate = string.Empty;
            try
            {
                UIHelper.LogMessage($"Bắt đầu vấn gửi yêu cầu tin tài khoản đến ngân hàng",
                    UIHelper.MsgType.StatusBar, false);
                certificate = File.ReadAllText(path);
                if (string.IsNullOrEmpty(certificate))
                {
                    UIHelper.LogMessage($"Không đọc được certificate, vui lòng kiểm tra lại",
                        UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var options = new RestClientOptions(ConfigurationManager.AppSettings["LinkBIDVAPI"]);
                var client = new RestClient(options);
                var request = new RestRequest(ConfigurationManager.AppSettings["InquiryBIDV"].ToString() + "?Scope=read&JWE=Yes", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-API-Interaction-ID", DateTime.Now.ToString("yyyyMMddHHmmss"));
                request.AddHeader("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssffff"));
                request.AddHeader("Channel", ConfigurationManager.AppSettings["ChannelBIDVAPI"]);
                request.AddHeader("User-Agent", ConfigurationManager.AppSettings["UserAgentIDVAPI"]);
                request.AddHeader("accept", "application/json");
                //OPTION: request.AddHeader("X-Idempotency-Key", "REPLACE_THIS_VALUE");
                //OPTION: request.AddHeader("X-Customer-IP-Address", "REPLACE_THIS_VALUE");
                //request.AddStringBody("REPLACE_BODY", "text/plain");

                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddHeader("X-Client-Certificate", certificate);

                var signature = GenSignature();
                if (string.IsNullOrEmpty(signature))
                {
                    UIHelper.LogMessage($"Không tạo được chữ ký, vui lòng kiểm tra lại",
                       UIHelper.MsgType.StatusBar, true);
                    return;
                }
                request.AddHeader("X-JWS-Signature", signature);

                var requestData = new BIDVInquiryRequest()
                {
                    actNumber = Bank,// "12010002159887",
                    curr = "VND",
                    fromDate = "20240901",
                    toDate = "20240930",
                    page = "1"
                };
                var symmetricKey = ConfigurationManager.AppSettings["SymmetricKey"];
                var symmetricKeys = HexToByteArray(symmetricKey);
                var json = JsonSerializer.Serialize(requestData);
                var encryptData = Jose.JWT.Encode(json, symmetricKeys, JweAlgorithm.A256KW, JweEncryption.A256GCM);
                var parts = encryptData.Split('.');
                var jweGeneralJson = new
                {
                    recipients = new List<object>
                    {
                        new
                        {
                            header = new { }, // Empty header for single recipient
                            encrypted_key = parts[1] // Second part of JWE
                        }
                    },
                    protectedField = parts[0],  // First part of JWE
                    ciphertext = parts[3],      // Fourth part of JWE
                    iv = parts[2],              // Third part of JWE
                    tag = parts[4]              // Fifth part of JWE
                };
                var jweJson = JsonSerializer.Serialize(jweGeneralJson);

                //request.AddParameter("application/json", json, ParameterType.RequestBody);
                request.AddStringBody(json, DataFormat.Json);
                var response = client.Execute(request);
                //var result = response.Content;

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var status = response.StatusCode;
                    if (status == System.Net.HttpStatusCode.InternalServerError)
                    {
                        UIHelper.LogMessage($"Lỗi kết nối. Vui lòng thử lại", UIHelper.MsgType.StatusBar, true);
                        return;
                    }
                    if (status == System.Net.HttpStatusCode.Unauthorized)
                    {
                        UIHelper.LogMessage($"Lỗi chứng thực. Vui lòng kiểm tra thông tin chứng thực và kết nối", UIHelper.MsgType.StatusBar, true);
                        return;
                    }
                    if (status == System.Net.HttpStatusCode.BadRequest)
                    {
                        UIHelper.LogMessage($"Gửi yêu cầu thất bại, vui lòng thử lại", UIHelper.MsgType.StatusBar, true);
                        return;
                    }

                    UIHelper.LogMessage($"Lỗi {response.ErrorMessage}", UIHelper.MsgType.StatusBar, true);
                    return;
                }

                var result = response.Content;

                var rps = JsonSerializer.Deserialize<BIDVInquiryResponse>(result);
                if (rps == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                if (rps.body.totalRecords <= 0)
                {
                    UIHelper.LogMessage($"Không có giao dịch", UIHelper.MsgType.Msgbox);
                    return;
                }
                var data = rps.body;

                if (this.grHdr != null && this.grHdr.DataTable != null)
                {
                    this.grHdr.DataTable.Rows.Add();
                    var index = grHdr.DataTable.Rows.Count - 1;
                    this.grHdr.DataTable.SetValue("BankAccount", index, Bank);
                    this.grHdr.DataTable.SetValue("Currency", index, "VND");
                    this.grHdr.DataTable.SetValue("BankName", index, "");
                    this.grHdr.DataTable.SetValue("OpeningBalance", index, data.startingBal);
                    this.grHdr.DataTable.SetValue("ClosingBalance", index, data.endingBal);
                    this.grHdr.AutoResizeColumns();
                }

                if (this.grDt != null && this.grDt.DataTable != null)
                {
                    foreach (var item in data.trans)
                    {
                        this.grDt.DataTable.Rows.Add();
                        var index = grDt.DataTable.Rows.Count - 1;
                        this.grDt.DataTable.SetValue("TransDate", index, item.tranDate);
                        this.grDt.DataTable.SetValue("Description", index, item.remark);
                        this.grDt.DataTable.SetValue("Debit", index, item.debitAmount ?? "0");
                        this.grDt.DataTable.SetValue("Credit", index, item.creditAmount ?? "0");
                        this.grDt.DataTable.SetValue("TransNo", index, item.seq);
                        this.grDt.DataTable.SetValue("Ref", index, item.@ref);
                        this.grDt.DataTable.SetValue("Currency", index, item.currCode);
                    }
                    this.grDt.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            { }
        }

        private byte[] HexToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length.");

            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                result[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return result;
        }

        private void CallVTBAPI()
        {
            try
            {
                //if (Bank == Banks.ViettinBank.GetDescription())
                //{
                UIHelper.LogMessage($"Bắt đầu vấn gửi yêu cầu tin tài khoản đến ngân hàng", UIHelper.MsgType.StatusBar, false);

                var options = new RestClientOptions(ConfigurationManager.AppSettings["LinkAPI"])
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request = new RestRequest(ConfigurationManager.AppSettings["Inquiry"], Method.Post);
                request.AddHeader("x-ibm-client-id", ConfigurationManager.AppSettings["ClientID"]);
                request.AddHeader("x-ibm-client-secret", ConfigurationManager.AppSettings["ClientSecret"]);
                request.AddHeader("Content-Type", "application/json");

                var data = new InquiryRequest()
                {
                    requestId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    merchantId = ConfigurationManager.AppSettings["MerchantId"],
                    providerId = ConfigurationManager.AppSettings["ProviderId"],
                    model = "2",
                    account = cbbAcc.Value,
                    fromDate = DateTime.ParseExact(FromDate, "yyyyMMdd", null).ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
                    toDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null).ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
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

                //request.signature = FPT.SHA256_RSA2048.Encrypt(request.signature, path);
                data.signature = FPT.SHA256_RSA2048.Encrypt(data.signature, path);
                var json = JsonSerializer.Serialize(data);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                var response = client.Execute(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    UIHelper.LogMessage($"Lỗi {response.ErrorMessage}", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var result = response.Content;

                var rps = JsonSerializer.Deserialize<VTResponse>(result);
                if (rps == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                if (rps.status.code == "0")
                {
                    UIHelper.LogMessage($"Lỗi {rps.status.message}", UIHelper.MsgType.Msgbox);
                    return;
                }

                var inquiry = JsonSerializer.Deserialize<InquiryHeader>(result);

                if (inquiry != null)
                {
                    if (this.grHdr != null && this.grHdr.DataTable != null)
                    {
                        this.grHdr.DataTable.Rows.Add();
                        var index = grHdr.DataTable.Rows.Count - 1;
                        this.grHdr.DataTable.SetValue("BankAccount", index, inquiry.account);
                        this.grHdr.DataTable.SetValue("BankName", index, inquiry.companyName);
                        this.grHdr.DataTable.SetValue("AccountType", index, inquiry.accountType);
                        this.grHdr.DataTable.SetValue("Currency", index, inquiry.curency);
                        this.grHdr.DataTable.SetValue("AvaiBalance", index, inquiry.availableBal);
                        this.grHdr.DataTable.SetValue("OpeningBalance", index, inquiry.openningBal);
                        this.grHdr.DataTable.SetValue("ClosingBalance", index, inquiry.closingBal);
                        this.grHdr.DataTable.SetValue("FromDate", index, inquiry.fromDate);
                        this.grHdr.DataTable.SetValue("TotalCredit", index, inquiry.totalCredit);
                        this.grHdr.DataTable.SetValue("NoCreditTrans", index, inquiry.numberCreditTransaction);
                        this.grHdr.DataTable.SetValue("TotalDebit", index, inquiry.totalDebit);
                        this.grHdr.DataTable.SetValue("NoDebitTrans", index, inquiry.numberDebitTransaction);
                        this.grHdr.DataTable.SetValue("BankAccount", index, inquiry.account);
                        this.grHdr.DataTable.SetValue("FromTime", index, inquiry.fromTime);
                        this.grHdr.DataTable.SetValue("ToTime", index, inquiry.toTime);
                        this.grHdr.AutoResizeColumns();
                    }

                    if (this.grDt != null && this.grDt.DataTable != null)
                    {
                        foreach (var item in inquiry.transactions)
                        {
                            this.grDt.DataTable.Rows.Add();
                            var index = grDt.DataTable.Rows.Count - 1;
                            this.grDt.DataTable.SetValue("TransDate", index, item.transactionDate);
                            this.grDt.DataTable.SetValue("Description", index, item.transactionContent);
                            this.grDt.DataTable.SetValue("Debit", index, item.debit ?? "0");
                            this.grDt.DataTable.SetValue("Credit", index, item.credit ?? "0");
                            this.grDt.DataTable.SetValue("AccountBal", index, item.accountBal);
                            this.grDt.DataTable.SetValue("TransNo", index, item.transactionNumber);
                            this.grDt.DataTable.SetValue("SenderAccount", index, item.corresponsiveAccount);
                            this.grDt.DataTable.SetValue("SenderName", index, item.corresponsiveAccountName);
                            this.grDt.DataTable.SetValue("Agency", index, item.agency ?? "");
                            this.grDt.DataTable.SetValue("VirtualAccount", index, item.virtualAccount ?? "");
                            this.grDt.DataTable.SetValue("SenderBank", index, item.corresponsiveBankName ?? "");
                            this.grDt.DataTable.SetValue("SenderBankId", index, item.corresponsiveBankId ?? "");
                            this.grDt.DataTable.SetValue("Chanel", index, item.channel);
                        }
                        this.grDt.AutoResizeColumns();
                    }
                }
                // }
            }
            catch (Exception ex)
            { }
        }

        private SAPbouiCOM.Button btnClear;

        private void btnClear_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            this.Clear();
            this.Freeze(false);
        }
        private void Clear()
        {
            if (this.grHdr != null && this.grHdr.DataTable != null)
            {
                this.grHdr.DataTable.Rows.Clear();
            }
            if (this.grDt != null && this.grDt.DataTable != null)
            {
                this.grDt.DataTable.Rows.Clear();
            }
        }

        private void cbbBank_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if(this.grHdr == null || this.grDt == null)
            {
                return;
            }
            this.Freeze(true);

            if (Bank == Banks.ViettinBank.GetDescription())
            {
                this.grHdr.DataTable = DT_Header_VT;

                this.grHdr.Columns.Item("AccountType").TitleObject.Caption = "Loại tài khoản";
                this.grHdr.Columns.Item("AccountType").Editable = false;
                this.grHdr.Columns.Item("AccountType").Width = 100;

                this.grHdr.Columns.Item("Balance").TitleObject.Caption = "Số dư tài khoản";
                this.grHdr.Columns.Item("Balance").Editable = false;
                this.grHdr.Columns.Item("Balance").Width = 80;

                this.grHdr.Columns.Item("AvaiBalance").TitleObject.Caption = "Số dư khả dụng";
                this.grHdr.Columns.Item("AvaiBalance").Editable = false;
                this.grHdr.Columns.Item("AvaiBalance").Width = 80;

                this.grHdr.Columns.Item("FromDate").TitleObject.Caption = "Từ ngày";
                this.grHdr.Columns.Item("FromDate").Editable = false;
                this.grHdr.Columns.Item("FromDate").Width = 80;

                this.grHdr.Columns.Item("TotalCredit").TitleObject.Caption = "Tổng thu";
                this.grHdr.Columns.Item("TotalCredit").Editable = false;
                this.grHdr.Columns.Item("TotalCredit").Width = 120;

                this.grHdr.Columns.Item("NoCreditTrans").TitleObject.Caption = "Số giao dịch thu";
                this.grHdr.Columns.Item("NoCreditTrans").Editable = false;
                this.grHdr.Columns.Item("NoCreditTrans").Width = 100;

                this.grHdr.Columns.Item("ToDate").TitleObject.Caption = "Đến ngày";
                this.grHdr.Columns.Item("ToDate").Editable = false;
                this.grHdr.Columns.Item("ToDate").Width = 80;

                this.grHdr.Columns.Item("TotalDebit").TitleObject.Caption = "Tổng chi";
                this.grHdr.Columns.Item("TotalDebit").Editable = false;
                this.grHdr.Columns.Item("TotalDebit").Width = 120;

                this.grHdr.Columns.Item("NoDebitTrans").TitleObject.Caption = "Số giao dịch chi";
                this.grHdr.Columns.Item("NoDebitTrans").Editable = false;
                this.grHdr.Columns.Item("NoDebitTrans").Width = 100;

                this.grHdr.Columns.Item("FromTime").TitleObject.Caption = "Từ thời gian";
                this.grHdr.Columns.Item("FromTime").Editable = false;
                this.grHdr.Columns.Item("FromTime").Width = 80;

                this.grHdr.Columns.Item("ToTime").TitleObject.Caption = "Đến thời gian";
                this.grHdr.Columns.Item("ToTime").Editable = false;
                this.grHdr.Columns.Item("ToTime").Width = 80;

                this.grDt.DataTable = DT_Detail_VT;

                this.grDt.Columns.Item("AccountBal").TitleObject.Caption = "Số dư";
                this.grDt.Columns.Item("AccountBal").Editable = false;
                this.grDt.Columns.Item("AccountBal").Width = 150;

                this.grDt.Columns.Item("SenderAccount").TitleObject.Caption = "Tài khoản người chuyển";
                this.grDt.Columns.Item("SenderAccount").Editable = false;
                this.grDt.Columns.Item("SenderAccount").Width = 100;

                this.grDt.Columns.Item("SenderName").TitleObject.Caption = "Tên người chuyển";
                this.grDt.Columns.Item("SenderName").Editable = false;
                this.grDt.Columns.Item("SenderName").Width = 150;

                this.grDt.Columns.Item("Agency").TitleObject.Caption = "Đại lý";
                this.grDt.Columns.Item("Agency").Editable = false;
                this.grDt.Columns.Item("Agency").Width = 100;

                this.grDt.Columns.Item("VirtualAccount").TitleObject.Caption = "Tài khoản ảo";
                this.grDt.Columns.Item("VirtualAccount").Editable = false;
                this.grDt.Columns.Item("VirtualAccount").Width = 100;

                this.grDt.Columns.Item("SenderBank").TitleObject.Caption = "Tên ngân hàng chuyển";
                this.grDt.Columns.Item("SenderBank").Editable = false;
                this.grDt.Columns.Item("SenderBank").Width = 150;

                this.grDt.Columns.Item("SenderBankId").TitleObject.Caption = "Mã ngân hàng chuyển";
                this.grDt.Columns.Item("SenderBankId").Editable = false;
                this.grDt.Columns.Item("SenderBankId").Width = 100;

                this.grDt.Columns.Item("Chanel").TitleObject.Caption = "Kênh";
                this.grDt.Columns.Item("Chanel").Editable = false;
                this.grDt.Columns.Item("Chanel").Width = 100;
            }
            else
            {
                this.grHdr.DataTable = DT_Header_BI;
                this.grDt.DataTable = DT_Detail_BI;

                this.grDt.Columns.Item("Currency").TitleObject.Caption = "Đồng tiền";
                this.grDt.Columns.Item("Currency").Editable = false;
                this.grDt.Columns.Item("Currency").Width = 80;

                this.grDt.Columns.Item("Ref").TitleObject.Caption = "Mã tham chiếu";
                this.grDt.Columns.Item("Ref").Editable = false;
                this.grDt.Columns.Item("Ref").Width = 80;
            }

            this.grHdr.Columns.Item("BankAccount").TitleObject.Caption = "Số tài khoản NH";
            this.grHdr.Columns.Item("BankAccount").Editable = false;
            this.grHdr.Columns.Item("BankAccount").Width = 100;

            this.grHdr.Columns.Item("BankName").TitleObject.Caption = "Tên tài khoản";
            this.grHdr.Columns.Item("BankName").Editable = false;
            this.grHdr.Columns.Item("BankName").Width = 250;

            this.grHdr.Columns.Item("Currency").TitleObject.Caption = "Đồng tiền";
            this.grHdr.Columns.Item("Currency").Editable = false;
            this.grHdr.Columns.Item("Currency").Width = 80;

            this.grHdr.Columns.Item("OpeningBalance").TitleObject.Caption = "Số dư đầu kỳ";
            this.grHdr.Columns.Item("OpeningBalance").Editable = false;
            this.grHdr.Columns.Item("OpeningBalance").Width = 80;

            this.grHdr.Columns.Item("ClosingBalance").TitleObject.Caption = "Số dư cuối kỳ";
            this.grHdr.Columns.Item("ClosingBalance").Editable = false;
            this.grHdr.Columns.Item("ClosingBalance").Width = 80;

            this.grDt.Columns.Item("TransDate").TitleObject.Caption = "Ngày giao dịch";
            this.grDt.Columns.Item("TransDate").Editable = false;
            this.grDt.Columns.Item("TransDate").Width = 100;

            this.grDt.Columns.Item("Description").TitleObject.Caption = "Diễn giải";
            this.grDt.Columns.Item("Description").Editable = false;
            this.grDt.Columns.Item("Description").Width = 150;

            this.grDt.Columns.Item("TransNo").TitleObject.Caption = "Số giao dịch";
            this.grDt.Columns.Item("TransNo").Editable = false;
            this.grDt.Columns.Item("TransNo").Width = 150;

            this.grDt.Columns.Item("Debit").TitleObject.Caption = "Số tiền chi";
            this.grDt.Columns.Item("Debit").Editable = false;
            this.grDt.Columns.Item("Debit").Width = 100;

            this.grDt.Columns.Item("Credit").TitleObject.Caption = "Số tiền thu";
            this.grDt.Columns.Item("Credit").Editable = false;
            this.grDt.Columns.Item("Credit").Width = 100;

            this.Freeze(false);
        }
    }
}
