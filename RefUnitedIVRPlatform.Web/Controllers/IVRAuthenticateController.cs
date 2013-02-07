using RefUnitedIVRPlatform.Common.Interfaces;
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
    public class IVRAuthenticateController : ApiController
    {
      private IIVRAuthenticateLogic ivrAuthLogic;

      public IVRAuthenticateController(IIVRAuthenticateLogic authLogic)
      {
        this.ivrAuthLogic = authLogic;
      }

      public HttpResponseMessage Post(VoiceRequest request, string language)
      {
        var response = ivrAuthLogic.GetAuthentication(request, language);

        return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
      }
    }
}
