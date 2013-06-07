using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Business.Tests
{
  [TestClass]
  public class TwiMLHelperTests
  {
    [TestClass]
    public class SayOrPlayTests
    {
      [TestMethod]
      public void ShouldUsePlayVerbIfImplementedAsMP3()
      {
        TwilioResponse mockResponse = new TwilioResponse();

        TwiMLHelper helper = new TwiMLHelper("ar", true);

        helper.SayOrPlay(mockResponse, "test message");

        Assert.IsTrue(mockResponse.Element.ToString().Contains("<Play>"));
      }

      [TestMethod]
      public void ShouldUseSayVerbIfNotImplementedAsMP3()
      {
        TwilioResponse mockResponse = new TwilioResponse();

        TwiMLHelper helper = new TwiMLHelper("en", false);

        helper.SayOrPlay(mockResponse, "test message");

        Assert.IsTrue(mockResponse.Element.ToString().Contains("<Say language=\"en\">"));
      }
    }
  }
}
