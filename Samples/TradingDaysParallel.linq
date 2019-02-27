<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.XML.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.Linq.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.XDocument.dll</Reference>
  <Namespace>System.Globalization</Namespace>
</Query>

//string url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml";
string url = @"\\atuin\Olaf\Entwicklung\LINQPad\LINQPad Queries\eurofxref-hist.xml";

Stopwatch sw = new Stopwatch();

sw.Restart();
XDocument doc1 = XDocument.Load(url);
sw.Stop();
$"\n\rXDocument geladen in {sw.ElapsedMilliseconds:N0} ms.".Dump();

$"\n\rSequentielle Objekterstellung...".Dump();
sw.Restart();
IEnumerable<TradingDay> qTradingdays = from nd in doc1.Root.Descendants()
									   where nd.Name.LocalName == "Cube"
											   && nd.Attributes().Any(at => at.Name == "time")
									   select new TradingDay(nd);

List<TradingDay> list = qTradingdays.ToList();
sw.Stop();
$"{list.Count} TradingDays erzeugt in {sw.ElapsedTicks:N0} tks".Dump();

sw.Restart();
XDocument doc2 = XDocument.Load(url);
sw.Stop();
$"\n\rXDocument geladen in {sw.ElapsedMilliseconds:N0} ms.".Dump();

$"\n\rParallele Objekterstellung...".Dump();
sw.Restart();
IEnumerable<TradingDay> qTradingdaysPar = from nd in doc2.Root.Descendants().AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism)
										  where nd.Name.LocalName == "Cube"
												  && nd.Attributes().Any(at => at.Name == "time")
										  // select new TradingDay(nd, true);
										  select new TradingDay(nd);

List<TradingDay> listPar = qTradingdaysPar.ToList();
sw.Stop();
$"{listPar.Count} TradingDays parallel erzeugt in {sw.ElapsedTicks:N0} tks".Dump();

}
	

class TradingDay
{
	public TradingDay(XElement tradingDayNode)
	{
		this.Date = Convert.ToDateTime(tradingDayNode.Attribute("time").Value);
		this.ExchangeRates = tradingDayNode.Descendants().Select(dn => new ExchangeRate()
		{
			CurrencyCode = dn.Attribute("currency").Value,
			Rate = Convert.ToDouble(dn.Attribute("rate").Value, new NumberFormatInfo() { NumberDecimalSeparator = "." })
		}).ToList();
	}

	public TradingDay(XElement tradingDayNode, bool asParallel)
	{
		this.Date = Convert.ToDateTime(tradingDayNode.Attribute("time").Value);
		this.ExchangeRates = tradingDayNode.Descendants().AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism).Select(dn => new ExchangeRate()
		{
			TradingDay = this,
			CurrencyCode = dn.Attribute("currency").Value,
			Rate = Convert.ToDouble(dn.Attribute("rate").Value, new NumberFormatInfo() { NumberDecimalSeparator = "." })
		}).ToList();

	}

	public DateTime Date { get; set; }
	public List<ExchangeRate> ExchangeRates { get; set; }
}

class ExchangeRate
{
	public TradingDay TradingDay { get; set; }
	public double Rate { get; set; }
	public string CurrencyCode { get; set; }
}

class EOF {