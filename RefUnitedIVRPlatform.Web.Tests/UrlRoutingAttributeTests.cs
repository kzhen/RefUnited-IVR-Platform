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
    internal class TestController : Controller
    {
      [IVRUrlRoute("ReturnSomeAction")]
      [HttpPost]
      public ActionResult IVRMethod_ReturnSomeAction()
      {
        return View();
      }

      [HttpPost]
      [IVRUrlRoute("ReturnSomeActionWithParam")]
      public ActionResult MyActionWithParam(int profileId)
      {
        return View();
      }

      [HttpPost]
      [IVRUrlRoute("ReturnSomeActionWithParam2")]
      public ActionResult MyActionWithParam2(VoiceRequest request, int profileId)
      {
        return View();
      }


    }

    [TestMethod]
    public void ShouldFindTheCorrectRoute()
    {
      string expectedUrl = "/Test/IVRMethod_ReturnSomeAction";

      IIVRRouteProvider routeProvider = new IVRRouteProvider();

      string actualUrl = routeProvider.GetUrlMethod("ReturnSomeAction");

      Assert.AreEqual(expectedUrl, actualUrl);
    }

    [TestMethod]
    public void ShouldGenerateTheCorrectLinkWithParam()
    {
      int profileId = 123;
      string expectedUrl = "/Test/MyActionWithParam?profileId=123";

      IIVRRouteProvider routeProvider = new IVRRouteProvider();

      string actualUrl = routeProvider.GetUrlMethod("ReturnSomeActionWithParam", profileId.ToString());

      Assert.AreEqual(expectedUrl, actualUrl);
    }

    [TestMethod]
    public void ShouldGenerateTheCorrectLinkWithParam2()
    {
      int profileId = 123;
      string expectedUrl = "/Test/MyActionWithParam2?profileId=123";

      IIVRRouteProvider routeProvider = new IVRRouteProvider();

      string actualUrl = routeProvider.GetUrlMethod("ReturnSomeActionWithParam2", profileId.ToString());

      Assert.AreEqual(expectedUrl, actualUrl);
    }
  }
}
