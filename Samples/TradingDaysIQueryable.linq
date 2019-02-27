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
context1.Database.Log = LogIt;
bool getPHP = true;

IQueryable<TradingDay> list = context1.TradingDays.Include("ExchangeRates").AsQueryable();
if (getPHP)
{
	list = list.Where(td => td.ExchangeRates.Any(er => er.CurrencyCode == "PHP"));
}
List<TradingDay> result = list.ToList();
sw.Stop();
$"{result.Count} TradingDays gefunden in {sw.ElapsedTicks:N0} Ticks ({sw.ElapsedMilliseconds:N0}ms).".Dump();
//list.Dump();
"------------------------------------------------------------------------------------------------------".Dump();


}


void LogIt(string logString)
{
	//return;
	logString.Dump();
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