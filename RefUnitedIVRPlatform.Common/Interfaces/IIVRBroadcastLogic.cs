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
    TwilioResponse ListenToBroadcastsMenu(VoiceRequest request, int profileId);
    TwilioResponse ListenToPublicBroadcasts(VoiceRequest request, int profileId, int idx);
    TwilioResponse BroadcastResponseSelection(VoiceRequest request, int profileId, int lastBroadcastIdx);
    TwilioResponse RecordPrivateReply(VoiceRequest request, int profileId, int lastBroadcastIdx);
    TwilioResponse SavePrivateReply(VoiceRequest request, int profileId, int lastBroadcastIdx);
    TwilioResponse RecordPublicReply(VoiceRequest request, int profileId, int lastBroadcastIdx);
    TwilioResponse SavePublicReply(VoiceRequest request, int profileId, int lastBroadcastIdx);
    TwilioResponse ListenToMatchedBroadcasts(VoiceRequest request, int profileId);
    TwilioResponse ListenToBroadcastReplies(VoiceRequest request, int profileId, int lastBroadcastIdx, int subBroadcastIdx);
  }
}
