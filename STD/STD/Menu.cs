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

    }
}
