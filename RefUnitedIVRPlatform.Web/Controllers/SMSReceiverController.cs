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
      private string accountSid = "ACa311b713fe54bdd5706788790878d2c0";
      private string authToken = "60e8b2cdc1168b3a66d4cd23c8c5f748";
      private string twilioPhoneNumber = "+442033229301";

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
