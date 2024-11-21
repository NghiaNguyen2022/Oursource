using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using PN.SmartLib.Helper;
using RestSharp;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.Config;
using SAPCore.Helper;
using STDApp.Models;

namespace STDApp.Bank
{
    [FormAttribute("STDApp.Bank.frmInquiry", "Bank/frmInquiry.b1f")]
    class frmInquiry : UserFormBase
    {
        public frmInquiry()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.lblBank = ((SAPbouiCOM.StaticText)(this.GetItem("lblBank").Specific));
            this.cbbBank = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBank").Specific));
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

            //ConfigurationManager.AppSettings["Taikhoan"]
            var account = ConfigurationManager.AppSettings["Taikhoan"];
            UIHelper.ComboboxSelectDefault(cbbBank);

            UIHelper.ClearSelectValidValues(cbbAcc);
            this.cbbAcc.ValidValues.Add(account, account);
            UIHelper.ComboboxSelectDefault(cbbAcc);
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

            this.CallAPI();
            this.Freeze(false);
        }

        private void CallAPI()
        {
            try
            {
                if (Bank == Banks.ViettinBank.GetDescription())
                {
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

                    var rps = JsonSerializer.Deserialize<InquiryResponse>(result);
                    if(rps == null)
                    {
                        UIHelper.LogMessage($"Lỗi không có phản hồi, vui lòng check lại", UIHelper.MsgType.StatusBar, true);
                        return;
                    }
                    if(rps.status.code == "0")
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
                                this.grDt.DataTable.SetValue("Debit", index, item.debit??"0");
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
                }
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
    }
}
