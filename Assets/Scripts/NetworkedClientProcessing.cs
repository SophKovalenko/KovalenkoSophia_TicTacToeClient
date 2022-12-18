using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedClientProcessing : MonoBehaviour
{
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg)
    {
        Debug.Log("msg received = " + msg + ".");

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.loginSuccessful)
        {
            FindObjectOfType<LoginManager>().StartGameLobby();
        }
        if (signifier == ServerToClientSignifiers.wrongPassword)
        {
            FindObjectOfType<LoginManager>().wrongPasswordUI.SetActive(true);

        }
        if (signifier == ServerToClientSignifiers.wrongUsername)
        {
            FindObjectOfType<LoginManager>().usernameTakenUI.SetActive(true);
        }
        //if (signifier == ServerToClientSignifiers.startGame)
        //{ 

        //}
        //else if (signifier == ServerToClientSignifiers.helloFromOtherPlayer)
        //{
        //    //gameLogic.DestroyBalloon(int.Parse(csv[1]));
        //}
    }

    static public void SendMessageToServer(string msg)
    {
        networkedClient.SendMessageToServer(msg);
    }

    #endregion

    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Network Connection Event!");
    }
    static public void DisconnectionEvent()
    {
        Debug.Log("Network Disconnection Event!");
    }
    static public bool IsConnectedToServer()
    {
        return networkedClient.IsConnected();
    }
    static public void ConnectToServer()
    {
       // networkedClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkedClient.Disconnect();
        NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.playerHasLeftMatch + "," + networkedClient.GetInstanceID());
    }

    #endregion

    #region Setup
    static NetworkedClient networkedClient;
    static GameLogic gameLogic;

    static public void SetNetworkedClient(NetworkedClient NetworkedClient)
    {
        networkedClient = NetworkedClient;
    }
    static public NetworkedClient GetNetworkedClient()
    {
        return networkedClient;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion

}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int verifyLogin = 1;
    public const int createAccount = 2;
    public const int playerHasLeftMatch = 3;
}

static public class ServerToClientSignifiers
{
    public const int loginSuccessful = 1;
    public const int wrongPassword = 2;
    public const int wrongUsername = 3;
    public const int startGame = 4;
    public const int helloFromOtherPlayer = 5;
}

#endregion


//private void ProcessRecievedMsg(string msg, int id)
//{


//    if (msg == "StartGame")
//    {
//        StartGameRoom();
//        Debug.Log("Got the start game msg!");
//    }

//    if (msg == "Hello")
//    {
//        Debug.Log("Hello from your fellow player!!");
//    }
//}