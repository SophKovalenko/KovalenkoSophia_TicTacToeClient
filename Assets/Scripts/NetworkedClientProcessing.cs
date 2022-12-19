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

        // LOGIN INFO
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

        //GAMEROOM INFO
        if (signifier == ServerToClientSignifiers.startGame)
        {
            FindObjectOfType<GameRoomManager>().StartGameRoom();
        }

        //GAMEPLAY INFO
        if (signifier == ServerToClientSignifiers.sideAssignment)
        {
            if (csv[1].ToString() == "X")
            { gameLogic.playerChoice = PlayerCharacterString.X; }

            if (csv[1].ToString() == "O")
            { gameLogic.playerChoice = PlayerCharacterString.O; }
        }
        if (signifier == ServerToClientSignifiers.buttonPressed)
        {
            //Change the button to pressed for both players
            for (int i = 0; i < gameLogic.buttonList.Length; i++)
            {
                if (gameLogic.buttonList[i].text == csv[1])
                {
                    gameLogic.buttonList[i].GetComponentInParent<GridSquare>().ChangeSquare();
                }
            }
        }
        if (signifier == ServerToClientSignifiers.changeTurn)
        {
            if (gameLogic.playerChoice.ToString() == csv[1])
            {
                gameLogic.StartTurn();
            }
        }
        if (signifier == ServerToClientSignifiers.gameOver)
        {
            if (csv[1].ToString() == "X")
            { gameLogic.playerChoice = PlayerCharacterString.X; }

            if (csv[1].ToString() == "O")
            { gameLogic.playerChoice = PlayerCharacterString.O; }

            //Display who won the game based on the player choice passed back from server and display in win text
            gameLogic.GameOver();
        }
        if (signifier == ServerToClientSignifiers.gameDraw)
        {
            gameLogic.SetGameOverText("It's a draw!");
        }
        else if (signifier == ServerToClientSignifiers.helloFromOtherPlayer)
        {
            Debug.Log("Hello from your fellow player!!");
        }
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
    public const int joinRoom = 3;
    public const int leaveRoom = 4;
    public const int sendMsg = 5;
    public const int playerHasLeftMatch = 6;
    //During Game
    public const int turnTaken = 7;
}

static public class ServerToClientSignifiers
{
    //Login
    public const int loginSuccessful = 1;
    public const int wrongPassword = 2;
    public const int wrongUsername = 3;

    //Gamr Room
    public const int startGame = 4;
    public const int helloFromOtherPlayer = 5;

    //During Game
    public const int sideAssignment = 6;
    public const int buttonPressed = 7;
    public const int changeTurn = 8;
    public const int gameDraw = 9;
    public const int gameOver = 10;
}

#endregion
