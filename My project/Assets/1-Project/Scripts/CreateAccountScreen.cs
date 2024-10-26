using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;
using System.IO;
using UnityEngine.SceneManagement;

public class CreateAccountScreen : MonoBehaviour
{
    public string eMail;
    public string username;
    public string name;
    public string password;

    [Header("Screens")]
    public GameObject emailScreen;
    public GameObject passwordScreen;
    public GameObject nameScreen;
    public GameObject usernameScreen;

    public GameObject logInScreen;

    [Header("CreateAccountPanel UI")]
    public TMP_InputField registerEmailInputField;
    public TMP_InputField registerPasswordInputField;
    public TMP_InputField registerNameInputField;
    public TMP_InputField registerUsernameInputField;

    private void Start()
    {
        gameObject.SetActive(false);

        emailScreen.SetActive(true);
        passwordScreen.SetActive(false);
        nameScreen.SetActive(false);
        usernameScreen.SetActive(false);
    }

    public void EmailScreenValidateButton()
    {
        eMail = registerEmailInputField.text;

        emailScreen.SetActive(false);
        passwordScreen.SetActive(true);
    }

    public void PasswordScreenValidateButton()
    {
        password = registerPasswordInputField.text;

        passwordScreen.SetActive(false);
        nameScreen.SetActive(true);
    }

    public void NameScreenValidateButton()
    {
        name = registerNameInputField.text;

        nameScreen.SetActive(false);
        usernameScreen.SetActive(true);
    }

    public void UsernameScreenValidateButton()
    {
        username = registerUsernameInputField.text;

        usernameScreen.SetActive(false);

        OnValidateAccountCreation();
    }

    public void OnValidateAccountCreation()
    {
        DatabaseManager.instance.Open();

        bool accountAlreadyExist = false;

        // On demande à vérifier si le compte n'est pas deja existant
        MySqlCommand cmd = new MySqlCommand("SELECT email FROM users WHERE email = '" + eMail + "';", DatabaseManager.instance.connection);
        MySqlDataReader firstReader = cmd.ExecuteReader();
        while (firstReader.Read())
        {
            if (firstReader["email"].ToString() != "")
            {
                Debug.Log($"<color=red> email alerady used ! </color>");
                accountAlreadyExist = true;
            }
        }
        firstReader.Close();

        MySqlCommand cmd2 = new MySqlCommand("SELECT name FROM users WHERE name = '" + name + "';", DatabaseManager.instance.connection);
        MySqlDataReader SecondReader = cmd2.ExecuteReader();
        while (SecondReader.Read())
        {
            if (SecondReader["name"].ToString() != "")
            {
                Debug.Log($"<color=red> name alerady used ! </color>");
                accountAlreadyExist = true;
            }
        }
        SecondReader.Close();


        // On cree un compte
        if (accountAlreadyExist == false)
        {
            string command = "INSERT INTO users VALUES (default, '" + eMail + "', '" + username + "','" + name + "', '" + password + "', default, 0, 0, 0, default);";
            MySqlCommand mySqlCommand_registerUser = new MySqlCommand(command, DatabaseManager.instance.connection);

            try
            {
                mySqlCommand_registerUser.ExecuteReader();
                Debug.Log($"<color=green> register successful ! </color>");
                Connect();
            }
            catch (IOException exeption)
            {
                Debug.Log(exeption.ToString());
            }
        }

        DatabaseManager.instance.connection.Close();
    }

    public void Connect()
    {
        Account.instance.email = eMail;

        print("connection");

        SceneManager.LoadScene("MainScene");
    }

    public void LogInButton()
    {
        logInScreen.SetActive(true);
        gameObject.SetActive(false);
    }
}
