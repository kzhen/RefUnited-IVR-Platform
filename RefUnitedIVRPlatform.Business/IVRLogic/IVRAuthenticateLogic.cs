﻿using RefUnitedIVRPlatform.Common;
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
    private IIVRRouteProvider routeProvider;

    public IVRAuthenticateLogic(IProfileManager profileManager, IIVRRouteProvider routeProvider)
    {
      this.profileManager = profileManager;
      this.routeProvider = routeProvider;
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

      string lookupPhoneNumber = request.GetOriginatingNumber();

      var pin = request.Digits;

      var result = profileManager.CheckPin(lookupPhoneNumber, pin);

      string correctPin = profileManager.GetPin(lookupPhoneNumber);

      if (result)
      {
        response.Redirect(routeProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU, language), "POST");
      }
      else
      {
        twiMLHelper.SayOrPlay(response, IVRAuthLang.IncorrectPIN);

        response.Hangup();
      }

      return response;
    }
  }
}
