using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class PanelChanger : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject collections_menu;
    [SerializeField] GameObject shop_menu;
    [SerializeField] GameObject collection;
    [SerializeField] GameObject shop;
    [SerializeField] GameObject fusion;
    [SerializeField] GameObject shopManager;
    [SerializeField] GameObject cardInfo;
    [SerializeField] GameObject connectingServer;
    [SerializeField] GameObject lobby;
    [SerializeField] GameObject quests;
    [SerializeField] GameObject packOpener;
    [SerializeField] TMP_Text connectingText;
    [SerializeField] GameObject errorMessage;
    private GameObject activePanel;
    private GameObject panelBeforeQuests;

    //Desactiva el panel menu y activa el panel collections_menu
    public void MenuToCollections()
    {
        menu.SetActive(false);
        collections_menu.SetActive(true);
        activePanel = collections_menu;
    }

    //Desactiva el panel collections_menu y activa el panel menu
    public void CollectionsToMenu()
    {
        menu.SetActive(true);
        collections_menu.SetActive(false);
        activePanel = menu;
    }

    //Desactiva el panel menu y activa el panel shop_menu
    public void MenuToShop()
    {
        menu.SetActive(false);
        shop_menu.SetActive(true);
        activePanel = shop_menu;
    }

    //Desactiva el panel shop_menu y activa el panel menu
    public void ShopToMenu()
    {
        menu.SetActive(true);
        shop_menu.SetActive(false);
        activePanel = menu;
    }

    //Prepara el panel de colecciones. Desactiva el panel collections_menu y activa el panel collection
    public void CollectionMenuToCollection(string col)
    {
        collection.GetComponent<CardCollectionManager>().PreparePanel(col);
        collection.SetActive(true);
        collections_menu.SetActive(false);
        activePanel = collection;
    }

    //Desactiva el panel collection y activa el panel collections_menu
    public void CollectionToCollectionMenu()
    {
        collections_menu.SetActive(true);
        collection.SetActive(false);
        activePanel = collections_menu;
    }

    //Prepara el panel shop. Desactiva el panel shop_menu y activa el panel shop
    public void ShopMenuToShop(string collection)
    {
        shopManager.GetComponent<ShopManager>().PreparePanel(collection);
        shop.SetActive(true);
        shop_menu.SetActive(false);
        activePanel = shop;
    }

    //Desactiva el panel shop y activa el panel shop_menu
    public void ShopToShopMenu()
    {
        shop_menu.SetActive(true);
        shop.SetActive(false);
        activePanel = shop_menu;
    }

    //Activa el panel cardInfo y muuestra la informacion de una carta dada
    public void OpenCardInfo(string num)
    {
        cardInfo.SetActive(true);
        Debug.Log("panelchanger "+num);
        collection.GetComponent<CardCollectionManager>().CardInfoPanelSetData(int.Parse(num));
        activePanel = cardInfo;
    }

    //Desactiva el panel cardInfo
    public void CloseCardInfo()
    {
        cardInfo.SetActive(false);
        activePanel = collection;
    }

    //Desactiva el panel menu y activa el panel fusion
    public void MenuToFusion()
    {
        fusion.GetComponent<FusionManager>().PrepareFusionManager();
        fusion.SetActive(true);
        menu.SetActive(false);
        activePanel = fusion;
    }

    //Desactiva el panel fusion y activa el panel menu
    public void FusionToMenu()
    {
        menu.SetActive(true);
        fusion.SetActive(false);
        activePanel = menu;
    }

    //Desactiva el panel menu y activa el panel connectingServer
    public void MenuToConnectingServer()
    {
        menu.SetActive(false);
        connectingServer.SetActive(true);
        StartCoroutine(ConectingToServer());
        activePanel = connectingServer;
    }

    private IEnumerator ConectingToServer()
    {
        switch (connectingText.text)
        {
            case "CONECTANDO...":
                connectingText.text = "CONECTANDO.";
                break;
            case "CONECTANDO..":
                connectingText.text = "CONECTANDO...";
                break;
            case "CONECTANDO.":
                connectingText.text = "CONECTANDO..";
                break;
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(ConectingToServer());
    }

    //Desactiva el panel connectingServer y activa el panel lobby
    public void ConnectingServerToLobby()
    {
        lobby.SetActive(true);
        connectingServer.SetActive(false);
        StopAllCoroutines();
        activePanel = lobby;
    }

    //Desactiva el panel lobby y activa el panel connectingServer
    public void LobbyToMenu()
    {
        menu.SetActive(true);
        lobby.SetActive(false);
        activePanel = menu;
    }

    //Desactiva el panel menu y activa el panel quests
    public void OpenQuestsPanel()
    {
        if (quests.activeSelf)
        {
            quests.SetActive(false);
            activePanel = panelBeforeQuests;
        }
        else
        {
            panelBeforeQuests = activePanel;
            quests.SetActive(true);
            activePanel = quests;
        }
        
    }

    //Desactiva el panel quests y activa el panel menu
    public void CloseQuestsPanel()
    {
        quests.SetActive(false);
    }

    public void GoBackToMenu()
    {
        errorMessage.SetActive(false);
        if (activePanel!=menu && activePanel!=connectingServer && activePanel != quests && activePanel != packOpener)
        {
            activePanel.SetActive(false);
            menu.SetActive(true);
            return;
        }

        if (activePanel==quests)
        {
            panelBeforeQuests.SetActive(false);
            CloseQuestsPanel();
            menu.SetActive(true);
        }
    }
}
