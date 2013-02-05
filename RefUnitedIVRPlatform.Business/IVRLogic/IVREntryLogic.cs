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
        }

        response.Say(IVREntryLang.Welcome, new { language = culture });
        response.Say(string.Format(IVREntryLang.PhoneLookup, string.Join(" ", lookupPhoneNumber.ToArray())), new { language = culture });

        if (!knownNumber)
        {
          response.Say(IVREntryLang.PhoneNumberNotFound, new { language = culture });
          response.Hangup();
          return response;
        }

        response.BeginGather(new { finishOnKey = "#", action = "/api/IVRAuthenticate" });
        response.Say(IVREntryLang.EnterPin, new { language = culture });
        response.EndGather();

      }
      catch (Exception ex)
      {
        response.Say(string.Format(IVREntryLang.Error, ex.Message), new { language = culture });
        response.Say(string.Format(IVREntryLang.Error, ex.Message), new { language = culture });
        response.Say(string.Format(IVREntryLang.Error, ex.Message), new { language = culture });
        response.Hangup();
      }
      return response;
    }
  }
}
