using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IIVRAuthenticateLogic
  {
    TwilioResponse GetAuthentication(VoiceRequest request, string language);
  }
}
