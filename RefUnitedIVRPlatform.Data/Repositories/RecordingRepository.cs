using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Data.DtoConverter;
using RefUnitedIVRPlatform.Data.DtoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Data.Repositories
{
  public class RecordingRepository : IRecordingRepository
  {
    private CloudStorageAccount storageAccount;
    private CloudTableClient tableClient;
    private CloudTable recordingsTable;

    private const string AZURE_RECORDING_TABLE = "IVRVoiceRecording";

    public RecordingRepository(string connectionString)
    {
      if (connectionString.Equals("DEVELOPMENT_CONNECTION"))
      {
        this.storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
      }
      else
      {
        this.storageAccount = CloudStorageAccount.Parse(connectionString);
      }

      this.tableClient = storageAccount.CreateCloudTableClient();
      CheckAndCreateStorage();
    }

    private void CheckAndCreateStorage()
    {
      recordingsTable = tableClient.GetTableReference(AZURE_RECORDING_TABLE);
      recordingsTable.CreateIfNotExists();
    }

    public List<Recording> GetForProfile(int profileId)
    {
      TableQuery<RecordingEntity> query = new TableQuery<RecordingEntity>().Where(
        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, profileId.ToString()));

      var results = recordingsTable.ExecuteQuery<RecordingEntity>(query).Select<RecordingEntity, Recording>(x =>
        {
          return RecordingToEntityMapper.ConvertFromEntity(x);
        }).ToList();

      return results;
    }

    public void DeleteById(int recordingId)
    {
      var recording = Get(recordingId);

      Delete(recording);
    }

    public Recording Get(int id)
    {
      throw new NotImplementedException();
      //string partitionKey = id.ToString().Substring(0, 2);
      //string rowKey = id.ToString();

      //TableOperation getOperation = TableOperation.Retrieve<RecordingEntity>(partitionKey, rowKey);

      //var result = recordingsTable.Execute(getOperation);

      //var entity = (RecordingEntity)result.Result;

      //var returnValue = RecordingToEntityMapper.ConvertFromEntity(entity);

      //return returnValue;
    }

    public Recording Create(Recording item)
    {
      var entity = RecordingToEntityMapper.ConvertToEntity(item);

      TableOperation insertOperation = TableOperation.Insert(entity);
      var result = recordingsTable.Execute(insertOperation);


      RecordingEntity entityResult = (RecordingEntity)result.Result;

      return RecordingToEntityMapper.ConvertFromEntity(entityResult);
    }

    public bool Delete(Recording item)
    {
      //Azure storage will throw a StorageException --- 404 --- if it cannot find the entity to delete.

      var entity = RecordingToEntityMapper.ConvertToEntity(item);

      entity.ETag = "*";

      TableOperation deleteOperation = TableOperation.Delete(entity);

      try
      {
        var result = recordingsTable.Execute(deleteOperation);

        return true;
      }
      catch (StorageException)
      {
        return false;
      }
    }

    public List<Recording> GetAll()
    {
      var query = new TableQuery<RecordingEntity>();

      var results = recordingsTable.ExecuteQuery(query).Select<RecordingEntity, Recording>(x =>
      {
        return RecordingToEntityMapper.ConvertFromEntity(x);
      }).ToList();

      return results;
    }

    public bool Update(Recording profile)
    {
      throw new NotImplementedException();
      //var profileEntity = RecordingToEntityMapper.ConvertToEntity(profile);

      //TableOperation retrievalOperation = TableOperation.Retrieve<IVRProfileEntity>(profileEntity.PartitionKey, profileEntity.RowKey);

      //var retrievalResult = recordingsTable.Execute(retrievalOperation);

      //var entityToUpdate = (IVRProfileEntity)retrievalResult.Result;

      //if (entityToUpdate != null)
      //{
      //  RecordingToEntityMapper.UpdateEntity(entityToUpdate, profileEntity);

      //  TableOperation updateOperation = TableOperation.Replace(entityToUpdate);
      //  var updateResult = recordingsTable.Execute(updateOperation);

      //  return true;
      //}

      //return false;
    }
  }
}
