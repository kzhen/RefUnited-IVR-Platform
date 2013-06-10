using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RefUnitedIVRPlatform.Data.Repositories;
using Microsoft.WindowsAzure.Storage;
using RefUnitedIVRPlatform.Data.DtoEntities;
using Microsoft.WindowsAzure.Storage.Table;

namespace RefUnitedIVRPlatform.Data.Tests
{
  [TestClass]
  public class RecordingRepositoryTests
  {
    public const string DEVELOPMENT_CONNECTION_STRING = "DEVELOPMENT_CONNECTION";

    [TestClass]
    public class GetForProfileTests
    {
      [TestInitialize]
      public void Setup()
      {
        var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        var tableClient = storageAccount.CreateCloudTableClient();
        var table1 = tableClient.GetTableReference("IVRVoiceRecording");
        table1.DeleteIfExists();
        table1.Create();

        for (int i = 0; i < 3; i++)
        {
          var entity = new RecordingEntity()
          {
            RecordingId = i,
            ToProfileId = 111,
            FromProfileId = 222,
            Url = "http://"
          };

          entity.PartitionKey = entity.ToProfileId.ToString();
          entity.RowKey = entity.RecordingId.ToString();

          var op = TableOperation.Insert(entity);
          table1.Execute(op);
        }
      }

      [TestMethod]
      public void Given_A_ProfileWithRecordings_Should_ReturnAListOfRecordings()
      {
        int profileId = 111;
        RecordingRepository repository = new RecordingRepository(DEVELOPMENT_CONNECTION_STRING);

        var results = repository.GetForProfile(profileId);

        Assert.AreEqual(3, results.Count);
      }

      [TestMethod]
      public void Given_A_ProfileWithNoRecordings_Should_ReturnAnEmptyList()
      {
        int profileId = 222;
        RecordingRepository repository = new RecordingRepository(DEVELOPMENT_CONNECTION_STRING);

        var results = repository.GetForProfile(profileId);

        Assert.AreEqual(0, results.Count);
      }
    }

    [TestClass]
    public class GetAllTests
    {
      [TestInitialize]
      public void Setup()
      {
        var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        var tableClient = storageAccount.CreateCloudTableClient();
        var table1 = tableClient.GetTableReference("IVRVoiceRecording");
        table1.DeleteIfExists();
        table1.Create();

        for (int i = 0; i < 3; i++)
        {
          var entity = new RecordingEntity()
          {
            RecordingId = i,
            ToProfileId = 111,
            FromProfileId = 222,
            Url = "http://"
          };

          entity.PartitionKey = entity.ToProfileId.ToString();
          entity.RowKey = entity.RecordingId.ToString();

          var op = TableOperation.Insert(entity);
          table1.Execute(op);
        }

        for (int i = 0; i < 3; i++)
        {
          var entity = new RecordingEntity()
          {
            RecordingId = i,
            ToProfileId = 112,
            FromProfileId = 222,
            Url = "http://"
          };

          entity.PartitionKey = entity.ToProfileId.ToString();
          entity.RowKey = entity.RecordingId.ToString();

          var op = TableOperation.Insert(entity);
          table1.Execute(op);
        }
      }

      [TestMethod]
      public void Given_A_RepositoryWith6Recordings_Should_ReturnAListWith6Recordings()
      {
        RecordingRepository repository = new RecordingRepository(DEVELOPMENT_CONNECTION_STRING);

        var results = repository.GetAll();

        Assert.AreEqual(6, results.Count);
      }

      [TestMethod]
      public void Given_An_EmptyRepository_Should_ReturnAnEmptyList()
      {
        var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
        var tableClient = storageAccount.CreateCloudTableClient();
        var table1 = tableClient.GetTableReference("IVRVoiceRecording");
        table1.DeleteIfExists();
        table1.Create();

        RecordingRepository repository = new RecordingRepository(DEVELOPMENT_CONNECTION_STRING);

        var results = repository.GetAll();

        Assert.AreEqual(0, results.Count);
      }
    }
  }
}
