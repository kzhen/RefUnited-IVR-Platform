﻿using System;
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

        ProfileRepository repository = new ProfileRepository(DEVELOPMENT_CONNECTION_STRING);

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

        ProfileRepository repository = new ProfileRepository(DEVELOPMENT_CONNECTION_STRING);

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

        ProfileRepository repository = new ProfileRepository(DEVELOPMENT_CONNECTION_STRING);

        var result = repository.Delete(new IVRProfile() { ProfileId = profileId });

        Assert.AreEqual(true, result);
      }

      [TestMethod]
      public void ShouldReturnFalseWhenUserProfileCannotBeDeleted()
      {
        int profileId = 5555555;

        ProfileRepository repository = new ProfileRepository(DEVELOPMENT_CONNECTION_STRING);

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

        ProfileRepository repository = new ProfileRepository(DEVELOPMENT_CONNECTION_STRING);

        var userPorfile = repository.GetByPhoneNumber(phoneNumberToLookup);

        Assert.IsNotNull(userPorfile);
        Assert.AreEqual(phoneNumberToLookup, userPorfile.PhoneNumber);
      }
    }

    [TestClass]
    public class GetAllTests
    {
      [TestMethod]
      public void ShouldReturnAllEntities()
      {
        ProfileRepository repository = new ProfileRepository(DEVELOPMENT_CONNECTION_STRING);

        var allProfiles = repository.GetAll();

        Assert.AreEqual(1, allProfiles.Count);
      }
    }

    [TestClass]
    public class UpdateTests
    {
      [TestMethod]
      public void ShouldUpdateProfile()
      {
        IVRProfile profile = new IVRProfile()
        {
          ProfileId = 11112,
          Culture = "en",
          PIN = "1234",
          FullName = "Basil",
          PhoneNumber = "+22222221"
        };

        ProfileRepository repository = new ProfileRepository(DEVELOPMENT_CONNECTION_STRING);

        var createdEntity = repository.Create(profile);

        createdEntity.Culture = "fr";
        createdEntity.PhoneNumber = "+1111112";
        createdEntity.PIN = "4321";

        repository.Update(createdEntity);

        var updatedEntity = repository.Get(11112);

        Assert.AreEqual(createdEntity.Culture, updatedEntity.Culture);
        Assert.AreEqual(createdEntity.PhoneNumber, updatedEntity.PhoneNumber);
        Assert.AreEqual(createdEntity.PIN, updatedEntity.PIN);
      }

      [TestMethod]
      [Ignore]
      public void ShouldReturnFalseWhenCannotUpdateProfile()
      {
        
      }
    }
  }
}
