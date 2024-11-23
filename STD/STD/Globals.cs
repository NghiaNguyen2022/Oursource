using SAPCore.Config;
using SAPCore.Form;
using SAPCore.SAP.DIAPI;

namespace STDApp
{
    public class GlobalsConfig : SysGlobals
    {
        public AddonUserForm PaymentFormInfo;
        public AddonUserForm PaymentReviewFormInfo;
        public AddonUserForm PaymentDetailFormInfo;
        public AddonUserForm PaymentApproveFormInfo;
        public AddonUserForm FilterFormInfo;

        private static GlobalsConfig instance;

        private GlobalsConfig()
        {
            AddonName = "Stada";
            Version = "1.0";
            FlagVersion = 0;
            CusPMFolderID = "IntegrationCustomize";
            CusPMFolderDesc = "Nhóm chức năng Tích hợp";
            ParentMenuID = "43520";

            //PaymentFormInfo = new AddonUserForm()
            //{
            //   // FormID = "Payment_F",
            //    MenuID = "PM_Create_M",
            //    MenuName = "Đề xuất thanh toán",
            //    ParentID = CusPMFolderID,
            //    FormType = "STDApp.Payment.frmPayment"
            //};

            //PaymentReviewFormInfo = new AddonUserForm()
            //{
            //    //FormID = "PaymentRe_F",
            //    MenuID = "PM_Review_M",
            //    MenuName = "Thanh toán",
            //    ParentID = CusPMFolderID,
            //    FormType = "STDApp.Payment.frmPaymentReview"
            //};

            //PaymentDetailFormInfo = new AddonUserForm()
            //{
            //    //FormID = "PaymentDetail_F",
            //    MenuID = "PM_Detail_M",
            //    MenuName = "Thanh toán chi tiết",
            //    ParentID = CusPMFolderID,
            //    FormType = "STDApp.Payment.frmAddPaymentLine"
            //};

            //PaymentApproveFormInfo = new AddonUserForm()
            //{
            //    //FormID = "PaymentAppr_F",
            //    MenuID = "PM_Appr_M",
            //    MenuName = "Phê duyệt Thanh toán",
            //    ParentID = CusPMFolderID,
            //    FormType = "STDApp.Payment.frmApprovePayment"
            //};

            //FilterFormInfo = new AddonUserForm()
            //{
            //    //FormID = "Filter_F",
            //    MenuID = "Filter_M",
            //    MenuName = "Chọn từ danh sách",
            //    ParentID = CusPMFolderID,
            //    FormType = "STDApp.Payment.frmObjectFilter"
            //};

        }

