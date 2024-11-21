using GTVUserDefine.Builder;
using SAPCore;
using SAPCore.Config;
using SAPCore.SAP;

namespace GTVUserDefine
{
    public class DBConfiguration : DatabaseConfiguration
    {
        private static DBConfiguration instance;
        public static DBConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new DBConfiguration();
                return instance;
            }
        }
        private DBConfiguration()
        {
        }
        public void Config()
        {
            ConfigVersion(new PaymentToolVersion());
           // ConfigVersion(new CostingToolVersion());
            //ConfigVersion(new SpecialReportVersion());
           // ConfigVersion(new SpecialReportVersion02());
        }

        protected void ConfigVersion(VerBuilder version)
        {
            var message = string.Empty;
            version.Init();

            version.OnMessage += VerionMessage;
            version.RunCreate(CoreSetting.DataConnection, ref message);
            version.OnMessage -= VerionMessage;
        }

        private void VerionMessage(string message)
        {
            UIHelper.LogMessage(message, UIHelper.MsgType.StatusBar);
        }
    }
}
