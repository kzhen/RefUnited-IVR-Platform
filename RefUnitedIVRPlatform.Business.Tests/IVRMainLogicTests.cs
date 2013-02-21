using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Business.IVRLogic;
using Moq;
using System.Collections.Generic;

namespace RefUnitedIVRPlatform.Business.Tests
{
  [TestClass]
  public class IVRMainLogicTests
  {
    [TestClass]
    public class ListFavouritesTests
    {
      private static string twilioSayNoFavourites = "<Say>You have no favourites to send voice messages to.</Say>";
      private static string twilioRedirect = "<Redirect>/IVRMain/MainMenu</Redirect>";
      private static string twilioSayListingFavourites = "<Say>Listing favourites</Say>";
      private static string twilioGatherFavouriteListResponse = "<Gather numDigits=\"1\" action=\"/IVRMain/SendFavMessage_RecordMsg?profileId=324784&amp;favs=0,1,2,3,4\">\r\n  <Say>To send a message to firstname0 surname0 press 1</Say>\r\n  <Say>To send a message to firstname1 surname1 press 2</Say>\r\n  <Say>To send a message to firstname2 surname2 press 3</Say>\r\n  <Say>To send a message to firstname3 surname3 press 4</Say>\r\n  <Say>To send a message to firstname4 surname4 press 5</Say>\r\n  <Say>Press star to return to main menu</Say>\r\n</Gather>";

      [TestMethod]
      public void ShouldReturnTwilioResponseWithThreeFavourites()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var refUnitedAcctManager = new Mock<IRefugeesUnitedAccountManager>();

        var favs = new List<RefugeesUnitedApi.ApiEntities.Profile>();

        for (int i = 0; i < 5; i++)
        {
          favs.Add(new RefugeesUnitedApi.ApiEntities.Profile() { ProfileId = i, FirstName = "firstname" + i, Surname = "surname" + i });
        }

        refUnitedAcctManager.Setup(m => m.GetFavourites(profileId, 0)).Returns(favs);

        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager.Object);

        var response = logic.ListFavourites(new Twilio.Mvc.VoiceRequest(), profileId, 0);

        Assert.IsNotNull(response);
        Assert.AreEqual(twilioSayListingFavourites, response.Element.FirstNode.ToString());
        Assert.AreEqual(twilioGatherFavouriteListResponse, response.Element.LastNode.ToString());
      }

      [TestMethod]
      public void ShouldReturnTwilioResponseWithNoFavourites()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var refUnitedAcctManager = new Mock<IRefugeesUnitedAccountManager>();

        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager.Object);

        var response = logic.ListFavourites(new Twilio.Mvc.VoiceRequest(), profileId, 0);

        Assert.IsNotNull(response);
        Assert.AreEqual(twilioSayNoFavourites, response.Element.FirstNode.ToString());
        Assert.AreEqual(twilioRedirect, response.Element.LastNode.ToString());
      }
    }
  }
}
