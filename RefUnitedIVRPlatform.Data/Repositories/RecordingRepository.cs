using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
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
      throw new NotImplementedException();
    }

    public void DeleteById(int recordingId)
    {
      throw new NotImplementedException();
    }

    public Recording Get(int id)
    {
      throw new NotImplementedException();
    }

    public Recording Create(Recording item)
    {
      throw new NotImplementedException();
    }

    public bool Delete(Recording item)
    {
      throw new NotImplementedException();
    }

    public List<Recording> GetAll()
    {
      throw new NotImplementedException();
    }

    public bool Update(Recording profile)
    {
      throw new NotImplementedException();
    }
  }
}
