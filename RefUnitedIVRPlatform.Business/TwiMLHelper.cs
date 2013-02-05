using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Business
{
  internal class TwiMLHelper
  {
    private string culture;
    private bool isImplementedAsMP3;

    public TwiMLHelper(string culture, bool isImplementedAsMP3)
    {
      this.culture = culture;
      this.isImplementedAsMP3 = isImplementedAsMP3;
    }

    internal void SayOrPlay(TwilioResponse response, string message)
    {
      if (isImplementedAsMP3)
      {
        response.Play(message);
      }
      else
      {
        response.Say(message, new { language = culture });
      }
    }
  }
}
