using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class UIhandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField createRoomInputField;
    [SerializeField] private InputField joinRoomInputField;

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
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Room Failed " + returnCode + " Message " + message);
    }
}
