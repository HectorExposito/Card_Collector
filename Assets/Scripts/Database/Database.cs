using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Database : MonoBehaviour
{
    public GameObject panel;
    DataService ds;
    private void Start()
    {
        StartSync();
    }
    private void StartSync()
    {
        ds = new DataService("cards.db");
        if (!PlayerPrefs.HasKey("AlreadyPlayed"))
        {
            PlayerPrefs.SetInt("AlreadyPlayed", 1);
            ds.DeletePlayerCards();
            ds.CreateDB();
            IEnumerable<Card> cards = ds.GetCardsFromCardTable();
        }
        
    }

    //Devuelve todas las cartas de una coleccion
    public IEnumerable<Card> GetCards(string collection)
    {
        return ds.GetCardsFromCardTable(collection);
    }

    //Devuelve todas las cartas que tiene el usuario de una coleccion
    public IEnumerable<PlayerCards> GetPlayerCards(string collection)
    {
        return ds.GetCardsFromPlayerCardsTable(collection);
    }

    //Devuelve todas las cartas que tiene el usuario
    public IEnumerable<PlayerCards> GetAllPlayerCards()
    {
        return ds.GetAllCardsFromPlayerCardsTable();
    }

    //Guarda una carta en la tabla PlayerCards
    internal void SaveCardOnPlayerCardsDb(Card card)
    {
        ds.SaveCardOnPlayerCardsDb(card);
    }
    
    //Obtiene las cartas de una coleccion y rareza especificas
    internal IEnumerable<Card> GetCards(string collection, string rarity)
    {
        return ds.GetCardsFromCardTable(collection,rarity);
    }

    //Borra una carta de la tabla PlayerCards
    internal void RemoveCardFromPlayerCards(PlayerCards playerCards)
    {
        ds.RemoveCardFromPlayerCards(playerCards);
    }

    internal Card GetCard(string cardImage)
    {
        return ds.GetCard(cardImage);
    }

    internal IEnumerable<PlayerCards> GetPlayerCards(string collection, string rarity)
    {
        return ds.GetCardsFromPlayerCardsTable(collection,rarity);
    }

    internal bool HasPlayerCard(string image)
    {
        return ds.HasPlayerCard(image);
    }
}
