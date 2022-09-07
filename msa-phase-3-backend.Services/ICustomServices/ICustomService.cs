using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace msa_phase_3_backend.Services.ICustomServices
{
    public interface ICustomService<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(int Id);
        void Insert(T entity); 
        void Update(T entity);
        void Delete(T entity);
        void Remove(T entity);
    }
}