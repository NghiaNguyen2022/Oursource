﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDApp.Models
{
    public class BIDVInquiryResponse
    {
        public InquiryResponse body { get; set; }
    }
    public class InquiryResponse
    {
        public int totalRecords { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public long startingBal { get; set; }
        public long endingBal { get; set; }
        public List<InquiryResponseTran> trans { get; set; }
    }
    public class InquiryResponseTran
    {
        public int seq { get; set; }
        public string tranDate { get; set; }
        public string remark { get; set; }
        public string debitAmount { get; set; }
        public string creditAmount { get; set; }
        public string @ref { get; set; }
        public string currCode { get; set; }
    }

    public class BIDVInquiryResponse401Error
    {
        public string httpCode { get; set; }
        public string httpMessage { get; set; }
        public string moreInformation { get; set; }
    }
    public class BIDVInquiryResponse400Error
    {
        public ErrorResponse errorResponse { get; set; }
    }
    public class ErrorResponse
    {
        public Metadata metadata { get; set; }
        public AdditionalInfo additionalInfo { get; set; }
    }
    public class Metadata
    {
        public Status status { get; set; }
    }
    public class AdditionalInfo
    {
        public DetailedError detailedError { get; set; }
    }
    public class Status
    {
        public string code { get; set; }
        public string desc { get; set; }
    }
    public class DetailedError
    {
        public int errorCode { get; set; }
        public string desc { get; set; }
    }
    public class BIDVInquiryResponse500Error
    {
        public ErrorResponse errorResponse { get; set; }
    }
}