using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField createRoomText;
    [SerializeField] private InputField joinRoomText;
    [SerializeField] private ErrorMessage errorMessage;
    const int MAX_PLAYERS = 2;
    public void CreateRoom()
    {
        if(createRoomText.text.Trim().Length<10 && createRoomText.text.Trim().Length > 3)
        {
            Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
            roomOptions.MaxPlayers = MAX_PLAYERS;
            PhotonNetwork.CreateRoom(createRoomText.text.Trim(), roomOptions);
        }
        else
        {
            errorMessage.ShowErrorMessage("CÓDIGO INCORRECTO.\nEl código debe tener más de 3 caracteres y menos de 10.");
        }
        
    }
    
    public void JoinRoom()
    {
        if (PhotonNetwork.JoinRoom(joinRoomText.text.Trim()))
        {
            //errorMessage.ShowErrorMessage("ERROR AL UNIRSE A LA SALA.\nComrpueba que has introducido correctamente el código." +
               // " También es posible que la sala esté completa");
        }
        
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("TradeScene");
    }

}
