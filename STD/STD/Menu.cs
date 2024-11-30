using SAPbouiCOM.Framework;
using SAPCore;
using STDApp.Bank;
using STDApp.ConfigMenu;
using STDApp.Payment;
using System;

namespace STDApp
{
    class Menu
    {
        BankConfig bankConfig;

        public Menu()
        {
            UIHelper.AddMenuFolder(GlobalsConfig.Instance.CusPMFolderID, GlobalsConfig.Instance.CusPMFolderDesc, GlobalsConfig.Instance.ParentMenuID);

            bankConfig = new BankConfig();
            bankConfig.LoadMenu(GlobalsConfig.Instance.CusPMFolderID, 0);
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                
                if (pVal.BeforeAction)
                {
                    if (pVal.MenuUID == bankConfig.InquiryForm.MenuID)
                    {
                        frmInquiry.ShowForm();
                    }
                    else if (pVal.MenuUID == bankConfig.PaymentForm.MenuID)
                    {
                        frmPayment.ShowForm();
                    }
                    //else if (pVal.MenuUID == GlobalsConfig.Instance.PaymentReviewFormInfo.MenuID)
                    //{
                    //    frmPaymentReview.ShowForm();
                    //}
                }
                              
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }

        public void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true; // Always set BubbleEvent to true
            try
            {
                // select cardcode and fill card name
                if (pVal.FormTypeEx == bankConfig.PaymentForm.FormType)
                {

                    if (!pVal.BeforeAction
                        && pVal.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                        && pVal.ItemUID == "grData")
                    {
                        
                    }
                    else
                    {
                        //if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                        //{
                        //    var oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                        //    if (oCFLEvento.BeforeAction == false)
                        //    {
                        //        var oDataTable = oCFLEvento.SelectedObjects;
                        //        var oForm = Application.SBO_Application.Forms.Item(FormUID);//.Item("Payment_F");
                        //        oForm.Freeze(true);
                        //        if (pVal.ItemUID == "txtCV")
                        //        {
                        //            var strCardCode = string.Empty;
                        //            if (oDataTable != null && oDataTable.Rows.Count > 0)
                        //            {
                        //                if (oDataTable.Rows.Count > 20)
                        //                {
                        //                    UIHelper.LogMessage("Vui lòng chọn duới 20 NCC/Khách hàng");
                        //                    oForm.Freeze(false);
                        //                    return;
                        //                }
                        //                for (var i = 0; i < oDataTable.Rows.Count; i++)
                        //                {
                        //                    strCardCode += System.Convert.ToString(oDataTable.GetValue(0, i));
                        //                    if (i < oDataTable.Rows.Count - 1)
                        //                    {
                        //                        strCardCode += ";";
                        //                    }
                        //                }
                        //            }
                        //            oForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = strCardCode;// System.Convert.ToString(oDataTable.GetValue(0, 0));

                        //            var txtCV = ((SAPbouiCOM.EditText)(oForm.Items.Item("txtCV").Specific));
                        //            //txtCV.Value = strCardCode;
                        //            txtCV.Item.Click();
                        //            //  oForm.DataSources.UserDataSources.Item("UD_Nam").ValueEx = System.Convert.ToString(oDataTable.GetValue(1, 0));
                        //        }
                        //        oForm.Freeze(false);
                        //    }
                        //}
                    }
                }
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
                else if (pVal.FormTypeEx == bankConfig.PaymentDetailFormInfo.FormType)
                {
                    try
                    {
                        if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                        {
                            var oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                            if (oCFLEvento.BeforeAction == false)
                            {
                                var oDataTable = oCFLEvento.SelectedObjects;
                                var oForm = Application.SBO_Application.Forms.Item(FormUID);//.Item("Payment_F");
                                oForm.Freeze(true);
                                if (pVal.ItemUID == "txtBP")
                                {
                                    oForm.DataSources.UserDataSources.Item("UD_Cod").ValueEx = System.Convert.ToString(oDataTable.GetValue(0, 0));
                                    oForm.DataSources.UserDataSources.Item("UD_Nam").ValueEx = System.Convert.ToString(oDataTable.GetValue(1, 0));
                                }
                                oForm.Freeze(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
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
