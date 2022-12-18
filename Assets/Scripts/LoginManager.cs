using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginManager : MonoBehaviour
{
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
    public GameObject usernameTakenUI;

    string loginStatus;
    bool loggedIntoAccount = false;
    // ******************************************

    void Start()
    {
        mainMenuUI = GameObject.FindGameObjectWithTag("LoginPanel");

        currentState = GameStates.StartState;
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
        NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.verifyLogin + "," + UserName + "," + PassWord);
    }

    public void CreateAccount()
    {
        GetUsernameAndPassword();
        NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.createAccount + "," + UserName + "," + PassWord);

        if (wrongPasswordUI.activeInHierarchy != true)
        { accountCreatedUI.SetActive(true); }
    }

    public void ClosePopupWindow()
    {
        wrongPasswordUI.SetActive(false);
        usernameTakenUI.SetActive(false);
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
}
