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
  public class IVREntryLogic : IIVREntryLogic
  {
    private IProfileManager profileManager;
    private TwiMLHelper twiMLHelper;

    public IVREntryLogic(IProfileManager profileManager)
    {
      this.profileManager = profileManager;
    }

    public TwilioResponse GetGreeting(VoiceRequest request)
    {
      string culture = LanguageHelper.GetDefaultCulture();
      var response = new TwilioResponse();
      
      try
      {
        string lookupPhoneNumber = string.Empty;

        if (request.Direction.Equals("inbound"))
        {
          lookupPhoneNumber = request.From;
        }
        else if (request.Direction.Equals("outbound-api"))
        {
          lookupPhoneNumber = request.To;
        }

        var knownNumber = profileManager.CheckNumber(lookupPhoneNumber);

        if (knownNumber)
        {
          culture = profileManager.GetCulture(lookupPhoneNumber);
          if (string.IsNullOrEmpty(culture))
          {
            culture = LanguageHelper.GetDefaultCulture();
          }
          else
          {
            culture = LanguageHelper.GetValidCulture(culture); 
          }

          IVREntryLang.Culture = new System.Globalization.CultureInfo(culture);
          twiMLHelper = new TwiMLHelper(culture, LanguageHelper.IsImplementedAsMP3(culture));
        }

        twiMLHelper.SayOrPlay(response, IVREntryLang.Welcome);

        twiMLHelper.SayOrPlay(response, string.Format(IVREntryLang.PhoneLookup, string.Join(" ", lookupPhoneNumber.ToArray())));

        if (!knownNumber)
        {
          twiMLHelper.SayOrPlay(response, IVREntryLang.PhoneNumberNotFound);
          response.Hangup();
          return response;
        }

        response.BeginGather(new { finishOnKey = "#", action = "/api/IVRAuthenticate" });
        twiMLHelper.SayOrPlay(response, IVREntryLang.EnterPin);
        response.EndGather();

      }
      catch (Exception ex)
      {
        twiMLHelper.SayOrPlay(response, string.Format(IVREntryLang.Error, ex.Message));
        twiMLHelper.SayOrPlay(response, string.Format(IVREntryLang.Error, ex.Message));
        twiMLHelper.SayOrPlay(response, string.Format(IVREntryLang.Error, ex.Message));
        response.Hangup();
      }
      return response;
    }
  }
}
