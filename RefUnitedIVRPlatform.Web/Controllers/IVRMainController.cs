using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
      response.Say("Press three to record a test message.");
      response.Say("Press four to listen to recorded messages.");
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
          case 2:
            response.Say("Looking up old messages");
            break;
          case 3:
            response.Say("Time to record something, press any key when you are done.", new { voice = "woman" });
            response.Record(new { action = "/IVRMain/SaveRecording" });
            break;
          case 4:
            var recordings = profileManager.GetRecordings();

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
  }
}
