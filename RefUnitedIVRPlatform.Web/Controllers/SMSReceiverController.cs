using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Twilio;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class SMSReceiverController : ApiController
  {
    private ISMSReceiverLogic smsLogic;

    public SMSReceiverController(ISMSReceiverLogic smsLogic)
    {
      this.smsLogic = smsLogic;
    }

    public HttpResponseMessage Post(SmsRequest request)
    {
      TwilioResponse response = smsLogic.ResponseToSms(request);
      return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
    }
  }
}
