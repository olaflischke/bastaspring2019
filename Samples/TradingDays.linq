<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.XML.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.Linq.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.XDocument.dll</Reference>
  <Namespace>System.Globalization</Namespace>
</Query>

string url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml";
//string url = @"\\atuin\Olaf\Entwicklung\LINQPad\LINQPad Queries\eurofxref-hist.xml";

Stopwatch sw = new Stopwatch();

sw.Start();
XDocument doc = XDocument.Load(url);
sw.Stop();
$"XML geladen in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds}ms).".Dump();


sw.Restart();
IEnumerable<TradingDay> qTradingdays = from nd in doc.Root.Descendants()
									   where nd.Name.LocalName == "Cube"
											   && nd.Attributes().Any(at => at.Name == "time")
									   select new TradingDay(nd);

List<TradingDay> list = qTradingdays.ToList();
sw.Stop();
$"{list.Count} TradingDays erstellt in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds}ms).".Dump();

list.Dump();

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

	public DateTime Date { get; set; }
	public List<ExchangeRate> ExchangeRates { get; set; }
}

class ExchangeRate
{

	public double Rate { get; set; }
	public string CurrencyCode { get; set; }
}

class EOF {