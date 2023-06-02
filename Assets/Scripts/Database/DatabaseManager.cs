
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;
using SQLite4Unity3d;

public class DatabaseManager : MonoBehaviour
{
    private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    private IDataReader reader;
    public InputField t_name, t_Address, t_id;
    public Text data_staff;

    string DatabaseName = "Cards.s3db";
    //Borrar
    public GameObject panel;
    public Image imagen;
    public Text texto;
    void Start()
    {
        string filepath = Application.persistentDataPath + "/" + DatabaseName;
        if (!File.Exists(filepath))
        {

            Debug.LogWarning("File \"" + filepath + "\" does not exist. Attempting to create from \"" +
                             Application.dataPath + "!/assets/Cards");
            texto.text = "File \"" + filepath + "\" does not exist. Attempting to create from \"" +
                             Application.dataPath + "!/assets/Cards";
            WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/StreamingAssets/"+DatabaseName);
            while (!loadDB.isDone) { }
            //Application.persistentDataPath.
           // File.WriteAllBytes(filepath, loadDB.bytes);

        }

        conn = "URI=file:" + filepath;

        Debug.Log("Stablishing connection to: " + conn);
       // dbconn = new SQLiteConnection(conn);
        dbconn.Open();
        
        string query;
        //Se crea la tabla cartas en caso de que no exista
        query = "CREATE TABLE IF NOT EXISTS cards " +
            "(collection VARCHAR(7)," +
            "number INTEGER," +
            "rarity VARCHAR(2)," +
            "image BLOB," +
            "CONSTRAINT cards_pk PRIMARY KEY(collection,number))";
        try
        {
            dbcmd = dbconn.CreateCommand(); 
            dbcmd.CommandText = query; 
            reader = dbcmd.ExecuteReader();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            texto.text = e.Message;
        }
        query = "CREATE TABLE IF NOT EXISTS userCards " +
            "(collection VARCHAR(7)," +
            " number INTEGER," +
            "rarity VARCHAR(15)," +
            "image BLOB," +
            "CONSTRAINT userCards_pk PRIMARY KEY(collection,number)," +
            "CONSTRAINT coll_num_fk FOREIGN KEY (collection,number) references cards(collection,number))";
        try
        {
            dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();
        }
        catch (Exception e)
        {

            Debug.Log(e);
            texto.text = e.Message;
        }
        FillCardsDb();
        ShowCards();
    }

    private void ShowCards()
    {
        if (dbconn.State == ConnectionState.Closed)
        {
            dbconn.Open();
        }
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT  * FROM cards";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        ///////////////////////////////////////////////////////////////////////
        
        while (reader.Read())
        {
            Image im = Instantiate(imagen);
            
            //Debug.Log(reader.GetValue(0)+"  "+ reader.GetValue(1) +"  "+reader.GetValue(2) +"  "+reader.GetString(3));
            String path = reader.GetString(3).Replace(".jpg", "").Replace(".\\Assets\\Resources\\","");
            texto.text += path;
            im.sprite = Resources.Load<Sprite>(path);
            im.transform.SetParent(panel.transform);
        }
        Destroy(imagen);
        //////////////////////////////////////////////////////////////////////////
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
    }

    public IDataReader GetCards(string collection)
    {
        if (dbconn.State == ConnectionState.Closed)
        {
            dbconn.Open();
        }
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT  * FROM cards WHERE collection LIKE "+collection;
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        return reader;
    }

    private void FillCardsDb()
    {
        int numberOfCollections;
        int numberOfCards;
        String[] directories=Directory.GetDirectories(".\\Assets\\Resources\\Cartas");
        for (int i = 0; i < directories.Length; i++)
        {
            numberOfCards = Directory.GetFiles(directories[i]).Length;
            String[] cardImages = Directory.GetFiles(directories[i]);
            for (int j=0;j< numberOfCards; j++)
            {
                if (cardImages[j].Split('.').Length == 3)
                {
                    String[] cardPathSplited = cardImages[j].Split('.');
                    String nameOfFile = cardPathSplited[1].Split("\\")[cardPathSplited[1].Split("\\").Length-1];
                    Debug.Log(nameOfFile+" "+ cardImages[j]);
                    texto.text += "carta add ";
                    InsertCardInCardsDb(nameOfFile, cardImages[j]);
                }
            }
        }
    }

    private void InsertCardInCardsDb(string nameOfFile,string cardImage)
    {
        String[] fields = nameOfFile.Split("_");
        
        if (!CardExists(fields,cardImage))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = string.Format("insert into Cards (collection,number,rarity,image)" +
            " values (\"{0}\",\"{1}\",\"{2}\",\"{3}\")", fields[0], int.Parse(fields[1]), fields[2], cardImage);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
        }
        dbconn.Close();
    }

    private bool CardExists(string[] nameOfFile,string cardImage)
    {
        if (dbconn.State == ConnectionState.Closed)
        {
            dbconn.Open();
        }
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT collection,number FROM cards WHERE collection='" + nameOfFile[0]+"' and number="+ int.Parse(nameOfFile[1]);
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        if (reader.FieldCount == 2)
        {
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            Debug.Log("True");
            return true;
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        Debug.Log("false");
        return false;
    }
}
