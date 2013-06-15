using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class IVRBaseController : Controller
  {
    public ActionResult IVRResult(TwilioResponse response)
    {
      Response.ContentType = "text/xml";
      return Content(response.Element.ToString());
    }
  }
}
