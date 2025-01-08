using System.Collections;

namespace SAPCore.SAP.DIAPI.Models
{
    public class ServiceConnection
    {
        public string SapUser { get; set; }
        public string SapPassword { get; set; }
        public string ServerName { get; set; }
        public string CompanyDB { get; set; }
        public string SLDServer { get; set; }
        public string DBUser { get; set; }
        public string DBPass { get; set; }

        public ServiceConnection(Hashtable data)
        {
            SapUser = data["SapUser"].ToString();
            SapPassword = data["SapPassword"].ToString();
            ServerName = data["ServerName"].ToString();
            CompanyDB = data["CompanyDB"].ToString();
            SLDServer = data["SLDServer"].ToString();
            DBUser = data["DBUser"].ToString();
            DBPass = data["DBPass"].ToString();
        }
    }
}
