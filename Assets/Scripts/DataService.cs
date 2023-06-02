using SQLite4Unity3d;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using System.Collections;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataService  {

	private SQLiteConnection connection;

	//Crea el archivo de la base de datos y la conexion a esta
	public DataService(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);     

	}

    internal IEnumerable<PlayerCards> GetCardsFromPlayerCardsTable(string collection, string rarity)
    {
        if (collection == null)
        {
			return connection.Table<PlayerCards>().Where(x => x.rarity == rarity);
		}
		else if(rarity==null)
		{
			return connection.Table<PlayerCards>().Where(x => x.collection == collection);
		}

		return connection.Table<PlayerCards>().Where(x => x.collection == collection).Where(y => y.rarity == rarity);
	}

    internal Card GetCard(string cardImage)
    {
		return connection.Table<Card>().Where(x=> x.image==cardImage).FirstOrDefault();
    }

    //Reduce la cantidad de una carta en la tabla PlayerCards
    internal void RemoveCardFromPlayerCards(PlayerCards playerCards)
    {
		PlayerCards c = connection.Table<PlayerCards>().Where(x => x.image == playerCards.image).FirstOrDefault();
		Debug.Log("Carta " + c);
		c.quantity--;
		Debug.Log("Cantidad despues: " + c.quantity);
		connection.Update(c);
		Debug.Log("Cantidad despues de insert: " + c.quantity);
		
	}

	//Devuelve las cartas de una coleccion y de una rareza de la tabla Card
	internal IEnumerable<Card> GetCardsFromCardTable(string collection, string rarity)
    {

		return connection.Table<Card>().Where(x => x.collection == collection).Where(y=>y.rarity==rarity);
    }
	//Devuelve las cartas de la tabla PlayerCards ordenadas por coleccion
	internal IEnumerable<PlayerCards> GetAllCardsFromPlayerCardsTable()
    {
		return connection.Table<PlayerCards>().OrderBy<String>(x=>x.collection);
    }

	//Devuelve las cartas de una coleccion de la tabla Card ordenadas por numero
	internal IEnumerable<Card> GetCardsFromCardTable(string collection)
    {
		return connection.Table<Card>().Where(x => x.collection == collection).OrderBy<Int32>(x=>x.number);
	}

	//Devuelve las cartas de una coleccion de la tabla PlayerCards ordenadas por numero
	internal IEnumerable<PlayerCards> GetCardsFromPlayerCardsTable(string collection)
	{
		return connection.Table<PlayerCards>().Where(x => x.collection == collection).OrderBy<Int32>(x => x.number);
	}

	//Crea las tablas y llena la tabla Card
	public void CreateDB()
	{
		//connection.DropTable<Person> ();
		//connection.CreateTable<Person> ();
		string query;
		//Se crea la tabla card y playercards en caso de que no exista
		connection.CreateTable<Card>();
		connection.CreateTable<PlayerCards>();
		FillCardTable();
		
	}

	//Llena la tabla Card con todas las cartas
    private void FillCardTable()
    {
		int numberOfCards;
		String[] directories=null;
		//Se mira si la aplicacion esta siendo usada desde android u otro dispositivo
        if (Application.platform == RuntimePlatform.Android)
        {
			try
			{
                foreach (var card in Resources.LoadAll<Sprite>("Cartas"))
                {
					string infoCard = card.ToString();
					infoCard = infoCard.Replace("(UnityEngine.Sprite)","");
                }
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
        }
        else
        {
			//Guarda el directorio donde estan las cartas de la base de datos
			directories = Directory.GetDirectories(".\\Assets\\Resources\\Cartas");
			//Se mira en los sudirectorios y se guarda en un array todas las imagenes de ese directorio
			for (int i = 0; i < directories.Length; i++)
			{
				numberOfCards = Directory.GetFiles(directories[i]).Length;
				String[] cardImages = Directory.GetFiles(directories[i]);
				//Por cada elemento del array se separa el nombre del archivo para obtener el nombre
				//real de la carta e insertarla asi en la tabla
				for (int j = 0; j < numberOfCards; j++)
				{
					if (cardImages[j].Split('.').Length == 3)
					{
						String[] cardPathSplited = cardImages[j].Split('.');
						String nameOfFile = cardPathSplited[1].Split("\\")[cardPathSplited[1].Split("\\").Length - 1];
						InsertCardInCardTable(nameOfFile, cardImages[j]);
					}
				}
			}
		}
		
	}

	//Guarda en la tabla Card las cartas
	private void InsertCardInCardTable(string nameOfFile, string cardImage)
	{
		//Se separa el mombre del archivo en base al separador y asi obtenemos toda la informacion para añadir a la tabla
		String[] fields = nameOfFile.Split("_");
		if (!CardExistsInCardTable( cardImage))
		{
			connection.Insert(new Card
			{
				collection = fields[0],
				number = int.Parse(fields[1]),
				rarity = fields[2],
				image=cardImage
			}) ;
		}
	}

	//Comprueba si la carta existe en la tabla Card
    private bool CardExistsInCardTable( string cardImage)
    {
		IEnumerable<Card> card = connection.Table<Card>().Where(x => x.image == cardImage);
        foreach (var item in card)
        {
			return true;
        }
		return false;
    }

	//Devuelve todas las cartas de la tabla Card
	internal IEnumerable<Card> GetCardsFromCardTable()
	{
		return connection.Table<Card>();
	}

	//Guarda una carta en la tabla PlayerCards
	internal void SaveCardOnPlayerCardsDb(Card card)
	{
		PlayerCards c = connection.Table<PlayerCards>().Where(x => x.image == card.image).FirstOrDefault();
		Debug.Log("Carta "+c);
        if (c == null)
        {
			c = new PlayerCards(card, 1);
			connection.Insert(c);
			Debug.Log(c.quantity);
		}
        else
        {
			Debug.Log("Cantidad antes: "+c.quantity);
			c.quantity++;
			Debug.Log("Cantidad despues: " + c.quantity);
			connection.Update(c);
			Debug.Log("Cantidad despues de insert: " + c.quantity);
		}
		
	}

	/*public IEnumerable<Person> GetPersons(){
		return connection.Table<Person>();
	}

	public IEnumerable<Person> GetPersonsNamedRoberto(){
		return connection.Table<Person>().Where(x => x.Name == "Roberto");
	}

	public Person GetJohnny(){
		return connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
	}

	public Person CreatePerson(){
		var p = new Person{
				Name = "Johnny",
				Surname = "Mnemonic",
				Age = 21
		};
		connection.Insert (p);
		return p;
	}*/
}
