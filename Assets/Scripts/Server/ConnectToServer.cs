using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]private PanelChanger pc;
    [SerializeField] private ErrorMessage errorMessage;
    public void Connect()
    {
        if (!PhotonNetwork.ConnectUsingSettings())
        {
            errorMessage.ShowErrorMessage("ERROR AL CONECTAR CON EL SERVIDOR.\nComprueba tu conexión a internet y vuelve a intentarlo");
            pc.LobbyToMenu();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.JoinLobby())
        {
            errorMessage.ShowErrorMessage("ERROR AL CONECTAR CON EL SERVIDOR.\nComprueba tu conexión a internet y vuelve a intentarlo");
            pc.LobbyToMenu();
        }
    }

    public override void OnJoinedLobby()
    {
        pc.ConnectingServerToLobby();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

}
