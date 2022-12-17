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

        if (signifier == ServerToClientSignifiers.verifyLogin)
        {
            float screenPositionXPercent = float.Parse(csv[1]);
            float screenPositionYPercent = float.Parse(csv[2]);
            int balloonID = int.Parse(csv[3]);
            Vector2 screenPosition = new Vector2(screenPositionXPercent * (float)Screen.width, screenPositionYPercent * (float)Screen.height);
          //  gameLogic.SpawnNewBalloon(screenPosition, balloonID);
        }
        if (signifier == ServerToClientSignifiers.startGame)
        { 
        
        }
        else if (signifier == ServerToClientSignifiers.helloFromOtherPlayer)
        {
            //gameLogic.DestroyBalloon(int.Parse(csv[1]));
        }
    }

    static public void SendMessageToServer(string msg)
    {
        networkedClient.SendMessageToHost(msg);
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
    public const int balloonClicked = 1;
    public const int playerHasLeftMatch = 2;
}

static public class ServerToClientSignifiers
{
    public const int verifyLogin = 1;
    public const int startGame = 2;
    public const int helloFromOtherPlayer = 3;
}

#endregion


//private void ProcessRecievedMsg(string msg, int id)
//{
//    Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

//    string[] verifyLogin = msg.Split(',');
//    loginStatus = verifyLogin[0];

//    if (loginStatus == "True")
//    {
//        loggedIntoAccount = true;
//    }
//    else if (loginStatus == "False")
//    {
//        loggedIntoAccount = false;
//        wrongPasswordUI.SetActive(true);
//    }

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