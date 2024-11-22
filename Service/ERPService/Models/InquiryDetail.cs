using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Models
{
    public class InquiryDetail
    {
        public string order { get; set; }
        public string transactionDate { get; set; }
        public string transactionContent { get; set; }
        public string debit { get; set; }
        public string credit { get; set; }
        public string accountBal { get; set; }
        public string transactionNumber { get; set; }
        public string corresponsiveAccount { get; set; }
        public string corresponsiveAccountName { get; set; }
        public object agency { get; set; }
        public object virtualAccount { get; set; }
        public string corresponsiveBankName { get; set; }
        public string corresponsiveBankId { get; set; }
        public string serviceBranchId { get; set; }
        public object serviceBankName { get; set; }
        public string channel { get; set; }
    }
}
