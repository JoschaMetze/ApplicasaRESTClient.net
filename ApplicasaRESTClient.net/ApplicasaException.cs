﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ApplicasaRESTClientnet
{
    public class ApplicasaException : Exception
    {
        public override string ToString()
        {
            return base.ToString();
        }
        public string ErrorMessage { get; protected set; }
        public string ErrorCode { get; protected set; }
        public ApplicasaException(HttpResponseMessage response,string jsonResponse):base(response.ReasonPhrase)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var responseObj = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                ErrorMessage = responseObj.ApplicasaMessage;
                ErrorCode = responseObj.ApplicasaCode;
            }
            else
            {
                ErrorMessage = "Look at Message:" + jsonResponse;
            }
        }
    }
}
