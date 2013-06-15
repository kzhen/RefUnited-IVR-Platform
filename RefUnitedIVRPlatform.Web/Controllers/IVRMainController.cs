using RefUnitedIVRPlatform.Common.Entities;
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
using RefUnitedIVRPlatform.Common.Attributes;
using RefUnitedIVRPlatform.Common;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class IVRMainController : Controller
  {
    private IProfileManager profileManager;
    private IRefugeesUnitedAccountManager refUnitedAcctManager;
    private IIVRMainLogic ivrMainLogic;

    public IVRMainController(IProfileManager profileManager, IRefugeesUnitedAccountManager refUnitedAcctManager, IIVRMainLogic ivrMainLogic)
    {
      this.profileManager = profileManager;
      this.refUnitedAcctManager = refUnitedAcctManager;
      this.ivrMainLogic = ivrMainLogic;
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.PLAY_MAIN_MENU)]
    public ActionResult MainMenu(VoiceRequest request)
    {
      var response = ivrMainLogic.GetMainMenu();

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.MAIN_MENU_SELECTION)]    
    public ActionResult MainMenuSelection(VoiceRequest request)
    {
      var response = ivrMainLogic.GetMenuSelection(request);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult SendFavMessage_ListFavs(VoiceRequest request, int profileId, int? pageIdx)
    {
      var response = ivrMainLogic.ListFavourites(request, profileId, pageIdx);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult SendFavMessage_RecordMsg(VoiceRequest request, int profileId, string favs, int pageIdx)
    {
      var response = ivrMainLogic.RecordMessageForFavourite(request, profileId, favs, pageIdx);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult SendFavMessage_SaveRecording(VoiceRequest request, int profileId, int targetProfileId)
    {
      var response = ivrMainLogic.SaveRecordingForFavourite(request, profileId, targetProfileId);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult PlayRecordedMessage(VoiceRequest request, int profileId, int? recordingIdx)
    {
      var response = ivrMainLogic.PlayRecordedVoiceMessage(request, profileId, recordingIdx);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult PlayRecordedMessage_Response(VoiceRequest request, int profileId, int recordingIdx, int fromProfileId)
    {
      var response = ivrMainLogic.PlayRecordedMessage_Response(request, profileId, recordingIdx, fromProfileId);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult PlayRecordedMessage_SaveResponse(VoiceRequest request, int profileId, int recordingIdx, int fromProfileId)
    {
      var response = ivrMainLogic.SaveVoiceMessageReply(request, profileId, recordingIdx, fromProfileId);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult ReadPlatformMessages(VoiceRequest request, int profileId)
    {
      var response = ivrMainLogic.PlayPlatformMessages(request, profileId);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }
  }
}
