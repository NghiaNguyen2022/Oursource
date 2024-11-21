using STD.DataReader;
using STDApp.AccessSAP;
using STDApp.DataReader;
using STDApp.Models;
using PN.SmartLib.Helper;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.Config;
using SAPCore.Helper;
using SAPCore.SAP;
using SAPCore.SAP.DIAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace STDApp.Payment
{
    [FormAttribute("STDApp.Payment.frmPayment", "Payment/frmPayment.b1f")]
    class frmPayment : UserFormBase
    {
        private static frmPayment instance;
        public bool IsGroup = false;
        private List<int> SelectedDataIndexs = new List<int>();
        public static bool IsFormOpen = false;
        private SAPbouiCOM.DataTable DataTableCbb;

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
        private string PostingDate
        {
            get
            {
                return UIHelper.GetTextboxValue(txtPosDa, DateTime.Now.ToString("yyyyMMdd"));
            }
        }

        private string Note
        {
            get
            {
                return UIHelper.GetTextboxValue(txtNote);
            }
        }

        private string CardCode
        {
            get
            {
                return UIHelper.GetTextboxValue(txtCusVen);
            }
        }
        private string CardCodeFilter { get; set; } = string.Empty;
        private string CardTypeSelect { get; set; } = string.Empty;
        private PaymentType _PaymentType
        {
            get
            {
                if (cbbType != null && cbbType.Selected != null)
                {
                    var type = cbbType.Selected.Value.GetEnumValueByDescription<PaymentType>();
                    return type;
                }

                return PaymentType.T;
            }
        }
        private string BankCode
        {
            get
            {
                // if(cbbBank)
                return UIHelper.GetComboValue(cbbBankAccount);
            }
        }

        private string CashCode
        {
            get
            {
                return UIHelper.GetComboValue(cbbAccCa);
            }
        }

        private string CashFlowID
        {
            get
            {
                return UIHelper.GetComboValue(cbbCFlow);
            }
        }

        private string Branch
        {
            get
            {
                return UIHelper.GetComboValue(cbbBr, "0");
            }
        }

        private decimal Rate
        {
            get
            {
                decimal rate = 0;
                decimal.TryParse(txtRate.Value, out rate);
                return rate;
            }
        }

        private PaymentMethod _PaymentMethod
        {
            get
            {
                if (cbbFeeType != null && cbbFeeType.Selected != null)
                {
                    var type = cbbFeeType.Selected.Value.GetEnumValueByDescription<PaymentMethod>();
                    return type;// cbbMeth.Selected.Value == "C" ? PaymentMethod.Cash : PaymentMethod.Bank;
                }

                return PaymentMethod.Cash;
            }
        }

        private PaymentDocumentType _PaymentDocumentType
        {
            get
            {
                if (cbbPmTyp != null && cbbPmTyp.Selected != null)
                {
                    var type = cbbPmTyp.Selected.Value.GetEnumValueByDescription<PaymentDocumentType>();

                    return type;// PaymentDocumentType.UC;
                    //return cbbPmTyp.Selected.Value == "PT" ? PaymentDocumentType.PT : PaymentType.C;
                }

                return PaymentDocumentType.PT;
            }
        }

        private string FromDateReport
        {
            get
            {
                return UIHelper.GetTextboxValue(txtFDateL);
            }
        }
        ChooseFromList oCFL;
        Conditions oConCustomers = null;
        Conditions oConVendors = null;
        private void ConfigCFL()
        {
            ChooseFromListCollection oCFLs = null;
            oCFLs = this.UIAPIRawForm.ChooseFromLists;
            oCFL = oCFLs.Item("CFL_BP");
            oConCustomers = null;
            // Condition oCon = null;
            oConCustomers = oCFL.GetConditions();

            var oConCustomer = oConCustomers.Add();
            oConCustomer.Alias = "CardType";

            oConCustomer.Operation = BoConditionOperation.co_EQUAL;
            oConCustomer.CondVal = "C";

            oConVendors = null;
            oConVendors = oCFL.GetConditions();

            var oConVendor = oConVendors.Add();
            oConVendor.Alias = "CardType";

            oConVendor.Operation = BoConditionOperation.co_EQUAL;
            oConVendor.CondVal = "S";
            SetCondition();
        }

        private void SetCondition(string type = "C")
        {
            if (oCFL != null)
            {
                if (type == "C" && oConCustomers != null)
                {
                    oCFL.SetConditions(oConCustomers);
                }
                else
                {
                    if (oConVendors != null)
                        oCFL.SetConditions(oConVendors);
                }
            }
        }

        private void SelectType(string type = "C")
        {
            if (type == "C")
            {
                this.lblCusVen.Caption = STRING_CONTRANTS.Title_Customer;
                //this.lblName.Caption = STRING_CONTRANTS.Title_CustomerName;// "Tên Khách hàng";
            }
            else
            {
                this.lblCusVen.Caption = STRING_CONTRANTS.Title_Vendor;
                //this.lblName.Caption = STRING_CONTRANTS.Title_VendorName;// "Tên Nhà Cung cấp";
            }
            SetCondition(type);
        }

        private string ToDateReport
        {
            get
            {
                return UIHelper.GetTextboxValue(txtTDateL);

            }
        }
        private string BranchReport
        {
            get
            {
                return UIHelper.GetComboValue(cbbBrL, "0");
            }
        }
        private PaymentDocumentType _PaymentDocumentTypeReport
        {
            get
            {
                if (cbbPmTyL != null && cbbPmTyL.Selected != null)
                {
                    var type = cbbPmTyL.Selected.Value.GetEnumValueByDescription<PaymentDocumentType>();

                    return type;// PaymentDocumentType.UC;
                }

                return PaymentDocumentType.PT;
            }
        }
        private frmPayment()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.folderCreate = ((SAPbouiCOM.Folder)(this.GetItem("F_Cr").Specific));
            this.folderList = ((SAPbouiCOM.Folder)(this.GetItem("F_Pm").Specific));
            this.lblFDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblFDa").Specific));
            this.lblTDate = ((SAPbouiCOM.StaticText)(this.GetItem("lblTDa").Specific));
            this.txtFDate = ((SAPbouiCOM.EditText)(this.GetItem("txtFDa").Specific));
            this.txtTDate = ((SAPbouiCOM.EditText)(this.GetItem("txtTDa").Specific));
            this.lblType = ((SAPbouiCOM.StaticText)(this.GetItem("lblTy").Specific));
            this.cbbType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbTy").Specific));
            this.cbbType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbType_ComboSelectAfter);
            this.btnFind = ((SAPbouiCOM.Button)(this.GetItem("btnFind").Specific));
            this.btnFind.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFind_ClickBefore);
            this.grData = ((SAPbouiCOM.Grid)(this.GetItem("grData").Specific));
            this.grData.DoubleClickAfter += new SAPbouiCOM._IGridEvents_DoubleClickAfterEventHandler(this.grData_DoubleClickAfter);
            this.grData.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.grData_ClickAfter);
            this.btnCreate = ((SAPbouiCOM.Button)(this.GetItem("btnCrt").Specific));
            this.btnCreate.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCreate_ClickBefore);
            this.lblBr = ((SAPbouiCOM.StaticText)(this.GetItem("lblBr").Specific));
            this.cbbBr = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBr").Specific));
            this.lblFeeType = ((SAPbouiCOM.StaticText)(this.GetItem("lblFeeT").Specific));
            this.cbbFeeType = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbFeeT").Specific));
            this.cbbFeeType.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.cbbFeeT_ComboSelectAfter);
            this.lblPmTyp = ((SAPbouiCOM.StaticText)(this.GetItem("lblPT").Specific));
            this.cbbPmTyp = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbPT").Specific));
            this.lblFDateL = ((SAPbouiCOM.StaticText)(this.GetItem("lblFDaL").Specific));
            this.lblTDateL = ((SAPbouiCOM.StaticText)(this.GetItem("lblTDaL").Specific));
            this.txtFDateL = ((SAPbouiCOM.EditText)(this.GetItem("txtFDaL").Specific));
            this.txtTDateL = ((SAPbouiCOM.EditText)(this.GetItem("txtTDaL").Specific));
            this.btnFindL = ((SAPbouiCOM.Button)(this.GetItem("btnFindL").Specific));
            this.btnFindL.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFindL_ClickBefore);
            this.lblBrL = ((SAPbouiCOM.StaticText)(this.GetItem("lblBrL").Specific));
            this.cbbBrL = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBrL").Specific));
            this.lblPmTyL = ((SAPbouiCOM.StaticText)(this.GetItem("lblPTL").Specific));
            this.cbbPmTyL = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbPTL").Specific));
            this.grHdr = ((SAPbouiCOM.Grid)(this.GetItem("grHdr").Specific));
            this.grHdr.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.grHdr_ClickAfter);
            this.grDt = ((SAPbouiCOM.Grid)(this.GetItem("grDt").Specific));
            this.btnAddL = ((SAPbouiCOM.Button)(this.GetItem("btnAddL").Specific));
            this.btnAddL.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnAddL_ClickBefore);
            this.btnRem = ((SAPbouiCOM.Button)(this.GetItem("btnRem").Specific));
            this.btnRem.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnRem_ClickBefore);
            this.ckbGr = ((SAPbouiCOM.CheckBox)(this.GetItem("ckbGr").Specific));
            this.ckbGr.PressedAfter += new SAPbouiCOM._ICheckBoxEvents_PressedAfterEventHandler(this.CheckBox0_PressedAfter);
            this.lblBankAccount = ((SAPbouiCOM.StaticText)(this.GetItem("lblBank").Specific));
            this.cbbBankAccount = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbBank").Specific));
            this.lblCFlow = ((SAPbouiCOM.StaticText)(this.GetItem("lblCF").Specific));
            this.cbbCFlow = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbCF").Specific));
            this.txtRate = ((SAPbouiCOM.EditText)(this.GetItem("txtRate").Specific));
            this.ckAppRate = ((SAPbouiCOM.CheckBox)(this.GetItem("ckAppRate").Specific));
            this.ckAppRate.PressedAfter += new SAPbouiCOM._ICheckBoxEvents_PressedAfterEventHandler(this.ckAppRate_PressedAfter);
            this.cbbAccCa = ((SAPbouiCOM.ComboBox)(this.GetItem("cbbAC").Specific));
            this.lblAccCa = ((SAPbouiCOM.StaticText)(this.GetItem("lblAccCa").Specific));
            this.btnChAl = ((SAPbouiCOM.Button)(this.GetItem("btnChAl").Specific));
            this.btnChAl.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnChAl_ClickBefore);
            this.btnUnck = ((SAPbouiCOM.Button)(this.GetItem("btnUnck").Specific));
            this.btnUnck.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnUnck_ClickBefore);
            this.lblPosDa = ((SAPbouiCOM.StaticText)(this.GetItem("lblPos").Specific));
            this.txtPosDa = ((SAPbouiCOM.EditText)(this.GetItem("txtPos").Specific));
            this.txtNote = ((SAPbouiCOM.EditText)(this.GetItem("txtNote").Specific));
            this.lblNote = ((SAPbouiCOM.StaticText)(this.GetItem("lblNote").Specific));
            this.lblCusVen = ((SAPbouiCOM.StaticText)(this.GetItem("lblCV").Specific));
            this.txtCusVen = ((SAPbouiCOM.EditText)(this.GetItem("txtCV").Specific));
            this.txtCusVen.LostFocusAfter += new SAPbouiCOM._IEditTextEvents_LostFocusAfterEventHandler(this.txtCusVen_LostFocusAfter);
            this.btnFilter = ((SAPbouiCOM.Button)(this.GetItem("btnFil").Specific));
            this.btnFilter.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnFilter_ClickBefore);
            this.DataTableCbb = this.UIAPIRawForm.DataSources.DataTables.Item("DT_FT");
            this.btnHis = ((SAPbouiCOM.Button)(this.GetItem("btnHis").Specific));
            this.btnHis.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnHis_ClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(this.Form_CloseAfter);
            // this.ResizeAfter += new ResizeAfterHandler(this.Form_ResizeAfter);
        }

        private void OnCustomInitialize()
        {

        }
        private void SetControlLocation()
        {
            var max = this.UIAPIRawForm.ClientHeight;
            var maxw = this.UIAPIRawForm.ClientWidth;
            SetLocationOfFolderCreateControl(max, maxw);
            SetLocationOfFolderListControl(max, maxw);
        }

        private void SetLocationOfFolderListControl(int max, int maxw)
        {
            this.folderList.Item.Width = 500;

            this.lblFDateL.Item.Top = this.folderCreate.Item.Top + CoreSetting.UF_VerMargin;
            this.lblFDateL.Item.Left = this.folderCreate.Item.Left + CoreSetting.UF_HorMargin;

            this.txtFDateL.Item.Top = this.lblFDateL.Item.Top;
            this.txtFDateL.Item.Left = this.lblFDateL.Item.Left + this.lblFDateL.Item.Width + 20;

            this.lblTDateL.Item.Top = this.lblFDateL.Item.Top;
            this.lblTDateL.Item.Left = this.txtFDateL.Item.Left + this.txtFDateL.Item.Width + 20;

            this.txtTDateL.Item.Top = this.lblFDateL.Item.Top;
            this.txtTDateL.Item.Left = this.lblTDateL.Item.Left + this.lblTDateL.Item.Width + 20;

            this.lblPmTyL.Item.Top = this.lblFDateL.Item.Top;
            this.lblPmTyL.Item.Left = this.txtTDateL.Item.Left + this.txtTDateL.Item.Width + 20;

            this.cbbPmTyL.Item.Top = this.lblFDateL.Item.Top;
            this.cbbPmTyL.Item.Left = this.lblPmTyL.Item.Left + this.lblPmTyL.Item.Width + 20;

            this.lblBrL.Item.Top = this.lblFDateL.Item.Top;
            this.lblBrL.Item.Left = this.cbbPmTyL.Item.Left + this.cbbPmTyL.Item.Width + 20;

            this.cbbBrL.Item.Top = this.lblFDateL.Item.Top;
            this.cbbBrL.Item.Left = this.lblBrL.Item.Left + this.lblBrL.Item.Width + 20;

            var labBottom = this.lblFDateL.Item.Top + this.lblFDateL.Item.Height;
            var bttTop = labBottom - this.btnFindL.Item.Height;
            this.btnFindL.Item.Top = bttTop;
            this.btnFindL.Item.Left = this.cbbBrL.Item.Left + this.cbbBrL.Item.Width + 30;

            this.grHdr.Item.Left = this.lblFDateL.Item.Left;
            this.grHdr.Item.Width = maxw - grHdr.Item.Left - 20;
            this.grHdr.Item.Top = lblFDateL.Item.Top + lblFDateL.Item.Height + 20;
            var bodyHeight = max - grHdr.Item.Top - 20;
            var headeHeight = (bodyHeight - 10) / 2;
            this.grHdr.Item.Height = headeHeight;

            this.grDt.Item.Left = this.lblFDateL.Item.Left;
            this.grDt.Item.Width = maxw - grDt.Item.Left - 20;
            this.grDt.Item.Top = this.grHdr.Item.Top + this.grHdr.Item.Height + 10;
            this.grDt.Item.Height = max - grDt.Item.Top - 20;
        }

        private void SetLocationOfFolderCreateControl(int max, int maxw)
        {
            this.folderCreate.Item.Width = 500;

            this.lblFDate.Item.Top = this.folderCreate.Item.Top + CoreSetting.UF_VerMargin;
            this.lblFDate.Item.Left = this.folderCreate.Item.Left + CoreSetting.UF_HorMargin;

            this.txtFDate.Item.Top = this.lblFDate.Item.Top;
            this.txtFDate.Item.Left = this.lblFDate.Item.Left + this.lblFDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblTDate.Item.Top = this.lblFDate.Item.Top;
            this.lblTDate.Item.Left = this.txtFDate.Item.Left + this.txtFDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.txtTDate.Item.Top = this.lblFDate.Item.Top;
            this.txtTDate.Item.Left = this.lblTDate.Item.Left + this.lblTDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblBankAccount.Item.Left = this.txtTDate.Item.Left + this.txtTDate.Item.Width + CoreSetting.UF_HorizontallySpaced;
            this.lblBankAccount.Item.Top = this.lblFDate.Item.Top;

            this.cbbBankAccount.Item.Top = this.lblFDate.Item.Top;
            this.cbbBankAccount.Item.Left = this.lblBankAccount.Item.Left + this.lblBankAccount.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblFeeType.Item.Left = this.cbbBankAccount.Item.Left + this.cbbBankAccount.Item.Width + CoreSetting.UF_HorizontallySpaced;
            this.lblFeeType.Item.Top = this.lblFDate.Item.Top;

            this.cbbFeeType.Item.Top = this.lblFDate.Item.Top;
            this.cbbFeeType.Item.Left = this.lblFeeType.Item.Left + this.lblFeeType.Item.Width + CoreSetting.UF_HorizontallySpaced;

            var labBottom = this.lblFDate.Item.Top + this.lblFDate.Item.Height;
            var bttTop = labBottom - this.btnFind.Item.Height;
            this.btnFind.Item.Top = bttTop;
            this.btnFind.Item.Left = maxw - btnFind.Item.Width - CoreSetting.UF_HorMargin;// this.txtCusVen.Item.Left + this.txtCusVen.Item.Width + 10;

            this.lblPmTyp.Item.Top = this.lblFDate.Item.Top + this.lblFDate.Item.Height + CoreSetting.UF_VerticallySpaced;
            this.lblPmTyp.Item.Left = this.lblFDate.Item.Left;
            this.lblPmTyp.Item.Width = this.lblTDate.Item.Width + this.txtFDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.cbbPmTyp.Item.Top = this.lblPmTyp.Item.Top;
            this.cbbPmTyp.Item.Left = this.lblTDate.Item.Left;
            this.cbbPmTyp.Item.Width = this.lblTDate.Item.Width + this.txtTDate.Item.Width + CoreSetting.UF_HorizontallySpaced;

            this.lblCFlow.Item.Top = this.cbbPmTyp.Item.Top;
            this.lblCFlow.Item.Left = this.lblBankAccount.Item.Left;
            this.lblCFlow.Item.Width = this.lblBankAccount.Item.Width;// + cbbType.Item.Width + 20;

            this.cbbCFlow.Item.Top = this.lblCFlow.Item.Top;
            this.cbbCFlow.Item.Left = this.cbbBankAccount.Item.Left;
            this.cbbCFlow.Item.Width = this.cbbBankAccount.Item.Width;// + this.cbbBr.Item.Width + 20;
                        
            this.lblPosDa.Item.Top = this.cbbCFlow.Item.Top;// this.txtPosDa.Item.Top;
            this.lblPosDa.Item.Left = this.lblFeeType.Item.Left;// - this.lblPosDa.Item.Width;
            this.lblPosDa.Item.Width = this.lblFeeType.Item.Width;

            this.txtPosDa.Item.Top = this.cbbCFlow.Item.Top;// this.grData.Item.Top - this.txtPosDa.Item.Height - 10;
            this.txtPosDa.Item.Left = this.lblPosDa.Item.Left + this.lblPosDa.Item.Width + 10;// maxw - this.txtPosDa.Item.Width - 20;
            
            this.btnCreate.Item.Left = this.btnFind.Item.Left;// this.txtPosDa.Item.Left + this.txtPosDa.Item.Width + 10;
            var labBottom1 = this.lblPmTyp.Item.Top + this.lblPmTyp.Item.Height;
            var bttTop1 = labBottom1 - this.btnCreate.Item.Height;
            this.btnCreate.Item.Top = bttTop1;
            this.btnCreate.Item.Width = btnFind.Item.Width;
            
            this.lblNote.Item.Left = this.txtFDate.Item.Left;
            this.lblNote.Item.Top = this.lblPmTyp.Item.Top + this.lblPmTyp.Item.Height + CoreSetting.UF_VerticallySpaced;

            this.txtNote.Item.Left = this.lblTDate.Item.Left;
            this.txtNote.Item.Top = this.lblNote.Item.Top;
            var lblNoteWidth = (this.btnCreate.Item.Left + this.btnCreate.Item.Width) - this.lblTDate.Item.Left;
            this.txtNote.Item.Width = lblNoteWidth;                      

            this.btnAddL.Item.Left = this.lblFDate.Item.Left;
            this.btnAddL.Item.Top = this.txtNote.Item.Top + this.txtNote.Item.Height + CoreSetting.UF_VerticallySpaced;
                       
            this.btnChAl.Item.Top = this.btnAddL.Item.Top;
            this.btnChAl.Item.Left = this.btnAddL.Item.Left + this.btnAddL.Item.Width + CoreSetting.UF_VerticallySpaced;

            this.btnUnck.Item.Top = this.btnAddL.Item.Top;
            this.btnUnck.Item.Left = this.btnChAl.Item.Left + this.btnChAl.Item.Width + CoreSetting.UF_VerticallySpaced;

            this.grData.Item.Left = this.lblFDate.Item.Left;
            this.grData.Item.Top = btnAddL.Item.Top + btnAddL.Item.Height + 10;
            this.grData.Item.Width = maxw - grData.Item.Left - 20;
            this.grData.Item.Height = max - grData.Item.Top - 20;

        }


        private SAPbouiCOM.Folder folderCreate;
        private SAPbouiCOM.Folder folderList;
        private SAPbouiCOM.StaticText lblFDate;
        private SAPbouiCOM.StaticText lblTDate;
        private SAPbouiCOM.EditText txtFDate;
        private SAPbouiCOM.EditText txtTDate;
        private SAPbouiCOM.StaticText lblType;
        private SAPbouiCOM.ComboBox cbbType;
        private SAPbouiCOM.Button btnFind;
        private SAPbouiCOM.Grid grData;
        private SAPbouiCOM.Button btnCreate;
        private SAPbouiCOM.StaticText lblBr;
        private SAPbouiCOM.ComboBox cbbBr;
        private SAPbouiCOM.StaticText lblFeeType;
        private SAPbouiCOM.ComboBox cbbFeeType;
        private SAPbouiCOM.StaticText lblPmTyp;
        private SAPbouiCOM.ComboBox cbbPmTyp;
        private SAPbouiCOM.StaticText lblFDateL;
        private SAPbouiCOM.StaticText lblTDateL;
        private SAPbouiCOM.EditText txtFDateL;
        private SAPbouiCOM.EditText txtTDateL;
        private SAPbouiCOM.Button btnFindL;
        private SAPbouiCOM.StaticText lblBrL;
        private SAPbouiCOM.ComboBox cbbBrL;
        private SAPbouiCOM.StaticText lblPmTyL;
        private SAPbouiCOM.ComboBox cbbPmTyL;
        private SAPbouiCOM.Grid grHdr;
        private SAPbouiCOM.Grid grDt;
        private SAPbouiCOM.Button btnAddL;
        private SAPbouiCOM.Button btnRem;
        private StaticText lblBankAccount;
        private ComboBox cbbBankAccount;
        private StaticText lblCFlow;
        private ComboBox cbbCFlow;

        public static void ShowForm()
        {
            if (instance == null)
            {
                try
                {
                    instance = new frmPayment();
                    instance.InitControl();
                    instance.Show();
                    IsFormOpen = true;
                }
                catch (Exception wex)
                {

                }
            }
        }
        private void InitControl()
        {
            this.folderCreate.Select();

            UIHelper.ComboboxSelectDefault(this.cbbType);
            UIHelper.ComboboxSelectDefault(this.cbbFeeType);

            LoadBranchestoCombobox();
            LoadPaymentTypeCombobox();
            LoadBankCombobox();
            LoadCashflowCombobox();
            ConfigCFL();
            this.txtRate.Item.Enabled = false;
            this.btnRem.Item.Enabled = false;

            this.txtPosDa.Value = DateTime.Now.ToString("yyyyMMdd");

        }
        private void LoadBranchestoCombobox()
        {
            try
            {
                this.Freeze(true);
                UIHelper.LoadComboboxFromDataSource(cbbBr, DataTableCbb, QueryString.BranchesLoad, "BPLId", "BPLName");
                UIHelper.LoadComboboxFromDataSource(cbbBrL, DataTableCbb, QueryString.BranchesLoad, "BPLId", "BPLName");
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage("Error GetBranch : " + ex.Message, UIHelper.MsgType.StatusBar, true);
            }
            finally
            {
                this.Freeze(false);
            }
        }

        private void LoadBankCombobox()
        {
            //try
            //{
            //    this.Freeze(true);
            //    UIHelper.LoadComboboxFromDataSource(cbbBank, DataTableCbb, QueryString.BanksLoad, "Code", "Name");
            //    UIHelper.LoadComboboxFromDataSource(cbbAccCa, DataTableCbb, QueryString.BanksLoad, "Code", "Name");
            //}
            //catch (Exception ex)
            //{
            //    UIHelper.LogMessage("Error GetBank : " + ex.Message, UIHelper.MsgType.StatusBar, true);
            //}
            //finally
            //{
            //    this.Freeze(false);
            //}

            UIHelper.ClearSelectValidValues(cbbBankAccount);
            UIHelper.ClearSelectValidValues(cbbAccCa);
            var values = DataHelper.ListBanks;
            if (values != null && values.Count() > 0)
            {
                foreach (var data in values)
                {
                    this.cbbBankAccount.ValidValues.Add(data["Code"].ToString(), data["Name"].ToString());
                    this.cbbAccCa.ValidValues.Add(data["Code"].ToString(), data["Name"].ToString());
                }
                UIHelper.ComboboxSelectDefault(cbbBankAccount);
                UIHelper.ComboboxSelectDefault(cbbAccCa);
            }
        }

        private void LoadCashflowCombobox()
        {
            //try
            //{
            //    this.Freeze(true);
            //    UIHelper.LoadComboboxFromDataSource(cbbCFlow, DataTableCbb, QueryString.CflowsLoad, "CFWId", "CFWName", 0, "-", STRING_CONTRANTS.NoChooseCashFlow);
            //}
            //catch (Exception ex)
            //{
            //    UIHelper.LogMessage("Error Get Cashflow : " + ex.Message, UIHelper.MsgType.StatusBar, true);
            //}
            //finally
            //{
            //    this.Freeze(false);
            //}

            UIHelper.ClearSelectValidValues(cbbCFlow);
            var values = DataHelper.ListCashFlows;
            this.cbbCFlow.ValidValues.Add("-", STRING_CONTRANTS.NoChooseCashFlow);
            if (values != null && values.Count() > 0)
            {
                foreach (var data in values)
                {
                    this.cbbCFlow.ValidValues.Add(data["CFWId"].ToString(), data["CFWName"].ToString());
                }
                UIHelper.ComboboxSelectDefault(cbbCFlow);
            }
        }

        private void LoadPaymentTypeCombobox()
        {
            UIHelper.ClearSelectValidValues(cbbPmTyp);
            this.txtCusVen.Value = string.Empty;
            if (_PaymentType == PaymentType.T)
            {
                cbbPmTyp.ValidValues.Add("PT", STRING_CONTRANTS.PaymentType_PT);
                SelectType("C");
            }
            else
            {
                cbbPmTyp.ValidValues.Add("PC", STRING_CONTRANTS.PaymentType_PC);
                cbbPmTyp.ValidValues.Add("UC", STRING_CONTRANTS.PaymentType_UC);
                SelectType("V");
            }
            UIHelper.ComboboxSelectDefault(cbbPmTyp);

            UIHelper.ClearSelectValidValues(cbbPmTyL);
            cbbPmTyL.ValidValues.Add("PT", STRING_CONTRANTS.PaymentType_PT);
            cbbPmTyL.ValidValues.Add("PC", STRING_CONTRANTS.PaymentType_PC);
            cbbPmTyL.ValidValues.Add("UC", STRING_CONTRANTS.PaymentType_UC);
            UIHelper.ComboboxSelectDefault(cbbPmTyL);
        }

        private void LoadDataGridReportDetail(string docnum)
        {
            if (this.grDt != null)
            {
                this.grDt.DataTable.Clear();
                this.grDt.DataTable.ExecuteQuery(string.Format(QueryString.LoadPaymentsReportDetail, docnum, _PaymentDocumentTypeReport.GetDescription(), BranchReport));

                var bpCaptionCode = _PaymentDocumentTypeReport == PaymentDocumentType.PT ? STRING_CONTRANTS.Title_CustomerCode : STRING_CONTRANTS.Title_VendorCode;

                this.grDt.Columns.Item("CardCode").TitleObject.Caption = bpCaptionCode;
                this.grDt.Columns.Item("CardCode").Editable = false;

                this.grDt.Columns.Item("CardName").Editable = false;
                this.grDt.Columns.Item("CardName").Visible = false;

                this.grDt.Columns.Item("Check").Visible = false;

                this.grDt.Columns.Item("DocNum").TitleObject.Caption = STRING_CONTRANTS.Title_DocNum;
                this.grDt.Columns.Item("DocNum").Editable = false;

                this.grDt.Columns.Item("InvCode").TitleObject.Caption = STRING_CONTRANTS.Title_InvCode;
                this.grDt.Columns.Item("InvCode").Editable = false;

                this.grDt.Columns.Item("DocDate").TitleObject.Caption = STRING_CONTRANTS.Title_DocDate;
                this.grDt.Columns.Item("DocDate").Editable = false;

                this.grDt.Columns.Item("DueDate").TitleObject.Caption = STRING_CONTRANTS.Title_DueDate;
                this.grDt.Columns.Item("DueDate").Editable = false;

                this.grDt.Columns.Item("DocCur").TitleObject.Caption = STRING_CONTRANTS.Title_Currency;
                this.grDt.Columns.Item("DocCur").Editable = false;

                this.grDt.Columns.Item("InsTotal").TitleObject.Caption = STRING_CONTRANTS.Title_InsTotal;
                this.grDt.Columns.Item("InsTotal").Editable = false;
                this.grDt.Columns.Item("MustPay").TitleObject.Caption = STRING_CONTRANTS.Title_MustPay;
                this.grDt.Columns.Item("MustPay").Editable = false;
                this.grDt.Columns.Item("MustPayAsCash").TitleObject.Caption = STRING_CONTRANTS.Title_PayAmount;
                this.grDt.Columns.Item("MustPayAsCash").Editable = false;
                //this.grDt.Columns.Item("MustPayAsBank").TitleObject.Caption = "Số tiền thanh toán (Bank)";
                this.grDt.Columns.Item("MustPayAsBank").Visible = false;

                this.grDt.Columns.Item("DocEntry").Visible = false;
                this.grDt.Columns.Item("Manual").Visible = false;

                SAPbouiCOM.EditTextColumn oCol2 = null;
                oCol2 = (SAPbouiCOM.EditTextColumn)this.grDt.Columns.Item("CardCode");
                oCol2.LinkedObjectType = SAPObjectType.oBusinessPartners;

                SAPbouiCOM.EditTextColumn oCol1 = null;
                oCol1 = (SAPbouiCOM.EditTextColumn)this.grDt.Columns.Item("DocNum");
                if (_PaymentDocumentTypeReport == PaymentDocumentType.PT)
                {
                    oCol1.LinkedObjectType = SAPObjectType.oInvoices;
                }
                else
                {
                    oCol1.LinkedObjectType = SAPObjectType.oPurchaseInvoices;
                }
                this.grDt.AutoResizeColumns();
                //ViewHelper.ColorGridRows(this.grDt, 0, true);
            }
        }

        private void LoadDataGridReport()
        {
            if (this.grHdr != null)
            {
                this.grHdr.DataTable.Clear();
                this.grHdr.DataTable.ExecuteQuery(string.Format(QueryString.LoadPaymentsReport, FromDateReport, ToDateReport, _PaymentDocumentTypeReport.GetDescription(), BranchReport));

                if (this.grHdr.DataTable.Rows.Count <= 0)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.StatusBar, false);
                    return;
                }
                this.grHdr.Columns.Item("PaymentKey").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentKey;
                this.grHdr.Columns.Item("PaymentKey").Editable = false;

                //this.grHdr.Columns.Item("Check").TitleObject.Caption = "Chọn";
                //this.grHdr.Columns.Item("Check").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                this.grHdr.Columns.Item("Check").Visible = false;

                this.grHdr.Columns.Item("PaymentType").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentType;// "Loại thanh toán";
                this.grHdr.Columns.Item("PaymentType").Editable = false;

                var bpCaptionCode = _PaymentDocumentTypeReport == PaymentDocumentType.PT ? STRING_CONTRANTS.Title_CustomerCode : STRING_CONTRANTS.Title_VendorCode;
                this.grHdr.Columns.Item("CardCode").TitleObject.Caption = bpCaptionCode;
                this.grHdr.Columns.Item("CardCode").Editable = false;

                this.grHdr.Columns.Item("DocNum").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocNum;
                this.grHdr.Columns.Item("DocNum").Editable = false;

                this.grHdr.Columns.Item("DocDate").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocDate;
                this.grHdr.Columns.Item("DocDate").Editable = false;

                this.grHdr.Columns.Item("DocCurr").TitleObject.Caption = STRING_CONTRANTS.Title_Currency;
                this.grHdr.Columns.Item("DocCurr").Editable = false;

                this.grHdr.Columns.Item("Cash").TitleObject.Caption = STRING_CONTRANTS.Title_MustPayAsCash;
                this.grHdr.Columns.Item("Cash").Editable = false;

                this.grHdr.Columns.Item("Bank").TitleObject.Caption = STRING_CONTRANTS.Title_MustPayAsBank;
                this.grHdr.Columns.Item("Bank").Editable = false;

                this.grHdr.Columns.Item("CreateName").TitleObject.Caption = STRING_CONTRANTS.Title_CreateBy;
                this.grHdr.Columns.Item("CreateName").Editable = false;

                this.grHdr.Columns.Item("StatusName").TitleObject.Caption = STRING_CONTRANTS.Title_Status;
                this.grHdr.Columns.Item("StatusName").Editable = false;
                this.grHdr.Columns.Item("Status").Visible = false;
                if (this._PaymentDocumentTypeReport != PaymentDocumentType.PT)
                {

                    this.grHdr.Columns.Item("CashAcct").Visible = false;
                    this.grHdr.Columns.Item("BankCode").Visible = false;
                    this.grHdr.Columns.Item("TrsfrAcct").Visible = false;


                    this.grHdr.Columns.Item("DocEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDraftEntry;
                    this.grHdr.Columns.Item("DocEntry").Editable = false;

                    this.grHdr.Columns.Item("PaymentEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocEntry;
                    this.grHdr.Columns.Item("PaymentEntry").Editable = false;

                    SAPbouiCOM.EditTextColumn oCol1 = null;
                    oCol1 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("PaymentEntry");
                    oCol1.LinkedObjectType = SAPObjectType.oVendorPayments;

                    SAPbouiCOM.EditTextColumn oCol2 = null;
                    oCol2 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("DocEntry");
                    oCol2.LinkedObjectType = SAPObjectType.oPaymentsDrafts;
                }
                else
                {
                    this.grHdr.Columns.Item("DocEntry").TitleObject.Caption = STRING_CONTRANTS.Title_PaymentDocEntry;
                    this.grHdr.Columns.Item("DocEntry").Editable = false;

                    SAPbouiCOM.EditTextColumn oCol1 = null;
                    oCol1 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("DocEntry");
                    oCol1.LinkedObjectType = SAPObjectType.oIncomingPayments;
                }
                this.grHdr.CollapseLevel = 1;
                this.grHdr.AutoResizeColumns();
                SAPbouiCOM.EditTextColumn oCol = null;
                oCol = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("CardCode");
                oCol.LinkedObjectType = SAPObjectType.oBusinessPartners;

                //SAPbouiCOM.EditTextColumn oCol1 = null;
                //oCol1 = (SAPbouiCOM.EditTextColumn)this.grHdr.Columns.Item("DocEntry");
                //if (_PaymentDocumentTypeReport == PaymentDocumentType.PT)
                //{
                //    oCol1.LinkedObjectType = SAPObjectType.oIncomingPayments;
                //}
                //else
                //{
                //    oCol1.LinkedObjectType = SAPObjectType.oVendorPayments;
                //}

                ViewHelper.ColorGridRows(this.grHdr, 0, true);
                var docnum = this.grHdr.DataTable.GetValue("DocNum", 0).ToString();
                if (!string.IsNullOrEmpty(docnum))
                {
                    LoadDataGridReportDetail(docnum);
                }
            }
        }
        private void LoadDataGridCreate(bool isAfter = false, bool isHistory = false, string keylog = "")
        {
            this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = CardCode;

            if (this.grData != null)
            {
                this.grData.DataTable.Clear();

                var history = "N";
                if (isHistory)
                {
                    isAfter = true;
                    history = "Y";
                }

                var after = "N";
                if (isAfter)
                {
                    after = "Y";
                    btnCreate.Item.Enabled = false;

                }
                else
                {

                    btnCreate.Item.Enabled = true;
                }

                this.grData.DataTable.ExecuteQuery(string.Format(QueryString.LoadInvoicesToPayment, FromDate, ToDate, _PaymentType.GetDescription(), Branch, CardCodeFilter, after, history, keylog));

                if (this.grData.DataTable.Rows.Count <= 0)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.StatusBar, false);
                    return;
                }

                var bpCaptionCode = _PaymentType == PaymentType.T ? STRING_CONTRANTS.Title_CustomerCode : STRING_CONTRANTS.Title_VendorCode;
                var bpCaption = _PaymentType == PaymentType.T ? STRING_CONTRANTS.Title_CustomerName : STRING_CONTRANTS.Title_VendorName;

                //this.grData.Columns.Item("CardCode").TitleObject.Caption = bpCaptionCode;
                // this.grData.Columns.Item("CardCode").Editable = false;

                this.grData.Columns.Item("CardCode").ColumnConfig(bpCaptionCode, false);

                this.grData.Columns.Item("CardName").TitleObject.Caption = bpCaption;
                this.grData.Columns.Item("CardName").Editable = false;


                if (!isAfter)
                {
                    this.grData.Columns.Item("Check").ColumnConfig(STRING_CONTRANTS.Title_Choose, true, true, BoGridColumnType.gct_CheckBox);

                    //this.grData.Columns.Item("Check").TitleObject.Caption = STRING_CONTRANTS.Title_Choose;
                    //this.grData.Columns.Item("Check").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                    //this.grData.Columns.Item("Check").Editable = true;

                    //this.grData.Columns.Item("HasRequest").Visible = false;

                    this.grData.Columns.Item("Message").Visible = false;
                }
                else
                {
                    this.grData.Columns.Item("Check").Visible = false;
                    this.grData.Columns.Item("Message").TitleObject.Caption = "Ghi chú";
                    this.grData.Columns.Item("Message").Visible = true;
                }

                this.grData.Columns.Item("DocNum").TitleObject.Caption = STRING_CONTRANTS.Title_DocNum;
                this.grData.Columns.Item("DocNum").Editable = false;

                this.grData.Columns.Item("InvCode").TitleObject.Caption = STRING_CONTRANTS.Title_InvCode;
                this.grData.Columns.Item("InvCode").Editable = false;

                // this.grData.Columns.Item("HasRequest").TitleObject.Caption = ;
                this.grData.Columns.Item("HasRequest").Visible = false;
                // this.grData.Columns.Item("Message").TitleObject.Caption ="Ghi chú";
                this.grData.Columns.Item("Message").Editable = false;

                this.grData.Columns.Item("DocDate").TitleObject.Caption = STRING_CONTRANTS.Title_DocDate;
                this.grData.Columns.Item("DocDate").Editable = false;

                this.grData.Columns.Item("DueDate").TitleObject.Caption = STRING_CONTRANTS.Title_DueDate;
                this.grData.Columns.Item("DueDate").Editable = false;

                this.grData.Columns.Item("PostingDate").TitleObject.Caption = STRING_CONTRANTS.Title_PostingDate;
                this.grData.Columns.Item("PostingDate").Visible = false;

                this.grData.Columns.Item("DocCur").TitleObject.Caption = STRING_CONTRANTS.Title_Currency;
                this.grData.Columns.Item("DocCur").Editable = false;

                this.grData.Columns.Item("JrnlMemo").TitleObject.Caption = STRING_CONTRANTS.Title_Remark;
                this.grData.Columns.Item("JrnlMemo").Editable = false;

                this.grData.Columns.Item("InsTotal").TitleObject.Caption = STRING_CONTRANTS.Title_InsTotal;
                this.grData.Columns.Item("InsTotal").Editable = false;

                this.grData.Columns.Item("InsTotalFC").TitleObject.Caption = STRING_CONTRANTS.Title_InsTotalFC;
                this.grData.Columns.Item("InsTotalFC").Editable = false;

                this.grData.Columns.Item("DocRate").TitleObject.Caption = STRING_CONTRANTS.Title_Rate;
                this.grData.Columns.Item("DocRate").Editable = true;
                //this.grData.Columns.Item("InsTotal").Type = BoGridColumnType.

                this.grData.Columns.Item("MustPay").TitleObject.Caption = STRING_CONTRANTS.Title_MustPay;
                this.grData.Columns.Item("MustPay").Editable = false;

                this.grData.Columns.Item("Account").TitleObject.Caption = STRING_CONTRANTS.Title_Account;
                this.grData.Columns.Item("Account").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
                this.grData.Columns.Item("Account").Editable = true;

                this.grData.Columns.Item("MustPayAsCash").TitleObject.Caption = STRING_CONTRANTS.Title_MustPayAsCash;
                this.grData.Columns.Item("MustPayAsCash").Editable = true;// _PaymentMethod == PaymentMethod.Cash;
                this.grData.Columns.Item("MustPayAsCash").LostFocusAfter += FrmPayment_LostFocusAfter;

                this.grData.Columns.Item("MustPayAsBank").TitleObject.Caption = STRING_CONTRANTS.Title_MustPayAsBank;
                this.grData.Columns.Item("MustPayAsBank").Editable = true;// _PaymentMethod != PaymentMethod.Cash;
                this.grData.Columns.Item("MustPayAsBank").LostFocusAfter += FrmPayment_LostFocusAfter;

                this.grData.Columns.Item("DocEntry").Visible = false;

                this.grData.Columns.Item("Manual").TitleObject.Caption = "Dữ liệu từ";
                this.grData.Columns.Item("Manual").Editable = false;
                this.grData.Columns.Item("Manual").Visible = false;

                this.grData.Columns.Item("Bank").TitleObject.Caption = STRING_CONTRANTS.Title_Bank;
                this.grData.Columns.Item("Bank").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
                this.grData.Columns.Item("Bank").Visible = true;
                this.grData.Columns.Item("Bank").Editable = true;

                this.grData.Columns.Item("CFlow").TitleObject.Caption = STRING_CONTRANTS.Title_CFlow;
                this.grData.Columns.Item("CFlow").Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
                this.grData.Columns.Item("CFlow").Visible = true;
                this.grData.Columns.Item("CFlow").Editable = true;

                SAPbouiCOM.EditTextColumn oCol2 = null;
                oCol2 = (SAPbouiCOM.EditTextColumn)this.grData.Columns.Item("CardCode");
                oCol2.LinkedObjectType = SAPObjectType.oBusinessPartners;

                SAPbouiCOM.EditTextColumn oCol1 = null;
                oCol1 = (SAPbouiCOM.EditTextColumn)this.grData.Columns.Item("DocNum");
                if (_PaymentDocumentType == PaymentDocumentType.PT)
                {
                    oCol1.LinkedObjectType = SAPObjectType.oInvoices;
                }
                else
                {
                    oCol1.LinkedObjectType = SAPObjectType.oPurchaseInvoices;
                }

                this.grData.CollapseLevel = 1;
                RemoveEmptyRow();
                this.grData.AutoResizeColumns();

                ViewHelper.ColorGridRows(this.grData, 0, true);

                var comboAcct = (SAPbouiCOM.ComboBoxColumn)this.grData.Columns.Item("Account");

                comboAcct.DisplayType = BoComboDisplayType.cdt_Description;
                foreach (var data in ViewHelper.Banks)
                {
                    comboAcct.ValidValues.Add(data.Code, data.Name);
                }
                //comboAcct.


                var comboBank = (SAPbouiCOM.ComboBoxColumn)this.grData.Columns.Item("Bank");
                comboBank.DisplayType = BoComboDisplayType.cdt_Description;
                foreach (var data in ViewHelper.Banks)
                {
                    comboBank.ValidValues.Add(data.Code, data.Name);
                }

                var comboCashflow = (SAPbouiCOM.ComboBoxColumn)this.grData.Columns.Item("CFlow");
                comboCashflow.DisplayType = BoComboDisplayType.cdt_Description;

                comboCashflow.ValidValues.Add("-", STRING_CONTRANTS.NoChooseCashFlow);
                foreach (var data in ViewHelper.CashFlows)
                {
                    comboCashflow.ValidValues.Add(data.Id, data.Name);
                }

                //if (txtCusVen != null)

            }
        }


        private void FrmPayment_LostFocusAfter(object sboObject, SBOItemEventArg pVal)
        {
            this.Freeze(true);
            try
            {
                var colTotal = "MustPay";
                var rowIndex = pVal.Row;
                var colIndex = ((GridColumnClass)sboObject).UniqueID;

                var cellValue = grData.DataTable.GetValue(colIndex, rowIndex).ToString();
                decimal value = 0;
                if (!decimal.TryParse(cellValue, out value))
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.WrongFormatCurrency, UIHelper.MsgType.Msgbox);
                    grData.DataTable.SetValue(colIndex, rowIndex, 0);
                    this.Freeze(false);
                    return;
                }

                var cellMustpay = grData.DataTable.GetValue(colTotal, rowIndex).ToString();
                decimal mustpay = 0;
                if (!decimal.TryParse(cellMustpay, out mustpay))
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.OverTotalPayment, UIHelper.MsgType.Msgbox);
                    grData.DataTable.SetValue(colIndex, rowIndex, 0);
                    this.Freeze(false);
                    return;
                }

                if (value > mustpay)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.OverTotalPayment, UIHelper.MsgType.Msgbox);
                    grData.DataTable.SetValue(colIndex, rowIndex, mustpay);
                }
            }
            catch (Exception ex)
            { }
            this.Freeze(false);
        }

        private void RemoveEmptyRow()
        {
            if (this.grData.Rows.Count > 0)
            {
                for (var index = this.grData.DataTable.Rows.Count - 1; index >= 0; index--)
                {
                    var cardcode = this.grData.DataTable.GetValue("CardCode", index).ToString();
                    if (string.IsNullOrEmpty(cardcode))
                    {
                        this.grData.DataTable.Rows.Remove(index);
                    }
                }
            }
        }


        //private void CreatePayment()
        //{
        //    var message = string.Empty;
        //    var payments = new List<PaymentDetail>();
        //    var currency = string.Empty;
        //    for (var i = 0; i < SelectedDataIndexs.Count; i++)
        //    {
        //        var index = SelectedDataIndexs[i];
        //        if (this.grData.DataTable.GetValue("Check", index).ToString() == "Y")
        //        {
        //            var data = new PaymentDetail();
        //            data.CardType = _PaymentType == PaymentType.T ? "C" : "S";
        //            data.CardCode = this.grData.DataTable.GetValue("CardCode", index).ToString();
        //            if (this.grData.DataTable.GetValue("Manual", index).ToString() == "Data")
        //            {
        //                data.DocNum = this.grData.DataTable.GetValue("DocNum", index).ToString();
        //                data.InvCode = this.grData.DataTable.GetValue("InvCode", index).ToString();
        //                data.DocEntry = this.grData.DataTable.GetValue("DocEntry", index).ToString();

        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("DocDate", index).ToString()))
        //                {
        //                    DateTime docdate;
        //                    if (DateTime.TryParse(this.grData.DataTable.GetValue("DocDate", index).ToString(), out docdate))
        //                    {
        //                        data.DocDate = docdate;
        //                    }
        //                }
        //            }
        //            //data.PaymentDate = 
        //            //if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("PostingDate", index).ToString()))
        //            //{
        //            DateTime duedate;
        //            if (DateTime.TryParseExact(PostingDate, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out duedate))
        //            // DateTime.TryParse(PostingDate, out duedate))
        //            {
        //                data.PaymentDate = duedate;
        //            }
        //            data.Remark = Note;
        //            //}

        //            decimal total = 0;
        //            if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPay", index).ToString()))
        //            {
        //                decimal mustpay;
        //                if (decimal.TryParse(this.grData.DataTable.GetValue("MustPay", index).ToString(), out mustpay))
        //                {
        //                    total = mustpay;
        //                }
        //            }

        //            if (_PaymentMethod == PaymentMethod.Cash)
        //            {
        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPayAsCash", index).ToString()))
        //                {
        //                    decimal mustpay;
        //                    if (decimal.TryParse(this.grData.DataTable.GetValue("MustPayAsCash", index).ToString(), out mustpay))
        //                    {
        //                        data.AmountCash = mustpay;
        //                        data.Amount = mustpay;
        //                    }
        //                }
        //            }
        //            else if (_PaymentMethod == PaymentMethod.Bank)
        //            {
        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPayAsBank", index).ToString()))
        //                {
        //                    decimal mustpay;
        //                    if (decimal.TryParse(this.grData.DataTable.GetValue("MustPayAsBank", index).ToString(), out mustpay))
        //                    {
        //                        data.AmountBank = mustpay;
        //                        data.Amount = mustpay;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                decimal cash = 0;
        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPayAsCash", index).ToString()))
        //                {
        //                    decimal mustpay;
        //                    if (decimal.TryParse(this.grData.DataTable.GetValue("MustPayAsCash", index).ToString(), out mustpay))
        //                    {
        //                        cash = mustpay;
        //                        //data.Amount = mustpay;
        //                    }
        //                }
        //                decimal bank = 0;
        //                if (!string.IsNullOrEmpty(this.grData.DataTable.GetValue("MustPayAsBank", index).ToString()))
        //                {
        //                    decimal mustpay;
        //                    if (decimal.TryParse(this.grData.DataTable.GetValue("MustPayAsBank", index).ToString(), out mustpay))
        //                    {
        //                        bank = mustpay;
        //                        //data.Amount = mustpay;
        //                    }
        //                }
        //                data.AmountBank = bank;
        //                data.AmountCash = cash;
        //                data.Amount = bank + cash;
        //            }

        //            var bankCode = this.grData.DataTable.GetValue("Bank", index).ToString();
        //            var cfID = this.grData.DataTable.GetValue("CFlow", index).ToString();

        //            if (string.IsNullOrEmpty(bankCode))
        //                bankCode = BankCode;
        //            data.BankInfo = bankCode;
        //            data.BankAccount = ViewHelper.Banks.Where(x => x.Code == bankCode).Select(x => x.Account).FirstOrDefault();

        //            var cashAccount = this.grData.DataTable.GetValue("Account", index).ToString();
        //            if (string.IsNullOrEmpty(cashAccount))
        //                cashAccount = CashCode;
        //            var CashAccount = ViewHelper.Banks.Where(x => x.Code == cashAccount).Select(x => x.Account).FirstOrDefault();
        //            data.Account = CashAccount;// this.grData.DataTable.GetValue("Account", index).ToString();

        //            data.Currency = this.grData.DataTable.GetValue("DocCur", index).ToString();
        //            if (data.Currency != GlobalsConfig.Instance.LocalCurrencyDefault)
        //            {
        //                decimal rate = 0;
        //                decimal.TryParse(this.grData.DataTable.GetValue("DocRate", index).ToString(), out rate);
        //                data.Rate = rate;
        //                // }
        //            }

        //            if (string.IsNullOrEmpty(cfID) || cfID == "-")
        //                cfID = CashFlowID;

        //            if (string.IsNullOrEmpty(cfID) || cfID == "-")
        //            {
        //                UIHelper.LogMessage(STRING_CONTRANTS.NoCFlow, UIHelper.MsgType.Msgbox, true);
        //                return;
        //            }

        //            data.Cashflow = cfID;

        //            if (data.Amount == 0)
        //            {
        //                UIHelper.LogMessage(STRING_CONTRANTS.PaymentZero, UIHelper.MsgType.Msgbox, true);
        //                return;
        //            }

        //            if (total < data.Amount)
        //            {
        //                UIHelper.LogMessage(STRING_CONTRANTS.OverTotalPayment, UIHelper.MsgType.Msgbox, true);
        //                return;
        //            }
        //            payments.Add(data);
        //            // }

        //        }
        //    }

        //    if (payments.Count <= 0)
        //    {
        //        UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.Msgbox, true);
        //        return;
        //    }
        //    var numberBranch = DataHelper.LoadKey(Branch, _PaymentDocumentType.GetDescription());
        //    if (numberBranch == null)
        //    {
        //        UIHelper.LogMessage(STRING_CONTRANTS.CanNotGenerateKey, UIHelper.MsgType.Msgbox, true);
        //        return;
        //    }
        //    var key = $"WL-{numberBranch.BranchID}-{_PaymentDocumentType.GetDescription()}-{DateTime.Now.Year}-{DateTime.Now.Month}-{numberBranch.Number.ToString("D5")}";

        //    var ret = PaymentViaDI.CreatePaymnents(_PaymentType, payments, key, _PaymentDocumentType, _PaymentMethod, Branch, ref message);

        //    var success = ret.Where(x => x.Flag).Count();
        //    var error = ret.Count - success;

        //    var mess = string.Format(STRING_CONTRANTS.Notice_EndAction, STRING_CONTRANTS.Action_Create, success, ret.Count, error);
        //    UIHelper.LogMessage(mess, UIHelper.MsgType.StatusBar);

        //    if (error > 0)
        //    {
        //        var errMessage = string.Empty;
        //        foreach (var data in ret.Where(x => !x.Flag))
        //        {
        //            errMessage += $"Thanh toán cho/của {data.CardCode} lỗi {data.Message} \n";
        //        }
        //        UIHelper.LogMessage(errMessage, UIHelper.MsgType.Msgbox);
        //    }

        //    LoadDataGridCreate();

        //}

        private PaymentDetail DataInRow(int index, ref bool isReturn, ref string message)
        {
            try
            {
                var data = new PaymentDetail();
                var cfID = this.grData.GetValueCustom("CFlow", index);

                if (string.IsNullOrEmpty(cfID) || cfID == "-")
                    cfID = CashFlowID;

                if (string.IsNullOrEmpty(cfID) || cfID == "-")
                {
                    message = STRING_CONTRANTS.NoCFlow;
                    isReturn = true;
                    return null;
                }

                var currency = this.grData.DataTable.GetValue("DocCur", index).ToString();
                if (string.IsNullOrEmpty(currency))
                {
                    message = STRING_CONTRANTS.NoCurrency;
                    isReturn = true;
                    return null;
                }

                data.Cashflow = cfID;
                data.Currency = currency;

                var bankCode = this.grData.GetValueCustom("Bank", index);
                if (string.IsNullOrEmpty(bankCode))
                    bankCode = BankCode;
                var cashAccount = this.grData.GetValueCustom("Account", index);
                if (string.IsNullOrEmpty(cashAccount))
                    cashAccount = CashCode;

                var BankAccount = ViewHelper.Banks.Where(x => x.Code == bankCode).Select(x => x.Account).FirstOrDefault();
                var CashAccount = ViewHelper.Banks.Where(x => x.Code == cashAccount).Select(x => x.Account).FirstOrDefault();

                data.BankInfo = bankCode;

                switch (_PaymentMethod)
                {
                    case PaymentMethod.Cash:
                        if (string.IsNullOrEmpty(CashAccount))
                        {
                            message = STRING_CONTRANTS.NoAccount;
                            isReturn = true;
                            return null;
                        }
                        break;
                    case PaymentMethod.Bank:
                        if (string.IsNullOrEmpty(BankAccount))
                        {
                            message = STRING_CONTRANTS.NoBank;
                            isReturn = true;
                            return null;
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(CashAccount) || string.IsNullOrEmpty(BankAccount))
                        {
                            message = STRING_CONTRANTS.NoAccount + " hoặc " + STRING_CONTRANTS.NoBank;
                            isReturn = true;
                            return null;
                        }
                        break;
                }

                data.BankAccount = BankAccount;// ViewHelper.Banks.Where(x => x.Code == bankCode).Select(x => x.Account).FirstOrDefault();  
                data.Account = CashAccount;// this.grData.DataTable.GetValue("Account", index).ToString();

                data.CardType = _PaymentType == PaymentType.T ? "C" : "S";
                data.CardCode = this.grData.GetValueCustom("CardCode", index);
                if (this.grData.GetValueCustom("Manual", index) == "Data")
                {
                    data.DocNum = this.grData.GetValueCustom("DocNum", index);
                    data.InvCode = this.grData.GetValueCustom("InvCode", index);
                    data.DocEntry = this.grData.GetValueCustom("DocEntry", index);
                    DateTime docdate = default(DateTime);
                    if (CustomConverter.ConvertStringToDate(this.grData.GetValueCustom("DocDate", index), ref docdate))
                    {
                        data.DocDate = docdate;
                    }
                }
                DateTime duedate;
                if (DateTime.TryParseExact(PostingDate, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out duedate))
                {
                    data.PaymentDate = duedate;
                }
                data.Remark = Note;

                decimal total = 0;
                decimal mustpay = 0;
                if (CustomConverter.ConvertStringToDecimal(this.grData.GetValueCustom("MustPay", index), ref mustpay))
                {
                    total = mustpay;
                }

                decimal mustpayCash = -1;
                decimal mustpayBank = -1;
                var isCash = CustomConverter.ConvertStringToDecimal(this.grData.GetValueCustom("MustPayAsCash", index), ref mustpayCash);
                var isBank = CustomConverter.ConvertStringToDecimal(this.grData.GetValueCustom("MustPayAsBank", index), ref mustpayBank);

                if (_PaymentMethod == PaymentMethod.Cash)
                {
                    if (isCash)
                    {
                        data.AmountCash = mustpayCash;
                        data.Amount = mustpayCash;
                    }
                }
                else if (_PaymentMethod == PaymentMethod.Bank)
                {
                    if (isBank)
                    {
                        data.AmountBank = mustpayBank;
                        data.Amount = mustpayBank;
                    }
                }
                else
                {
                    decimal cash = 0;
                    if (isCash)
                    {
                        cash = mustpayCash;
                    }
                    decimal bank = 0;
                    if (isBank)
                    {
                        bank = mustpayBank;
                    }
                    data.AmountBank = bank;
                    data.AmountCash = cash;
                    data.Amount = bank + cash;
                }

                if (data.Currency != GlobalsConfig.Instance.LocalCurrencyDefault)
                {
                    decimal rate = 0;
                    decimal.TryParse(this.grData.GetValueCustom("DocRate", index), out rate);
                    data.Rate = rate;
                }
                if (data.Amount == 0)
                {
                    message = STRING_CONTRANTS.PaymentZero;
                    isReturn = true;
                    return null;
                }

                if (total < data.Amount)
                {
                    message = STRING_CONTRANTS.OverTotalPayment;
                    isReturn = true;
                    return null;
                }
                isReturn = false;
                return data;
            }
            catch (Exception ex)
            {
                message = $"Error {ex.Message}";
                isReturn = true;
                return null;
            }

        }
        /// <summary>
        /// New version with log
        /// </summary>
        private void CreatePayments()
        {
            var message = string.Empty;
            var paymentDts = new List<PaymentDetail>();
            var currency = string.Empty;
            var isReturn = false;

            var keyLog = $"PM_{DateTime.Now.ToString("yyMMddHHmmss")}";
            UIHelper.LogMessage("Bắt đầu lấy dữ liệu", UIHelper.MsgType.StatusBar, false);
            for (var i = 0; i < SelectedDataIndexs.Count; i++)
            {
                var index = SelectedDataIndexs[i];

                if (this.grData.GetValueCustom("Check", index) != "Y")
                {
                    continue;
                }

                var data = DataInRow(index, ref isReturn, ref message);

                UIHelper.LogMessage("Hoàn tất lấy dữ liệu", UIHelper.MsgType.StatusBar, false);
                if (isReturn)
                {
                    UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar, true);
                    return;
                }
                paymentDts.Add(data);
            }


            UIHelper.LogMessage("Bắt đầu kiểm tra dữ liệu", UIHelper.MsgType.StatusBar, false);

            if (paymentDts.Count <= 0)
            {
                UIHelper.LogMessage(STRING_CONTRANTS.NoData, UIHelper.MsgType.StatusBar, true);
                return;
            }

            var bpList = paymentDts.Select(x => x.CardCode).Distinct().ToList();
            var paymentList = new List<PaymentDocument>();
            foreach (var cardCode in bpList)
            {
                var payment = new PaymentDocument();
                payment.Details = new List<PaymentDetail>();
                payment.CardCode = cardCode;

                var invoices = paymentDts.Where(x => x.CardCode == cardCode).ToList();
                var currencies = invoices.Select(x => x.Currency).Distinct().ToList();

                payment.Details.AddRange(invoices);
                if (currencies.Count > 1 || currencies[0] == "")
                {
                    payment.Message = STRING_CONTRANTS.DifferenceCurrency;
                    payment.Error = true;
                    paymentList.Add(payment);
                    continue;
                }
                else
                {
                    payment.Currency = currencies[0];
                    decimal rate = 1;
                    if (currencies[0] != GlobalsConfig.Instance.LocalCurrencyDefault)
                    {
                        var rate2 = invoices.Select(x => x.Rate).Distinct().ToList();
                        if (rate2.Count > 1)
                        {
                            payment.Message = STRING_CONTRANTS.DifferenceRate;
                            payment.Error = true;
                            paymentList.Add(payment);
                            continue;
                        }
                        rate = rate2[0];
                    }
                    payment.Rate = rate;
                }

                var banks = invoices.Select(x => x.BankInfo).Distinct().ToList();
                if (banks.Count > 1)
                {
                    payment.Message = STRING_CONTRANTS.DifferenceBank;
                    payment.Error = true;
                    paymentList.Add(payment);
                    continue;
                }
                else
                {
                    payment.Bank = banks[0];
                }

                var cfs = invoices.Select(x => x.Cashflow).Distinct().ToList();
                if (cfs.Count > 1)
                {
                    payment.Message = STRING_CONTRANTS.DifferenceCashFlow;
                    payment.Error = true;
                    paymentList.Add(payment);
                    continue;
                }
                else
                {
                    payment.Cashflow = cfs[0];
                }
                var accounts = invoices.Select(x => x.Account).Distinct().ToList();
                if (accounts.Count > 1)
                {
                    payment.Message = STRING_CONTRANTS.DifferenceAccount;
                    payment.Error = true;
                    paymentList.Add(payment);
                    continue;
                }
                else
                {
                    payment.Account = accounts[0];
                }
                var bankAccounts = invoices.Select(x => x.BankAccount).Distinct().ToList();
                if (accounts.Count > 1)
                {
                    payment.Message = STRING_CONTRANTS.DifferenceBank;
                    payment.Error = true;
                    paymentList.Add(payment);
                    continue;
                }
                else
                {
                    payment.BankAccount = bankAccounts[0];
                }
                payment.PaymentDate = invoices.Select(x => x.PaymentDate).Distinct().FirstOrDefault();
                payment.Remark = invoices.Select(x => x.Remark).Distinct().FirstOrDefault();

                payment.Error = false;
                paymentList.Add(payment);
            }
            UIHelper.LogMessage("Hoàn tất kiểm tra dữ liệu", UIHelper.MsgType.StatusBar, false);

            var isError = paymentList.Where(x => x.Error).ToList().Count() > 0;
            if (isError)
            {
                UIHelper.LogMessage("Hoàn tất kiểm tra dữ liệu. Có xuất hiện lỗi, Vui lòng kiểm tra lại", UIHelper.MsgType.StatusBar, true);
                var query = string.Empty;
                foreach (var pm in paymentList.Where(X => X.Error))
                {
                    foreach (var dt in pm.Details)
                    {
                        //if (query != string.Empty)
                        //   query += ";\n";
                        query = "Call \"" + DIConnection.Instance.CompanyDB + "\".\"SP_LogPaymentTool\" ('" + pm.CardCode + "', '" + dt.DocEntry + "', '" + _PaymentType + "', '', 'F', '" + pm.Message + "', '" + keyLog + "') ";

                        var ret1 = dbProvider.ExecuteNonQuery(query);
                    }
                }
            }
            else
            {
                var numberBranch = DataHelper.LoadKey(Branch, _PaymentDocumentType.GetDescription());
                if (numberBranch == null)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.CanNotGenerateKey, UIHelper.MsgType.StatusBar, true);
                    return;
                }
                var key = $"WL-{numberBranch.BranchID}-{_PaymentDocumentType.GetDescription()}-{DateTime.Now.Year}-{DateTime.Now.Month}-{numberBranch.Number.ToString("D5")}";

                UIHelper.LogMessage("Bắt đầu tạo thanh toán", UIHelper.MsgType.StatusBar, false);
                var ret = PaymentViaDI.CreatePayments(_PaymentType, paymentList, key, _PaymentDocumentType, _PaymentMethod, Branch, ref message);

                var flag = "";

                if (ret.Where(x => !x.Flag).Count() > 0)
                {
                    UIHelper.LogMessage("Có xuất hiện lỗi, Vui lòng kiểm tra lại", UIHelper.MsgType.StatusBar, true);
                    flag = "F";

                }
                else
                {
                    UIHelper.LogMessage("Hoàn tất tạo thanh toán", UIHelper.MsgType.StatusBar, false);
                    flag = "S";
                }
                var query = string.Empty;
                foreach (var detail in ret)
                {
                    foreach (var dt in paymentList.Where(x => x.CardCode == detail.CardCode).FirstOrDefault().Details)
                    {
                        // if (query != string.Empty)
                        //  query += ";\n";
                        query = "Call \"" + DIConnection.Instance.CompanyDB + "\".\"SP_LogPaymentTool\" ('" + dt.CardCode + "', '" + dt.DocEntry + "', '" + _PaymentType + "', '" + key + "', '" + flag + "', '" + (flag == "F" ? detail.Message : "") + "', '" + keyLog + "') ";

                        var ret1 = dbProvider.ExecuteNonQuery(query);
                    }
                }
            }
            LoadDataGridCreate(true, false, keyLog);
        }
        private void AutoFillData(int selectedIndex = -1)
        {
            this.Freeze(true);
            try
            {
                if (selectedIndex == -1)
                {
                    foreach (var index in SelectedDataIndexs)
                    {
                        var mustpay = this.grData.DataTable.GetValue("MustPay", index);
                        if (_PaymentMethod == PaymentMethod.CashBank)
                        {
                            this.grData.DataTable.SetValue("MustPayAsCash", index, mustpay);
                            this.grData.DataTable.SetValue("MustPayAsBank", index, 0);
                        }
                        else if (_PaymentMethod == PaymentMethod.Cash)
                        {

                            this.grData.DataTable.SetValue("MustPayAsCash", index, mustpay);
                            this.grData.DataTable.SetValue("MustPayAsBank", index, 0);
                        }
                        else
                        {

                            this.grData.DataTable.SetValue("MustPayAsCash", index, 0);
                            this.grData.DataTable.SetValue("MustPayAsBank", index, mustpay);
                        }

                        //var bank = this.grData.DataTable.GetValue("Bank", index);
                        //var cashflow = this.grData.DataTable.GetValue("CFlow", index);
                    }
                }
                else
                {
                    var mustpay = this.grData.DataTable.GetValue("MustPay", selectedIndex);
                    if (_PaymentMethod == PaymentMethod.CashBank)
                    {
                        this.grData.DataTable.SetValue("MustPayAsCash", selectedIndex, mustpay);
                        this.grData.DataTable.SetValue("MustPayAsBank", selectedIndex, 0);
                    }
                    else if (_PaymentMethod == PaymentMethod.Cash)
                    {

                        this.grData.DataTable.SetValue("MustPayAsCash", selectedIndex, mustpay);
                        this.grData.DataTable.SetValue("MustPayAsBank", selectedIndex, 0);
                    }
                    else
                    {

                        this.grData.DataTable.SetValue("MustPayAsCash", selectedIndex, 0);
                        this.grData.DataTable.SetValue("MustPayAsBank", selectedIndex, mustpay);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            this.Freeze(false);
        }
        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            instance = null;
            IsFormOpen = false;
        }

        private void btnFind_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

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
            try
            {
                LoadDataGridCreate();
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndLoadData);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private void btnCreate_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            UIHelper.LogMessage(STRING_CONTRANTS.Notice_CreatePayment);
            try
            {
                CreatePayments();
                //UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndCreatePayment);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_CreatePayment, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            SetControlLocation();
            this.Freeze(false);
        }

        private void cbbType_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            LoadPaymentTypeCombobox();
            this.Freeze(false);
        }

        private void grData_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            this.Freeze(true);
            if (pVal.Row < 0)
            {
                this.Freeze(false);
                return;
            }
            if (pVal.ColUID != "Check")
            {
                this.Freeze(false);
                return;
            }

            UIHelper.CheckInGrid(this.grData, pVal.Row, SelectedDataIndexs, "CardCode");

            foreach (var index in SelectedDataIndexs)
            {
                AutoFillData(index);
            }

            EnableButton();

            //this.grData.Rows.se
            ViewHelper.ColorGridRows(this.grData, 0, true);
            this.Freeze(false);
        }

        private void EnableButton()
        {
            if (SelectedDataIndexs.Count > 0)
            {
                this.btnCreate.Item.Enabled = true;
                this.btnUnck.Item.Enabled = true;
            }
            else
            {
                this.btnCreate.Item.Enabled = false;
                this.btnUnck.Item.Enabled = false;
            }

            if (SelectedDataIndexs.Count == this.grData.DataTable.Rows.Count)
            {
                this.btnChAl.Item.Enabled = false;
            }
            else
            {
                this.btnChAl.Item.Enabled = true;
            }
        }

        private void grData_DoubleClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

        }

        private void btnAddL_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //this.Freeze(true);
            if (this.grData == null || this.grData.DataTable == null)
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadDataBefAdd, UIHelper.MsgType.StatusBar);
                return;
            }
            try
            {
                frmAddPaymentLine.Instance.OnInputPaymentData += Instance_OnInputPaymentData;
                frmAddPaymentLine.Instance.ShowForm(_PaymentType);
            }
            catch (Exception ex)
            { }
            // this.Freeze(false);
        }

        private void Instance_OnInputPaymentData(object sender, Models.PaymentEventArgs e)
        {
            try
            {
                var data = e.Selected;
                ((frmAddPaymentLine)sender).Close();
                if (data == null)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.Notice_NoDataToAdd, UIHelper.MsgType.StatusBar);
                    return;
                }

                if (this.grData == null || this.grData.DataTable == null)
                {
                    UIHelper.LogMessage(STRING_CONTRANTS.Notice_NoDataToAdd, UIHelper.MsgType.StatusBar);
                    return;
                }

                this.Freeze(true);
                this.grData.DataTable.Rows.Add();
                var index = grData.DataTable.Rows.Count - 1;

                //this.grData.DataTable.SetValue("Check", index, "Y");
                this.grData.DataTable.SetValue("CardCode", index, data.CardCode);
                this.grData.DataTable.SetValue("CardName", index, data.CardName);
                this.grData.DataTable.SetValue("DocCur", index, data.Currency);
                this.grData.DataTable.SetValue("MustPay", index, data.Amount.ToString());
                //this.grData.DataTable.SetValue("DueDate", index, data.PaymentDate);


                //this.grData.CommonSetting.SetCellEditable(index, 11, true);
                //this.grData.CommonSetting.SetCellEditable(index, 11, true);


                //this.grData.
                this.Freeze(false);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage($"Error ", UIHelper.MsgType.StatusBar, true);
            }
        }

        private void btnRem_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            try
            {

            }
            catch (Exception ex)
            { }
            this.Freeze(false);
        }

        private void optGr_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {

        }

        private CheckBox ckbGr;

        private void CheckBox0_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (IsGroup != this.ckbGr.Checked)
            {
                this.Freeze(true);
                //GroupData();

                IsGroup = this.ckbGr.Checked;
                this.Freeze(false);
            }
        }

        private void cbbFeeT_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            AutoFillData();
            if (_PaymentMethod == PaymentMethod.CashBank)
            {
                this.cbbBankAccount.Item.Enabled = true;
                this.cbbAccCa.Item.Enabled = true;
            }
            else if (_PaymentMethod == PaymentMethod.Cash)
            {

                this.cbbBankAccount.Item.Enabled = false;
                this.cbbAccCa.Item.Enabled = true;
            }
            else
            {

                this.cbbBankAccount.Item.Enabled = true;
                this.cbbAccCa.Item.Enabled = false;
            }
            //ChangePaymentMethod();
        }

        private void btnFindL_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

            if (string.IsNullOrEmpty(FromDateReport) || string.IsNullOrEmpty(ToDateReport))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_DateSelectNull, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            if (!StringUtils.CheckFromDateEarlyToDate(FromDateReport, ToDateReport))
            {
                UIHelper.LogMessage(STRING_CONTRANTS.Validate_FromDateEarlyToDate, UIHelper.MsgType.Msgbox, true);
                this.Freeze(false);
                return;
            }

            try
            {
                LoadDataGridReport();
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndLoadData);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);

        }

        private void grHdr_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            this.Freeze(true);
            if (pVal.Row < 0)
            {
                this.Freeze(false);
                return;
            }

            try
            {
                var docnum = this.grHdr.DataTable.GetValue("DocNum", this.grHdr.GetDataTableRowIndex(pVal.Row)).ToString();
                if (!string.IsNullOrEmpty(docnum))
                {
                    LoadDataGridReportDetail(docnum);
                    ViewHelper.ColorGridRows(this.grHdr, pVal.Row);
                }
            }
            catch (Exception ex)
            {
                if (this.grDt != null)
                {
                    this.grDt.DataTable.Clear();
                }
            }
            this.Freeze(false);
        }
        private EditText txtRate;
        private CheckBox ckAppRate;

        private void ckAppRate_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            //this.txtRate.Item.Enabled = this.ckAppRate.Checked;
            //if(this.grData != null && this.grData.DataTable != null)
            //{
            //    this.grData.Columns.Item("DocRate").Editable = !this.ckAppRate.Checked;
            //}
        }

        private ComboBox cbbAccCa;
        private StaticText lblAccCa;
        private Button btnChAl;
        private Button btnUnck;

        private void btnChAl_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            //UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

            try
            {
                for (var i = 0; i < this.grData.DataTable.Rows.Count; i++)
                {

                    if (!this.SelectedDataIndexs.Contains(i))
                    {
                        this.grData.DataTable.SetValue("Check", i, "Y");
                        this.SelectedDataIndexs.Add(i);
                    }
                }
                EnableButton();
                //this.SelectedDataIndexs = this.grData.DataTable.Rows.Count;

            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private void btnUnck_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            // UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

            try
            {
                for (var i = 0; i < this.grData.DataTable.Rows.Count; i++)
                {

                    if (this.SelectedDataIndexs.Contains(i))
                    {
                        this.grData.DataTable.SetValue("Check", i, "N");
                        this.SelectedDataIndexs.Remove(i);
                    }
                }
                EnableButton();
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }

        private StaticText lblPosDa;
        private EditText txtPosDa;
        private EditText txtNote;
        private StaticText lblNote;
        private StaticText lblCusVen;
        private EditText txtCusVen;

        private string oldVal = string.Empty;

        private void txtCusVen_LostFocusAfter(object sboObject, SBOItemEventArg pVal)
        {
            this.txtCusVen.Item.Click();
            if (txtCusVen != null)
                this.UIAPIRawForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = txtCusVen.Value;

            //var newVal = this.txtCusVen.Value;
            //if (newVal != oldVal)
            //    this.txtCusVen.Value = oldVal;
        }

        private Button btnFilter;

        private void btnFilter_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                var type = _PaymentDocumentType == PaymentDocumentType.PT ? "C" : "S";
                if (type != CardTypeSelect)
                {
                    CardTypeSelect = type;
                    CardCodeFilter = string.Empty;
                }
                if (!frmObjectFilter.IsFormOpen)
                {
                    frmObjectFilter.Instance.OnSelectData += Instance_OnSelectData;
                    frmObjectFilter.Instance.ShowForm(CardCodeFilter, _PaymentDocumentType == PaymentDocumentType.PT ? "C" : "S", "Đề xuất Thanh toán");
                }
                else
                {
                    UIHelper.LogMessage($"Bộ lọc đang mở cho màn hình {frmObjectFilter.Instance.BaseForm}", UIHelper.MsgType.StatusBar);
                }
            }
            catch (Exception ex)
            { }
        }

        private void Instance_OnSelectData(object sender, SelectionEventArgs e)
        {
            var data = e.Selected;
            ((frmObjectFilter)sender).Close();
            if (data != null)
            {
                CardCodeFilter = data.ToString();
                CardTypeSelect = e.Type;
            }
            else
            {
                CardCodeFilter = string.Empty;
                CardTypeSelect = string.Empty;
            }
        }

        private Button btnHis;

        private void btnHis_ClickBefore(object sboObject, SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.Freeze(true);
            UIHelper.LogMessage(STRING_CONTRANTS.Notice_LoadData);

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
            try
            {
                LoadDataGridCreate(true, true);
                UIHelper.LogMessage(STRING_CONTRANTS.Notice_EndLoadData);
            }
            catch (Exception ex)
            {
                UIHelper.LogMessage(string.Format(STRING_CONTRANTS.Error_LoadData, ex.Message), UIHelper.MsgType.StatusBar, true);
            }
            this.Freeze(false);
        }
    }
}
