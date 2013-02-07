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
    private TwiMLHelper twiMLHelper;

    public IVRAuthenticateLogic(IProfileManager profileManager)
    {
      this.profileManager = profileManager;
    }

    public TwilioResponse GetAuthentication(VoiceRequest request, string language)
    {
      var response = new TwilioResponse();

      if (string.IsNullOrEmpty(language))
      {
        language = LanguageHelper.GetDefaultCulture();
      }
      else
      {
        language = LanguageHelper.GetValidCulture(language);
      }

      IVRAuthLang.Culture = new System.Globalization.CultureInfo(language);
      twiMLHelper = new TwiMLHelper(language, LanguageHelper.IsImplementedAsMP3(language));

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
        response.Redirect(string.Format("/IVRMain/MainMenu?language={0}", language), "POST");
      }
      else
      {
        twiMLHelper.SayOrPlay(response, IVRAuthLang.IncorrectPIN);

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
