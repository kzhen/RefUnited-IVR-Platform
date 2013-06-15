using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RefUnitedIVRPlatform.Common.Interfaces;
using Twilio.Mvc;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class IVRBroadcastController : Controller
  {
    IIVRBroadcastLogic broadcastLogic;

    public IVRBroadcastController(IIVRBroadcastLogic broadcastLogic)
    {
      this.broadcastLogic = broadcastLogic;
    }

    [HttpPost]
    public ActionResult RecordBroadcast(VoiceRequest request, int profileId)
    {
      var response = broadcastLogic.RecordBroadcast(request, profileId);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult RecordBroadcast_SaveRecording(VoiceRequest request, int profileId)
    {
      var response = broadcastLogic.RecordBroadcast_SaveRecording(request, profileId);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }

    [HttpPost]
    public ActionResult ListenToBroadcasts(VoiceRequest request, int profileId)
    {
      var response = broadcastLogic.ListenToBroadcasts(request, profileId);

      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }
  }
}
