using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    Card[] commonCards;
    Card[] uncommonCards;
    Card[] rareCards;
    Card[] ultrarareCards;

    Card[] cardsToReturn;
    [SerializeField] Database db;
    [SerializeField] GameObject buyPanel;
    [SerializeField] TMP_Text contentText;

    string collection;
    [SerializeField] PackOpener packOpener;
    int packType;

    [SerializeField] Image[] packImages;
    [SerializeField] Sprite[] conanImages;
    [SerializeField] Sprite[] narutoImages;
    [SerializeField] Sprite[] onePieceImages;
    [SerializeField] Sprite[] pokemonImages;
    [SerializeField] private ErrorMessage errorMesage;

    //Carga las cartas que se van a usar
    public void LoadCards(string collection)
    {
        //Se guarda el nombre de la coleccion y se inicia el numero de cartas de cada tipo a 0
        this.collection = collection;
        int cCards = 0;
        int ucCards = 0;
        int rCards = 0;
        int urCards = 0;

        //Obtenemos todas las cartas de la coleccion de la base de datos y recorriendo
        //el IEnumerable de cartas aumentamos la cantidad de cartas de cada rareza.
        IEnumerable<Card> c = db.GetCards(collection);
        foreach (Card card in c)
        {
            switch (card.rarity)
            {
                case CardInfo.common:
                    cCards++;
                    break;
                case CardInfo.uncommon:
                    ucCards++;
                    break;
                case CardInfo.rare:
                    rCards++;
                    break;
                case CardInfo.ultrarare:
                    urCards++;
                    break;
            }
        }

        //Se crean los arrays de cartas de cada tipo con el tamaño que hemos obtenido antes
        commonCards = new Card[cCards];
        uncommonCards = new Card[ucCards];
        rareCards = new Card[rCards];
        ultrarareCards = new Card[urCards];

        //Se vuelven a poner a 0 las cantidades de cartas de cada tipo
        cCards = 0;
        ucCards = 0;
        rCards = 0;
        urCards = 0;

        //Se recorre otra vez el IEnumerable de cartas y en base a su rareza se guardan esas cartas
        //en su array correspondiente
        foreach (Card card in c)
        {
            switch (card.rarity)
            {
                case CardInfo.common:
                    commonCards[cCards] = card;
                    cCards++;
                    break;
                case CardInfo.uncommon:
                    uncommonCards[ucCards] = card;
                    ucCards++;
                    break;
                case CardInfo.rare:
                    rareCards[rCards] = card;
                    rCards++;
                    break;
                case CardInfo.ultrarare:
                    ultrarareCards[urCards] = card;
                    urCards++;
                    break;

            }
        }
    }

    //Activa el panel de compra de paquetes
    public void OpenBuyPanel(string pack)
    {
        //Se guarda el numero del tipo de paquete
        int p = int.Parse(pack);
        this.packType = p;

        //Se guarda en el contentText la información correspondiente al paquete
        SetContentForContentText(p);

        //Se activa el panel de compra de paquetes
        buyPanel.SetActive(true);
    }

    //Según el tipo de paquete que es se escribe en el contentText la descripción correspondiente
    private void SetContentForContentText(int pack)
    {
        
        switch (pack)
        {
            case 0:
                contentText.text="";
                break;
            case 1:
                contentText.text = "TOTAL DE CARTAS:\n4\nPROBABILIDADES DE CARTAS:\nCARTAS COMUNES: 80%\n" +
                    "CARTAS POCO COMUNES: 18%\nCARTAS RARAS: 1'9%\nCARTAS ULTRA RARAS: 0'1%";
                break;
            case 2:
                contentText.text = "TOTAL DE CARTAS:\n6\nPROBABILIDADES DE CARTAS:\nCARTAS COMUNES: 75%\n" +
                    "CARTAS POCO COMUNES: 20%\nCARTAS RARAS: 4'8%\nCARTAS ULTRA RARAS: 0'2%";
                break;
            case 3:
                contentText.text = "TOTAL DE CARTAS:\n8\nPROBABILIDADES DE CARTAS:\nCARTAS COMUNES: 70%\n" +
                    "CARTAS POCO COMUNES: 22%\nCARTAS RARAS: 7'5%\nCARTAS ULTRA RARAS: 0'5%";
                break;
            case 4:
                contentText.text = "TOTAL DE CARTAS:\n10\nPROBABILIDADES DE CARTAS:\nCARTAS COMUNES: 65%\n" +
                    "CARTAS POCO COMUNES: 27'5%\nCARTAS RARAS: 6'5%\nCARTAS ULTRA RARAS: 1%";
                break;
        }
    }

    //Desactiva el panel de compra de paquetes
    public void CloseBuyPanel()
    {
        buyPanel.SetActive(false);
    }

    //Realiza el proceso de compra de un paquete
    public void BuyPack()
    {
        //Guarda el precio del paquete
        int price = PackPrice();

        //Comprueba que el jugador tiene monedas suficientes, en caso negativo lo avisa,
        //en caso positivo realiza la compra
        if (PlayerPrefs.GetInt("Coins") >= price)
        {
            //Reduce la cantidad de monedas del jugador.
            PlayerPrefs.SetInt("Coins",PlayerPrefs.GetInt("Coins")-price);

            //Se inicializa el numero de cartas totales y rarezas a 0
            int common = 0;
            int uncommon = 0;
            int rare = 0;
            int ultrarare = 0;
            int numOfCards = 0;
            int rand = 0;

            //Se inicializa las probabilidades de las cartas a 0
            System.Random r = new System.Random();
            int commonProbability=0;
            int uncommonProbability = 0;
            int rareProbability = 0;

            //En base al tipo de sobre que se va a comprar se asigna la probabilidad a cada tipo de carta
            switch (packType)
            {
                case 1:
                    commonProbability=800;
                    uncommonProbability=980;
                    rareProbability=999;
                    numOfCards = 4;
                    break;
                case 2:
                    commonProbability = 750;
                    uncommonProbability = 950;
                    rareProbability = 998;
                    numOfCards = 6;
                    break;
                case 3:
                    commonProbability = 700;
                    uncommonProbability = 920;
                    rareProbability = 995;
                    numOfCards = 8;
                    break;
                case 4:
                    commonProbability = 650;
                    uncommonProbability = 925;
                    rareProbability = 990;
                    numOfCards = 10;
                    break;
            }

            //Se generá un numero del 0 al 100 para ver que carta extra sale.
            for (int i = 0; i < numOfCards; i++)
            {
                rand = r.Next(0, 1001);
                if (rand <= commonProbability)
                {
                    common++;
                }
                else if (rand > commonProbability && rand <= uncommonProbability)
                {
                    uncommon++;
                }
                else if (rand > uncommonProbability && rand <= rareProbability)
                {
                    rare++;
                }
                else
                {
                    ultrarare++;
                }
            }
            

            numOfCards = common + uncommon + rare + ultrarare;
            
            //En base a las cartas que habra en el sobre se obtienen las cartas finales del sobre
            Card[] cards = GetCards(numOfCards, common, uncommon, rare, ultrarare);

            //Se guardan las cartas en la base de datos, en la tabla PlayerCards
            SaveCardsOnDataBase(cards);

            packOpener.GetComponent<PackOpener>().PrepareCards(collection, cards);

            FindObjectOfType<QuestManager>().UpdateQuests(Quest.QuestType.PACK);
        }
        else
        {
            errorMesage.ShowErrorMessage("NO TIENES MONEDAS SUFICIENTES.\nTe faltan "+(price - PlayerPrefs.GetInt("Coins")) +" monedas");
        }
        
    }

    private void SaveCardsOnDataBase(Card[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            db.SaveCardOnPlayerCardsDb(cards[i]);
        }
    }

    private int PackPrice()
    {
        switch (packType)
        {
            case 1:
                return 200;
            case 2:
                return 400;
            case 3:
                return 600;
            case 4:
                return 800;
        }
        return 0;
    }

    public Card[] GetCards(int numberOfCards,int numberOfCommon, int numberOfUncommon,int numberOfRare, int numberOfUltrarare)
    {
        
        cardsToReturn = new Card[numberOfCards];
        int cardsPicked = 0;
        System.Random r = new System.Random();
        List<int> numbers = new List<int>();
        //We select the cards in order of rarity, starting in common and ending in ultrarare
        
        for (int i = 0; i < numberOfCommon; i++)
        {
            int num;
            do
            {
                num = r.Next(0, commonCards.Length);
            } while (numbers.Contains(num));
            numbers.Add(num);
            cardsToReturn[cardsPicked] = commonCards[num];
            cardsPicked++;
        }
        
        if (numberOfUncommon > 0)
        {
            for (int i = 0; i < numberOfUncommon; i++)
            {
                int num;
                do
                {
                    num = r.Next(0, uncommonCards.Length);
                } while (numbers.Contains(num));
                numbers.Add(num);
                cardsToReturn[cardsPicked] = uncommonCards[num];
                cardsPicked++;
            }
        }
        
        if (numberOfRare > 0)
        {
            for (int i = 0; i < numberOfRare; i++)
            {
                int num;
                do
                {
                    num = r.Next(0, rareCards.Length);
                } while (numbers.Contains(num));
                numbers.Add(num);
                cardsToReturn[cardsPicked] = rareCards[num];
                cardsPicked++;

                FindObjectOfType<QuestManager>().UpdateQuests(Quest.QuestType.CARD);
            }
        }
        
        if (numberOfUltrarare>0)
        {
            for (int i = 0; i < numberOfUltrarare; i++)
            {
                int num;
                do
                {
                    num = r.Next(0, ultrarareCards.Length);
                } while (numbers.Contains(num));
                numbers.Add(num);
                cardsToReturn[cardsPicked] = ultrarareCards[num];
                cardsPicked++;

                FindObjectOfType<QuestManager>().UpdateQuests(Quest.QuestType.CARD);
            }
        }

        
        return cardsToReturn;
    }
    
    public void PreparePanel(string collection)
    {
        this.collection = collection;
        SetPackImages();
        LoadCards();
    }

    private void SetPackImages()
    {
        for (int i = 0; i < packImages.Length; i++)
        {
            switch (collection)
            {
                case CardInfo.Conan:
                    packImages[i].sprite = conanImages[i];
                    break;
                case CardInfo.Naruto:
                    packImages[i].sprite = narutoImages[i];
                    break;
                case CardInfo.OnePiece:
                    packImages[i].sprite = onePieceImages[i];
                    break;
                case CardInfo.Pokemon:
                    packImages[i].sprite = pokemonImages[i];
                    break;
            }
        }
    }

    private void LoadCards()
    {
        //Se inicia el numero de cartas de cada tipo a 0
        int cCards = 0;
        int ucCards = 0;
        int rCards = 0;
        int urCards = 0;

        //Obtenemos todas las cartas de la coleccion de la base de datos y recorriendo
        //el IEnumerable de cartas aumentamos la cantidad de cartas de cada rareza.
        IEnumerable<Card> c = db.GetCards(collection);
        foreach (Card card in c)
        {
            switch (card.rarity)
            {
                case CardInfo.common:
                    cCards++;
                    break;
                case CardInfo.uncommon:
                    ucCards++;
                    break;
                case CardInfo.rare:
                    rCards++;
                    break;
                case CardInfo.ultrarare:
                    urCards++;
                    break;
            }
        }

        //Se crean los arrays de cartas de cada tipo con el tamaño que hemos obtenido antes
        commonCards = new Card[cCards];
        uncommonCards = new Card[ucCards];
        rareCards = new Card[rCards];
        ultrarareCards = new Card[urCards];

        //Se vuelven a poner a 0 las cantidades de cartas de cada tipo
        cCards = 0;
        ucCards = 0;
        rCards = 0;
        urCards = 0;

        //Se recorre otra vez el IEnumerable de cartas y en base a su rareza se guardan esas cartas
        //en su array correspondiente
        foreach (Card card in c)
        {
            switch (card.rarity)
            {
                case CardInfo.common:
                    commonCards[cCards] = card;
                    cCards++;
                    break;
                case CardInfo.uncommon:
                    uncommonCards[ucCards] = card;
                    ucCards++;
                    break;
                case CardInfo.rare:
                    rareCards[rCards] = card;
                    rCards++;
                    break;
                case CardInfo.ultrarare:
                    ultrarareCards[urCards] = card;
                    urCards++;
                    break;

            }
        }
    }
}
