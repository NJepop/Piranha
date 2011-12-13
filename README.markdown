byBrick Piranha 
===============
byBrick Piranha är ett komponentpaket för att bygga MVC-baserade webbapplikationer
i .NET. Paketet innehåller både generella komponenter för t.ex. datahantering men
även delar som är specifika för paketets CMS-hantering.

Paketet riktar sig främst mot små och medelstora projekt där man vill han en enkel
och ren CMS-struktur som man kan bygga vidare på inom projektet och inte behöver en
stor produkt.

byBrick.Data - Active Record
----------------------------
Active record implementationen är byggd för att man snabbt ska kunna hämta upp data
ifrån databasen som typade objekt, antingen för att använda direkt i applikationen
eller för att t.ex. exponera vidare via en WCF-tjänst. För att komma igång behöver
man bara följa några enkla steg.

####Konfigurera databasen
För att konfigurera databasen använder man den standardfunktionalitet som finns i .NET
genom att lägga till en connectionstring med namnet default i sin applikations-config.

	<connectionStrings> 
		<add name="default" providerName="System.Data.SqlClient" 
			connectionString="server=myserver;database=mydatabase;uid=myuser;pwd=mypassword" /> 
	</connectionStrings>

####Definiera ditt Active record objekt
Skapa en klass som definierar ditt data objekt och märk upp klassen med ActiveRecord
attributet. Som default förutsätts att tabellens namn är samma som klassens och att
kolumnen som är primärnyckel heter "id". Om det inte stämmer kan man fritt namnge båda
dessa, som i exemplet nedan.

	[Table(Name="page", PrimaryKey="page_id")]
	public class Page : ActiveRecord<Page>
	{
		[Column(Name="page_id")]
		public Guid Id { get ; set ; }

		[Column(Name="page_title")]
		public string Title { get ; set ; }
	}

####Hämta lite data
Det var det som krävdes, nu kan man hämta upp informationen ifrån databasen. Följande kodsnutt
hämtar alla tillgängliga objekt.

	List<Page> pages = Page.Get() ;

Eller varför inte hämta alla objekt där titeln börjar med ordet "The" och sortera dem i 
bokstavsordning.

	List<Page> page = Page.Get("page_title LIKE '@0%'", "The", 
		new Params() { OrderBy = "page_title ASC" }) ;

####Manipulera data
När du väl hämtat upp informationen kan du ändra dess värden och enkelt spara den.

	Page page = Page.GetSingle(new Guid("66A734E5-8907-423C-AC58-69F5E2E93CFD")) ;
	page.Title = "This is the new title" ;
	page.Save() ;

Det finns också stöd för att spara och ta bort objekt inuti en transaktion för mer avancerade
operationer.

	List<Page> pages ;
	try {
		using (var tx = Database.OpenTransaction()) {
			foreach (Page page in pages)
				page.Save() ;
			tx.Commit() ;
		}
	} catch {
		// Handle application error
	}

####Constructors
	public Table()

####Properties
	public bool IsNew { get ; set ; }

####Methods
	public static T GetSingle(object id)
	public static T GetSingle(string where, params object[] args)
	public static List<T> Get(Params param)
	public static List<T> Get(string where = "", params object[] args)
	public static List<T> GetFields(string fields, Params param)
	public static  List<T> GetFields(string fields, string where = "", params object[] args)
	public static List<T> Query(string query, params object[] args)
	public static int Execute(string statement, IDbTransaction tx = null, params object[] args)
	public bool Save(IDbTransaction tx = null)
	public bool Delete(IDbTransaction tx = null)

Array extensions
----------------
###Implode
Kombinera alla strängarna i arrayen till en sträng. Om den frivilliga "separator" är angiven
är detta värde placerat mellan alla strängarna.

	public static string Implode(this string[] arr, string sep = "")

Exampel:

	string[] arr = new string[] {"This", "is", "just", "an", "array"} ;

	// Följande ger resultatet "This is just an array"
	string str = arr.Implode(" ") ;

###ImplodeWhere
Kombinerar alla strängarna som uppfyller det givna predikatet.

	public static string ImplodeWhere(this string[] arr, string sep, Func<string, bool> predicate)

Exampel:

	string[] arr = new string[] {"This", "is", "just", "an", "array"} ;

	// Följande ger resultatet "This is an array"
	string str2 = arr.ImplodeWhere(" ", (str) => str != "just") ;

###Append
Slå ihop två arrayer med varandra.

	public static T[] Append<T>(this T[] head, T[] tail)

Exampel:

	string[] a = new string[] {"This", is", "just"} ;
	string[] b = new string[] {"an", "array"} ;

	// Följande resulterar i arrayen {"This", "is", "just", "an", "array"}
	string[] c = a.Append<string>(b) ;

###Each
Ett enkelt och jQuery inspirerat sätt att enumerera en Generic List.

	public static void Each<T>(this T[] arr, Action<int, T> proc)

Exampel:

	string[] arr = new string[] {"This", "is", "just", "an", "array"} ;
	List<string> lst = new List<string>() ;
	
	// Lägger till alla strängar som innehåller bokstaven "a" i listan "lst"
	arr.Each<string>((index, itm) => {
		if (itm.Contains('a'))
			lst.Add(itm) ;
		});

Metoden finns också tillgänglig för alla collections som implementerar IEnumerable och
IEnumerable<T>.

	public static void Each(this IEnumerable ienum, Action<int, object> proc)
	public static void Each<T>(this IEnumerable<T> ienum, Action<int, T> proc)
