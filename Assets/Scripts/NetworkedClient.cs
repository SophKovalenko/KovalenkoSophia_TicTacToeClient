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

    private GameStates currentState;

    //Login Data ******************************************
    private string UserName;
    private string PassWord;

    public TMP_InputField inputUserName;
    public TMP_InputField inputPassWord;

    public static string loginFileNames = "";

    //Create a new list for usernames/ passwords
    static List<string> fileNames = new List<string>();

    public Button loginButton;
    public Button createAccountButton;

    private GameObject mainMenuUI;
    public GameObject successfulLoginUI;
    public GameObject accountCreatedUI;
    public GameObject wrongPasswordUI;

    string loginStatus;
    bool loggedIntoAccount = false;
    // ******************************************

    //Game Room Data ******************************************
    public TMP_InputField usersGameRoomInput;
    private string gameRoomName;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuUI = GameObject.FindGameObjectWithTag("LoginPanel");

        currentState = GameStates.StartState;

        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNetworkConnection();

        if (loggedIntoAccount == true)
        {
            StartGameLobby();
            loggedIntoAccount = false;
        }
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
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
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

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] verifyLogin = msg.Split(',');
        loginStatus = verifyLogin[0];

        if (loginStatus == "True")
        {
            loggedIntoAccount = true;
        }
        else if (loginStatus == "False")
        {
            loggedIntoAccount = false;
            wrongPasswordUI.SetActive(true);
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }


    public void GetUsernameAndPassword()
    {
        //Get the login from playerPrefs
        UserName = inputUserName.text;

        //Get the password from playerPrefs
        PassWord = inputPassWord.text;

        //Clear the input fields
        inputUserName.text = "";
        inputPassWord.text = "";

    }

    public void VerifyAccount()
    {
        GetUsernameAndPassword();
        SendMessageToHost("" + UserName + "," + PassWord);
    }

    public void CreateAccount()
    {
        GetUsernameAndPassword();
        SendMessageToHost("" + UserName + "," + PassWord);

        if (wrongPasswordUI.activeInHierarchy != true)
        { accountCreatedUI.SetActive(true); }
    }

    public void ClosePopupWindow()
    {
        wrongPasswordUI.SetActive(false);
        accountCreatedUI.SetActive(false);
    }

    public void StartGameLobby()
    {
        //Switch state to run state
        currentState = GameStates.RunState;

        //Hide main menu UI
        mainMenuUI.SetActive(false);

        //Enable new UI
        successfulLoginUI.SetActive(true);
    }

    public void FindOrCreateGameRoom()
    {
        gameRoomName = usersGameRoomInput.text;

        SendMessageToHost(gameRoomName);

        usersGameRoomInput.text = "";
    }
}