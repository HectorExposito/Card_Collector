using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardCollectionManager : MonoBehaviour
{
    string collection;
    const int CARDS_PER_PAGE = 9;
    int page;
    int lastPage;
    [SerializeField] TMP_Text pageText;
    [SerializeField] Image[] cardsImages;
    Card[] cards;//Todas las cartas de una coleccion
    PlayerCards[] playerCards;//Todas las cartas que tiene el jugador de una coleccion
    [SerializeField] Database db;

    [SerializeField] PanelChanger panelChanger;
    
    //Colores
    Color32 notHasCardColor = new Color32(73, 73, 73, 100);
    Color32 hasCardColor = new Color32(255, 255, 255, 255);

    //Dorsos
    [SerializeField] Sprite conan_Back;
    [SerializeField] Sprite pokemon_Back;
    [SerializeField] Sprite naruto_Back;
    [SerializeField] Sprite onePiece_Back;
    Sprite active_Back;
    
    //CardInfo_Panel objetos
    [SerializeField] Image cardsInfoImage;
    [SerializeField] TMP_Text cardInfoRarity;
    [SerializeField] TMP_Text cardInfoQuantity;

    //Prepara los datos iniciales
    public void PreparePanel(string collection)
    {
        this.collection = collection;
        page = 0;
        ChangePageText();
        LoadCards();
        SetActiveBack();
        SetCardsImages();
    }

    //Establece el dorso de la carta en base a la coleccion
    private void SetActiveBack()
    {
        switch (collection)
        {
            case CardInfo.Conan:
                active_Back = conan_Back;
                break;
            case CardInfo.Naruto:
                active_Back = naruto_Back;
                break;
            case CardInfo.OnePiece:
                active_Back = onePiece_Back;
                break;
            case CardInfo.Pokemon:
                active_Back = pokemon_Back;
                break;
        }
    }

    //Carga la imagen de la carta
    private void SetCardsImages()
    {
        //Carga tantas cartas como esten definidas en la constante CARDS_PER_PAGE
        for (int i = 0; i < CARDS_PER_PAGE; i++)
        {
            //Mira si el numero de carta es menor al total de cartas que hay. Si es menor carga la imagen de la carta.
            //Si es mayor carga la parte trasera de la carta
            if (i + page * CARDS_PER_PAGE < cards.Length)
            {
                string cardPath = cards[i + page * CARDS_PER_PAGE].image.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
                cardsImages[i].sprite = Resources.Load<Sprite>(cardPath);
                //Asigna el color a la carta en base si el jugador la tiene o no
                if (!CheckIfPlayerHasCard(i))
                {
                    cardsImages[i].GetComponent<Image>().color = notHasCardColor;
                }
                else
                {
                    cardsImages[i].GetComponent<Image>().color = hasCardColor;
                }

                if (cardsImages[i].gameObject.GetComponent<Button>() == null)
                {
                    int tempVar = i;
                    cardsImages[i].gameObject.AddComponent<Button>();
                    cardsImages[i].gameObject.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        Debug.Log("cardCollection "+ tempVar.ToString());
                        panelChanger.OpenCardInfo(tempVar.ToString()); });
                }
                
            }
            else
            {
                cardsImages[i].sprite = active_Back;
                cardsImages[i].GetComponent<Image>().color = hasCardColor;
                Destroy(cardsImages[i].gameObject.GetComponent<Button>() );
            }

        }
    }

    //Se comprueba si el jugador tiene una carta
    private bool CheckIfPlayerHasCard(int i)
    {
        foreach (var card in playerCards)
        {
            if (card.image == cards[i + page * CARDS_PER_PAGE].image)
            {
                return true;
            }
        }
        return false;
    }

    //Carga todas las cartas
    private void LoadCards()
    {
        //Primero cargamos todas las cartas de la base de datos en un array
        List<Card> cardList;
        cardList = new List<Card>();
        IEnumerable<Card> c = db.GetCards(collection);
        foreach (var card in c)
        {
            cardList.Add(card);
        }
        cards = cardList.ToArray();
        lastPage = (cards.Length / CARDS_PER_PAGE);

        //Despues cargamos todas las cartas del jugador en un array
        List<PlayerCards> playerCardList = new List<PlayerCards>();
        IEnumerable<PlayerCards> pc = db.GetPlayerCards(collection);
        foreach (var card in pc)
        {
            playerCardList.Add(card);
        }
        playerCards = playerCardList.ToArray();
    }

    //Pasa de pagina
    public void NextPage()
    {
        if (page != lastPage)
        {
            page++;
            SetCardsImages();
        }
        else if (page == lastPage)
        {
            page = 0;
            SetCardsImages();
        }
        ChangePageText();
    }

    //Retrocede una pagina
    public void PreviousPage()
    {
        if (page != 0)
        {
            page--;
            SetCardsImages();
        }
        else if (page == 0)
        {
            page = lastPage;
            SetCardsImages();
        }
        ChangePageText();
    }

    //Cambia el texto donde aparece el numero de la pagina
    private void ChangePageText()
    {
        int num = page + 1;
        pageText.text = "PAGINA " + num;
    }

    //Carga los datos de una carta en el panel de informacion
    public void CardInfoPanelSetData(int cardNum)
    {
        Debug.Log("cardinfo "+cardNum);
        //Primero se selecciona la carta
        Card c = cards[cardNum + page * CARDS_PER_PAGE];
        //Se carga la imagen de la carta
        string cardPath = c.image.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
        cardsInfoImage.sprite = Resources.Load<Sprite>(cardPath);
        //En base a si el jugador tiene la carta, se le asigna un color y la cantidad de cartas que tiene
        if (!CheckIfPlayerHasCard(cardNum))
        {
            cardsInfoImage.GetComponent<Image>().color = notHasCardColor;
            cardInfoQuantity.text = "CANTIDAD: 0";
        }
        else
        {
            cardsInfoImage.GetComponent<Image>().color = hasCardColor;
            cardInfoQuantity.text = "CANTIDAD: " + GetCardQuantity(c);
        }
        SetCardInfoRarityText(c.rarity);

    }

    //Devuelve la cantidad de cartas que tiene
    private int GetCardQuantity(Card c)
    {
        foreach (var card in playerCards)
        {
            if (card.image == c.image)
            {
                return card.quantity;
            }
        }
        return 0;
    }

    //Escribe en el texto de rareza cual es la rareza de esa carta y lo hace en el color correspondiente
    private void SetCardInfoRarityText(string rarity)
    {
        switch (rarity)
        {
            case CardInfo.common:
                cardInfoRarity.text = "COMUN";
                cardInfoRarity.color = Color.white;
                break;
            case CardInfo.uncommon:
                cardInfoRarity.text = "POCO COMUN";
                cardInfoRarity.color = Color.green;
                break;
            case CardInfo.rare:
                cardInfoRarity.text = "RARA";
                cardInfoRarity.color = Color.blue;
                break;
            case CardInfo.ultrarare:
                cardInfoRarity.text = "ULTRA RARA";
                cardInfoRarity.color = new Color32(147, 112, 219, 255);
                break;
        }
    }
}
