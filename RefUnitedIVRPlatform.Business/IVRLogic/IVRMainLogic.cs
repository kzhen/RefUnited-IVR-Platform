using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Mvc;
using Twilio.TwiML;
using RefUnitedIVRPlatform.Common;

namespace RefUnitedIVRPlatform.Business.IVRLogic
{
  public class IVRMainLogic : IIVRMainLogic
  {
    private IProfileManager profileManager;
    private IRefugeesUnitedAccountManager refUnitedAcctManager;

    public IVRMainLogic(IProfileManager profileManager, IRefugeesUnitedAccountManager refUnitedAcctManager)
    {
      this.profileManager = profileManager;
      this.refUnitedAcctManager = refUnitedAcctManager;
    }

    public TwilioResponse GetMainMenu()
    {
      var response = new TwilioResponse();

      response.Say("Main menu.");
      response.BeginGather(new { numDigits = 1, action = "/IVRMain/MainMenuSelection" });
      response.Say("Press one to check messages.");
      response.Say("Press two to listen to old messages.");
      response.Say("Press three to send a voice message to a favourite.");
      response.Say("Press four to listen to voice messages.");
      response.Say("Press five to send a public broadcast.");
      response.Say("Press six to listen public broadcasts.");

      response.EndGather();

      return response;
    }

    public TwilioResponse GetMenuSelection(VoiceRequest request)
    {
      var response = new TwilioResponse();

      try
      {
        string lookupPhoneNumber = request.GetOriginatingNumber();

        int profileId = profileManager.GetProfileId(lookupPhoneNumber);

        var selection = int.Parse(request.Digits);

        switch (selection)
        {
          case 1:
            response.Say("Looking up unread messages.");
            var unreadCount = refUnitedAcctManager.GetUnreadMessageCount(profileId);
            response.Say(string.Format("You have {0} message{1}", unreadCount, (unreadCount == 1) ? "" : "s"));
            break;
          case 2:
            response.Say("Looking up messages");
            response.Redirect(string.Format("/IVRMain/ReadPlatformMessages?profileId={0}", profileId));
            break;
          case 3:
            response.Redirect(string.Format("/IVRMain/SendFavMessage_ListFavs?profileId={0}", profileId));
            break;
          case 4:
            response.Redirect(string.Format("/IVRMain/PlayRecordedMessage?profileId={0}", profileId));
            break;
          case 5:
            response.Redirect(string.Format("/IVRBroadcast/RecordBroadcast?profileId={0}", profileId));
            break;
          case 6:
            response.Redirect(string.Format("/IVRBroadcast/ListenToBroadcasts?profileId={0}", profileId));
            break;
          default:
            break;
        }
        response.Redirect("/IVRMain/MainMenu");
      }
      catch (Exception ex)
      {
        response.Say("an error has occured. " + ex.Message);
      }

      return response;
    }

    public TwilioResponse ListFavourites(VoiceRequest request, int profileId, int? pageIdx)
    {
      var response = new TwilioResponse();

      if (!pageIdx.HasValue)
      {
        pageIdx = 0;
      }

      var favourites = refUnitedAcctManager.GetFavourites(profileId, pageIdx.Value);

      if (favourites == null || favourites.Count == 0)
      {
        response.Say("You have no favourites to send voice messages to.");
        response.Redirect("/IVRMain/MainMenu");

        return response;
      }

      response.Say("Listing favourites");

      string favs = string.Join(",", favourites.Select(f => f.ProfileId).ToList());

      response.BeginGather(new { numDigits = 1, action = string.Format("/IVRMain/SendFavMessage_RecordMsg?profileId={0}&favs={1}&pageIdx={2}", profileId, favs, pageIdx.Value) });

      for (int i = 1; i <= favourites.Count; i++)
      {
        var fav = favourites[i - 1];
        response.Say(string.Format("To send a message to {0} {1} press {2}", fav.FirstName, fav.Surname, i));
      }

      response.Say("Press star to return to main menu");

      if (favourites.Count > 9)
      {
        response.Say("To go to the next page press hash");
      }

      response.EndGather();

      return response;
    }

    public TwilioResponse RecordMessageForFavourite(VoiceRequest request, int profileId, string favs, int pageIdx)
    {
      var response = new TwilioResponse();

      var favsArray = favs.Split(',');

      if (request.Digits.Equals("#"))
      {
        response.Redirect(string.Format("/IVRMain/SendFavMessage_ListFavs?profileId={0}&pageIdx={1}", profileId, ++pageIdx));
        return response;
      }
      else if (request.Digits.Equals("*"))
      {
        response.Redirect("/IVRMain/MainMenu");

        return response;
      }

      var selection = int.Parse(request.Digits);

      string targetProfileId = favsArray[selection - 1];

      response.Say("Record your message after the tone, press any key when you are done.");
      response.Record(new { action = string.Format("/IVRMain/SendFavMessage_SaveRecording?profileId={0}&targetProfileId={1}", profileId, targetProfileId) });

      return response;
    }

