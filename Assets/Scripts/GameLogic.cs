using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public TMP_Text[] buttonList;
    private PlayerCharacterString playerChoice;

    public GameObject gameOverPanel; 
    public TMP_Text gameOverText;

    private void Awake()
    {
        gameOverPanel.SetActive(false);
        SetGameControllerRefForButtons();
        playerChoice = PlayerCharacterString.X;
    }

    void SetGameControllerRefForButtons()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<GridSquare>().SetGameControllerRef(this);
        }
    }

    public PlayerCharacterString GetPlayerChoice()
    {
        return playerChoice;
    }

    //Check all possible combinations for a win
    public void EndTurn()
    {
        //Check all the rows
        if (buttonList[0].text == playerChoice.ToString() && buttonList[1].text == playerChoice.ToString() && buttonList[2].text == playerChoice.ToString())
        {
            GameOver();
        }
        if (buttonList[3].text == playerChoice.ToString() && buttonList[4].text == playerChoice.ToString() && buttonList[5].text == playerChoice.ToString())
        {
            GameOver();
        }
        if (buttonList[6].text == playerChoice.ToString() && buttonList[7].text == playerChoice.ToString() && buttonList[8].text == playerChoice.ToString())
        {
            GameOver();
        }

        //Check all the colomns
        if (buttonList[0].text == playerChoice.ToString() && buttonList[3].text == playerChoice.ToString() && buttonList[6].text == playerChoice.ToString())
        {
            GameOver();
        }
        if (buttonList[1].text == playerChoice.ToString() && buttonList[4].text == playerChoice.ToString() && buttonList[7].text == playerChoice.ToString())
        {
            GameOver();
        }
        if (buttonList[2].text == playerChoice.ToString() && buttonList[5].text == playerChoice.ToString() && buttonList[8].text == playerChoice.ToString())
        {
            GameOver();
        }

        //Check all the diagonals
        if (buttonList[0].text == playerChoice.ToString() && buttonList[4].text == playerChoice.ToString() && buttonList[8].text == playerChoice.ToString())
        {
            GameOver();
        }
        if (buttonList[2].text == playerChoice.ToString() && buttonList[4].text == playerChoice.ToString() && buttonList[6].text == playerChoice.ToString())
        {
            GameOver();
        }

        //Tell the server to increment the turns taken variable
        NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.turnTaken + ",");

        //ChangeTurn();
    }

    void GameOver()
    {
        SetBoardInteractable(false);
        gameOverPanel.SetActive(true);
        SetGameOverText(playerChoice.ToString() + " wins the game!");
    }

    public void SetGameOverText(string value)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = value;
    }

    //Server sends msg to do this
    public void RestartGame()
    {
        playerChoice = PlayerCharacterString.X;
       // turnsTaken = 0;
        gameOverPanel.SetActive(false);

        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].text = "";
        }
        SetBoardInteractable(true);
    }

    void SetBoardInteractable(bool toggle)
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            //Disable all buttons if the game has ended or if other players turn
            buttonList[i].GetComponentInParent<Button>().interactable = toggle;
        }
    }
   
}
