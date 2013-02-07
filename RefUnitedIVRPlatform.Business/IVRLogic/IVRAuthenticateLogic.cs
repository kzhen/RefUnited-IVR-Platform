using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Business.IVRLogic
{
  public class IVRAuthenticateLogic : IIVRAuthenticateLogic
  {
    private IProfileManager profileManager;

    public IVRAuthenticateLogic(IProfileManager profileManager)
    {
      this.profileManager = profileManager;
    }

    public TwilioResponse GetAuthentication(VoiceRequest request, string language)
    {
      if (string.IsNullOrEmpty(language))
      {
        language = LanguageHelper.GetDefaultCulture();
      }
      else
      {
        language = LanguageHelper.GetValidCulture(language);
      }

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
        //DEBUG
        response.Say("You entered: " + string.Join(" ", pin.ToArray()));
        response.Say("But the correct pin was: " + string.Join(" ", correctPin.ToArray()));
        //DEBUG
        
        response.Hangup();
      }

      return response;
    }
  }
}
