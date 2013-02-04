using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefUnitedIVRPlatform.Common.Entities;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IProfileRepository : IRepository<IVRProfile>
  {
    IVRProfile GetByPhoneNumber(string lookupPhoneNumber);
  }
}
