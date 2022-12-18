using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameRoomManager : MonoBehaviour
{
    //Game Room Data ******************************************
    public TMP_InputField usersGameRoomInput;
    public GameObject gameRoomWaitingPanel;
    public GameObject gameRoomPlayingPanel;
    public GameRoom gameRoom;
    private string gameRoomName;

    public void FindOrCreateGameRoom()
    {
        gameRoomName = usersGameRoomInput.text;

        NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.joinRoom + "," + gameRoomName);

        usersGameRoomInput.text = "";

        FindObjectOfType<LoginManager>().successfulLoginUI.SetActive(false);
        gameRoomWaitingPanel.SetActive(true);

        gameRoom.roomName = gameRoomName;
        gameRoom.nameTextWaiting.text = gameRoomName;
    }

    public void StartGameRoom()
    {
        gameRoomWaitingPanel.SetActive(false);
        gameRoomPlayingPanel.SetActive(true);
    }

    public void BackToLobby()
    {
        if (gameRoomWaitingPanel.activeInHierarchy == true)
        {
            gameRoomWaitingPanel.SetActive(false);
            FindObjectOfType<LoginManager>().successfulLoginUI.SetActive(true);
            NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.leaveRoom + ",");
        }

        if (gameRoomPlayingPanel.activeInHierarchy == true)
        {
            gameRoomPlayingPanel.SetActive(false);
            FindObjectOfType<LoginManager>().successfulLoginUI.SetActive(true);
            NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.leaveRoom + ",");
        }
    }

    public void SendMessageToOtherPlayer()
    {
        NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.sendMsg + ",");
    }
}
