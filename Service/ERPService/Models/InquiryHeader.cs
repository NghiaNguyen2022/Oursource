using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Models
{
    public class Status
    {
        public string code { get; set; }
        public string message { get; set; }
    }
    internal class InquiryHeader
    {
        public string requestId { get; set; }
        public string providerId { get; set; }
        public object merchantId { get; set; }
        public Status status { get; set; }
        public string account { get; set; }
        public string companyName { get; set; }
        public string accountType { get; set; }
        public string curency { get; set; }
        public string accountBal { get; set; }
        public string availableBal { get; set; }
        public string openningBal { get; set; }
        public string closingBal { get; set; }
        public string fromDate { get; set; }
        public string totalCredit { get; set; }
        public string numberCreditTransaction { get; set; }
        public string totalDebit { get; set; }
        public string toDate { get; set; }
        public string numberDebitTransaction { get; set; }
        public string fromTime { get; set; }
        public string toTime { get; set; }
        public List<InquiryDetail> transactions { get; set; }
        public string signature { get; set; }
    }
}
