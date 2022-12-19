using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public TMP_Text[] buttonList;
    public Button buttonPressed;
    public PlayerCharacterString playerChoice;

    public GameObject gameOverPanel; 
    public TMP_Text gameOverText;

    private void Awake()
    {
        gameOverPanel.SetActive(false);
        SetGameControllerRefForButtons();
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

    public void StartTurn()
    {
        SetBoardInteractable(true);
    }

    //Check all possible combinations for a win
    public void EndTurn()
    {
        //Tell the server which button was clicked/ to increment the turns taken variable
        NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.turnTaken + "," + buttonPressed.name);

        //Diable input if no longer this player's turn
        SetBoardInteractable(false);
    }

    public void GameOver()
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

    //Have this called on UI button click in scene 
    public void RestartGame()
    {
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
