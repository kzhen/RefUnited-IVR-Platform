using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Data.DtoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Data.DtoConverter
{
  internal static class RecordingToEntityMapper
  {
    internal static RecordingEntity ConvertToEntity(Recording recordingToConvert)
    {
      return new RecordingEntity()
      {

      };
    }

    internal static Recording ConvertFromEntity(RecordingEntity eneityToConvert)
    {
      return new Recording()
      {

      };
    }

    internal static void UpdateEntity(IVRProfileEntity entityToUpdate, RecordingEntity profileEntity)
    {
      throw new NotImplementedException();
    }
  }
}
