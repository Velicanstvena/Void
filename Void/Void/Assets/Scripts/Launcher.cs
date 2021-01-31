using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject connectedScreen;
    [SerializeField] private GameObject disconnectedScreen;

    [SerializeField] private InputField createRoomInputField;
    [SerializeField] private InputField joinRoomInputField;

    public void OnClickConnectButton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        disconnectedScreen.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        if (disconnectedScreen.activeSelf) disconnectedScreen.SetActive(false);

        connectedScreen.SetActive(true);
    }

    public void OnClickCreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomInputField.text, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public void OnClickJoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomInputField.text, null);
    }

    public override void OnJoinedRoom()
    {
        print("Room Joined Sucess");
        //if (PhotonNetwork.CountOfPlayers == 2)
        //{
            PhotonNetwork.LoadLevel(1);
        //}
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Room Failed " + returnCode + " Message " + message);
    }
}
