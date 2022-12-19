using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayerCharacterString
{ 
    NONE,
    X,
    O
}

public class GridSquare : MonoBehaviour
{
    public Button button;
    public TMP_Text buttonText;

    private GameLogic gameController;

    public void ChangeSquare()
    {
        buttonText.text = gameController.GetPlayerChoice().ToString();
        button.interactable = false;
        gameController.EndTurn();
        FindObjectOfType<GameLogic>().buttonPressed = this.button;
    }

    public void SetGameControllerRef(GameLogic gameLogic)
    {
        gameController = gameLogic;
    }
}
