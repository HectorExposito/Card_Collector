
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FusionManager : MonoBehaviour
{
    [SerializeField] GameObject card;
    [SerializeField] GameObject addCardButton;
    [SerializeField] GameObject selectedCardsPanel;
    [SerializeField] GameObject cardSelectionCanvas;
    [SerializeField] GameObject cardSelectionPanel;
    [SerializeField] GameObject cardToSelect;
    [SerializeField] GameObject fusionCard;
    [SerializeField] GameObject fusionCardPanel;
    [SerializeField] Image arrow;

    [SerializeField] private ErrorMessage errorMesage;
    float selectedCardOriginalHeight;

    [SerializeField] TMP_Text playerPoints;

    PlayerCards[] cardsSelected;
    Card cardObtained;
    IEnumerable<PlayerCards> pc;

    string collection;
    [SerializeField] Database db;

    //Valor de las cartas del jugador
    const int COMMON_PLAYER_VALUE=1;
    const int UNCOMMON_PLAYER_VALUE = 3;
    const int RARE_PLAYER_VALUE = 5;
    const int ULTRARARE_PLAYER_VALUE = 15;

    //Valor de las cartas
    const int COMMON_VALUE = 3;
    const int UNCOMMON_VALUE = 9;
    const int RARE_VALUE = 15;
    const int ULTRARARE_VALUE = 45;

    //Dorsos
    [SerializeField] Sprite conan_Back;
    [SerializeField] Sprite pokemon_Back;
    [SerializeField] Sprite naruto_Back;
    [SerializeField] Sprite onePiece_Back;
    Sprite active_Back;
    [SerializeField] Image back_Image;
    private int numberOfClicks;

    //Dropdowns
    [SerializeField] TMP_Dropdown collectionsDropdown;
    [SerializeField] TMP_Dropdown rarityDropdown;

    private void OnDisable()
    {
        CleanSelectedCards();
        ResetScrollSize();
    }

    private void ResetScrollSize()
    {
        selectedCardsPanel.GetComponent<RectTransform>().sizeDelta=
            new Vector2(selectedCardsPanel.GetComponent<RectTransform>().sizeDelta.x, selectedCardOriginalHeight);
    }

    private void Awake()
    {
        selectedCardOriginalHeight = selectedCardsPanel.GetComponent<RectTransform>().sizeDelta.y;
    }
    private void Start()
    {
        PrepareFusionManager();
    }

    public void PrepareFusionManager()
    {
        cardsSelected = new PlayerCards[ULTRARARE_VALUE];
        playerPoints.text = ""+0;
        collectionsDropdown.onValueChanged.AddListener(delegate { ReloadCards(); });
        rarityDropdown.onValueChanged.AddListener(delegate { ReloadCards(); });
    }

    //Añade a cardsSelected la carta que ha elegido el usuario
    public void AddCard(PlayerCards playerCard)
    {
        if (active_Back == null)
        {
            SetActiveBack(playerCard.collection);
            collection = playerCard.collection;
        }
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

                    if (CountCardsSelected()==0)
                    {
                        active_Back = null;
                        back_Image.color = Color.black;
                    }

                    Destroy(cardCopy);
                });
                playerPoints.text = SelectedCardsPoints()+"";

                ChangeArrowColor();

                //Quitamos el panel de seleccion de cartas
                cardSelectionCanvas.SetActive(false);
                return;
            }
        }
        
    }

    private void ChangeArrowColor()
    {
        switch (SelectedCardsPoints())
        {
            case 3:
                arrow.color = Color.white;
                break;
            case 9:
                arrow.color = Color.green;
                break;
            case 15:
                arrow.color = Color.blue;
                break;
            case 45:
                arrow.color = new Color32(147, 112, 219, 255);
                break;
            default:
                arrow.color = Color.black;
                break;
        }
    }

    //Borra una carta de las seleccionadas
    private void RemoveCard(PlayerCards playerCard)
    {
        for (int i = 0; i < cardsSelected.Length; i++)
        {
            if (cardsSelected[i] != null)
            {
                if (cardsSelected[i].image == playerCard.image)
                {
                    cardsSelected[i] = null;
                    playerPoints.text = SelectedCardsPoints() + "";
                    ChangeArrowColor();
                    return;
                }
            }
        }
    }

    private void SetActiveBack(string collection)
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
        back_Image.sprite = active_Back;
        back_Image.color = Color.white;
    }

    //Carga las cartas que se pueden seleccionar en el panel
    public void LoadCardsToSelect()
    {
        int count=0;
        //Si no hay ninguna carta seleccionada se cargan todas las cartas que tiene el jugador
        if (CountCardsSelected()==0)
        {
            pc = db.GetAllPlayerCards();
            foreach (PlayerCards playerCard in pc)
            {
                if (playerCard.quantity > 1)
                {
                    GenerateCards(playerCard);
                    count++;
                }
            }
        }
        //En el caso de que ya haya alguna carta seleccionada, se cargan las de la misma coleccion
        else
        {
            pc = db.GetPlayerCards(this.collection);
            foreach (PlayerCards playerCard in pc)
            {
                if (CheckIfItCanBeSelected(playerCard))
                {
                    GenerateCards(playerCard);
                    count++;
                }
            }
        }
        cardSelectionCanvas.SetActive(true);

    }

    //Comprueba el numero de cartas que hay seleccionadas
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

    //Genera la carta del panel de seleccion de cartas
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

    //Vacia el panel de cartas a excepción de la que se usa como "molde"
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

    //Borra todas las cartas seleccionadas
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

        active_Back = null;
        back_Image.color = Color.black;

        ChangeArrowColor();
    }

    //Comprueba si la carta es seleccionable o no
    private bool CheckIfItCanBeSelected(PlayerCards playerCard)
    {
        int count = 0;
        /*
         * Se mira primero si la cantidad de esa carta es mayor que 1, en caso positivo se comprueba si hay mas cartas de 
         * ese tipo seleccionadas, en caso negatico esa carta no se puede seleccionar y se devuelve false
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
             * que 1 la carta es seleccioable. Si es menor se devuelve false.
             */
            if (playerCard.quantity - count > 1)
            {
                return true;
            }
        }
        return false;
    }

    public void Fusion()
    {
        switch (SelectedCardsPoints())
        {
            case COMMON_VALUE:
                cardObtained = GetCard(CardInfo.common);
                break;
            case UNCOMMON_VALUE:
                cardObtained = GetCard(CardInfo.uncommon);
                break;
            case RARE_VALUE:
                cardObtained = GetCard(CardInfo.rare);
                break;
            case ULTRARARE_VALUE:
                cardObtained = GetCard(CardInfo.ultrarare);
                break;
            default:
                errorMesage.ShowErrorMessage("PUNTOS INSUFICIENTES.\nNecesitas sumar los puntos exactos que aparecen" +
                    " a la derecha de la flecha para poder conseguir una carta.");
                return;
        }

        fusionCardPanel.SetActive(true);
        fusionCard.GetComponent<Image>().sprite = active_Back;

        db.SaveCardOnPlayerCardsDb(cardObtained);
        RemoveCardFromPlayerCards();

        playerPoints.text = "0";
        CleanSelectedCards();
        selectedCardsPanel.GetComponent<RectTransform>().sizeDelta
                        = new Vector2(selectedCardsPanel.GetComponent<RectTransform>().sizeDelta.x,
                        selectedCardOriginalHeight * ((CountCardsSelected() / 3) + 1));
        
    }

    private void RemoveCardFromPlayerCards()
    {
        for (int i = 0; i < cardsSelected.Length; i++)
        {
            if (cardsSelected[i] != null)
            {
                db.RemoveCardFromPlayerCards(cardsSelected[i]);
            }
        }
    }

    //Obtiene la carta resultado de la fusion
    private Card GetCard(string rarity)
    {
        //Primero se obtiene de la base de datos las cartas que se pueden conseguir en base a la coleccion y su rareza
        IEnumerable<Card> cards = db.GetCards(collection,rarity);

        //Se guardan las posibles cartas en un array
        int numberOfCards = 0;
        foreach (Card card in cards)
        {
            numberOfCards++;
        }
        Card[] possibleCards = new Card[numberOfCards];

        int count = 0;
        foreach (Card card in cards)
        {
            possibleCards[count] = card;
            count++;
        }

        //Se genera un numero aleatorio entre 0 y el tamaño del array y se devuelve la carta que este en esa posicion en el array
        System.Random r=new System.Random();
        int number = r.Next(0,possibleCards.Length);

        return possibleCards[number];
    }

    //Obtiene la suma de puntos de las cartas seleccionadas
    private int SelectedCardsPoints()
    {
        int total = 0;
        for (int i = 0; i < cardsSelected.Length; i++)
        {
            if (cardsSelected[i] != null)
            {
                switch (cardsSelected[i].rarity)
                {
                    case CardInfo.common:
                        total += COMMON_PLAYER_VALUE;
                        break;
                    case CardInfo.uncommon:
                        total += UNCOMMON_PLAYER_VALUE;
                        break;
                    case CardInfo.rare:
                        total += RARE_PLAYER_VALUE;
                        break;
                    case CardInfo.ultrarare:
                        total += ULTRARARE_PLAYER_VALUE;
                        break;
                }
            }
        }

        return total;
    }

    public void FusionCardPanelButton()
    {
        if (numberOfClicks==0)
        {
            string cardPath = cardObtained.image.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
            fusionCard.GetComponent<Image>().sprite= Resources.Load<Sprite>(cardPath);
            numberOfClicks++;
        }
        else
        {
            numberOfClicks = 0;
            fusionCardPanel.SetActive(false);
            FindObjectOfType<QuestManager>().UpdateQuests(Quest.QuestType.FUSION);
        }
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
        if (rarity==null && collection == null)
        {
            LoadCardsToSelect();
        }
        else
        {
            LoadCardsToSelect(collection,rarity);
        }
    }

    public void LoadCardsToSelect(string collection, string rarity)
    {
        int count = 0;
        //Si no hay ninguna carta seleccionada se cargan todas las cartas que tiene el jugador
        if (CountCardsSelected() == 0)
        {
            pc = db.GetPlayerCards(collection,rarity);
            foreach (PlayerCards playerCard in pc)
            {
                if (playerCard.quantity > 1)
                {
                    GenerateCards(playerCard);
                    count++;
                }
            }
        }
        //En el caso de que ya haya alguna carta seleccionada, se cargan las de la misma coleccion
        else
        {
            pc = db.GetPlayerCards(this.collection,rarity);
            foreach (PlayerCards playerCard in pc)
            {
                if (CheckIfItCanBeSelected(playerCard))
                {
                    GenerateCards(playerCard);
                    count++;
                }
            }
        }
        /*
         * En caso de que haya cartas seleccionadas se ajusta el tamaño del panel y su posicion
         * para que se vea bien. Después se activa.
         */
        if (cardSelectionPanel.transform.childCount >= 1)
        {
            cardSelectionPanel.GetComponent<RectTransform>().sizeDelta
                        = new Vector2(cardSelectionPanel.GetComponent<RectTransform>().sizeDelta.x,
                        cardToSelect.GetComponent<RectTransform>().sizeDelta.y * ((count / 3) + 1));

            cardSelectionPanel.GetComponent<RectTransform>().offsetMin =
                new Vector2(cardSelectionPanel.GetComponent<RectTransform>().offsetMin.x,
                -cardSelectionPanel.GetComponent<RectTransform>().sizeDelta.y);

            cardSelectionPanel.GetComponent<RectTransform>().offsetMax =
                new Vector2(cardSelectionPanel.GetComponent<RectTransform>().offsetMax.x, 0);

            cardSelectionCanvas.SetActive(true);
        }

    }

    public void ReturnCardSelectionButton()
    {
        cardSelectionCanvas.SetActive(false);
        EraseCards();
    }
}
