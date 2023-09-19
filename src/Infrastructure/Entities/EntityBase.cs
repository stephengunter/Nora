using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Interfaces;

namespace Infrastructure.Entities;
public abstract class EntityBase : IAggregateRoot
{
	public int Id { get; set; }
}
