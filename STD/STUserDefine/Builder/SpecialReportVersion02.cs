using SAPCore.SAP;
using SAPCore.SAP.UserDefine;
using System.Collections.Generic;

namespace GTVUserDefine.Builder
{
    public class SpecialReportVersion02 : VerBuilder
    {
        public SpecialReportVersion02()
        {
            InternalVersion = VerBuilder.Version;
            ListField = new List<SapUDF>();
            VerBuilder.Version++;
        }

        public override int Init()
        {
            Dictionary<string, string> _activies = new Dictionary<string, string>();
            _activies.Add("Y", "Hoạt động");
            _activies.Add("N", "Ngừng hoạt động");

            var V_V_KWHM3_Active = new SapUDF(InternalVersion.ToString(), "V_KWHM3", "Active", "Hoạt động", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _activies, "", "", "", true);
            ListField.Add(V_V_KWHM3_Active);

            var V_XUONGINFO_Xuong = new SapUDF(InternalVersion.ToString(), "V_XUONGINFO", "Xuong", "Xưởng", SAPbobsCOM.BoFieldTypes.db_Alpha, 
                SAPbobsCOM.BoFldSubTypes.st_None, 30, "", true);
            ListField.Add(V_XUONGINFO_Xuong);

            var OITM_Line = new SapUDF(InternalVersion.ToString(), "OITM", "CLOAI", "Chủng loại", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 30);
            ListField.Add(OITM_Line);
            return 1;
        }
    }
}
