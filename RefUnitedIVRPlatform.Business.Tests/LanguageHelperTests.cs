using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RefUnitedIVRPlatform.Business.Tests
{
  [TestClass]
  public class LanguageHelperTests
  {
    [TestClass]
    public class GetValidCultureTests
    {
      [TestMethod]
      public void ShouldReturnDefaultCulture()
      {
        var expectedResult = LanguageHelper.GetDefaultCulture();

        var culture = LanguageHelper.GetValidCulture(null);

        Assert.AreEqual(expectedResult, culture);
      }

      [TestMethod]
      public void ShouldReturnAKnownCulture()
      {
        var expected = "en";

        var culture = LanguageHelper.GetValidCulture("en");

        Assert.AreEqual(expected, culture);
      }

      [TestMethod]
      public void ShouldReturnAClosestMatch()
      {
        var expected = "en";

        var culture = LanguageHelper.GetValidCulture("en-US");

        Assert.AreEqual(expected, culture);
      }

      [TestMethod]
      public void ShouldReturnDefaultCultureIfNoMatch()
      {
        var expected = "en";

        var culture = LanguageHelper.GetValidCulture("fr");

        Assert.AreEqual(expected, culture);
      }
    }

    [TestClass]
    public class IsImplementedAsMP3Tests
    {
      [TestMethod]
      public void ShouldReturnTrueIfItIsImplementedAsMP3()
      {
        var expected = true;

        var isImplementedAsMP3 = LanguageHelper.IsImplementedAsMP3("ar");

        Assert.AreEqual(expected, isImplementedAsMP3);
      }

      [TestMethod]
      public void ShouldReturnFalseIfItIsntImplementedAsMP3()
      {
        var expected = false;

        var isImplementedAsMP3 = LanguageHelper.IsImplementedAsMP3("en");

        Assert.AreEqual(expected, isImplementedAsMP3);
      }
    }
  }
}
