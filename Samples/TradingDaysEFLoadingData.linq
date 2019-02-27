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
          <add name="TradingDayModelLarge" connectionString="data source=(localdb)\mssqllocaldb;initial catalog=TradingDayContextLarge;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
        </connectionStrings>
      </configuration>
    </Content>
  </AppConfig>
</Query>

Stopwatch sw = new Stopwatch();

sw.Start();
TradingDayContext context1 = new TradingDayContext();
context1.Database.Log=LogIt;
List<TradingDay> list = context1.TradingDays.ToList();
sw.Stop();
$"{list.Count} TradingDays flach geladen in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds:N0}ms).".Dump();
//list.Dump();
"------------------------------------------------------------------------------------------------------".Dump();

//sw.Restart();
//TradingDayContext context2 = new TradingDayContext();
//context2.Database.Log = LogIt;
//List<TradingDay> list2 = context2.TradingDays.ToList();
//foreach (TradingDay element in list2)
//{
//	List<ExchangeRate> rates = context2.ExchangeRates.Where(rate => rate.TradingDay.Id == element.Id).ToList();
//	element.ExchangeRates = rates;
//}

//sw.Stop();
//$"{list2.Count} TradingDays mit ExchangeRates per Schleife geladen in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds:N0}ms).".Dump();
////list2.Dump();
//"------------------------------------------------------------------------------------------------------".Dump();

//sw.Restart();
//TradingDayContext context3 = new TradingDayContext();
//context3.Database.Log = LogIt;
//List<TradingDay> list3 = context3.TradingDays.Include("ExchangeRates").ToList();
//sw.Stop();
//$"{list3.Count} TradingDays mit ExchangeRates per Include('NavigationPropName') geladen in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds:N0}ms).".Dump();
////list3.Dump();
//"------------------------------------------------------------------------------------------------------".Dump();
//
//sw.Restart();
//TradingDayContext context4 = new TradingDayContext();
//context4.Database.Log = LogIt;
//List<TradingDay> list4 = context4.TradingDays.Include(td => td.ExchangeRates).ToList();
//sw.Stop();
//$"{list4.Count} TradingDays mit ExchangeRates per Include(Lambda) geladen in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds:N0}ms).".Dump();
////list4.Dump();
//"------------------------------------------------------------------------------------------------------".Dump();
//
//sw.Restart();
//TradingDayContext contex5 = new TradingDayContext();
//contex5.Database.Log = LogIt;
//var qDays = (from td in contex5.TradingDays
//			 select new
//			 {
//				 td.Date,
//				 td.ExchangeRates
//			 }).ToList();
//sw.Stop();
//$"{qDays.Count} TradingDays mit ExchangeRates per Projektion geladen in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds:N0}ms).".Dump();
////qDays.Dump();
//"------------------------------------------------------------------------------------------------------".Dump();
//
//sw.Restart();
//TradingDayContext context6 = new TradingDayContext();
//context6.Database.Log = LogIt;
//List<TradingDay> list6 = context4.TradingDays.Include(td => td.ExchangeRates).AsNoTracking().ToList();
//sw.Stop();
//$"{list6.Count} TradingDays mit ExchangeRates per Include(Lambda) ohne Tracking geladen in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds:N0}ms).".Dump();
////list6.Dump();
//"------------------------------------------------------------------------------------------------------".Dump();

	}


void LogIt(string logString)
{
	return;
//	logString.Dump();
}

class TradingDay
{
	public TradingDay()
	{

	}

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
	public TradingDayContext() : base("name=TradingDayModelLarge")
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