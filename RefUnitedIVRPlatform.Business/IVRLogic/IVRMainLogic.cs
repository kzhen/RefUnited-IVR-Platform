using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Mvc;
using Twilio.TwiML;

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
      response.Say("Press eight to record a test message.");
      response.Say("Press nine to listen to all recorded messages.");
      response.EndGather();

      return response;
    }


    public TwilioResponse GetMenuSelection(VoiceRequest request)
    {
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

        int profileId = profileManager.GetProfileId(lookupPhoneNumber);

        var selection = int.Parse(request.Digits);

        switch (selection)
        {
          case 1:
            //listen to unread messages...
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
          case 8:
            response.Say("Time to record something, press any key when you are done.", new { voice = "woman" });
            response.Record(new { action = "/IVRMain/SaveRecording" });
            break;
          case 9:
            var recordings = profileManager.GetRecordingUrls();

            if (recordings.Count == 0)
            {
              response.Say("You have no recorded messages.");
            }
            else
            {
              response.Say(string.Format("You have {0} recorded message{1}", recordings.Count, (recordings.Count == 1) ? "" : "s"));
              foreach (var recording in recordings)
              {
                response.Play(recording);
              }
            }
            break;
          default:
            break;
        }

        response.Redirect("/IVRMain/MainMenu");
      }
      catch (Exception ex)
      {
        response.Say("an error has occured. " + ex.Message);
        response.Say("an error has occured. " + ex.Message);
        response.Say("an error has occured. " + ex.Message);
      }

      return response;
    }


    public TwilioResponse SaveRecording(VoiceRequest request)
    {
      var url = request.RecordingUrl;

      profileManager.SaveRecording(url);

      var response = new TwilioResponse();

      response.Say("Thank you. The recording has been saved.");

      response.Redirect("/IVRMain/MainMenu");

      return response;
    }


    public TwilioResponse ListFavourites(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      response.Say("Listing favourites");

      var favourites = refUnitedAcctManager.GetFavourites(profileId);

      if (favourites.Count == 0)
      {
        response.Say("You have no favourites to send voice messages to.");
        response.Redirect("/IVRMain/MainMenu");
      }

      StringBuilder sb = new StringBuilder();

      favourites.ForEach(x =>
      {
        sb.Append(x.ProfileId);
        sb.Append(",");
      });

      string favs = sb.ToString().Substring(0, sb.Length - 1);

      response.BeginGather(new { numDigits = 1, action = string.Format("/IVRMain/SendFavMessage_RecordMsg?profileId={0}&favs={1}", profileId, favs) });

      //TODO: add paging support!

      int max = (favourites.Count <= 9) ? favourites.Count : 9;

      for (int i = 1; i <= max; i++)
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


    public TwilioResponse RecordMessageForFavourite(VoiceRequest request, int profileId, string favs)
    {
      var response = new TwilioResponse();

      var favsArray = favs.Split(',');

      if (request.Digits.Equals("#"))
      {
        //goto next page...
        //implement
        response.Say("This is no yet implemented. Sorry!");
        response.Redirect("/IVRMain/MainMenu");

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


    public TwilioResponse PlayRecordedVoiceMessageSelection(VoiceRequest request, int profileId, int recordingIdx, int fromProfileId)
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
