using Ardalis.Specification;

namespace Infrastructure.Interfaces;

// from Ardalis.Specification
public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
   void Update(T entity);
	void Update(T existingEntity, T model);
}
