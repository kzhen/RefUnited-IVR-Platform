using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RefUnitedIVRPlatform.Business.Managers;
using RefugeesUnitedApi;
using Moq;
using System.Collections.Generic;
using RefUnitedIVRPlatform.Common.Entities;

namespace RefUnitedIVRPlatform.Business.Tests
{
  [TestClass]
  public class RefugeesUnitedAccountManagerTests
  {
    [TestClass]
    public class GetFavouritesTests
    {
      List<RefugeesUnitedApi.ApiEntities.Profile> favourites;

      [TestInitialize]
      public void Setup()
      {
         favourites = new List<RefugeesUnitedApi.ApiEntities.Profile>();

         for (int i = 0; i < 14; i++)
         {
           favourites.Add(new RefugeesUnitedApi.ApiEntities.Profile());
         }
      }

      [TestMethod]
      public void ShouldReturnNineFavouritesFirstPage()
      {
        int profileId = 324784;

        var apiRequest = new Mock<IApiRequest>();
        apiRequest.Setup(m => m.GetFavourites(profileId)).Returns(favourites);

        var manager = new RefugeesUnitedAccountManager(apiRequest.Object);

        var favs = manager.GetFavourites(profileId, 0);

        Assert.IsNotNull(favs);
        Assert.AreEqual(9, favs.Count);
      }

      [TestMethod]
      public void ShouldReturnFiveFavouritesFromSecondPage()
      {
        int profileId = 324784;

        var apiRequest = new Mock<IApiRequest>();
        apiRequest.Setup(m => m.GetFavourites(profileId)).Returns(favourites);

        var manager = new RefugeesUnitedAccountManager(apiRequest.Object);

        var favs = manager.GetFavourites(profileId, 1);

        Assert.IsNotNull(favs);
        Assert.AreEqual(5, favs.Count);
      }
    }
  }
}
