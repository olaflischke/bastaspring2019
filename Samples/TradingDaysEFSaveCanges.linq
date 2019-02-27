<Query Kind="Statements">
  <Reference>&lt;RuntimeDirectory&gt;\System.XML.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.Linq.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xml.XDocument.dll</Reference>
  <NuGetReference>EntityFramework</NuGetReference>
  <Namespace>System.Data.Entity</Namespace>
  <Namespace>System.Globalization</Namespace>
  <AppConfig>
    <Content>
      <configuration>
        <configSections>
          <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
          <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
        </configSections>
        <entityFramework>
          <defaultConnectionFactory type="System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework">
            <parameters>
              <parameter value="mssqllocaldb" />
            </parameters>
          </defaultConnectionFactory>
          <providers>
            <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
          </providers>
        </entityFramework>
        <connectionStrings>
          <add name="TradingDayModel" connectionString="data source=(localdb)\mssqllocaldb;initial catalog=TradingDayContext;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
        </connectionStrings>
      </configuration>
    </Content>
  </AppConfig>
</Query>

//string url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml";
string url = @"\\atuin\Olaf\Entwicklung\LINQPad\LINQPad Queries\eurofxref-hist-90d.xml";


Stopwatch sw = new Stopwatch();

sw.Start();
XDocument doc = XDocument.Load(url);
sw.Stop();
$"XML geladen in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds}ms).".Dump();

RecreateDb();

sw.Restart();
IEnumerable<TradingDay> qTradingdays = from nd in doc.Root.Descendants()
									   where nd.Name.LocalName == "Cube"
											   && nd.Attributes().Any(at => at.Name == "time")
									   select new TradingDay(nd);

List<TradingDay> list = qTradingdays.ToList();

sw.Stop();
$"{list.Count} TradingDays erstellt in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds}ms).".Dump();

TradingDayContext context1 = new TradingDayContext();
context1.Database.Log=LogIt;
sw.Restart();
foreach (TradingDay item in qTradingdays)
{
	context1.TradingDays.Add(item);
	context1.SaveChanges();
}
sw.Stop();
$"Tradingdays einzeln hinzugefügt: {sw.ElapsedMilliseconds:N0}ms.".Dump();

RecreateDb();

TradingDayContext context2 = new TradingDayContext();
context2.Database.Log=LogIt;

sw.Restart();
context2.TradingDays.AddRange(qTradingdays);
context2.SaveChanges();
sw.Stop();
$"Tradingdays per AddRange hinzugefügt: {sw.ElapsedMilliseconds:N0}ms.".Dump();

}
	
void RecreateDb()
{
	Stopwatch sw = new Stopwatch();
	TradingDayContext context = new TradingDayContext();
	sw.Start();
	if (context.Database.Exists()) context.Database.Delete();
	sw.Stop();
	$"Datenbank gelöscht ({sw.ElapsedMilliseconds:N0}ms)...".Dump();
	sw.Restart();
	context.Database.Create();
	sw.Stop();
	$"...und neu erzeugt ({sw.ElapsedMilliseconds:N0}ms).".Dump();
}

void LogIt(string logString)
{
	logString.Dump();
}

class TradingDay
{
	public TradingDay(XElement tradingDayNode)
	{
		this.Date = Convert.ToDateTime(tradingDayNode.Attribute("time").Value);
		this.ExchangeRates = tradingDayNode.Descendants().Select(dn => new ExchangeRate()
		{
			TradingDay = this,
			CurrencyCode = dn.Attribute("currency").Value,
			Rate = Convert.ToDouble(dn.Attribute("rate").Value, new NumberFormatInfo() { NumberDecimalSeparator = "." })
		}).ToList();
	}

	public int Id { get; set; }
	public DateTime Date { get; set; }
	public List<ExchangeRate> ExchangeRates { get; set; }
}

class ExchangeRate
{
	public int Id { get; set; }
	public TradingDay TradingDay { get; set; }
	public double Rate { get; set; }
	public string CurrencyCode { get; set; }
}

class TradingDayContext : DbContext
{
	public TradingDayContext():base("name=TradingDayModel")
	{
			
	}
	
	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
	}

	public DbSet<TradingDay> TradingDays { get; set; }
	public DbSet<ExchangeRate> ExchangeRates { get; set; }
}

class EOF {