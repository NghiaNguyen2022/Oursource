using System;
using System.Collections.Generic;
using SAPbouiCOM.Framework;
using SAPCore;
using SAPCore.SAP;

namespace STDApp
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application MainApplication = null;
                if (args.Length < 1)
                {
                    MainApplication = new Application();
                }
                else
                {
                    MainApplication = new Application(args[0]);
                }
               // ConfigDatabase();
                LoadDatabaseConfig();
                GlobalsConfig.Instance.FlagVersion = 1; //ConfigHelper.GetAddonVersion(GlobalsConfig.Instance.AddonName, GlobalsConfig.Instance.Version);

                if (GlobalsConfig.Instance.FlagVersion == 0)
                {
                    UIHelper.LogMessage("Version hiện tại đang cũ, hãy nâng cấp Version mới", UIHelper.MsgType.StatusBar, true);
                    return;
                }

                var ch = GlobalsConfig.Instance.Language;
                //Globals.ConnectionContextCookie = DIConnection.Instance.CookieConnection;
                Menu MyMenu = new Menu();
                MainApplication.RegisterMenuEventHandler(MyMenu.SBO_Application_MenuEvent);
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
                MainApplication.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        static void LoadDatabaseConfig()
        {
            //DataHelper.LoadConfig();
        }
        static void ConfigDatabase()
        {
            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_PaymentKey"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Payment Key", "OVPM"));
            //    SAPHandler.Instance.CreateUDF("OVPM", "PaymentKey", "Payment Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_PaymentKey"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Payment Key", "ORCT"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "PaymentKey", "Payment Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_PayType"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Payment Type", "OVPM"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("PT", "Phiếu Thu");
            //    values.Add("PC", "Phiếu Chi");
            //    values.Add("UC", "Ủy Nhiệm Chi");
            //    SAPHandler.Instance.CreateUDF("OVPM", "PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "PT");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_PayType"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Payment Type", "ORCT"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("PT", "Phiếu Thu");
            //    values.Add("PC", "Phiếu Chi");
            //    values.Add("UC", "Ủy Nhiệm Chi");
            //    SAPHandler.Instance.CreateUDF("ORCT", "PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "PT");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_Bank"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Bank", "OVPM"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "Bank", "Bank", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_CashFlow"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Cash Flow", "OVPM"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "CashFlow", "Cash Flow", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_Bank"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Bank", "ORCT"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "Bank", "Bank", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "V_BANK");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_CashFlow"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Cash Flow", "ORCT"));
            //    SAPHandler.Instance.CreateUDF("ORCT", "CashFlow", "Cash Flow", SAPbobsCOM.BoFieldTypes.db_Alpha, 50, "");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_Review"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Review", "OVPM"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("Y", "Xác nhận");
            //    values.Add("N", "Chưa xác nhận");
            //    SAPHandler.Instance.CreateUDF("OVPM", "Review", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "N");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_Review"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Review", "ORCT"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("Y", "Xác nhận");
            //    values.Add("N", "Chưa xác nhận");
            //    SAPHandler.Instance.CreateUDF("ORCT", "Review", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "N");
            //}

            //if (!SAPHandler.Instance.FieldIsExisted("OVPM", "U_Status"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Status", "OVPM"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("N", "Yêu cầu thanh toán");
            //    values.Add("R", "Đề xuất thanh toán");
            //    values.Add("A", "Đã phê duyệt");
            //    values.Add("J", "Đã từ chối");
            //    values.Add("S", "Đã tạo");
            //    SAPHandler.Instance.CreateUDF("OVPM", "Status", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "N");
            //}
            //if (!SAPHandler.Instance.FieldIsExisted("ORCT", "U_Status"))
            //{
            //    UIHelper.LogMessage(string.Format(STRING_CONTRANTS.ConfigUDFNotice, "Status", "ORCT"));
            //    Dictionary<string, string> values = new Dictionary<string, string>();
            //    values.Add("N", "Yêu cầu thanh toán");
            //    values.Add("R", "Đề xuất thanh toán");
            //    values.Add("A", "Đã phê duyệt");
            //    values.Add("J", "Đã từ chối");
            //    values.Add("S", "Đã tạo");
            //    SAPHandler.Instance.CreateUDF("ORCT", "Status", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, 10, "", values, "N");
            //}
        }

        static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            var message = string.Empty;
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    //Exit Add-On
                    SAPUtils.StopAddon(GlobalsConfig.Instance.AddonName, ref message);
                    Application.SBO_Application.MessageBox(message);
                    System.Windows.Forms.Application.Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    SAPUtils.StopAddon(GlobalsConfig.Instance.AddonName, ref message);
                    Application.SBO_Application.MessageBox(message);
                    break;
                default:
                    break;
            }
        }


        private static void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true; // Always set BubbleEvent to true
            try
            {
                // select cardcode and fill card name
                //if (pVal.FormTypeEx == GlobalsConfig.Instance.PaymentFormInfo.FormType)
                //{

                //    if (!pVal.BeforeAction
                //        && pVal.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                //        && pVal.ItemUID == "grData")
                //    {
                //        var oForm = Application.SBO_Application.Forms.ActiveForm;//.Item("Payment_F");
                //        try
                //        {
                //            oForm.Freeze(true);
                //            var oGrid = (SAPbouiCOM.Grid)oForm.Items.Item(pVal.ItemUID).Specific;

                //            if (pVal.ColUID == "CardCode")
                //            {
                //                var selectedRowIndex = pVal.Row;
                //                var comboColumn = (SAPbouiCOM.ComboBoxColumn)oGrid.Columns.Item(pVal.ColUID);
                //                var cardname = comboColumn.GetSelectedValue(selectedRowIndex).Description;

                //                oGrid.DataTable.SetValue("CardName", selectedRowIndex, cardname);
                //            }

                //            if (pVal.ColUID == "Bank")
                //            {
                //                var selectedRowIndex = oGrid.GetDataTableRowIndex(pVal.Row);
                //                var comboColumn = (SAPbouiCOM.ComboBoxColumn)oGrid.Columns.Item(pVal.ColUID);
                //                var bank = comboColumn.GetSelectedValue(selectedRowIndex).Value;

                //                oGrid.DataTable.SetValue("Bank", selectedRowIndex, bank);
                //            }

                //            //if (pVal.ColUID == "CFlow")
                //            //{
                //            //    var selectedRowIndex = pVal.Row;
                //            //    var comboColumn = (SAPbouiCOM.ComboBoxColumn)oGrid.Columns.Item(pVal.ColUID);
                //            //    var cflow = comboColumn.GetSelectedValue(selectedRowIndex).Value;

                //            //    oGrid.DataTable.SetValue("CFlow", selectedRowIndex, cflow);
                //            //}
                //            oForm.Freeze(false);
                //        }
                //        catch
                //        {
                //            oForm.Freeze(false);
                //        }
                //    }
                //    else
                //    {
                //        if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                //        {
                //            var oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                //            if (oCFLEvento.BeforeAction == false)
                //            {
                //                var oDataTable = oCFLEvento.SelectedObjects;
                //                var oForm = Application.SBO_Application.Forms.Item(FormUID);//.Item("Payment_F");
                //                oForm.Freeze(true);
                //                if (pVal.ItemUID == "txtCV")
                //                {
                //                    var strCardCode = string.Empty;
                //                    if (oDataTable != null && oDataTable.Rows.Count > 0)
                //                    {
                //                        if (oDataTable.Rows.Count > 20)
                //                        {
                //                            UIHelper.LogMessage("Vui lòng chọn duới 20 NCC/Khách hàng");
                //                            oForm.Freeze(false);
                //                            return;
                //                        }
                //                        for (var i = 0; i < oDataTable.Rows.Count; i++)
                //                        {
                //                            strCardCode += System.Convert.ToString(oDataTable.GetValue(0, i));
                //                            if (i < oDataTable.Rows.Count - 1)
                //                            {
                //                                strCardCode += ";";
                //                            }
                //                        }
                //                    }
                //                    oForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = strCardCode;// System.Convert.ToString(oDataTable.GetValue(0, 0));

                //                    var txtCV = ((SAPbouiCOM.EditText)(oForm.Items.Item("txtCV").Specific));
                //                    //txtCV.Value = strCardCode;
                //                    txtCV.Item.Click();
                //                    //  oForm.DataSources.UserDataSources.Item("UD_Nam").ValueEx = System.Convert.ToString(oDataTable.GetValue(1, 0));
                //                }
                //                oForm.Freeze(false);
                //            }
                //        }
                //    }
                //}
                //else if (pVal.FormTypeEx == GlobalsConfig.Instance.PaymentReviewFormInfo.FormType)
                //{
                //    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                //    {
                //        var oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                //        if (oCFLEvento.BeforeAction == false)
                //        {
                //            var oDataTable = oCFLEvento.SelectedObjects;
                //            var oForm = Application.SBO_Application.Forms.Item(FormUID);//.Item("Payment_F");
                //            oForm.Freeze(true);
                //            if (pVal.ItemUID == "txtCV")
                //            {
                //                var strCardCode = string.Empty;
                //                if (oDataTable != null && oDataTable.Rows.Count > 0)
                //                {
                //                    if (oDataTable.Rows.Count > 20)
                //                    {
                //                        UIHelper.LogMessage("Vui lòng chọn duới 20 NCC/Khách hàng");
                //                        oForm.Freeze(false);
                //                        return;
                //                    }
                //                    for (var i = 0; i < oDataTable.Rows.Count; i++)
                //                    {
                //                        strCardCode += System.Convert.ToString(oDataTable.GetValue(0, i));
                //                        if (i < oDataTable.Rows.Count - 1)
                //                        {
                //                            strCardCode += ";";
                //                        }
                //                    }
                //                }
                //                oForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = strCardCode;// System.Convert.ToString(oDataTable.GetValue(0, 0));
                //                //  oForm.DataSources.UserDataSources.Item("UD_Nam").ValueEx = System.Convert.ToString(oDataTable.GetValue(1, 0));
                //                var txtCV = ((SAPbouiCOM.EditText)(oForm.Items.Item("txtCV").Specific));
                //                txtCV.Item.Click();
                //            }
                //            oForm.Freeze(false);
                //        }
                //    }
                //}
                //else if (pVal.FormTypeEx == GlobalsConfig.Instance.PaymentDetailFormInfo.FormType)
                //{
                //    try
                //    {
                //        if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                //        {
                //            var oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                //            if (oCFLEvento.BeforeAction == false)
                //            {
                //                var oDataTable = oCFLEvento.SelectedObjects;
                //                var oForm = Application.SBO_Application.Forms.Item(FormUID);//.Item("Payment_F");
                //                oForm.Freeze(true);
                //                if (pVal.ItemUID == "txtBP")
                //                {
                //                    oForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = System.Convert.ToString(oDataTable.GetValue(0, 0));
                //                    oForm.DataSources.UserDataSources.Item("UD_Nam").ValueEx = System.Convert.ToString(oDataTable.GetValue(1, 0));
                //                }
                //                oForm.Freeze(false);
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //    }
                //}
                //else if (pVal.FormTypeEx == GlobalsConfig.Instance.FilterFormInfo.FormType)
                //{

                //    if (pVal.BeforeAction                        
                //      //  && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_KEY_DOWN
                //        && pVal.ItemUID == "grData")
                //    {
                //        if (((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift)
                //            || ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control))
                //        {
                //            BubbleEvent = false;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.Message);
            }
        }
    }
}
