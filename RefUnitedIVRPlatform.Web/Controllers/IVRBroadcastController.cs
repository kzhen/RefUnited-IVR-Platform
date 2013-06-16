using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RefUnitedIVRPlatform.Common;
using RefUnitedIVRPlatform.Common.Attributes;
using RefUnitedIVRPlatform.Common.Interfaces;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class IVRBroadcastController : IVRBaseController
  {
    IIVRBroadcastLogic broadcastLogic;

    public IVRBroadcastController(IIVRBroadcastLogic broadcastLogic)
    {
      this.broadcastLogic = broadcastLogic;
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCAST_RECORD)]
    public ActionResult RecordBroadcast(VoiceRequest request, int profileId)
    {
      var response = broadcastLogic.RecordBroadcast(request, profileId);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCAST_SAVE_PUBLIC_BROADCAST)]
    public ActionResult RecordBroadcast_SaveRecording(VoiceRequest request, int profileId)
    {
      var response = broadcastLogic.RecordBroadcast_SaveRecording(request, profileId);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCAST_MENU)]
    public ActionResult ListenToBroadcasts(VoiceRequest request, int profileId)
    {
      var response = broadcastLogic.ListenToBroadcastsMenu(request, profileId);
      
      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCASTS_LISTEN_TO_ALL_PUBLIC)]
    public ActionResult ListenToPublicBroadcasts(VoiceRequest request, int profileId, int idx)
    {
      var response = broadcastLogic.ListenToPublicBroadcasts(request, profileId, idx);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCAST_RESPONSE_SELECTION)]
    public ActionResult BroadcastResponseSelection(VoiceRequest request, int profileId, int lastBroadcastIdx)
    {
      var response = broadcastLogic.BroadcastResponseSelection(request, profileId, lastBroadcastIdx);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCASTS_REPLY_PRIVATELY)]
    public ActionResult Broadcast_PrivateReply(VoiceRequest request, int profileId, int lastBroadcastIdx, int? subBroadcastIdx)
    {
      var response = broadcastLogic.RecordPrivateReply(request, profileId, lastBroadcastIdx, subBroadcastIdx);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCAST_SAVE_PRIVATE_REPLY)]
    public ActionResult SavePrivateReply(VoiceRequest request, int profileId, int lastBroadcastIdx, int? subBroadcastIdx)
    {
      var response = broadcastLogic.SavePrivateReply(request, profileId, lastBroadcastIdx, subBroadcastIdx);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCASTS_REPLY_PUBLICLY)]
    public ActionResult RecordPublicReply(VoiceRequest request, int profileId, int lastBroadcastIdx, int? subBroadcastIdx)
    {
      var response = broadcastLogic.RecordPublicReply(request, profileId, lastBroadcastIdx, subBroadcastIdx);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCAST_SAVE_PUBLIC_REPLY)]
    public ActionResult SavePublicReply(VoiceRequest request, int profileId, int lastBroadcastIdx, int? subBroadcastIdx)
    {
      var response = broadcastLogic.SavePublicReply(request, profileId, lastBroadcastIdx, subBroadcastIdx);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCAST_LISTEN_TO_MATCHED)]
    public ActionResult ListenToMatchedBroadcasts(VoiceRequest request, int profileId)
    {
      var response = broadcastLogic.ListenToMatchedBroadcasts(request, profileId);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCASTS_PLAY_PUBLIC_REPLY)]
    public ActionResult ListenToBroadcastReplies(VoiceRequest request, int profileId, int lastBroadcastIdx, int subBroadcastIdx)
    {
      var response = broadcastLogic.ListenToBroadcastReplies(request, profileId, lastBroadcastIdx, subBroadcastIdx);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCAST_REPLY_RESPONSE_SELECTION)]
    public ActionResult BroadcastReplyMenuSelection(VoiceRequest request, int profileId, int lastBroadcastIdx, int subBroadcastIdx)
    {
      var response = broadcastLogic.BroadcastReplyMenuSelection(request, profileId, lastBroadcastIdx, subBroadcastIdx);

      return IVRResult(response);
    }

    [HttpPost]
    [IVRUrlRoute(IVRRoutes.BROADCASTS_ADD_REPLIER_AS_FAVOURITE)]
    public ActionResult AddResponderAsFavourite(VoiceRequest request, int profileId, int lastBroadcastIdx, int subBroadcastIdx)
    {
      TwilioResponse response = broadcastLogic.AddResponderAsFavourite(request, profileId, lastBroadcastIdx, subBroadcastIdx);

      return IVRResult(response);
    }
  }
}
