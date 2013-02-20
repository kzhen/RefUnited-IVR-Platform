using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Mvc;

namespace RefUnitedIVRPlatform.Common
{
  public static class VoiceRequestExtensionMethods
  {
    public static string GetOriginatingNumber(this VoiceRequest request)
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

      return lookupPhoneNumber;
    }
  }
}
