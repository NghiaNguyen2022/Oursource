using PN.SmartLib.Helper;
using RestSharp;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.Config;
using SAPCore.Helper;
using STDApp.Common;
using STDApp.Models;
using System;
using System.Configuration;
using System.IO;
using System.Text.Json;

namespace STDApp.Payoo
{
    [FormAttribute("STDApp.Payoo.frmBatch", "Payoo/frmBatch.b1f")]
    class frmBatch : UserFormBase
    {
        public frmBatch()
        {
        }
        

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblBank = ((SAPbouiCOM.StaticText)(this.GetItem("lblBank").Specific));
            this.cbbBank = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBank").Specific));
            this.cbbBank.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbBank_ComboSelectAfter);
            this.lblTrDa = ((SAPbouiCOM.StaticText)(this.GetItem("lblTrDa").Specific));
            this.cbbBat = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBat").Specific));
            this.lblBat = ((SAPbouiCOM.StaticText)(this.GetItem("lblBat").Specific));
            this.lblPag = ((SAPbouiCOM.StaticText)(this.GetItem("lblPag").Specific));
            this.txtTrDa = ((SAPbouiCOM.EditText)(this.GetItem("txtTrDa").Specific));
            this.txtPag = ((SAPbouiCOM.EditText)(this.GetItem("txtPag").Specific));
            this.btnLoad = ((SAPbouiCOM.Button)(this.GetItem("btnLoad").Specific));
            this.btnLoad.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnLoad_ClickBefore);
            this.grHdr = ((SAPbouiCOM.Grid)(this.GetItem("grHdr").Specific));
            this.grDt = ((SAPbouiCOM.Grid)(this.GetItem("grDt").Specific));
            this.btnClear = ((SAPbouiCOM.Button)(this.GetItem("btnClear").Specific));
            this.btnClear.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnClear_ClickBefore);
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
        private SAPbouiCOM.StaticText lblTrDa;
        private SAPbouiCOM.ComboBox cbbBat;
        private SAPbouiCOM.StaticText lblBat;
        private SAPbouiCOM.StaticText lblPag;
        private SAPbouiCOM.EditText txtTrDa;
        private SAPbouiCOM.EditText txtPag;
        private SAPbouiCOM.Button btnLoad;
        private SAPbouiCOM.Grid grHdr;

        public static bool IsFormOpen = false;
        private static frmBatch instance;

        private string TransDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtTrDa);
            }
        }

        private string Page
        {
            get
            {
                return UIHelper.GetTextboxValue(txtPag);
            }
        }

        private string Bank
        {
            get
            {
                return UIHelper.GetComboValue(cbbBank);
            }
        }
        private string BatchNo
        {
            get
            {
                return UIHelper.GetComboValue(cbbBat);
            }
        }
        public static void ShowForm()
        {
            if (instance == null)
            {
                try
                {
                    instance = new frmBatch();
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
            
            UIHelper.ComboboxSelectDefault(cbbBank);
            cbbBank.Item.Enabled = false;
        }

        private void SetLocation()
        {
            var max = this.UIAPIRawForm.ClientHeight;
            var maxw = this.UIAPIRawForm.ClientWidth;

            this.lblBank.Item.Top = 15;
            this.lblBank.Item.Left = CoreSetting.UF_HorMargin;

            this.cbbBank.Item.Top = this.lblBank.Item.Top;
            this.cbbBank.Item.Left = this.lblBank.Item.Left + this.lblBank.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblTrDa.Item.Top = this.lblBank.Item.Top;
            this.lblTrDa.Item.Left = this.cbbBank.Item.Left + this.cbbBank.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.txtTrDa.Item.Top = this.lblBank.Item.Top;
            this.txtTrDa.Item.Left = this.lblTrDa.Item.Left + this.lblTrDa.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblBat.Item.Top = this.lblBank.Item.Top;
            this.lblBat.Item.Left = this.txtTrDa.Item.Left + this.txtTrDa.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.cbbBat.Item.Top = this.lblBank.Item.Top;
            this.cbbBat.Item.Left = this.lblBat.Item.Left + this.lblBat.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblPag.Item.Top = this.lblBank.Item.Top;
            this.lblPag.Item.Left = this.txtTrDa.Item.Left + this.txtTrDa.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.txtPag.Item.Top = this.lblBank.Item.Top;
            this.txtPag.Item.Left = this.lblPag.Item.Left + this.lblPag.Item.Width + CoreSetting.UF_HorizontallySpaced;
            
            var labBottom = this.lblBank.Item.Top + this.lblBank.Item.Height;
            var bttTop = labBottom - this.btnLoad.Item.Height;
            this.btnLoad.Item.Top = bttTop;
            this.btnLoad.Item.Left = this.txtPag.Item.Left + this.txtPag.Item.Width + CoreSetting.UF_HorizontallySpaced;

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
            if (string.IsNullOrEmpty(TransDate))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_TransDateSelectNull, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }
            this.Clear();

            if (Bank == Banks.ViettinBank.GetDescription())
            {
                this.CallPayooAPI();
            }
            //else
            //{
            //    this.CallBIDVAPI();
            //}
            this.Freeze(false);
        }

        //private string GetToken()
        //{
        //    try
        //    {
        //        var query = string.Format(QueryString.GetAPICode);
        //        var data = DataProvider.QuerySingle(CoreSetting.DataConnection, query);
        //        if (data == null)
        //        {
        //            return string.Empty;
        //        }
        //        var code = data["Code"].ToString();
        //        if (string.IsNullOrEmpty(code))
        //        {
        //            return string.Empty;
        //        }

        //        var options = new RestClientOptions(ConfigurationManager.AppSettings["LinkBIDVAPI"])
        //        {
        //            MaxTimeout = -1,
        //        };
        //        var client = new RestClient(options);
        //        var request = new RestRequest(ConfigurationManager.AppSettings["AuthenBIDV"], Method.Post);

        //        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        //        request.AddParameter("grant_type", "authorization_code");
        //        request.AddParameter("client_id", ConfigurationManager.AppSettings["ClientIDBIDV"]);
        //        request.AddParameter("client_secret", ConfigurationManager.AppSettings["ClientSecretBIDV"]);
        //        request.AddParameter("code", code);
        //        request.AddParameter("redirect_uri", ConfigurationManager.AppSettings["redirect_uri"]);

        //        var response = client.Execute(request);
        //        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //        {
        //            var error = JsonSerializer.Deserialize<BIDVAccesstokenResponseErrro>(response.Content);
        //            if (error.error_description.Contains("code expired"))
        //            {
        //                UIHelper.LogMessage($"Code để lấy token đã bị hết hạn, vui lòng lấy lại code mới và thử lại", UIHelper.MsgType.StatusBar, true);
        //            }
        //            else
        //            {
        //                UIHelper.LogMessage($"Lỗi {error.error_description}", UIHelper.MsgType.StatusBar, true);
        //            }
        //        }
        //        else
        //        {
        //            var result = JsonSerializer.Deserialize<BIDVAccesstokenResponse>(response.Content);
        //            if (result != null)
        //                return result.access_token;
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //    return string.Empty;
        //}
        //private string Base64UrlEncode(byte[] input)
        //{
        //    return Convert.ToBase64String(input)
        //        .TrimEnd('=')
        //        .Replace('+', '-')
        //        .Replace('/', '_');
        //}
        ////private static RSAParameters ConvertToRSAParameters(RsaPrivateCrtKeyParameters rsaKey)
        //{
        //    RSAParameters result = default(RSAParameters);
        //    result.Modulus = rsaKey.Modulus.ToByteArrayUnsigned();
        //    result.Exponent = rsaKey.PublicExponent.ToByteArrayUnsigned();
        //    result.D = rsaKey.Exponent.ToByteArrayUnsigned();
        //    result.P = rsaKey.P.ToByteArrayUnsigned();
        //    result.Q = rsaKey.Q.ToByteArrayUnsigned();
        //    result.DP = rsaKey.DP.ToByteArrayUnsigned();
        //    result.DQ = rsaKey.DQ.ToByteArrayUnsigned();
        //    result.InverseQ = rsaKey.QInv.ToByteArrayUnsigned();
        //    return result;
        //}
        //private RSA LoadPrivateKey(string filePath)
        //{
        //    try
        //    {
        //        string text = File.ReadAllText(filePath);
        //        text = text.Replace("-----BEGIN PRIVATE KEY-----", "").Replace("-----END PRIVATE KEY-----", "").Replace("\n", "")
        //            .Replace("\r", "")
        //            .Trim();
        //        var obj = Convert.FromBase64String(text);
        //        var instance = PrivateKeyInfo.GetInstance(obj);
        //        var asymmetricKeyParameter = PrivateKeyFactory.CreateKey(instance);
        //        if (asymmetricKeyParameter is RsaPrivateCrtKeyParameters rsaKey)
        //        {
        //            RSA rSA = RSA.Create();
        //            rSA.ImportParameters(ConvertToRSAParameters(rsaKey));
        //            return rSA;
        //        }

        //        throw new ArgumentException("Key is not an RSA private key.");
        //    }
        //    catch (Exception innerException)
        //    {
        //        throw new Exception("Error loading private key", innerException);
        //    }
        //}
        //static byte[] SignData(RSA rsa, string data)
        //{
        //    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        //    return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        //}
        //private string GenSignature()
        //{

        //    // Header (base64 encoded JSON)
        //    var header = new
        //    {
        //        alg = "RS256", // RSA with SHA-256
        //        typ = "JWT"
        //    };
        //    var headerJson = JsonSerializer.Serialize(header);
        //    var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        //    var detachedPayload = string.Empty;

        //    var signingInput = $"{headerBase64}.{detachedPayload}";
        //    var certPath = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

        //    var rsa = LoadPrivateKey(certPath);
        //    var signature = SignData(rsa, signingInput);

        //    var signatureBase64 = Base64UrlEncode(signature);
        //    var jws = $"{headerBase64}.{detachedPayload}.{signatureBase64}";
        //    return jws;
        //    // Console.WriteLine("Detached JWS:");
        //    //Console.WriteLine(jws);

        //    // Example: Verification
        //    //bool isValid = VerifySignature(rsa, signingInput, signature);
        //    //Console.WriteLine("Signature valid: " + isValid);
        //}
        private void CallBIDVAPI()
        {
            var mesage = string.Empty;
            var token = APIHelper.GetToken(ref mesage);

            if (string.IsNullOrEmpty(token))
            {
                if(string.IsNullOrEmpty(mesage))
                UIHelper.LogMessage($"Không lấy được token từ phía ngân hàng, vui lòng thử lại",
                    UIHelper.MsgType.StatusBar, true);
                else
                    UIHelper.LogMessage(mesage,
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
                var rsaPrivateKeyHeaderPem = "-----BEGIN PRIVATE KEY-----";
                var rsaPrivateKeyFooterPem = "-----END PRIVATE KEY-----";
                certificate = certificate.Replace(rsaPrivateKeyHeaderPem, "").Replace(rsaPrivateKeyFooterPem, "").Replace("\n\r", "");
                var options = new RestClientOptions(APIBIDVConstrant.APILink);
                var client = new RestClient(options);
                var request = new RestRequest(APIBIDVConstrant.InquiryBIDV + "?Scope=read&JWE=Yes", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-API-Interaction-ID", DateTime.Now.ToString("yyyyMMddHHmmss"));
                request.AddHeader("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssffff"));
                request.AddHeader("Channel", APIBIDVConstrant.ChannelBIDVAPI);// ConfigurationManager.AppSettings["ChannelBIDVAPI"]);
                request.AddHeader("User-Agent", APIBIDVConstrant.UserAgentIDVAPI);// ConfigurationManager.AppSettings["UserAgentIDVAPI"]);
                request.AddHeader("accept", "application/json");
                //OPTION: request.AddHeader("X-Idempotency-Key", "REPLACE_THIS_VALUE");
                //OPTION: request.AddHeader("X-Customer-IP-Address", "REPLACE_THIS_VALUE");
                //request.AddStringBody("REPLACE_BODY", "text/plain");

                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddHeader("X-Client-Certificate", certificate);


                var requestData = new BIDVInquiryRequest()
                {
                    actNumber = Bank,// "12010002159887",
                    curr = "VND",
                    //fromDate = DateTime.ParseExact(FromDate, "yyyyMMdd", null).ToString("dd/MM/yyyy"),//"20240901",
                    //toDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null).ToString("dd/MM/yyyy"),//,
                    page = "1"
                };
                var symmetricKey = ConfigurationManager.AppSettings["SymmetricKey"];
                var symmetricKeys = HexToByteArray(symmetricKey);

                var json = JsonSerializer.Serialize(requestData);
                var encryptData = APIUtil.doEncryptJWE(json, symmetricKey);             
                request.AddStringBody(encryptData, DataFormat.Json);

                var signature = APIUtil.DoSignatureJWS(encryptData);
                if (string.IsNullOrEmpty(signature))
                {
                    UIHelper.LogMessage($"Không tạo được chữ ký, vui lòng kiểm tra lại",
                       UIHelper.MsgType.StatusBar, true);
                    return;
                }
                request.AddHeader("X-JWS-Signature", signature);

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

        private void CallPayooAPI()
        {

            try
            {
                UIHelper.LogMessage($"Bắt đầu gửi thông tin qua hệ thống Payoo để lấy thanh toán", UIHelper.MsgType.StatusBar, false);

                var options = new RestClientOptions(APIPayooConstant.APILink)
                {
                    MaxTimeout = -1,
                };

                var client = new RestClient(options);
                var request = new RestRequest(APIPayooConstant.SettlementTransactionsLink,  Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("APIUsername", APIPayooConstant.APIUsername);
                request.AddHeader("APIPassword", APIPayooConstant.APIPassword);
                request.AddHeader("APISignature", APIPayooConstant.APISignature);
                request.AddHeader("Accept", "application/json");
                var transDate = DateTime.ParseExact(TransDate, "yyyyMMdd", null).ToString("yyyyMMdd");
               
                var requestData = new
                {
                    SettlementDate = transDate,
                    BatchNumber = BatchNo,
                    PageNumber = Page
                };
                var requestDataJson = JsonSerializer.Serialize(requestData);
                var requestDataFull = new
                {
                    RequestData = requestDataJson,
                    SecureHash = APIUtil.EncryptSHA512(requestDataJson)//  "35b8fd5ca7e314b134126add44ad7be804462e17f0e28956df3bd0d4c673339e856bbf3b83635ada03ee5efd54941a62e8c370931cf3bdd8d9f555e75e5accc9"
                };
                var finalRequestJson = JsonSerializer.Serialize(requestDataFull);               
                request.AddStringBody(finalRequestJson, DataFormat.Json);

                var response = client.Execute(request);

                var result = response.Content;

                var resp = JsonSerializer.Deserialize<PayooResponse>(result);
                if(resp == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var respDataStr = resp.ResponseData.Replace(@"\", "");
                var respDataObj = JsonSerializer.Deserialize<PayooResponseData>(respDataStr);
                if (respDataObj == null)
                {
                    UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                    return;
                }

                PayooResponseCode rpcode = (PayooResponseCode)CoreExtensions.GetEnumValue<PayooResponseCode>(respDataObj.ResponseCode);
                if(rpcode != PayooResponseCode.Res0)
                {
                    UIHelper.LogMessage(rpcode.GetDescription(), UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var rpDataTransaction = JsonSerializer.Deserialize<PayooResponseDataExt>(respDataStr);
                    
               // var inquiry = JsonSerializer.Deserialize<InquiryHeader>(result);

                if (rpDataTransaction != null)
                {
                    if (this.grHdr != null && this.grHdr.DataTable != null)
                    {
                        this.grHdr.DataTable.Rows.Add();
                        var index = grHdr.DataTable.Rows.Count - 1;
                        this.grHdr.DataTable.SetValue("BatchNo", index, rpDataTransaction.BatchNo);
                        this.grHdr.DataTable.SetValue("Amount", index, rpDataTransaction.TotalSettlementAmount);
                        this.grHdr.DataTable.SetValue("RowCount", index, rpDataTransaction.TotalSettlementRowCount);
                        this.grHdr.DataTable.SetValue("PageSize", index, rpDataTransaction.PageSize);
                        this.grHdr.AutoResizeColumns();
                    }

                    if (this.grDt != null && this.grDt.DataTable != null)
                    {
                        foreach (var item in rpDataTransaction.TransactionList)
                        {
                            this.grDt.DataTable.Rows.Add();
                            var index = grDt.DataTable.Rows.Count - 1;
                            this.grDt.DataTable.SetValue("OrdeNo", index, item.OrderNo);
                            this.grDt.DataTable.SetValue("ShopId", index, item.ShopId);
                            this.grDt.DataTable.SetValue("SellerName", index, item.SellerName);
                            this.grDt.DataTable.SetValue("TransferAmount", index, item.MoneyAmount);
                            this.grDt.DataTable.SetValue("InvoiceDate", index, item.PurchaseDate);
                            this.grDt.DataTable.SetValue("Status", index, item.Status);
                            this.grDt.DataTable.SetValue("IntegDate", index, DateTime.Now.ToString("yyyyMMdd"));
                            this.grDt.DataTable.SetValue("IntegTime", index, DateTime.Now.ToString("HHmmss"));
                            this.grDt.DataTable.SetValue("RecWithBank", index, "Chưa");
                            this.grDt.DataTable.SetValue("BankRefNo", index, "");

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
            //if(this.grHdr == null || this.grDt == null)
            //{
            //    return;
            //}
            //this.Freeze(true);

          
            //this.Freeze(false);
        }
    }
}
