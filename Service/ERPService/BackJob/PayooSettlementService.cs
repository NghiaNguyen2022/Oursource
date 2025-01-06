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
using System.Timers;
using APIHandler;
using ERPService.Common;

namespace ERPService.BackJob
{
    partial class PayooSettlementService : ServiceBase
    {
        Timer timer = new Timer();
        public PayooSettlementService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Utils.WriteToFile($"On Start");
                run();
            }
            catch (FormatException ex)
            {
                Utils.WriteToFile($"Error: {ex.Message}");
                return;
            }
        }
        private void run()
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 60000;
            timer.Enabled = true;
            timer.Start();
        }
        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        public void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {

                if (GlobalConfig.InquiryRunner == null)
                    GlobalConfig.InquiryRunner = new RunnerTime();

                var timeRun1String = ConfigurationManager.AppSettings["timeRun1"];
                var timeRun2String = ConfigurationManager.AppSettings["timeRun2"];
                var timeRun1 = TimeSpan.Parse(timeRun1String);
                var timeRun2 = TimeSpan.Parse(timeRun2String);

                DateTime now = DateTime.Now;
                TimeSpan currentTime = now.TimeOfDay;
                //DateTime fromDate, toDate;
                var message = string.Empty;
                if (GlobalConfig.InquiryRunner != null)
                {
                    if (GlobalConfig.InquiryRunner.RunDate.Date != now.Date)
                    {
                        GlobalConfig.InquiryRunner.RunDate = now;
                        GlobalConfig.InquiryRunner.Timer = 0;
                    }

                    Utils.WriteToFile($"Currently {GlobalConfig.InquiryRunner.RunDate.Date } - {GlobalConfig.InquiryRunner.Timer}");
                    // Match the current time to the configured times
                    if (currentTime.Hours == timeRun1.Hours && currentTime.Minutes >= timeRun1.Minutes
                        && GlobalConfig.InquiryRunner.Timer == 0)
                    {
                        PayooHandler.CallPayooAPI(now.ToString("yyyyMMdd"), 1, ref message);
                        Utils.WriteToFile($"{message}");
                        GlobalConfig.InquiryRunner.Timer = 1;
                    }
                    else if (currentTime.Hours == timeRun2.Hours && currentTime.Minutes >= timeRun2.Minutes
                        && GlobalConfig.InquiryRunner.Timer == 1)
                    {
                        PayooHandler.CallPayooAPI(now.ToString("yyyyMMdd"), 2, ref message);
                        Utils.WriteToFile($"{message}");
                        GlobalConfig.InquiryRunner.Timer = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.WriteToFile($"Error catching runTime and generate fromDate toDate: {ex.Message}");
                return;
            }
        }

    }
}
