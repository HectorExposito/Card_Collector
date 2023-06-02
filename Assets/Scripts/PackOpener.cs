using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PackOpener : MonoBehaviour
{
    [SerializeField] GameObject packOpenerPanel;
    [SerializeField] GameObject conan;
    [SerializeField] GameObject naruto;
    [SerializeField] GameObject pokemon;
    [SerializeField] GameObject onePiece;
    [SerializeField] Image card;
    [SerializeField] TMP_Text text;
    Card[] cards;
    int numOfCards;
    int counter;

    //Prepara todo para abrir sobres
    public void PrepareCards(string collection,Card[] cards)
    {
        //Se activa el panel
        packOpenerPanel.SetActive(true);

        //Se guardan las cartas del sobre y el numero de ellas que hay
        this.cards = cards;
        numOfCards = cards.Length;
        counter = 0;
        //Se desactivan los objetos carta y texto
        card.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        //En base a la coleccion se activa el dorso correspondiente
        switch (collection)
        {
            case CardInfo.Conan:
                conan.SetActive(true);
                naruto.SetActive(false);
                pokemon.SetActive(false);
                onePiece.SetActive(false);
                break;
            case CardInfo.Naruto:
                naruto.SetActive(true);
                pokemon.SetActive(false);
                onePiece.SetActive(false);
                conan.SetActive(false);
                break;
            case CardInfo.Pokemon:
                pokemon.SetActive(true);
                onePiece.SetActive(false);
                conan.SetActive(false);
                naruto.SetActive(false);
                break;
            case CardInfo.OnePiece:
                onePiece.SetActive(true);
                conan.SetActive(false);
                naruto.SetActive(false);
                pokemon.SetActive(false);
                break;
        }
        
    }

    //Cambia de carta
    public void NextCard()
    {
        //Si el contador es 0 significa que esta viendo el dorso, entonces se carga la imagen de la primera carta y se activan
        //los objetos card y text.
        if (counter==0)
        {
            string cardPath = cards[counter].image.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
            card.sprite = Resources.Load<Sprite>(cardPath);
            card.gameObject.SetActive(true);
            ChangeRarityText(cards[counter].rarity);
            text.gameObject.SetActive(true);
        }
        //Carga la imagen y el texto de la siguiente carta
        else if (counter < numOfCards)
        {
            string cardPath = cards[counter].image.Replace(".jpg", "").Replace(".\\Assets\\Resources\\", "");
            card.sprite = Resources.Load<Sprite>(cardPath);
            ChangeRarityText(cards[counter].rarity);
        }
        //Si el contador es igual al numero de cartas se desactiva el panel y se pone en null el array cards
        else
        {
            cards = null;
            packOpenerPanel.SetActive(false);
        }
        counter++;
    }

    //Cambia el texto de las rarezas
    private void ChangeRarityText(string rarity)
    {
        switch (rarity)
        {
            case CardInfo.common:
                text.text = "COMUN";
                text.color = Color.white;
                break;
            case CardInfo.uncommon:
                text.text = "POCO COMUN";
                text.color = Color.green;
                break;
            case CardInfo.rare:
                text.text = "RARA";
                text.color = Color.blue;
                break;
            case CardInfo.ultrarare:
                text.text = "ULTRA RARA";
                text.color = new Color32(147, 112, 219,255);
                break;
        }
    }
}
