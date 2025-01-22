using System.Collections;

namespace STDApp.Models
{
    public class ApprovalInfo
    {
        public string Author { get; set; }
        public string Approval1 { get; set; }
        public string Approval2 { get; set; }

        public ApprovalInfo(Hashtable data)
        {
            if(data != null)
            {
                Author = data["Author"].ToString();
                Approval1 = data["Approval1"].ToString();
                Approval2 = data["Approval2"].ToString();
            }
        }
        public ApprovalInfo()
        {

        }
    }
}
