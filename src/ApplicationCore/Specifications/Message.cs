using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;

public class MessageFilterSpecification : Specification<Message>
{
	public MessageFilterSpecification()
	{
		Query.Where(item => !item.Removed);
	}

	public MessageFilterSpecification(bool returned)
	{
		Query.Where(item => !item.Removed && item.Returned == returned);
	}

}

