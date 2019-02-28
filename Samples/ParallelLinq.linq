<Query Kind="Statements" />

int divisor = 3;

int low = 1;
int high = 8999999;

Stopwatch sw = new Stopwatch();

IList<int> numbers = Enumerable.Range(low, high).ToList();

sw.Start();
var evenNums = from num in numbers
			   where num % divisor == 0
			   select Math.Sqrt(num);

int result = evenNums.Count();
sw.Stop();

$"Normale Query: {sw.ElapsedTicks:N0} ticks, #{result:N0}".Dump();

IList<int> numbers2 = Enumerable.Range(low, high).ToList();

sw.Restart();
var evenNumsPar = from num in numbers2.AsParallel()
				  where num % divisor == 0
				  select Math.Sqrt(num);

int result2 = evenNumsPar.Count();
sw.Stop();
$"Parallele Query: {sw.ElapsedTicks:N0} ticks, #{result2:N0}".Dump();

IList<int> numbers3 = Enumerable.Range(low, high).ToList();
sw.Restart();
var evenNumsParOrd = from num in numbers3.AsParallel().AsOrdered()
					 where num % divisor == 0
					 select Math.Sqrt(num);

int result3 = evenNumsParOrd.Count();
sw.Stop();

$"Parallel.AsOrdered: {sw.ElapsedTicks:N0} ticks, #{result3:N0}".Dump();