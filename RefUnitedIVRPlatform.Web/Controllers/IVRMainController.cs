using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class IVRMainController : Controller
  {
    private IProfileManager profileManager;
    private IRefugeesUnitedAccountManager refUnitedAcctManager;

    public IVRMainController(IProfileManager profileManager, IRefugeesUnitedAccountManager refUnitedAcctManager)
    {
      this.profileManager = profileManager;
      this.refUnitedAcctManager = refUnitedAcctManager;
    }

    [HttpPost]
    public ActionResult MainMenu(VoiceRequest request)
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

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult MainMenuSelection(VoiceRequest request)
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
          case 3:
            response.Redirect(string.Format("/IVRMain/SendFavMessage_ListFavs?profileId={0}", profileId));
            break;
          case 2:
            response.Say("Looking up old messages");
            break;
          case 4:
            response.Redirect(string.Format("/IVRMain/PlayRecordedMessages?profileId={0}", profileId));
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

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult SaveRecording(VoiceRequest request)
    {
      var url = request.RecordingUrl;

      profileManager.SaveRecording(url);

      var response = new TwilioResponse();

      response.Say("Thank you. The recording has been saved.");

      response.Redirect("/IVRMain/MainMenu");

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult SendFavMessage_ListFavs(VoiceRequest request, int profileId)
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
          sb.Append(x.Id);
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

      if (favourites.Count > 9)
      {
        response.Say("To go to the next page press hash");
      }

      response.EndGather();

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult SendFavMessage_RecordMsg(VoiceRequest request, int profileId, string favs)
    {
      var response = new TwilioResponse();

      var favsArray = favs.Split(',');

      var selection = int.Parse(request.Digits);

      string targetProfileId = favsArray[selection - 1];

      response.Say("Record your message after the tone, press any key when you are done.", new { voice = "woman" });
      response.Record(new { action = string.Format("/IVRMain/SendFavMessage_SaveRecording?profileId={0}&targetProfileId={1}", profileId, targetProfileId) });

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult SendFavMessage_SaveRecording(VoiceRequest request, int profileId, int targetProfileId)
    {
      var url = request.RecordingUrl;

      profileManager.SaveRecording(profileId, targetProfileId, url);

      var response = new TwilioResponse();

      response.Say("Thank you. Your message has been saved.");

      response.Redirect("/IVRMain/MainMenu");

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult PlayRecordedMessages(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      var voiceMessages = profileManager.GetRecordings(profileId);

      if (voiceMessages == null || voiceMessages.Count == 0)
      {
        response.Say("You have no voice messages");
        response.Redirect("/IVRMain/MainMenu");
        Response.ContentType = "text/xml";
        return Content(response.Element.ToString());
      }

      //response.Say("Playing recorded voice messages");
      response.Say(string.Format("You have {0} voice message{1}", voiceMessages.Count, (voiceMessages.Count == 1) ? "" : "s"));

      foreach (var msg in voiceMessages)
      {
        response.Play(msg.Url);
      }

      response.Say("Finished playing voice messages");
      response.Redirect("/IVRMain/MainMenu");

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }
  }
}
