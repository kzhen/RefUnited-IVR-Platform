using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IIVRMainLogic
  {
    TwilioResponse GetMainMenu();
    TwilioResponse GetMenuSelection(VoiceRequest request);

    TwilioResponse SaveRecording(VoiceRequest request);

    TwilioResponse ListFavourites(VoiceRequest request, int profileId);
    TwilioResponse RecordMessageForFavourite(VoiceRequest request, int profileId, string favs);
    TwilioResponse SaveRecordingForFavourite(VoiceRequest request, int profileId, int targetProfileId);

    TwilioResponse PlayRecordedVoiceMessage(VoiceRequest request, int profileId, int? recordingIdx);

    TwilioResponse PlayRecordedVoiceMessageSelection(VoiceRequest request, int profileId, int recordingIdx, int fromProfileId);

    TwilioResponse SaveVoiceMessageReply(VoiceRequest request, int profileId, int recordingIdx, int fromProfileId);

    TwilioResponse PlayPlatformMessages(VoiceRequest request, int profileId);
  }
}
