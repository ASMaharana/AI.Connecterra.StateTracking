using Connecterra.StateTracking.Common.Core;
using System.Threading.Tasks;

namespace Connecterra.StateTracking.MicroService.Domain.Repository
{
    public interface IRepository<T>
    {
        Task<Result<T>> LoadAsync(string id);
        Task<Result> SaveAsync(T cow);
    }
}