using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RefUnitedIVRPlatform.Data.Repositories;
using RefUnitedIVRPlatform.Common.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RefUnitedIVRPlatform.Data.DtoEntities;

namespace RefUnitedIVRPlatform.Data.Tests
{
  [TestClass]
  public class ProfileRepositoryAzureIntegrationTests
  {
    public const string DEVELOPMENT_CONNECTION_STRING = "DEVELOPMENT_CONNECTION";

    [TestClass]
    public class CreateTests
    {
      [TestInitialize]
      public void Setup()
      {
        var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        var tableClient = storageAccount.CreateCloudTableClient();
        var table1 = tableClient.GetTableReference("ivrProfile");
        table1.DeleteIfExists();
        table1.Create();
      }

      [TestMethod]
      public void ShouldCreateAProfile()
      {
        IVRProfile profileToCreate = new IVRProfile()
        {
          ProfileId = 1000001,
          FullName = "basil",
          PIN = "1234"
        };

        ProfileRepositoryAzure repository = new ProfileRepositoryAzure(DEVELOPMENT_CONNECTION_STRING);

        var result = repository.Create(profileToCreate);

        Assert.IsNotNull(result);
      }
    }

    [TestClass]
    public class GetTests
    {
      [TestInitialize]
      public void Setup()
      {
        var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        var tableClient = storageAccount.CreateCloudTableClient();
        var table1 = tableClient.GetTableReference("ivrProfile");
        table1.DeleteIfExists();
        table1.Create();

        var entity = new IVRProfileEntity()
        {
          ProfileId = 123456,
          FullName = "Test User1",
          PhoneNumber = "+1111111",
          RowKey = "123456",
          PartitionKey = (123456).ToString().Substring(0, 2)
        };

        var op = TableOperation.Insert(entity);
        table1.Execute(op);
      }

      [TestMethod]
      public void ShouldReturnUserProfile()
      {
        int userId = 123456;

        ProfileRepositoryAzure repository = new ProfileRepositoryAzure(DEVELOPMENT_CONNECTION_STRING);

        var user = repository.Get(userId);

        Assert.IsNotNull(user);
      }
    }

    [TestClass]
    public class DeleteTests
    {
      [TestInitialize]
      public void Setup()
      {
        var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        var tableClient = storageAccount.CreateCloudTableClient();
        var table1 = tableClient.GetTableReference("ivrProfile");
        table1.DeleteIfExists();
        table1.Create();

        var entity = new IVRProfileEntity()
        {
          ProfileId = 123456,
          FullName = "Test User1",
          PhoneNumber = "+1111111",
          RowKey = "123456",
          PartitionKey = (123456).ToString().Substring(0, 2)
        };

        var op = TableOperation.Insert(entity);
        table1.Execute(op);
      }

      [TestMethod]
      public void ShouldDeleteUserProfile()
      {
        int profileId = 123456;

        ProfileRepositoryAzure repository = new ProfileRepositoryAzure(DEVELOPMENT_CONNECTION_STRING);

        var result = repository.Delete(new IVRProfile() { ProfileId = profileId });

        Assert.AreEqual(true, result);
      }

      [TestMethod]
      public void ShouldReturnFalseWhenUserProfileCannotBeDeleted()
      {
        int profileId = 5555555;

        ProfileRepositoryAzure repository = new ProfileRepositoryAzure(DEVELOPMENT_CONNECTION_STRING);

        var result = repository.Delete(new IVRProfile() { ProfileId = profileId });

        Assert.AreEqual(false, result);
      }
    }

    [TestClass]
    public class GetByPhoneNumberTests
    {
      [TestInitialize]
      public void Setup()
      {
        var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        var tableClient = storageAccount.CreateCloudTableClient();
        var table1 = tableClient.GetTableReference("ivrProfile");
        table1.DeleteIfExists();
        table1.Create();

        var entity = new IVRProfileEntity()
        {
          ProfileId = 123456,
          FullName = "Test User1",
          PhoneNumber = "+1111111",
          RowKey = "123456",
          PartitionKey = (123456).ToString().Substring(0, 2)
        };

        var op = TableOperation.Insert(entity);
        table1.Execute(op);
      }

      [TestMethod]
      public void ShouldReturnAUserProfileByLookingUpPhoneNumber()
      {
        var phoneNumberToLookup = "+1111111";

        ProfileRepositoryAzure repository = new ProfileRepositoryAzure(DEVELOPMENT_CONNECTION_STRING);

        var userPorfile = repository.GetByPhoneNumber(phoneNumberToLookup);

        Assert.IsNotNull(userPorfile);
        Assert.AreEqual(phoneNumberToLookup, userPorfile.PhoneNumber);
      }
    }
  }
}
