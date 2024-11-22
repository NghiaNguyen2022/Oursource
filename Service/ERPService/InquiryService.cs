using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using System.Web.Script.Serialization;
using ERPService.DataReader;
using ERPService.Models;
using PN.SmartLib.Helper;
using RestSharp;
using SAPbobsCOM;
using SAPCore;
using SAPCore.Config;
using SAPCore.SAP.DIAPI;
using static System.Net.WebRequestMethods;

namespace ERPService
{
    public partial class InquiryService : ServiceBase
    {
        Timer timer = new Timer();
        public InquiryService()
        {
            InitializeComponent();
        }
        protected override void OnStop()
        {
            //WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //WriteToFile("Service is recall at " + DateTime.Now);
        }

        protected override void OnStart(string[] args)
        {
            Process();

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000 * 3600; //number in milisecinds
            timer.Enabled = true;
        }

        bool hasData = false;
        Hashtable[] Datas;

        private string CreateBody(bool first = true)
        {
            var data = new InquiryRequest()
            {
                requestId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                model = "2",
                providerId = ConfigurationManager.AppSettings["ProviderId"],
                account = ConfigurationManager.AppSettings["Taikhoan"],
                
                fromDate = first ? DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy") :  DateTime.Now.ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime
                toDate = DateTime.Now.ToString("dd/MM/yyyy"), // Chuyển đổi từ chuỗi sang DateTime

                fromTime = first ?  "17:15:00" : "12:15:00",
                toTime = first ? "11:15:00" : "17:15:00",
                accountType = "D",
                collectionType = "c,d",
                agencyType = "a",
                transTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                channel = "ERP",
                version = "1",
                clientIP = "",
                language = "vi",
                signature = ""
            }; 
            var path = AppDomain.CurrentDomain.BaseDirectory + @"\Info\" + "private.pem";

            data.signature = FPT.SHA256_RSA2048.Encrypt(data.signature, path);

            var objJson = new JavaScriptSerializer();
            var json = objJson.Serialize(data);

            return string.Empty;
        }
        private void CallAPI()
        {
            var options = new RestClientOptions("https://api-uat.vietinbank.vn")
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("/vtb-api-uat/development/erp/v1/statement/inquiry", Method.Post);
            request.AddHeader("x-ibm-client-id", ConfigurationManager.AppSettings["ClientID"]);
            request.AddHeader("x-ibm-client-secret", ConfigurationManager.AppSettings["ClientSecret"]);
            request.AddHeader("Content-Type", "application/json");
            //request.AddHeader("Cookie", "TS013bf802=013caf07cb3822867b1b0e1ee5c7c3cb089b742bce940d1b15cc8d50ea123b077ef15e8ebbc8df562125275bde3b2daafd75473e44");


            hasData = false;
            Datas = null;

            var querydata = QueryString.GetDocumentApprovedQuery;
            Datas = DataProvider.QueryList(CoreSetting.DataConnection, querydata);

            if (Datas != null && Datas.Length > 0)
            {
                hasData = true;
            }
            else
            {
                hasData = false;
                Datas = null;
            }
        }

        private bool ReadConfigAndConnect(ref string message)
        {
            //var query = QueryString.SAPConnection;
            //var data = DataProvider.QuerySingle(CoreSetting.DataConnection, QueryString.SAPConnection);
            //if(data == null)
            //{
            //    message = StringConstrants.NoDIConnectConfig;
            //    return false;
            //}

            //var config = new ServiceConnection(data);
            //var cont = DIServiceConnection.Instance.ConnectDI(config, ref message);

            //if(!cont)
            //{
            //    message = StringConstrants.CanNotConnectCompany;
            //    return false;
            //}

            return true;

        }
        private void DisConnect()
        {
           // DIServiceConnection.Instance.DIDisconnect();
        }
        private void Process()
        {
            //WriteToFile("Service is started at " + DateTime.Now);

            CallAPI();

            if (!hasData)
            {
               // WriteToFile(StringConstrants.NotHaveData + DateTime.Now);
                return;
            }

            var message = string.Empty;
            if(!ReadConfigAndConnect(ref message))
            {
                WriteToFile(message + DateTime.Now);
                return;
            }
            foreach (var dt in Datas)
            {
                message = string.Empty;
                var doc = new ERPDocument(dt);
                var ret = ConvertDrafToDocument(doc, ref message);
               
                var query = string.Empty;
                if(ret)
                {
                    query = string.Format(QueryString.UpdateAfterExecute, doc.Objtype, doc.DocEntry, doc.DocNum, "S", message);
                }
                else
                {
                    query = string.Format(QueryString.UpdateAfterExecute, doc.Objtype, doc.DocEntry, doc.DocNum, "F", message);
                }

                DataProvider.ExecuteNonQuery(CoreSetting.DataConnection, query);
                WriteToFile($"After: {message} " + DateTime.Now);
            }
            DisConnect();

        }

        private bool ConvertDrafToDocument(ERPDocument document, ref string message)
        {
            return false;
           // return DIServiceConnection.Instance.ConvertDraft(document.DocEntry, document.Objtype, ref message);
          
        }
      
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!System.IO.File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = System.IO.File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
