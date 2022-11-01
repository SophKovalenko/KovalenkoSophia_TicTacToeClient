using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    private string UserName;
    private string PassWord;

    public TMP_InputField newUserName;
    public TMP_InputField newPassWord;
    public TMP_InputField existingUserName;
    public TMP_InputField existingPassWord;

    public Button loginButton;
    public Button createAccountButton;

    private GameObject currentUI;
    private GameObject currentUI2;
    public GameObject newUI;
    public GameObject newUI2;

    // Start is called before the first frame update
    void Start()
    {
        currentUI = GameObject.FindGameObjectWithTag("LoginPanel");
        currentUI2 = GameObject.FindGameObjectWithTag("NewAccountPanel");

        currentState = GameStates.StartState;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GameStates.RunState)
        {
            Connect();

            if (Input.GetKeyDown(KeyCode.S))
                SendMessageToHost("" + UserName);

            UpdateNetworkConnection();
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

                PlayerPrefs.SetString("" + UserName + connectionID, UserName + "," + connectionID);
                Debug.Log(PlayerPrefs.GetString("" + UserName + connectionID));
            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    public void MakeNewUsernameAndPassword()
    {
        UserName = newUserName.text;
        PassWord = newPassWord.text;

        //Save the login to playerPrefs if it doesnt already exist
        if (PlayerPrefs.HasKey("" + UserName) == false)
        {
            PlayerPrefs.SetString("" + UserName, UserName);
            Debug.Log(UserName);

            //Save the password to playerPrefs
            PlayerPrefs.SetString("" + PassWord, PassWord);
            Debug.Log(PassWord);

            CreateNewAccount();
        }
        else if (PlayerPrefs.HasKey("" + UserName) == true)
        {
            Debug.Log("Username already exists, please choose another!");
        }

        //Clear the input fields
        newUserName.text = "";
        newPassWord.text = "";
    }

    public void GetExisitingUsernameAndPassword()
    {
        //Get the login from playerPrefs
        UserName = existingUserName.text;

        //Get the password from playerPrefs
        PassWord = existingPassWord.text;

        if (PlayerPrefs.HasKey("" + UserName) == true && PlayerPrefs.HasKey("" + PassWord) == true)
        {
            PlayerPrefs.GetString("" + UserName, UserName);
            Debug.Log(UserName);

            PlayerPrefs.GetString("" + PassWord, PassWord);
            Debug.Log(PassWord);

            VerifyExistingAccount();
        }
        else if (PlayerPrefs.HasKey("" + UserName) == false || PlayerPrefs.HasKey("" + PassWord) == false)
        {
            Debug.Log("Username or password is incorrect. Please try again or create an account!");
        }
        
        //Clear the input fields
        existingUserName.text = "";
        existingPassWord.text = "";

    }

    public void VerifyExistingAccount()
    {
        //Switch state to run state
        currentState = GameStates.RunState;

        //Hide main menu UI
        currentUI.SetActive(false);
        currentUI2.SetActive(false);

        //Enable new UI
        newUI.SetActive(true); 
    }
    public void CreateNewAccount()
    {
        //Hide main menu UI
        currentUI.SetActive(false);
        currentUI2.SetActive(false);

        //Enable new UI
        newUI2.SetActive(true);
    }
    public void ReturnToMenu()
    {
        //If new account created UI is active and we want to close it and return to menu
        if (newUI2.activeInHierarchy == true )
        {
            newUI2.SetActive(false);
            currentUI.SetActive(true);
            currentUI2.SetActive(true);
        }
    }
}