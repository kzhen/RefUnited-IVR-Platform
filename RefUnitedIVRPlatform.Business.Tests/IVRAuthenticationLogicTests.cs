using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RefUnitedIVRPlatform.Business.IVRLogic;
using RefUnitedIVRPlatform.Common.Interfaces;
using Twilio.Mvc;

namespace RefUnitedIVRPlatform.Business.Tests
{
  [TestClass]
  public class IVRAuthenticationLogicTests
  {
    [TestClass]
    public class GetAuthenticationTests
    {
      [TestMethod]
      public void Given_A_CorrectPINEntered_Should_RedirectToMainMenu()
      {
        var fromNumber = "+44123456789";
        var pinNumber = "1234";
        var profileManager = new Mock<IProfileManager>();
        var routeProvider = new Mock<IIVRRouteProvider>();

        routeProvider.Setup(m => m.GetUrlMethod(RefUnitedIVRPlatform.Common.IVRRoutes.PLAY_MAIN_MENU, "en")).Returns("/IVRMain/MainMenu");

        profileManager.Setup(m => m.CheckPin(fromNumber, pinNumber)).Returns(true);

        var ivrAuthenticationLogic = new IVRAuthenticateLogic(profileManager.Object, routeProvider.Object);

        var request = new VoiceRequest();
        request.Digits = pinNumber;
        request.Direction = "inbound";
        request.From = fromNumber;

        var response = ivrAuthenticationLogic.GetAuthentication(request, "en");

        Assert.IsNotNull(response);
        Assert.AreEqual(@"<Response>
  <Redirect method=""POST"">/IVRMain/MainMenu</Redirect>
</Response>", response.Element.ToString());
      }

      [TestMethod]
      public void Given_An_IncorrectPINEntered_Should_Hangup()
      {
        var fromNumber = "+44123456789";
        var pinNumber = "1234";
        var profileManager = new Mock<IProfileManager>();
        var routeProvider = new Mock<IIVRRouteProvider>();

        routeProvider.Setup(m => m.GetUrlMethod(RefUnitedIVRPlatform.Common.IVRRoutes.PLAY_MAIN_MENU, "en")).Returns("/IVRMain/MainMenu");

        profileManager.Setup(m => m.CheckPin(fromNumber, pinNumber)).Returns(false);

        var ivrAuthenticationLogic = new IVRAuthenticateLogic(profileManager.Object, routeProvider.Object);

        var request = new VoiceRequest();
        request.Digits = pinNumber;
        request.Direction = "inbound";
        request.From = fromNumber;

        var response = ivrAuthenticationLogic.GetAuthentication(request, "en");

        Assert.IsNotNull(response);
        Assert.AreEqual(@"<Response>
  <Say language=""en"">Your PIN was incorrect, sad face</Say>
  <Hangup />
</Response>", response.Element.ToString());
      }
    }
  }
}
