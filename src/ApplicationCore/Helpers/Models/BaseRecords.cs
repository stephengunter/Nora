using AutoMapper;
using Infrastructure.Entities;
using Infrastructure.Views;

namespace ApplicationCore.Helpers;

public static class BaseRecordHelpers
{
	public static void CheckActiveOrder(this BaseRecord entity, BaseRecordView model)
   {
      if(model.Active && entity.Order < 0) entity.Order = 0;
		else entity.Order = -1;
   }
}

