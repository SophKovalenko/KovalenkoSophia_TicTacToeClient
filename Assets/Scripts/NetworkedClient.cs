using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    //Game Room Data ******************************************
    public TMP_InputField usersGameRoomInput;
    public GameObject gameRoomWaitingPanel;
    public GameObject gameRoomPlayingPanel;
    public GameRoom gameRoom;
    private string gameRoomName;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    NetworkedClientProcessing.ConnectionEvent();
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    NetworkedClientProcessing.ReceivedMessageFromServer(msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    NetworkedClientProcessing.DisconnectionEvent();
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.0.107", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);
            }
            else
                Debug.Log("Network client init failed!");
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
        Debug.Log("Disconnecting from server");
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    public bool IsConnected()
    {
        return isConnected;
    }


    public void FindOrCreateGameRoom()
    {
        gameRoomName = usersGameRoomInput.text;

        SendMessageToHost("JoinRoom" + "," + gameRoomName);

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
            SendMessageToHost("LeaveRoom");
        }

        if (gameRoomPlayingPanel.activeInHierarchy == true)
        {
            gameRoomPlayingPanel.SetActive(false);
            FindObjectOfType<LoginManager>().successfulLoginUI.SetActive(true);
            SendMessageToHost("LeaveRoom");
        }
    }

    public void SendMessageToOtherPlayer()
    {
        SendMessageToHost("SendMessage");
    }
}