using APICore.Authen.Atts;
using PN.ApplicationAPI.APICore.Controller;
using PN.ApplicationAPI.Models;
using SAPCore.Config;
using STD.DataReader;
using System.Web.Http;

namespace PN.ApplicationAPI.Controllers
{
    public class BankController : BaseController
    {
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Create([FromBody]BIDV_Accesstoken input)
        {
            if (input == null)
            {
                return Ok(ResponseFaild("Data chưa đúng cấu trúc và định dạng"));
            }

            //var query = string.Format(QueryString.CheckOrderExists, input.OrderNo);
            //var data = DataProvider.QuerySingle(CoreSetting.DataConnection, query);
            //if (data != null && data["Existed"].ToString() == "Existed")
            //{
            //    return Ok(ResponseFaild("Order No đã tồn tại"));
            //}
            var message = string.Empty;
            if (input.InsertData(ref message))
                return Ok(ResponseSuccessed(message));
            return Ok(ResponseFaild(message));
        }
    }
}