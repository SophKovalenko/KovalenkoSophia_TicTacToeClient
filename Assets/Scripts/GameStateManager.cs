using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameStateManager : MonoBehaviour
{
    private GameStates currentState;
    public bool isLoginSuccessful = false;
    public bool serverClosesConnection = false;

    public void Start()
    {
        currentState = GameStates.StartState;
    }

    public void Update()
    {
        if (isLoginSuccessful == true)
        {
            currentState = GameStates.RunState;
        }

        if (serverClosesConnection == true)
        {
            currentState = GameStates.ExitState;
        }
    }

}
