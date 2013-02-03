using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Data.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class IVREntryController : ApiController
  {
    private IProfileManager profileManager;

    public IVREntryController(IProfileManager profileManager)
    {
      this.profileManager = profileManager;
    }

    public HttpResponseMessage Post(VoiceRequest request)
    {
      var response = new TwilioResponse();

      response.Say("Welcome to Refugees United.");

      string lookupPhoneNumber = string.Empty;

      if (request.Direction.Equals("inbound"))
      {
        lookupPhoneNumber = request.From;
      }
      else if (request.Direction.Equals("outbound-api"))
      {
        lookupPhoneNumber = request.To;
      }

      response.Say("we are looking up phone number " + string.Join(" ", lookupPhoneNumber.ToArray()));

      try
      {
        if (!profileManager.CheckNumber(lookupPhoneNumber))
        {
          response.Say("Sorry! That number isn't recognized. Please register via the web platform.");
          response.Hangup();
        }
        else
        {
          response.BeginGather(new { finishOnKey = "#", action = "/api/IVRAuthenticate" });
          response.Say("Please enter your pin, followed by hash.");
          response.EndGather();
        }
      }
      catch (Exception ex)
      {
        response.Say("an error has occured: " + ex.Message);
        response.Say("an error has occured: " + ex.Message);
        response.Say("an error has occured: " + ex.Message);
        response.Hangup();
      }

      return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
    }
  }
}
