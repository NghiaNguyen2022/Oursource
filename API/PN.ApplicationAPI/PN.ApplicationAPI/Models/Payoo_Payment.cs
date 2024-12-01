using SAPCore.SAP.DIAPI;
using STD.DataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PN.ApplicationAPI.Models
{
    public class Payoo_Payment
    {
        public int PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; }
        public string PurDate { get; set; }
        public string MerchantUserName { get; set; }
        public decimal ShopId { get; set; }
        public decimal MasterShopID { get; set; }
        public string OrderNo { get; set; }
        public decimal OrderCash { get; set; }
        public string BankName { get; set; }
        public string CardNumber { get; set; }
        public int CardIssuanceType { get; set; }
        public int PaymentStatus { get; set; }
        public string MDD1 { get; set; }
        public string MDD2 { get; set; }
        public string PaymentSource { get; set; }
        public decimal VoucherTotalAmount { get; set; }
        public string VoucherID { get; set; }

        public bool InsertData(ref string message)
        {
            //using(var trans)
            try
            {
                var query = "INSERT INTO \"" + DIConnection.Instance.CompanyDB + "\".\"tb_Payoo_PaymentINT\" VALUES ( ";
                query += $"{PaymentMethod},";
                query += $"'{PaymentMethodName}',";
                query += $"'{PurDate}',";
                query += $"'{MerchantUserName}',";
                query += $"{ShopId},";
                query += $"{MasterShopID},";
                query += $"'{OrderNo}',";
                query += $"{OrderCash},";
                query += $"'{BankName}',";
                query += $"'{CardNumber}',";
                query += $"{CardIssuanceType},";
                query += $"{PaymentStatus},";
                query += $"'{MDD1}',";
                query += $"'{MDD2}',";
                query += $"'{PaymentSource}',";
                query += $"{VoucherTotalAmount},";
                query += $"'{VoucherID}',";
                query += $"'{DateTime.Now.ToString("yyyyMMdd")}',";
                query += $"'{DateTime.Now.ToString("HHmmss")}'";
                query += ")";

                var ret1 = dbProvider.ExecuteNonQuery(query);
                if(ret1 == 1)
                {
                    message = "Lưu thành công";
                }
                else
                {
                    message = "Lưu thất bại";
                }
                return ret1 == 1;
            }
            catch (Exception ex)
            {
               
            }
            return false;
        }
    }
}