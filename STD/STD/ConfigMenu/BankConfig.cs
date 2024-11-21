using SAPCore.Form;

namespace STDApp.ConfigMenu
{
    public class BankConfig : FunctionConfig
    {
        public BankConfig() : base()
        { }

        public AddonUserForm InquiryForm { get; set; }


        protected override void AddFolder(string mainParentID, int index = 0)
        {
            FolderInfomation = new AddonFolder()
            {
                MenuID = "F_Bank",
                MenuName = "Tích hợp ngân hàng",
                ParentID = mainParentID
            };
            base.AddFolder(mainParentID, index);
        }

        protected override void AddMenus()
        {

            InquiryForm = new AddonUserForm()
            {
                FormID = "Inquiry_F",
                MenuID = "PM_Inquiry_M",
                MenuName = "Tích hợp vấn tin tài khoản",
                ParentID = FolderInfomation.MenuID,
                FormType = "frmInquiry"
            };

            AddForms(InquiryForm, 0);
        }
    }
}
