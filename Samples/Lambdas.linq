<Query Kind="Statements" />

// Menge zum Filtern
IList<int> numbers = Enumerable.Range(100, 999).ToList();

// C# 1
//IPredicate<int> predicate = new PredicateClass(); // <- Interface, erfordert eigene Überladung im ListHelper!
//PredicateDelegate<int> predicate = DividableByNine; // <- Delegat, erfordert eigene Überladung im ListHelper!

// C# 2
//Predicate<int> predicate = DividableByNine;

// C# 3
//Predicate<int> predicate = delegate(int element)
//  {
//      return element % 9 == 0;
//  };

// C# 4
Predicate<int> predicate = element => element % 9 == 0;

IList<int> result = ListHelper.Filter(numbers, predicate);

result.Dump();

}


public static class ListHelper
{
	public static IList<T> Filter<T>(IList<T> source, Predicate<T> filterCondition)
	{
		IList<T> returnList = new List<T>();

		foreach (T item in source)
		{
			if (filterCondition(item))
			{
				returnList.Add(item);
			}
		}

		return returnList;
	}
	// Überladung für C# 1 mit PredicateDelegate<T>
	public static IList<T> Filter<T>(IList<T> source, PredicateDelegate<T> filterCondition)
	{
		IList<T> returnList = new List<T>();

		foreach (T item in source)
		{
			if (filterCondition(item))
			{
				returnList.Add(item);
			}
		}

		return returnList;
	}


	// Überladung für C# 1 mit IPredicate - Nachteil: Methode muss Interface-Methoden kennen
	public static IList<T> Filter<T>(IList<T> source, IPredicate<T> conditionPredicate)
	{
		IList<T> returnList = new List<T>();

		foreach (T item in source)
		{
			if (conditionPredicate.DividableByNine(item))
			{
				returnList.Add(item);
			}
		}

		return returnList;
	}

}

// Konstrukte für C# 1:
public delegate bool PredicateDelegate<T>(T obj);

public interface IPredicate<T>
{
	bool DividableByNine(T element);
}

public class PredicateClass : IPredicate<int>
{
	public bool DividableByNine(int element)
	{
		return element % 9 == 0;
	}
}


class EOF {