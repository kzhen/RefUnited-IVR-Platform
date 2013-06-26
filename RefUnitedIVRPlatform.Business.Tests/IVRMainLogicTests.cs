using System;
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
        var routeProvider = new Mock<IIVRRouteProvider>();

        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctMaanger, routeProvider.Object);

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
        var routeProvider = new Mock<IIVRRouteProvider>();

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

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
        var routeProvider = new Mock<IIVRRouteProvider>();

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

        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager.Object, routeProvider.Object);

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
        var routeProvider = new Mock<IIVRRouteProvider>();

        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager.Object, routeProvider.Object);

        var response = logic.ListFavourites(new Twilio.Mvc.VoiceRequest(), profileId, 0);

        Assert.IsNotNull(response);
        Assert.AreEqual(twilioSayNoFavourites, response.Element.FirstNode.ToString());
        Assert.AreEqual(twilioRedirect, response.Element.LastNode.ToString());
      }
    }

    [TestClass]
    public class PlayRecordedVoiceMessageTests
    {
      [TestMethod]
      public void Given_A_ProfileWithNoVoiceMessages_Should_ReturnNoVoiceMessagesPendingMessage()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);
        var routeProvider = new Mock<IIVRRouteProvider>();

        profileManager.Setup(m => m.GetRecordings(profileId)).Returns(new List<Common.Entities.Recording>());

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

        var result = logic.PlayRecordedVoiceMessage(new VoiceRequest(), profileId, null);

        Assert.AreEqual("<Response>\r\n  <Say>You have no voice messages</Say>\r\n  <Redirect>/IVRMain/MainMenu</Redirect>\r\n</Response>", result.ToString());
      }

      [TestMethod]
      public void Given_A_ProfileWithNoMoreMessagesToPlay_Should_ReturnMessageAndRedirectToMenu()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);
        var routeProvider = new Mock<IIVRRouteProvider>();

        profileManager.Setup(m => m.GetRecordings(profileId)).Returns(new List<Common.Entities.Recording>()
          {
            new Common.Entities.Recording(),
            new Common.Entities.Recording(),
            new Common.Entities.Recording(),
            new Common.Entities.Recording()
          });

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

        var result = logic.PlayRecordedVoiceMessage(new VoiceRequest(), profileId, 4);

        Assert.AreEqual("<Response>\r\n  <Say>No more messages.</Say>\r\n  <Redirect>/IVRMain/MainMenu</Redirect>\r\n</Response>", result.ToString());
      }

      [TestMethod]
      public void Given_A_ProfileWithOneVoiceMessage_Should_ReturnTheFirstVoiceMessage()
      
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);
        var routeProvider = new Mock<IIVRRouteProvider>();

        profileManager.Setup(m => m.GetRecordings(profileId)).Returns(new List<Common.Entities.Recording>()
          {
            new Common.Entities.Recording() { FromProfileId = 111, ToProfileId = profileId, Url = "url" }
          });

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

        var result = logic.PlayRecordedVoiceMessage(new VoiceRequest(), profileId, 0);

        Assert.AreEqual("<Response>\r\n  <Say>Playing message 1</Say>\r\n  <Play>url</Play>\r\n  <Gather numDigits=\"1\" action=\"/IVRMain/PlayRecordedMessage_Response?profileId=324784&amp;recordingIdx=0&amp;fromProfileId=111\">\r\n    <Say>Press one to repeat this message</Say>\r\n    <Say>Press two to delete this message</Say>\r\n    <Say>Press three to reply to this message</Say>\r\n    <Say>Press four to go to the next message</Say>\r\n  </Gather>\r\n</Response>"
, result.ToString());
      }
    }

    [TestClass]
    public class PlayRecordedMessage_ResponseTests
    {
      [TestMethod]
      public void Given_TheUser_Presses1_Should_RepeatTheMessage()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);
        var routeProvider = new Mock<IIVRRouteProvider>();

        var voiceRequest = new VoiceRequest() { Digits = "1" };

        profileManager.Setup(m => m.GetRecordings(profileId)).Returns(new List<Common.Entities.Recording>()
          {
            new Common.Entities.Recording() { FromProfileId = 111, ToProfileId = profileId, Url = "url" }
          });

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

        //var result = logic.PlayRecordedVoiceMessage(new VoiceRequest(), profileId, 0);
        var result = logic.PlayRecordedMessage_Response(voiceRequest, profileId, 0, 111);

        Assert.AreEqual("<Response>\r\n  <Redirect>/IVRMain/PlayRecordedMessage?profileId=324784&amp;recordingIdx=0</Redirect>\r\n</Response>"
, result.ToString());
      }

      [TestMethod]
      public void Given_TheUser_Presses2_Should_DeleteRecordingAndPlayNextMessage()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);
        var routeProvider = new Mock<IIVRRouteProvider>();

        var voiceRequest = new VoiceRequest() { Digits = "2" };

        profileManager.Setup(m => m.GetRecordings(profileId)).Returns(new List<Common.Entities.Recording>()
          {
            new Common.Entities.Recording() { FromProfileId = 111, ToProfileId = profileId, Url = "url" }
          });

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

        var result = logic.PlayRecordedMessage_Response(voiceRequest, profileId, 0, 111);

        Assert.AreEqual("<Response>\r\n  <Redirect>/IVRMain/PlayRecordedMessage?profileId=324784&amp;recordingIdx=0</Redirect>\r\n</Response>"
, result.ToString());
        profileManager.Verify(m=>m.DeleteRecording(profileId, 0));
      }

      [TestMethod]
      public void Given_TheUser_Presses3_Should_RecordResponse()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);
        var routeProvider = new Mock<IIVRRouteProvider>();

        var voiceRequest = new VoiceRequest() { Digits = "3" };

        profileManager.Setup(m => m.GetRecordings(profileId)).Returns(new List<Common.Entities.Recording>()
          {
            new Common.Entities.Recording() { FromProfileId = 111, ToProfileId = profileId, Url = "url" }
          });

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

        var result = logic.PlayRecordedMessage_Response(voiceRequest, profileId, 0, 111);

        Assert.AreEqual("<Response>\r\n  <Say>At the tone please record your response. Press any key when you are done.</Say>\r\n  <Record action=\"/IVRMain/PlayRecordedMessage_SaveResponse?profileId=324784&amp;recordingIdx=0&amp;fromProfileId=111\" />\r\n</Response>"
                ,result.ToString());
      }

      [TestMethod]
      public void Given_TheUser_Pressed4_Should_GoToTheNextMessage()
      {
        var profileId = 324784;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);
        var routeProvider = new Mock<IIVRRouteProvider>();

        var voiceRequest = new VoiceRequest() { Digits = "4" };

        profileManager.Setup(m => m.GetRecordings(profileId)).Returns(new List<Common.Entities.Recording>()
          {
            new Common.Entities.Recording() { FromProfileId = 111, ToProfileId = profileId, Url = "url" }
          });

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

        var result = logic.PlayRecordedMessage_Response(voiceRequest, profileId, 0, 111);

        Assert.AreEqual("<Response>\r\n  <Redirect>/IVRMain/PlayRecordedMessage?profileId=324784&amp;recordingIdx=1</Redirect>\r\n</Response>"
                , result.ToString());
      }
    }

    [TestClass]
    public class SaveVoiceMessageReplyTests
    {
      [TestMethod]
      public void Given_TheUser_HasRecordedAReplyMessage_Should_SaveTheReply()
      
      {
        var profileId = 324784;
        var fromProfileId = 111;
        var profileManager = new Mock<IProfileManager>();
        var apiRequest = new Mock<IApiRequest>();
        var refUnitedAcctManager = new RefugeesUnitedAccountManager(apiRequest.Object);
        var routeProvider = new Mock<IIVRRouteProvider>();

        var voiceRequest = new VoiceRequest() { Digits = "4" };

        profileManager.Setup(m => m.GetRecordings(profileId)).Returns(new List<Common.Entities.Recording>()
          {
            new Common.Entities.Recording() { FromProfileId = fromProfileId, ToProfileId = profileId, Url = "url" }
          });

        //this is what we are testing!
        var logic = new IVRMainLogic(profileManager.Object, refUnitedAcctManager, routeProvider.Object);

        var result = logic.SaveVoiceMessageReply(voiceRequest, profileId, 0, fromProfileId);

        Assert.AreEqual("<Response>\r\n  <Say>Your message has been saved.</Say>\r\n  <Redirect>/IVRMain/PlayRecordedMessage?profileId=324784&amp;recordingIdx=1</Redirect>\r\n</Response>"
                        , result.ToString());
      }
    }
  }
}
