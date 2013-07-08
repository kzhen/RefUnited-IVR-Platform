using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using RefUnitedIVRPlatform.Common.Attributes;
using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Common;
using Twilio.Mvc;

namespace RefUnitedIVRPlatform.Web.Tests
{
  [TestClass]
  public class UrlRoutingAttributeTests
  {
    internal class MyRoutes
    {
      public const string ACTION1 = "ReturnSomeAction";
      public const string ACTION2 = "ReturnSomeActionWithParam";
      public const string ACTION3 = "ReturnSomeActionWithParam2";
      public const string ACTION4 = "MyActionWithParam3";
      public string iShouldBeIgnored = "abc";
    }

    internal class MyRoutesWithMissing : MyRoutes
    {
      public const string ACTION5 = "ThisIsTheMissingAction";
    }

    internal class TestController : Controller
    {
      [IVRUrlRoute(MyRoutes.ACTION1)]
      [HttpPost]
      public ActionResult IVRMethod_ReturnSomeAction()
      {
        return View();
      }

      [HttpPost]
      [IVRUrlRoute(MyRoutes.ACTION2)]
      public ActionResult MyActionWithParam(int profileId)
      {
        return View();
      }

      [HttpPost]
      [IVRUrlRoute(MyRoutes.ACTION3)]
      public ActionResult MyActionWithParam2(VoiceRequest request, int profileId)
      {
        return View();
      }

      [HttpGet]
      [IVRUrlRoute(MyRoutes.ACTION4)]
      public ActionResult MyActionWithParam3(VoiceRequest request, int profileId, int nextParam)
      {
        return View();
      }
    }

    [TestMethod]
    public void ShouldFindTheCorrectRoute()
    {
      string expectedUrl = "/Test/IVRMethod_ReturnSomeAction";

      IIVRRouteProvider routeProvider = new IVRRouteProvider();

      string actualUrl = routeProvider.GetUrlMethod(MyRoutes.ACTION1);

      Assert.AreEqual(expectedUrl, actualUrl);
    }

    [TestMethod]
    public void ShouldGenerateTheCorrectLinkWithParam()
    {
      int profileId = 123;
      string expectedUrl = "/Test/MyActionWithParam?profileId=123";

      IIVRRouteProvider routeProvider = new IVRRouteProvider();

      string actualUrl = routeProvider.GetUrlMethod(MyRoutes.ACTION2, profileId.ToString());

      Assert.AreEqual(expectedUrl, actualUrl);
    }

    [TestMethod]
    public void ShouldGenerateTheCorrectLinkWithParam2()
    {
      int profileId = 123;
      string expectedUrl = "/Test/MyActionWithParam2?profileId=123";

      IIVRRouteProvider routeProvider = new IVRRouteProvider();

      string actualUrl = routeProvider.GetUrlMethod(MyRoutes.ACTION3, profileId.ToString());

      Assert.AreEqual(expectedUrl, actualUrl);
    }

    [TestMethod]
    public void ShouldGenerateTheCorrectLinkWithParam3()
    {
      int profileId = 123;
      int nextId = 321;

      string expectedUrl = "/Test/MyActionWithParam3?profileId=123&nextParam=321";

      IIVRRouteProvider routeProvider = new IVRRouteProvider();

      string actualUrl = routeProvider.GetUrlMethod(MyRoutes.ACTION4, profileId, nextId);

      Assert.AreEqual(expectedUrl, actualUrl);
    }

    [TestMethod]
    public void ShouldVerifyAllRoutesHaveMethods()
    {
      IIVRRouteProvider routeProvider = new IVRRouteProvider();
      routeProvider.VerifyAllRoutes(typeof(MyRoutes));
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Given_A_MissingRoute_Should_ThrowException()
    {
      IVRRouteProvider routeProvider = new IVRRouteProvider();
      routeProvider.VerifyAllRoutes(typeof(MyRoutesWithMissing));
    }
  }
}