        public static GlobalsConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = new GlobalsConfig();
                return instance;
            }
        }
    }

    public class STRING_CONTRANTS
    {
        public static string StartConfigUDF = "Cấu hình UDF";
        public static string ConfigUDFNotice = "Tạo UDF: {0} cho {1}";
        public static string EndConfigUDF = "Cấu hình UDF hoàn tất";

        public static string Notice_Version = "Version hiện tại đang cũ, hãy nâng cấp Version mới";

        public static string Notice_LoadData = "Đang tải dữ liệu";
        public static string Notice_EndLoadData = "Tải dữ liệu hoàn tất";
        public static string Error_LoadData = "Tải dữ liệu bị lỗi: {0}";

        public static string Notice_CreatePayment = "Đang tạo đơn thanh toán";
        public static string Notice_LoadDataBefAdd = "Vui lòng tải dữ liệu trước khi thêm mới";
        public static string Notice_EndCreatePayment = "Hoàn tất quá trình tạo thanh toán";
        public static string Error_CreatePayment = "Quà trình tạo đơn thanh toán lỗi: {0}";
        public static string Notice_NoDataToAdd = "Không có thông tin được thêm";

        public static string Notice_UpdatePayment = "Đang cập nhật đơn thanh toán";
        public static string Notice_EndUpdatePayment = "Hoàn tất quá trình cập nhật thanh toán";

        public static string Notice_UpdatePaymentSuccess = "Cập nhật thành công";
        public static string Notice_CanNotloadDraft = "Không thể tải đơn nháp";
        public static string Notice_EndAction = "{0} thành công {1}/{2} đơn thanh toán, và {3}/{2} không thành công";

        public static string Validate_DateSelectNull = "Bộ lọc [Từ Ngày]/[Đến Ngày] phải có giá trị đúng";
        public static string Validate_FromDateEarlyToDate = "Chọn bộ lọc [Từ ngày] phải trước bộ lọc [Đến ngày]";

        public static string NoData = "Không có dữ liệu";
        public static string NoDataCheck = "Chưa chọn dữ liệu";
        public static string NoChooseCashFlow = "Không chọn dòng tiền";
        public static string AllOptionDesc = "Tất cả";
        public static string AllOption = "All";

        public static string Action_Create = "Tạo";

        public static string DifferenceCurrency = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Đơn Vị Tiền Tệ.";
        public static string NoCurrency = "Không có thông tin Đơn Vị Tiền Tệ.";
        public static string DifferenceBank = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Ngân hàng.";
        public static string NoBank = "Không có thông tin Ngân hàng.";
        public static string DifferenceCashFlow = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Dòng tiền.";
        public static string NoCashFlow = "Không có thông tin Dòng tiền.";
        public static string DifferenceAccount = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Tài khoản tiền mặt.";
        public static string NoAccount = "Không có thông tin Tài khoản tiền mặt.";
        public static string WrongFormatCurrency = "Vui lòng nhập đúng định dạng giá tri tiền";
        public static string OverTotalPayment = "Cần thanh toán ít hơn hay bằng số tiền phải trả [Số Tiền chưa Thanh toán]";
        public static string PaymentZero = "Số tiền thanh toán phải lớn hơn 0";
        public static string NoCFlow = "Cần phải chọn dòng tiền cho đơn thanh toán";
        public static string NoRate = "Vui lòng nhập tỉ giá";
        public static string DifferenceRate = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về  tỉ giá.";


        public static string CanNotGenerateKey = "Không thể tạo key cho đơn thanh toán, vui lòng thử lại";
        public static string CanNotPayment = "Không thể chọn chứng từ có Số Tiền Thanh Toán bằng 0";
        public static string CanNotConnectDIAPI = "Không thể kết nối DI API, vui lòng thử lại";

        public static string PaymentType_PC = "Phiếu Chi";
        public static string PaymentType_PT = "Phiếu Thu";
        public static string PaymentType_UC = "Ủy Nhiệm Chi";

        public static string PaymentStatus_Pending = "Yêu cầu thanh toán";
        public static string PaymentStatus_Viewed = "Đề nghị thanh toán";
        public static string PaymentStatus_Approved = "Đã phê duyệt";
        public static string PaymentStatus_Rejected = "Từ chối";
        public static string PaymentStatus_Generated = "Tạo thành công";
        public static string PaymentStatus_All = "Tất cả";

        public static string Title_CustomerCode = "Mã Khách hàng";
        public static string Title_Customer = "Khách hàng";
        public static string Title_CustomerName = "Tên Khách hàng";
        public static string Title_VendorCode = "Mã Nhà cung cấp";
        public static string Title_Vendor= "Nhà cung cấp";
        public static string Title_VendorName = "Tên Nhà cung cấp";
        public static string Title_Choose = "Chọn";
        public static string Title_DocNum = "Số chứng từ";
        public static string Title_InvCode = "Số Hóa Đơn";
        public static string Title_DocDate = "Ngày Chứng từ";
        public static string Title_DueDate = "Hạn Thanh Toán";
        public static string Title_PostingDate = "Ngày Thanh Toán";
        public static string Title_Currency = "Đơn vị tiền tệ";
        public static string Title_Remark = "Ghi chú";
        public static string Title_InsTotal = "Số tiền hóa đơn";
        public static string Title_InsTotalFC = "Số Tiền Hóa Đơn (Ngoại tệ)";
        public static string Title_MustPay = "Số tiền chưa thanh toán";
        public static string Title_PayAmount = "Số tiền thanh toán";
        public static string Title_Rate = "Tỉ giá ngoại tệ";
        public static string Title_Account = "Tài khoản";
        public static string Title_MustPayAsCash = "Số Tiền Thanh toán (Cash)";
        public static string Title_MustPayAsBank = "Số Tiền Thanh toán (Bank)";
        public static string Title_PaymentKey = "Mã thanh toán";
        public static string Title_PaymentType = "Loại thanh toán";
        public static string Title_PaymentDocEntry = "Số TT";
        public static string Title_PaymentDraftEntry = "Số TT nháp";
        public static string Title_PaymentDocNum = "Số chứng từ TT";
        public static string Title_PaymentDocDate = "Ngày chứng từ TT";
        public static string Title_CreateBy = "Người Tạo";
        public static string Title_Status = "Trạng Thái";
        public static string Title_Bank = "Ngân hàng";
        public static string Title_BankAccount = "Tài khoản Ngân hàng";
        public static string Title_CFlow = "Dòng tiền";
    }
}
