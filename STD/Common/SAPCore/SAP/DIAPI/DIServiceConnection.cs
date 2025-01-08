using PN.SmartLib.Helper;
using SAPbobsCOM;
using SAPCore.SAP.DIAPI.Models;
using System;

namespace SAPCore.SAP.DIAPI
{
    public class DIServiceConnection
    {
        private static DIServiceConnection instance;

        public Company Company { get; set; }
        /// <summary>
        /// Singleton pattern to control creation one Instance to connect DI API
        /// </summary>
        public static DIServiceConnection Instance
        {
            get
            {
                if (instance == null)
                    instance = new DIServiceConnection();
                return instance;
            }
        }

        private DIServiceConnection()
        {
            Company = new Company();
        }
        public bool DIDisconnect()
        {
            try
            {
                if (Company.Connected)
                    Company.Disconnect();
            }
            catch (Exception ex)
            {
                // message = string.Format("Disconnect fail - {0}", ex.Message);
                return false;
            }
            return true;
        }

        public bool ConnectDI(ServiceConnection model, ref string message)
        {

            if (Company == null)
            {
                message = "Không thể kết nối";
                return false;
            }

            if (model == null)
            {
                message = "Không thể kết nối";
                return false;
            }

            if (Company.Connected == true)
            {
                Company.Disconnect();
            }

            try
            {
                Company.Server = model.ServerName;
                Company.DbServerType = BoDataServerTypes.dst_HANADB;
                Company.CompanyDB = model.CompanyDB;
                Company.UseTrusted = false;

                Company.UserName = model.SapUser;
                Company.Password = StringUtils.DecryptString(model.SapPassword);

                Company.DbUserName = model.DBUser;
                Company.DbPassword = StringUtils.DecryptString(model.DBPass);

                Company.LicenseServer = model.SLDServer;
                Company.SLDServer = model.SLDServer;
                Company.language = BoSuppLangs.ln_English;

                var ret = Company.Connect();
                if (ret != 0)
                {
                    int lErrCode;
                    Company.GetLastError(out lErrCode, out message);
                    return false;
                }
                else
                {
                    message = $"DI API connected to {model.CompanyDB}";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public static bool CancelDocument(int docentry, ref string message)
        {

            try
            {
                SAPbobsCOM.Documents oInvoice = (SAPbobsCOM.Documents)Instance.Company.GetBusinessObject(BoObjectTypes.oInvoices);

                if (oInvoice.GetByKey(docentry))
                {

                    var ret = oInvoice.Cancel();
                    if (ret == 0)
                    {
                        message = "Cancel success";
                        return true;
                    }
                    else
                    {
                        message = Instance.Company.GetLastErrorDescription();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            return false;
        }
    }
}

