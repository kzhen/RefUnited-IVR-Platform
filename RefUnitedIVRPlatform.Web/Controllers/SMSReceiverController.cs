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
        var response = new TwilioResponse();

        try
        {
          string outboundPhoneNumber = request.From;

          var client = new TwilioRestClient(accountSid, authToken);

          var call = client.InitiateOutboundCall(
            twilioPhoneNumber,
            outboundPhoneNumber,
            "http://refuniteivr.azurewebsites.net/api/IVREntry");

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
          response.Sms("exception: " + ex.Message);
          return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
        }
      }
    }
}