    public TwilioResponse SaveRecordingForFavourite(VoiceRequest request, int profileId, int targetProfileId)
    {
      var url = request.RecordingUrl;

      profileManager.SaveRecording(profileId, targetProfileId, url);

      var response = new TwilioResponse();

      response.Say("Thank you. Your message has been saved.");

      response.Redirect("/IVRMain/MainMenu");

      return response;
    }

    public TwilioResponse PlayRecordedVoiceMessage(VoiceRequest request, int profileId, int? recordingIdx)
    {
      var response = new TwilioResponse();

      var voiceMessages = profileManager.GetRecordings(profileId);

      if (voiceMessages == null || voiceMessages.Count == 0)
      {
        response.Say("You have no voice messages");
        response.Redirect("/IVRMain/MainMenu");

        return response;
      }

      if (recordingIdx.HasValue && recordingIdx.Value >= voiceMessages.Count)
      {
        response.Say("No more messages.");
        response.Redirect("/IVRMain/MainMenu");

        return response;
      }

      Recording voiceMessage;

      if (recordingIdx.HasValue)
      {
        voiceMessage = voiceMessages[recordingIdx.Value];
      }
      else
      {
        recordingIdx = 0;
        voiceMessage = voiceMessages[recordingIdx.Value];
      }

      try
      {
        var profile = profileManager.GetProfile(voiceMessage.FromProfileId);

        response.Say(string.Format("Playing message {0} from {1}", recordingIdx + 1, profile.FullName));
      }
      catch (Exception)
      {
        response.Say(string.Format("Playing message {0}", recordingIdx + 1));
      }

      response.Play(voiceMessage.Url);

      response.BeginGather(new { numDigits = 1, action = string.Format("/IVRMain/PlayRecordedMessage_Response?profileId={0}&recordingIdx={1}&fromProfileId={2}", profileId, recordingIdx.Value, voiceMessage.FromProfileId) });
      response.Say("Press one to repeat this message");
      response.Say("Press two to delete this message");
      response.Say("Press three to reply to this message");
      response.Say("Press four to go to the next message");
      response.EndGather();

      return response;
    }

    public TwilioResponse PlayRecordedMessage_Response(VoiceRequest request, int profileId, int recordingIdx, int fromProfileId)
    {
      var response = new TwilioResponse();

      var digits = int.Parse(request.Digits);

      switch (digits)
      {
        case 1:
          response.Redirect(string.Format("/IVRMain/PlayRecordedMessage?profileId={0}&recordingIdx={1}", profileId, recordingIdx));
          break;
        case 2:
          profileManager.DeleteRecording(profileId, recordingIdx);
          response.Redirect(string.Format("/IVRMain/PlayRecordedMessage?profileId={0}&recordingIdx={1}", profileId, recordingIdx));
          break;
        case 3:
          response.Say("At the tone please record your response. Press any key when you are done.");
          response.Record(new { action = string.Format("/IVRMain/PlayRecordedMessage_SaveResponse?profileId={0}&recordingIdx={1}&fromProfileId={2}", profileId, recordingIdx, fromProfileId) });
          break;
        case 4:
          response.Redirect(string.Format("/IVRMain/PlayRecordedMessage?profileId={0}&recordingIdx={1}", profileId, ++recordingIdx));
          break;
      }

      return response;
    }

    public TwilioResponse SaveVoiceMessageReply(VoiceRequest request, int profileId, int recordingIdx, int fromProfileId)
    {
      var response = new TwilioResponse();

      profileManager.SaveRecording(profileId, fromProfileId, request.RecordingUrl);

      response.Say("Your message has been saved.");

      response.Redirect(string.Format("/IVRMain/PlayRecordedMessage?profileId={0}&recordingIdx={1}", profileId, ++recordingIdx));

      return response;
    }

    public TwilioResponse PlayPlatformMessages(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      var messages = refUnitedAcctManager.GetMessages(profileId);

      if (messages.threads == null || messages.threads.Length == 0)
      {
        response.Say("You have no messages.");
        response.Redirect("/IVRMain/MainMenu");

        return response;
      }

      foreach (var msg in messages.threads)
      {
        response.Say("Next message");
        response.Say(msg.TextSnippet);
      }

      response.Redirect("/IVRMain/MainMenu");

      return response;
    }
  }
}