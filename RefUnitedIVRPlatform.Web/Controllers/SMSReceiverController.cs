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
      private string accountSid = "";
      private string authToken = "";
      private string twilioPhoneNumber = "";

      public HttpResponseMessage Get(SmsRequest request)
      {
        try
        {
          string outboundPhoneNumber = "+447903467912";

          var client = new TwilioRestClient(accountSid, authToken);

          var call = client.InitiateOutboundCall(
            twilioPhoneNumber,
            outboundPhoneNumber,
            "http://refuniteivr.azurewebsites.net/api/IVREntry");

          var response = new TwilioResponse();

          if (call.RestException == null)
          {
            response.Sms("starting call to " + outboundPhoneNumber);
          }
          else
          {
            response.Sms("failed call to " + outboundPhoneNumber + " " + call.RestException.Message);
          }

          return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
        }
        catch (Exception ex)
        {
          throw new Exception("there was a problem " + ex.Message, ex);
        }
      }
    }
}
