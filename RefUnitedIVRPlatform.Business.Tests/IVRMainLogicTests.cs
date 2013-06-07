﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Business.IVRLogic;
using Moq;
using System.Collections.Generic;
using System.Text;
using RefUnitedIVRPlatform.Business.Managers;
using RefugeesUnitedApi;
using Twilio.Mvc;

namespace RefUnitedIVRPlatform.Business.Tests
{
  [TestClass]
  public class IVRMainLogicTests
  {
    [TestClass]
    public class RecordMessageForFavouriteTests
    {
      private static string twilioRedirectToNextPage = "<Redirect>/IVRMain/SendFavMessage_ListFavs?profileId={0}&amp;pageIdx={1}</Redirect>";

      [TestMethod]
      public void ShouldReturnTwilioResponseRedirectingToNextPage()
      {
        //Arrange
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctMaanger = new RefugeesUnitedAccountManager(apiRequest.Object);

        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctMaanger);

        string favs = string.Empty;
        int pageIdx = 0;

        var voiceRequest = new VoiceRequest() { Digits = "#" };

        //Act
        var response = logic.RecordMessageForFavourite(voiceRequest, profileId, favs, pageIdx);

        //Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(string.Format(twilioRedirectToNextPage, profileId, pageIdx + 1), response.Element.FirstNode.ToString());
      }
    }

    [TestClass]
    public class ListFavouritesTests
    {
      private static string twilioSayNoFavourites = "<Say>You have no favourites to send voice messages to.</Say>";
      private static string twilioRedirect = "<Redirect>/IVRMain/MainMenu</Redirect>";
      private static string twilioSayListingFavourites = "<Say>Listing favourites</Say>";
      private static string twilioGatherFavouriteListResponse = "<Gather numDigits=\"1\" action=\"/IVRMain/SendFavMessage_RecordMsg?profileId={0}&amp;favs={1}&amp;pageIdx={2}\">\r\n{3}  <Say>Press star to return to main menu</Say>\r\n</Gather>";

      [TestMethod]
      public void ShouldReturnTwilioResponseWithThreeFavouritesFromPage2WhenTwelveFavourites()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager);

        var favs = new List<RefugeesUnitedApi.ApiEntities.Profile>();

        StringBuilder favsSb = new StringBuilder();
        StringBuilder favsSaySb = new StringBuilder();

        int diff = -9;

        for (int i = 0; i < 12; i++)
        {
          favs.Add(new RefugeesUnitedApi.ApiEntities.Profile() { ProfileId = i, FirstName = "firstname" + i, Surname = "surname" + i });

          if ((i % 9) == 0)
          {
            favsSaySb = new StringBuilder();
            favsSb = new StringBuilder();
            diff += 9;
          }

          favsSb.Append(i);
          favsSb.Append(",");
          favsSaySb.Append(string.Format("  <Say>To send a message to firstname{0} surname{0} press {1}</Say>\r\n", i, (i + 1) - diff));
        }

        string favsId = favsSb.ToString().Substring(0, favsSb.Length - 1);

        apiRequest.Setup(m => m.GetFavourites(profileId)).Returns(favs);

        

        var response = logic.ListFavourites(new Twilio.Mvc.VoiceRequest(), profileId, 1);

        Assert.IsNotNull(response);
        Assert.AreEqual(twilioSayListingFavourites, response.Element.FirstNode.ToString());
        Assert.AreEqual(string.Format(twilioGatherFavouriteListResponse, profileId, favsId, 1, favsSaySb.ToString()), response.Element.LastNode.ToString());
      }

      [TestMethod]
      public void ShouldReturnTwilioResponseWithNineFavouritesWhenOnlyNineFavourites()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var refUnitedAcctManager = new Mock<IRefugeesUnitedAccountManager>();

        var favs = new List<RefugeesUnitedApi.ApiEntities.Profile>();

        StringBuilder favsSb = new StringBuilder();
        StringBuilder favsSaySb = new StringBuilder();

        for (int i = 0; i < 9; i++)
        {
          favs.Add(new RefugeesUnitedApi.ApiEntities.Profile() { ProfileId = i, FirstName = "firstname" + i, Surname = "surname" + i });
          favsSb.Append(i);
          favsSb.Append(",");

          favsSaySb.Append(string.Format("  <Say>To send a message to firstname{0} surname{0} press {1}</Say>\r\n", i, i + 1));
        }

        string favsId = favsSb.ToString().Substring(0, favsSb.Length - 1);

        refUnitedAcctManager.Setup(m => m.GetFavourites(profileId, 0)).Returns(favs);

        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager.Object);

        var response = logic.ListFavourites(new Twilio.Mvc.VoiceRequest(), profileId, 0);

        Assert.IsNotNull(response);
        Assert.AreEqual(twilioSayListingFavourites, response.Element.FirstNode.ToString());
        Assert.AreEqual(string.Format(twilioGatherFavouriteListResponse, profileId, favsId, 0, favsSaySb.ToString()), response.Element.LastNode.ToString());
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