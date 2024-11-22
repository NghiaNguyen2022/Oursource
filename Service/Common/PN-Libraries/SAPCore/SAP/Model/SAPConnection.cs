using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPCore.SAP.Model
{
    public class SAPConnection
    {
            public string SapUser { get; set; }
            public string SapPassword { get; set; }
            public string ServerName { get; set; }
            public string CompanyDB { get; set; }
            public string SLDServer { get; set; }
            public string DBUser { get; set; }
            public string DBPass { get; set; }

        public SAPConnection()
        {

        }
    }
}
