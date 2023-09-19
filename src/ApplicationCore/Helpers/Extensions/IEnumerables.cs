using System.Reflection.Metadata.Ecma335;
using System.Net.Http.Headers;
using System;

namespace ApplicationCore.Helpers;
public static class IEnumerableHelpers
{
	public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
	{
		if (enumerable is null) return true;

		var collection = enumerable as ICollection<T>;
		if (collection != null) return collection.Count < 1;
		return !enumerable.Any();
	}

	public static bool HasItems<T>(this IEnumerable<T> enumerable) => !IsNullOrEmpty(enumerable);

	public static bool AllTheSame(this IEnumerable<int> listA, IEnumerable<int> listB)
		=> listB.All(listA.Contains) && listA.Count() == listB.Count();

	public static IEnumerable<T> GetList<T>(this IEnumerable<T>? enumerable)
		=> enumerable.IsNullOrEmpty() ? new List<T>() : enumerable!.ToList();
}
