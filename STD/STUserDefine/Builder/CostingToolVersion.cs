using SAPCore.SAP;
using SAPCore.SAP.UserDefine;
using System.Collections.Generic;

namespace GTVUserDefine.Builder
{
    public class CostingToolVersion : VerBuilder
    {
        public CostingToolVersion()
        {
            InternalVersion = VerBuilder.Version;
            ListField = new List<SapUDF>();
            VerBuilder.Version++;
        }
        public override int Init()
        {
            Dictionary<string, string> _PayTypeValues = new Dictionary<string, string>();
            _PayTypeValues.Add("Y", "Cho phép phân bổ");
            _PayTypeValues.Add("N", "Không phân bổ");

            var OWHS_PB = new SapUDF(InternalVersion.ToString(), "OWHS", "PB", "PB (Phân bổ)", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _PayTypeValues, "N");
            var OITB_PB = new SapUDF(InternalVersion.ToString(), "OITB", "PB", "PB (Phân bổ)", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 1, _PayTypeValues, "N");

            ListField.Add(OWHS_PB);
            ListField.Add(OITB_PB);
            return 1;
        }
    }
}
