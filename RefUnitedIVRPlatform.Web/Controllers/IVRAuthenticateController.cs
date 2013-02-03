using RefUnitedIVRPlatform.Common.Interfaces;
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
    public class IVRAuthenticateController : ApiController
    {
      private IProfileManager profileManager;

      public IVRAuthenticateController(IProfileManager profileManager)
      {
        this.profileManager = profileManager;
      }

      public HttpResponseMessage Post(VoiceRequest request)
      {
        var response = new TwilioResponse();

        string lookupPhoneNumber = string.Empty;

        if (request.Direction.Equals("inbound"))
        {
          lookupPhoneNumber = request.From;
        }
        else if (request.Direction.Equals("outbound-api"))
        {
          lookupPhoneNumber = request.To;
        }

        var pin = request.Digits;

        var result = profileManager.CheckPin(lookupPhoneNumber, pin);

        string correctPin = profileManager.GetPin(lookupPhoneNumber);

        if (result)
        {
          response.Redirect("/IVRMain/MainMenu", "POST");
        }
        else
        {
          response.Say("Your PIN was incorrect, sad face.");
          response.Say("You entered: " + string.Join(" ", pin.ToArray()));
          response.Say("But the correct pin was: " + string.Join(" ", correctPin.ToArray()));
          response.Hangup();
        }

        return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
      }
    }
}
