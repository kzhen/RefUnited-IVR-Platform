using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Business.IVRLogic
{
  public class IVRBroadcastLogic : IIVRBroadcastLogic
  {
    IBroadcastManager broadcastManager;

    public IVRBroadcastLogic(IBroadcastManager broadcastManager)
    {
      this.broadcastManager = broadcastManager;
    }

    public TwilioResponse RecordBroadcast(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      response.Say("At the tone record your message, to finish press any key. Please note that this will be public to all people on the Refugees United platform.");
      response.Record(new { action = string.Format("/IVRMain/RecordBroadcast_SaveRecording?profileId={0}", profileId), playBeep = true });

      return response;
    }


    public TwilioResponse RecordBroadcast_SaveRecording(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      broadcastManager.SaveBroadcast(new PublicBroadcast()
      {
        FromProfileId = profileId,
        Url = request.RecordingUrl
      });

      response.Say("Thankyou, your broadcast has been sent.");
      response.Redirect("/IVRMain/MainMenu");

      return response;
    }

    public TwilioResponse ListenToBroadcasts(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      if (string.IsNullOrEmpty(request.Digits))
      {
        response.BeginGather(new { numDigits = 1, action = string.Format("/IVRMain/ListenToBroadcasts?profileId={0}", profileId) });
        response.Say("Press one to listen to all public broadcasts");
        response.Say("Press two to listen to public broadcasts which match your country of origin");
        response.EndGather();

        return response;
      }

      switch (request.Digits)
      {
        case "1":
          response.Redirect("/");
          return response;
        case "2":
          return response;
        default:
          response.Redirect("/IVRMain/MainMenu");
          return response;
      }
    }
  }
}
