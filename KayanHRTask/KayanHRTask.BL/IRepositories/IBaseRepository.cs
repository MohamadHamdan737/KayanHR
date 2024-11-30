using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KayanHRTask.BL.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        void Add(T model);
        void Update(T model);
        void Delete(int id);
        
        void SaveData();

        // Async Methods

        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
