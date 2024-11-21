using SAPCore.SAP;
using SAPCore.SAP.UserDefine;
using System.Collections.Generic;

namespace GTVUserDefine.Builder
{
    public class SpecialReportVersion : VerBuilder
    {
        public override int Init()
        {
            var tlcd = new SapUDT(InternalVersion.ToString(), "V_TLCD", "Tỉ lệ công đoạn", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(tlcd);

            var pua = new SapUDT(InternalVersion.ToString(), "V_PUA", "PUA (USD)", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(pua);

            var tgqd = new SapUDT(InternalVersion.ToString(), "V_TGQD", "Tỉ giá quy đổi Slượng", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(tgqd);

            var slqd = new SapUDT(InternalVersion.ToString(), "V_SLQD", "Sản lượng quy đổi", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(slqd);

            var nsp = new SapUDT(InternalVersion.ToString(), "V_ITEMGROUP", "Nhóm Sản phẩm", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(nsp);

            var dsp = new SapUDT(InternalVersion.ToString(), "V_ITEMLINE", "Dòng Sản phẩm", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(dsp);

            var ttxuong = new SapUDT(InternalVersion.ToString(), "V_XUONGINFO", "Thông tin xưởng", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(ttxuong);

            var nhomto = new SapUDT(InternalVersion.ToString(), "V_NHOMTO", "Nhóm tổ", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(nhomto);
            var nhomtodetail = new SapUDT(InternalVersion.ToString(), "V_NHOMTODETAIL", "Chi tiết Nhóm tổ", SAPbobsCOM.BoUTBTableType.bott_MasterDataLines);
            ListTable.Add(nhomtodetail);

            var kwhm3 = new SapUDT(InternalVersion.ToString(), "V_KWHM3", "Định mức điện theo nhóm", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(kwhm3);

            var dntt = new SapUDT(InternalVersion.ToString(), "V_MONTHLYCONSUM", "Điện năng tiêu thụ", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(dntt);
            var dnttct = new SapUDT(InternalVersion.ToString(), "V_MONTHLYCONSUMDETA", "Điện năng tiêu thụ chi tiết", SAPbobsCOM.BoUTBTableType.bott_MasterDataLines);
            ListTable.Add(dnttct);


            var cpnc = new SapUDT(InternalVersion.ToString(), "V_LABORCOST", "Đơn giá Chi phí nhân công", SAPbobsCOM.BoUTBTableType.bott_MasterData);
            ListTable.Add(cpnc);
            var cpncct = new SapUDT(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "Chi phí nhân công chi tiết", SAPbobsCOM.BoUTBTableType.bott_MasterDataLines);
            ListTable.Add(cpncct);

            Dictionary<string, string> _isToValues = new Dictionary<string, string>();
            _isToValues.Add("Y", "Yes");
            _isToValues.Add("N", "No");
            var ORSC_IsTo = new SapUDF(InternalVersion.ToString(), "ORSC", "IsTo", "Is To", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _isToValues, "N");
            ListField.Add(ORSC_IsTo);

            // OITM
            var OITM_M3SP = new SapUDF(InternalVersion.ToString(), "OITM", "M3SP", "M3/SP", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10);
            ListField.Add(OITM_M3SP);

            var OITM_Group = new SapUDF(InternalVersion.ToString(), "OITM", "Group", "Nhóm Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "V_ITEMGROUP", "UDT");
            ListField.Add(OITM_Group);

            var OITM_Line = new SapUDF(InternalVersion.ToString(), "OITM", "Line", "Dòng Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "V_ITEMLINE", "UDT");
            ListField.Add(OITM_Line);



            Dictionary<string, string> _activies = new Dictionary<string, string>();
            _activies.Add("Y", "Hoạt động");
            _activies.Add("N", "Ngừng hoạt động");

            // V_TLCD
            var V_TLCD_ItemCode = new SapUDF(InternalVersion.ToString(), "V_TLCD", "ItemCode", "Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "OITM", "OITM", "", true);
            ListField.Add(V_TLCD_ItemCode);

            var V_TLCD_Fac = new SapUDF(InternalVersion.ToString(), "V_TLCD", "Factory", "Nhà máy", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "G_SAY4", "UDT", "", true);
            ListField.Add(V_TLCD_Fac);

            var V_TLCD_Year = new SapUDF(InternalVersion.ToString(), "V_TLCD", "Year", "Năm", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10, "" , true);
            ListField.Add(V_TLCD_Year);

            var V_TLCD_Active = new SapUDF(InternalVersion.ToString(), "V_TLCD", "Active", "Hoạt động", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _activies, "", "", "", true);
            ListField.Add(V_TLCD_Active);

            var V_TLCD_Rate = new SapUDF(InternalVersion.ToString(), "V_TLCD", "Rate", "Tỉ lệ quy đổi", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10, "", true);
            ListField.Add(V_TLCD_Rate);

            var V_TLCD_ApplyDate = new SapUDF(InternalVersion.ToString(), "V_TLCD", "ApplyDate", "Ngày áp dụng", SAPbobsCOM.BoFieldTypes.db_Date, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_TLCD_ApplyDate);

            // PUA
            var V_PUA_ItemCode = new SapUDF(InternalVersion.ToString(), "V_PUA", "ItemCode", "Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "OITM", "OITM", "", true);
            ListField.Add(V_PUA_ItemCode);

            var V_PUA_Fac = new SapUDF(InternalVersion.ToString(), "V_PUA", "Factory", "Nhà máy", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "G_SAY4", "UDT", "", true);
            ListField.Add(V_PUA_Fac);

            var V_PUA_Year = new SapUDF(InternalVersion.ToString(), "V_PUA", "Year", "Năm", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_PUA_Year);

            var V_PUA_Active = new SapUDF(InternalVersion.ToString(), "V_PUA", "Active", "Hoạt động", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _activies, "", "", "", true);
            ListField.Add(V_PUA_Active);

            var V_PUA_Rate = new SapUDF(InternalVersion.ToString(), "V_PUA", "Price", "Giá", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Price, 10, "", true);
            ListField.Add(V_PUA_Rate);

            var V_PUA_ApplyDate = new SapUDF(InternalVersion.ToString(), "V_PUA", "ApplyDate", "Ngày áp dụng", SAPbobsCOM.BoFieldTypes.db_Date, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_PUA_ApplyDate);

            // V_TGQD
            var V_TGQD_Fac = new SapUDF(InternalVersion.ToString(), "V_TGQD", "Factory", "Nhà máy", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "G_SAY4", "UDT", "", true);
            ListField.Add(V_TGQD_Fac);

            var V_TGQD_Year = new SapUDF(InternalVersion.ToString(), "V_TGQD", "Year", "Năm", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_TGQD_Year);

            var V_TGQD_Active = new SapUDF(InternalVersion.ToString(), "V_TGQD", "Active", "Hoạt động", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _activies, "", "", "", true);
            ListField.Add(V_TGQD_Active);

            var V_TGQD_Rate = new SapUDF(InternalVersion.ToString(), "V_TGQD", "Rate", "Tỉ giá", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10, "", true);
            ListField.Add(V_TGQD_Rate);

            var V_TGQD_ApplyDate = new SapUDF(InternalVersion.ToString(), "V_TGQD", "ApplyDate", "Ngày áp dụng", SAPbobsCOM.BoFieldTypes.db_Date, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_TGQD_ApplyDate);

            // V_SLQD
            var V_SLQD_ItemCode = new SapUDF(InternalVersion.ToString(), "V_SLQD", "ItemCode", "Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "OITM", "OITM", "", true);
            ListField.Add(V_SLQD_ItemCode);

            var V_SLQD_To = new SapUDF(InternalVersion.ToString(), "V_SLQD", "To", "Tổ", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "", true);
            ListField.Add(V_SLQD_To);

            var V_SLQD_Active = new SapUDF(InternalVersion.ToString(), "V_SLQD", "Active", "Hoạt động", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _activies, "", "", "", true);
            ListField.Add(V_SLQD_Active);

            var V_SLQD_Rate = new SapUDF(InternalVersion.ToString(), "V_SLQD", "Price", "Giá", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Price, 10, "", true);
            ListField.Add(V_SLQD_Rate);

            // V_XUONGINFO
            var V_XUONGINFO_Branch = new SapUDF(InternalVersion.ToString(), "V_XUONGINFO", "Branch", "Mã Công ty", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, "", true);
            ListField.Add(V_XUONGINFO_Branch);

            var V_XUONGINFO_BranchName = new SapUDF(InternalVersion.ToString(), "V_XUONGINFO", "BranchName", "Tên Công ty", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 200, "", true);
            ListField.Add(V_XUONGINFO_BranchName);

            var V_XUONGINFO_Fac = new SapUDF(InternalVersion.ToString(), "V_XUONGINFO", "Factory", "Nhà máy", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "G_SAY4", "UDT", "", true);
            ListField.Add(V_XUONGINFO_Fac);

            var V_XUONGINFO_Xuong = new SapUDF(InternalVersion.ToString(), "V_XUONGINFO", "Xuong", "Xưởng", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "", true);
            ListField.Add(V_XUONGINFO_Xuong);

            var V_XUONGINFO_Khoi = new SapUDF(InternalVersion.ToString(), "V_XUONGINFO", "Khoi", "Khối", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "", true);
            ListField.Add(V_XUONGINFO_Khoi);

            // V_NHOMTODETAIL
            var V_NHOMTODETAIL_To = new SapUDF(InternalVersion.ToString(), "V_NHOMTODETAIL", "To", "Tổ", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "", true);
            ListField.Add(V_NHOMTODETAIL_To);

            Dictionary<string, string> _isCalcSLValues = new Dictionary<string, string>();
            _isCalcSLValues.Add("Y", "Yes");
            _isCalcSLValues.Add("N", "N");

            var V_NHOMTODETAIL_CalcSL = new SapUDF(InternalVersion.ToString(), "V_NHOMTODETAIL", "CalcSL", "Tính Sản lượng", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _isCalcSLValues, "", "", "", true);
            ListField.Add(V_NHOMTODETAIL_CalcSL);

            // V_KWHM3
            var V_KWHM3_Group = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "Group", "Nhóm Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "V_ITEMGROUP", "UDT", "", true);
            ListField.Add(V_KWHM3_Group);

            var V_KWHM3_Line = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "Line", "Dòng Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "V_ITEMLINE", "UDT", "", true);
            ListField.Add(V_KWHM3_Line);

            var V_KWHM3_Say = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "Say", "CĐ Sấy", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10, "", true);
            ListField.Add(V_KWHM3_Say);

            var V_KWHM3_LpSc = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "LpSc", "CĐ LP và SC", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10, "", true);
            ListField.Add(V_KWHM3_LpSc);

            var V_KWHM3_TC = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "TC", "CĐ Tinh chế", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10, "", true);
            ListField.Add(V_KWHM3_TC);

            var V_KWHM3_HtDg = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "HtDg", "CĐ HT và Đg", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10, "", true);
            ListField.Add(V_KWHM3_HtDg);

            var V_KWHM3_Rate = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "Rate", "Tỉ giá", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Quantity, 10, "", true);
            ListField.Add(V_KWHM3_Rate);

            var V_KWHM3_ApplyDate = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "ApplyDate", "Ngày áp dụng", SAPbobsCOM.BoFieldTypes.db_Date, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_KWHM3_ApplyDate);

            // V_MONTHLYCONSUM

            var V_MONTHLYCONSUM_Year = new SapUDF(InternalVersion.ToString(), "V_MONTHLYCONSUM", "Year", "Năm", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_MONTHLYCONSUM_Year);

            var V_MONTHLYCONSUM_Month = new SapUDF(InternalVersion.ToString(), "V_MONTHLYCONSUM", "Month", "Tháng", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_MONTHLYCONSUM_Month);

            // V_MONTHLYCONSUMDETAIL
            var V_MONTHLYCONSUMDETAIL_Fac = new SapUDF(InternalVersion.ToString(), "V_MONTHLYCONSUMDETA", "Factory", "Nhà máy", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "G_SAY4", "UDT", "", true);
            ListField.Add(V_MONTHLYCONSUMDETAIL_Fac);

            var V_MONTHLYCONSUMDETAIL_Xuong = new SapUDF(InternalVersion.ToString(), "V_MONTHLYCONSUMDETA", "Xuong", "Xưởng", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "", true);
            ListField.Add(V_MONTHLYCONSUMDETAIL_Xuong);

            var V_MONTHLYCONSUMDETAIL_NhomTo = new SapUDF(InternalVersion.ToString(), "V_MONTHLYCONSUMDETA", "NhomTo", "Nhóm tổ", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "V_NHOMTO", "UDT", "", true);
            ListField.Add(V_MONTHLYCONSUMDETAIL_NhomTo);

            var V_MONTHLYCONSUMDETAIL_kwm3 = new SapUDF(InternalVersion.ToString(), "V_MONTHLYCONSUMDETA", "Kwm3", "Số tiêu thụ điện", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Measurement, 10, "", true);
            ListField.Add(V_MONTHLYCONSUMDETAIL_kwm3);

            var V_MONTHLYCONSUMDETAIL_Note = new SapUDF(InternalVersion.ToString(), "V_MONTHLYCONSUMDETA", "Note", "Ghi chú", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 254, "", true);
            ListField.Add(V_MONTHLYCONSUMDETAIL_Note);

            // V_LABORCOST

            var V_LABORCOST_ApplyDate = new SapUDF(InternalVersion.ToString(), "V_LABORCOST", "ApplyDate", "Ngày áp dụng", SAPbobsCOM.BoFieldTypes.db_Date, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_LABORCOST_ApplyDate);

            var V_LABORCOST_Fac = new SapUDF(InternalVersion.ToString(), "V_LABORCOST", "Factory", "Nhà máy", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "G_SAY4", "UDT", "", true);
            ListField.Add(V_LABORCOST_Fac);

            // V_LABORCOSTDETAIL
            var V_LABORCOSTDETAIL_CD = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "CDoan", "Công đoạn", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "V_CDOAN", "UDT", "", true);
            ListField.Add(V_LABORCOSTDETAIL_CD);

            var V_LABORCOSTDETAIL_To = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "To", "Tổ", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "", true);
            ListField.Add(V_LABORCOSTDETAIL_To);

            var V_LABORCOSTDETAIL_ItemCode = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "ItemCode", "Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30, "OITM", "OITM", "", true);
            ListField.Add(V_LABORCOSTDETAIL_ItemCode);

            var V_LABORCOSTDETAIL_ItemName = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "ItemName", "Tên Sản phẩm", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 100, "", true);
            ListField.Add(V_LABORCOSTDETAIL_ItemName);

            var V_LABORCOSTDETAIL_WLCode = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "WLCode", "Mã WL", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 100, "", true);
            ListField.Add(V_LABORCOSTDETAIL_WLCode);

            var V_LABORCOSTDETAIL_Dai = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "Dai", "Dài", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_LABORCOSTDETAIL_Dai);

            var V_LABORCOSTDETAIL_Day= new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "Day", "Dày", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_LABORCOSTDETAIL_Day);

            var V_LABORCOSTDETAIL_Rong = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "Rong", "Rộng", SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_LABORCOSTDETAIL_Rong);

            var V_LABORCOSTDETAIL_Price = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "Price", "Đơn giá", SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Price, 10, "", true);
            ListField.Add(V_LABORCOSTDETAIL_Price);

            var V_LABORCOSTDETAIL_IG = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "ItemGroup", "Chủng loại", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 100, "", true);
            ListField.Add(V_LABORCOSTDETAIL_IG);

            var V_LABORCOSTDETAIL_ApplyDate = new SapUDF(InternalVersion.ToString(), "V_LABORCOSTDETAIL", "ApplyDate", "Ngày áp dụng", SAPbobsCOM.BoFieldTypes.db_Date, SAPbobsCOM.BoFldSubTypes.st_None, 10, "", true);
            ListField.Add(V_LABORCOSTDETAIL_ApplyDate);
            return 1;
        }

        public SpecialReportVersion()
        {
            InternalVersion = VerBuilder.Version;
            ListField = new List<SapUDF>();
            ListTable = new List<SapUDT>();
            VerBuilder.Version++;
        }
    }
}
