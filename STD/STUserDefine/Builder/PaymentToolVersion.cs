using SAPCore.SAP;
using SAPCore.SAP.UserDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTVUserDefine.Builder
{
    public class PaymentToolVersion : VerBuilder
    {
        public PaymentToolVersion()
        {
            InternalVersion = VerBuilder.Version;
            ListField = new List<SapUDF>();
            VerBuilder.Version++;
        }

        public override int Init()
        {
            var OVPM_PaymentKey = new SapUDF(InternalVersion.ToString(), "OVPM", "PaymentKey", "Payment Key", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, "");
            var ORCT_PaymentKey = new SapUDF(InternalVersion.ToString(), "ORCT", "PaymentKey", "Payment Key", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, "");

            Dictionary<string, string> _PayTypeValues = new Dictionary<string, string>();
            _PayTypeValues.Add("PT", "Phiếu Thu");
            _PayTypeValues.Add("PC", "Phiếu Chi");
            _PayTypeValues.Add("UC", "Ủy Nhiệm Chi");

            var OVPM_PayType = new SapUDF(InternalVersion.ToString(), "OVPM", "PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, _PayTypeValues, "PT");
            var ORCT_PayType = new SapUDF(InternalVersion.ToString(), "ORCT", "PayType", "Payment Type", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, _PayTypeValues, "PT");

            var OVPM_CashFlow = new SapUDF(InternalVersion.ToString(), "OVPM", "CashFlow", "Cash Flow", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, "");
            var ORCT_CashFlow = new SapUDF(InternalVersion.ToString(), "ORCT", "CashFlow", "Cash Flow", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, "");

            ListField.Add(OVPM_PaymentKey);
            ListField.Add(ORCT_PaymentKey);
            ListField.Add(OVPM_PayType);
            ListField.Add(ORCT_PayType);
            ListField.Add(OVPM_CashFlow);
            ListField.Add(ORCT_CashFlow);

            var OVPM_Bank = new SapUDF(InternalVersion.ToString(), "OVPM", "Bank", "Bank", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, "");
            var ORCT_Bank = new SapUDF(InternalVersion.ToString(), "ORCT", "Bank", "Bank", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 50, "");
            ListField.Add(ORCT_Bank);
            ListField.Add(OVPM_Bank);

            var _paymnetReviewValues = new Dictionary<string, string>();
            _paymnetReviewValues.Add("Y", "Xác nhận");
            _paymnetReviewValues.Add("N", "Chưa xác nhận");

            var OVPM_Review = new SapUDF(InternalVersion.ToString(), "OVPM", "Review", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 10, _paymnetReviewValues, "N");
            var ORCT_Review = new SapUDF(InternalVersion.ToString(), "ORCT", "Review", "Xác nhận", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 10, _paymnetReviewValues, "N");

            ListField.Add(ORCT_Review);
            ListField.Add(OVPM_Review);

            var _StatusValues = new Dictionary<string, string>();
            _StatusValues.Add("N", "Yêu cầu thanh toán");
            _StatusValues.Add("R", "Đề xuất thanh toán");
            _StatusValues.Add("A", "Đã phê duyệt");
            _StatusValues.Add("J", "Đã từ chối");
            _StatusValues.Add("S", "Đã tạo");
            var OVPM_Status = new SapUDF(InternalVersion.ToString(), "OVPM", "Status", "Trạng thái", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 10, _StatusValues, "N");
            var ORCT_Status = new SapUDF(InternalVersion.ToString(), "ORCT", "Status", "Trạng thái", SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None, 10, _StatusValues, "N");

            ListField.Add(OVPM_Status);
            ListField.Add(ORCT_Status);
            return 1;
        }

    }
}
