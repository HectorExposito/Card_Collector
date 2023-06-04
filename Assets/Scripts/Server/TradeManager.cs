using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class TradeManager : MonoBehaviourPun
{
    private const int SEND_NAME = 0;
    private const int ADD_CARD_TO_LIST = 1;
    private const int REMOVE_CARD_FROM_LIST = 2;
    private const int READY_TO_TRADE = 3;

    IEnumerable<PlayerCards> pc;
    [SerializeField] Database db;

    [SerializeField] GameObject card;
    [SerializeField] GameObject friendCardImage;
    [SerializeField] GameObject addCardButton;
    [SerializeField] GameObject selectedCardsPanel;
    [SerializeField] GameObject friendCardsPanel;
    [SerializeField] GameObject cardSelectionCanvas;
    [SerializeField] GameObject cardSelectionPanel;
    [SerializeField] GameObject cardToSelect;
    [SerializeField] TMP_Text tradeButtonText;
    [SerializeField] Button tradeButton;
    [SerializeField] TMP_Text friendsNameText;
    [SerializeField] TMP_Text playerPointsText;
    [SerializeField] private ErrorMessage errorMesage;

    PlayerCards[] cardsSelected;
    Card[] friendCards;

    const int COMMON_PLAYER_VALUE = 1;
    const int UNCOMMON_PLAYER_VALUE = 3;
    const int RARE_PLAYER_VALUE = 5;
    const int ULTRARARE_PLAYER_VALUE = 15;

    bool readyToTrade;
    bool friendReadyToTrade;

    //Dropdowns
    [SerializeField] TMP_Dropdown collectionsDropdown;
    [SerializeField] TMP_Dropdown rarityDropdown;

    int numberOfPlayers;
    string friendsName;

    private void Start()
    {
        cardsSelected = new PlayerCards[ULTRARARE_PLAYER_VALUE];
        friendCards = new Card[ULTRARARE_PLAYER_VALUE];
        collectionsDropdown.onValueChanged.AddListener(delegate { ReloadCards(); });
        rarityDropdown.onValueChanged.AddListener(delegate { ReloadCards(); });
        SetPlayerPointsText();
    }

    private void Update()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount==1 && PhotonNetwork.CurrentRoom.PlayerCount != numberOfPlayers)
        {
            numberOfPlayers = 1;
            friendsName = "";
            SetFriendPointsText();
            CleanFriendCards();
            CleanSelectedCards();
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.CurrentRoom.PlayerCount != numberOfPlayers)
        {
            numberOfPlayers = 2;
            object[] datas = new object[] { PlayerPrefs.GetString("Name") };
            PhotonNetwork.RaiseEvent(SEND_NAME, datas, null, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(ExitGames.Client.Photon.EventData obj)
    {
        Debug.Log("Mensaje");
        object[] datas = (object[])obj.CustomData;
        switch (obj.Code)
        {
            case SEND_NAME:
                friendsName = (string)datas[0];
                SetFriendPointsText();
                break;
            case ADD_CARD_TO_LIST:
                Card friendCard = GetCard((string)datas[0]);
                AddFriendCard(friendCard);
                readyToTrade = false;
                friendReadyToTrade = false;
                break;
            case REMOVE_CARD_FROM_LIST:
                RemoveFriendCard((string)datas[0]);
                readyToTrade = false;
                friendReadyToTrade = false;
                break;
            case READY_TO_TRADE:
                friendReadyToTrade = true;
                if (readyToTrade)
                {
                    Trade();
                }
                break;
        }
        ChangeTradeButton();
    }

    private void Trade()
    {
        for (int i = 0; i < friendCards.Length; i++)
        {
            if (friendCards[i] != null)
            {
                db.SaveCardOnPlayerCardsDb(friendCards[i]);
            }
        }

        for (int i = 0; i < cardsSelected.Length; i++)
        {
            if (cardsSelected[i] != null)
            {
                db.RemoveCardFromPlayerCards(cardsSelected[i]);
            }
        }
        CleanSelectedCards();
        CleanFriendCards();
        readyToTrade = false;
        friendReadyToTrade = false;
        ChangeTradeButton();
        FindObjectOfType<QuestManager>().UpdateQuests(Quest.QuestType.TRADE);
    }

    private void CleanFriendCards()
    {
        for (int i = 0; i < friendCards.Length; i++)
        {
            friendCards[i] = null;
        }

        for (int i = 0; i < friendCardsPanel.transform.childCount; i++)
        {
            Transform aux = friendCardsPanel.transform.GetChild(i);
            if (aux.gameObject.activeSelf)
            {
                Destroy(aux.gameObject);
            }
        }

        SetFriendPointsText();
    }

    private void RemoveFriendCard(string friendCardImage)
    {
        for (int i = 0; i < friendCards.Length; i++)
        {
            if (friendCards[i]!=null)
            {
                if (friendCards[i].image == friendCardImage)
                {
                    friendCards[i] = null;
                    RemoveFriendCardImage(friendCardImage);
                    SetFriendPointsText();
                    return;
                }
            }
            
        }
    }

    private void RemoveFriendCardImage(string friendCardImage)
    {
        string cardPath = friendCardImage.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
        for (int i = 0; i < friendCardsPanel.transform.childCount; i++)
        {
            Debug.Log(cardPath+" "+ friendCardsPanel.transform.GetChild(i).GetComponentInChildren<Image>().sprite);
            if(friendCardsPanel.transform.GetChild(i).GetComponentInChildren<Image>().sprite== Resources.Load<Sprite>(cardPath))
            {
                Destroy(friendCardsPanel.transform.GetChild(i).gameObject);
                return;
            }
        }
    }

    private Card GetCard(string cardImage)
    {
        return db.GetCard(cardImage);
    }

    private void AddFriendCard(Card friendCard)
    {
        Debug.Log(friendCard.image);
        for (int i = 0; i < friendCards.Length; i++)
        {
            if (friendCards[i] == null)
            {
                //Guardamos la carta en el array de cartas seleccionadas
                friendCards[i] = friendCard;
                /*
                 * Preparamos el panel que muestra las cartas seleccionadas
                 * Primero se desactiva el boton de añadir carta y se ponne null su padre. Esto se hace para que al poner otra 
                 * vez que su padre es el panel de cartas seleccionadas, aparezca siempre al final.
                 * Despues se crea una carta a la que se le pone de imagen la carta que acabamos de seleccionar.
                 * Finalmente se vuelve a poner el panel de cartas seleccionadas como padre del botón de añadir cartas y este
                 * se vuelve a activar.
                 */

                GameObject cardCopy = Instantiate(friendCardImage);
                cardCopy.transform.SetParent(friendCardsPanel.transform);
                cardCopy.transform.localScale = Vector3.one;
                string cardPath = friendCard.image.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
                cardCopy.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(cardPath);
                cardCopy.SetActive(true);

                SetFriendPointsText();
                return;
            }
        }
    }

    private void SetFriendPointsText()
    {
        friendsNameText.text = "CARTAS DE " + friendsName + " (" + GetFriendPoints() + ")";
    }

    private void SetPlayerPointsText()
    {
        playerPointsText.text = "TUS CARTAS (" + GetPlayerPoints() + ")";
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    public void AddCard(PlayerCards playerCard)
    {
        for (int i = 0; i < cardsSelected.Length; i++)
        {

            if (cardsSelected[i] == null)
            {
                //Guardamos la carta en el array de cartas seleccionadas
                cardsSelected[i] = playerCard;
                /*
                 * Preparamos el panel que muestra las cartas seleccionadas
                 * Primero se desactiva el boton de añadir carta y se ponne null su padre. Esto se hace para que al poner otra 
                 * vez que su padre es el panel de cartas seleccionadas, aparezca siempre al final.
                 * Despues se crea una carta a la que se le pone de imagen la carta que acabamos de seleccionar.
                 * Finalmente se vuelve a poner el panel de cartas seleccionadas como padre del botón de añadir cartas y este
                 * se vuelve a activar.
                 */
                //addCardButton.SetActive(false);
                //addCardButton.transform.SetParent(null);

                GameObject cardCopy = Instantiate(card);
                cardCopy.transform.SetParent(selectedCardsPanel.transform);
                cardCopy.transform.localScale = Vector3.one;
                string cardPath = playerCard.image.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
                cardCopy.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(cardPath);
                cardCopy.SetActive(true);
                cardCopy.GetComponentInChildren<Button>().onClick.AddListener(delegate () {
                    RemoveCard(playerCard);


                    Destroy(cardCopy);
                });
                SetPlayerPointsText();
                //addCardButton.transform.SetParent(selectedCardsPanel.transform);
                //addCardButton.SetActive(true);
                //Quitamos el panel de seleccion de cartas
                cardSelectionCanvas.SetActive(false);

                Debug.Log("Yo añado carta");
                object[] datas = new object[] { playerCard.image };
                PhotonNetwork.RaiseEvent(ADD_CARD_TO_LIST, datas, null, ExitGames.Client.Photon.SendOptions.SendReliable);

                return;
            }
        }

        
    }

    public void LoadCardsToSelect()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount==2)
        {
            int count = 0;
            pc = db.GetAllPlayerCards();
            foreach (PlayerCards playerCard in pc)
            {
                if (CheckIfItCanBeSelected(playerCard))
                {
                    GenerateCards(playerCard);
                    count++;
                }
            }

            cardSelectionCanvas.SetActive(true);
        }
        else
        {
            errorMesage.ShowErrorMessage("NO PUEDES AÑADIR CARTAS.\nNo puedes añadir cartas hasta que" +
                " no haya alguien en la sala contigo.");
        }
    }

    private bool CheckIfItCanBeSelected(PlayerCards playerCard)
    {
        int count = 0;
        /*
         * Se mira primero si la cantidad de esa carta es mayor que 1, en caso positivo se comprueba si hay mas cartas de 
         * ese tipo seleccionadas, en caso negativo esa carta no se puede seleccionar y se devuelve false
         */
        if (playerCard.quantity > 1)
        {
            for (int i = 0; i < cardsSelected.Length; i++)
            {
                if (cardsSelected[i] != null)
                {
                    if (cardsSelected[i].image.Equals(playerCard.image))
                    {
                        count++;
                    }
                }
            }
            /*
             * Si la cantidad de cartas que tiene el jugador de un tipo menos las que ya ha seleccionado de ese tipo es mayor
             * que 1 la carta es seleccionable. Si es menor se devuelve false.
             */
            if (playerCard.quantity - count > 1)
            {
                return true;
            }
        }
        return false;
    }

    private void GenerateCards(PlayerCards playerCard)
    {
        string cardPath = playerCard.image.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
        GameObject cardCopy = Instantiate(cardToSelect);
        cardCopy.GetComponent<Image>().sprite = Resources.Load<Sprite>(cardPath);
        cardCopy.transform.SetParent(cardSelectionPanel.transform);
        cardCopy.transform.localScale = Vector3.one;
        cardCopy.GetComponent<Button>().onClick.AddListener(delegate () {
            AddCard(playerCard);
            EraseCards();
        });
        cardCopy.SetActive(true);
    }

    private void EraseCards()
    {
        for (int i = 0; i < cardSelectionPanel.transform.childCount; i++)
        {
            Transform aux = cardSelectionPanel.transform.GetChild(i);
            if (aux.gameObject.activeSelf)
            {
                Destroy(aux.gameObject);
            }
        }
    }

    private void RemoveCard(PlayerCards playerCard)
    {
        object[] datas = new object[] { playerCard.image };
        PhotonNetwork.RaiseEvent(REMOVE_CARD_FROM_LIST, datas, null, ExitGames.Client.Photon.SendOptions.SendReliable);

        for (int i = 0; i < cardsSelected.Length; i++)
        {
            if (cardsSelected[i] != null)
            {
                if (cardsSelected[i].image == playerCard.image)
                {
                    cardsSelected[i] = null;
                    SetPlayerPointsText();
                    return;
                }
            }
        }
    }

    private int CountCardsSelected()
    {
        int count = 0;
        for (int i = 0; i < cardsSelected.Length; i++)
        {
            if (cardsSelected[i] != null)
            {
                count++;
            }
        }
        return count;
    }

    public void TradeButton()
    {
        int yourPoints = GetPlayerPoints();
        int friendPoints = GetFriendPoints();

        if (yourPoints == friendPoints)
        {
            readyToTrade = true;
            PhotonNetwork.RaiseEvent(READY_TO_TRADE, null, null, ExitGames.Client.Photon.SendOptions.SendReliable);
            if (friendReadyToTrade)
            {
                Trade();
            }
            else
            {
                ChangeTradeButton();
            }
        }
        else
        {
            readyToTrade = false;
            ChangeTradeButton();
        }
    }

    private void ChangeTradeButton()
    {
        if (readyToTrade)
        {
            tradeButtonText.text = "ESPERANDO A ";
            tradeButton.enabled = false;
        }
        else
        {
            tradeButtonText.text = "INTERCAMBIAR";
            tradeButton.enabled = true;
        }
    }
    private int GetFriendPoints()
    {
        int count = 0;
        for (int i = 0; i < friendCards.Length; i++)
        {
            if (friendCards[i] != null)
            {
                switch (friendCards[i].rarity)
                {
                    case CardInfo.common:
                        count += COMMON_PLAYER_VALUE;
                        break;
                    case CardInfo.uncommon:
                        count += UNCOMMON_PLAYER_VALUE;
                        break;
                    case CardInfo.rare:
                        count += RARE_PLAYER_VALUE;
                        break;
                    case CardInfo.ultrarare:
                        count += ULTRARARE_PLAYER_VALUE;
                        break;
                }
            }
        }
        return count;
    }

    private int GetPlayerPoints()
    {
        int count = 0;
        for (int i = 0; i < cardsSelected.Length; i++)
        {
            if (cardsSelected[i] != null)
            {
                switch (cardsSelected[i].rarity)
                {
                    case CardInfo.common:
                        count += COMMON_PLAYER_VALUE;
                        break;
                    case CardInfo.uncommon:
                        count += UNCOMMON_PLAYER_VALUE;
                        break;
                    case CardInfo.rare:
                        count += RARE_PLAYER_VALUE;
                        break;
                    case CardInfo.ultrarare:
                        count += ULTRARARE_PLAYER_VALUE;
                        break;
                }
            }
        }
        return count;
    }

    public void CleanSelectedCards()
    {
        for (int i = 0; i < cardsSelected.Length; i++)
        {
            cardsSelected[i] = null;
        }

        addCardButton.SetActive(false);
        for (int i = 0; i < selectedCardsPanel.transform.childCount; i++)
        {
            Transform aux = selectedCardsPanel.transform.GetChild(i);
            if (aux.gameObject.activeSelf)
            {
                Destroy(aux.gameObject);
            }
        }
        addCardButton.SetActive(true);
        SetPlayerPointsText();
    }

    public void ReloadCards()
    {
        EraseCards();
        int collectionValue = collectionsDropdown.value;
        string collection;
        int rarityValue = rarityDropdown.value;
        string rarity;

        switch (collectionValue)
        {
            case 1:
                collection = CardInfo.Conan;
                break;
            case 2:
                collection = CardInfo.Naruto;
                break;
            case 3:
                collection = CardInfo.Pokemon;
                break;
            case 4:
                collection = CardInfo.OnePiece;
                break;
            default:
                collection = null;
                break;
        }

        switch (rarityValue)
        {
            case 1:
                rarity = CardInfo.common;
                break;
            case 2:
                rarity = CardInfo.uncommon;
                break;
            case 3:
                rarity = CardInfo.rare;
                break;
            case 4:
                rarity = CardInfo.ultrarare;
                break;
            default:
                rarity = null;
                break;
        }
        Debug.Log(collection + " " + rarity);
        if (rarity == null && collection == null)
        {
            LoadCardsToSelect();
        }
        else
        {
            LoadCardsToSelect(collection, rarity);
        }
    }

    public void LoadCardsToSelect(string collection, string rarity)
    {
        int count = 0;
        pc = db.GetPlayerCards(collection, rarity);
        foreach (PlayerCards playerCard in pc)
        {
            if (CheckIfItCanBeSelected(playerCard))
            {
                GenerateCards(playerCard);
                count++;
            }
        }

        cardSelectionCanvas.SetActive(true);

    }
}
