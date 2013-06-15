using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IIVRBroadcastLogic
  {
    TwilioResponse RecordBroadcast(VoiceRequest request, int profileId);
    TwilioResponse RecordBroadcast_SaveRecording(VoiceRequest request, int profileId);
    TwilioResponse ListenToBroadcasts(VoiceRequest request, int profileId);
  }
}
