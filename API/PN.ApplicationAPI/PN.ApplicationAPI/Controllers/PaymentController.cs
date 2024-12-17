using APICore.Authen.Atts;
using PN.ApplicationAPI.APICore.Controller;
using PN.ApplicationAPI.Models;
using PN.SmartLib.Helper;
using SAPCore.Config;
using STD.DataReader;
using System.Collections.Generic;
using System.Web.Http;

namespace PN.ApplicationAPI.Controllers
{
    public class PaymentController : BaseController
    {
        [BasicAuthentication]
        [System.Web.Http.HttpPost]
        //[Route("get1/{param1}")]
        public IHttpActionResult Create([FromBody]Payoo_Payment input)
        {
            if (input == null)
            {
                return Ok(ResponseFaild("Data chưa đúng cấu trúc và định dạng"));
            }

            var query = string.Format(QueryString.CheckOrderExists, input.OrderNo);
            var data = DataProvider.QuerySingle(CoreSetting.DataConnection, query);
            if(data != null && data["Existed"].ToString() == "Existed")
            {
                return Ok(ResponseFaild("Order No đã tồn tại"));
            }
            var message = string.Empty;
            if (input.InsertData(ref message))
                return Ok(ResponseSuccessed(message));
            return Ok(ResponseFaild(message));
        }

        [Route("api/Payment/Invoice")]
        //[BasicAuthentication]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Invoice(string customerId, string taxNumber)
        {
            var data = new Payoo_InvoiceRes()
            {
                invoice_number = "123",
                vat_invoice_serial = "CTK/ST",
                vat_invoice_number = "AK123",
                invoice_date = "20240909",
                total_amount = 100000,
                paid_amount = 80000,
                remaining_amount = 20000
            };
            var data1 = new Payoo_InvoiceRes()
            {
                invoice_number = "124",
                vat_invoice_serial = "CTK/ST",
                vat_invoice_number = "AK124",
                invoice_date = "20240909",
                total_amount = 340000,
                paid_amount = 120000,
                remaining_amount = 220000
            };
            var datas = new List<Payoo_InvoiceRes>();
            datas.Add(data);
            datas.Add(data1);
            return Ok(datas);
        }
    }
}