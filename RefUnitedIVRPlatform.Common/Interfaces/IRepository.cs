using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IRepository<T>
  {
    T Get(int id);
    T Create(T item);
    bool Delete(T item);
    List<T> GetAll();
    void Update(T profile);
  }
}
