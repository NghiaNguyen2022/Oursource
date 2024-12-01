using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ERPService.Common;

namespace ERPService.BackJob
{
    partial class PayooSettlementService : ServiceBase
    {
        public PayooSettlementService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Utils.WriteToFile($"On Start");

            }
            catch (FormatException ex)
            {
                Utils.WriteToFile($"Error: {ex.Message}");
                return;
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
