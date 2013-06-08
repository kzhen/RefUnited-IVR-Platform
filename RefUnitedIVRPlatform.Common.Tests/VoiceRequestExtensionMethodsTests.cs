using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twilio.Mvc;

namespace RefUnitedIVRPlatform.Common.Tests
{
  [TestClass]
  public class VoiceRequestExtensionMethodsTests
  {
    [TestClass]
    public class GetOriginatingNumberTests
    {
      [TestMethod]
      public void ShouldReturnOriginatingNumberForInboundCall()
      {
        VoiceRequest request = new VoiceRequest()
        {
          Direction = "inbound",
          From = "+1234567890"
        };

        var originatingNumber = request.GetOriginatingNumber();

        Assert.AreEqual("+1234567890", originatingNumber);
      }

      [TestMethod]
      public void ShouldReturnOriginatingNumberForOutboundCall()
      {
        VoiceRequest request = new VoiceRequest()
        {
          Direction = "outbound-api",
          To = "+1234567890"
        };

        var originatingNumber = request.GetOriginatingNumber();

        Assert.AreEqual("+1234567890", originatingNumber);
      }
    }
  }
}
