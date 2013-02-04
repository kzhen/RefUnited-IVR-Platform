using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Data.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class IVREntryController : ApiController
  {
    private IIVREntryLogic ivrEntryLogic;

    public IVREntryController(IIVREntryLogic ivrEntryLogic)
    {
      this.ivrEntryLogic = ivrEntryLogic;
    }

    public HttpResponseMessage Post(VoiceRequest request)
    {
      var response = ivrEntryLogic.GetGreeting(request);

      return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
    }
  }
}
