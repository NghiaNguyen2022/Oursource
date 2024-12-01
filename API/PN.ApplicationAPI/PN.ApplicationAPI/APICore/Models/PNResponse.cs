using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PN.ApplicationAPI.APICore.Models
{
    public enum PNResponseCode
    {
        Failed = -1,
        Succeed = 0,
        Pending = 1
    }
    public class BaseResponse
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public BaseResponse(int code, string message)
        {
            ResponseCode = code;
            Message = message;
        }
    }
    public class PNResponse : BaseResponse
    {
        public PNResponse(int code, string message) : base(code, message)
        {
        } 
    }
}