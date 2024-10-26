using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using TMPro;
using UnityEngine.SceneManagement;

public class LogInScreen : MonoBehaviour
{
    public string eMail;
    public string password;

    public GameObject createAccountScreen;

    [Header("LogInPanel UI")]
    public TMP_InputField logInEmailInputField;
    public TMP_InputField logInPasswordInputField;

    private void Start()
    {
        gameObject.SetActive(true);
        createAccountScreen.SetActive(false);
    }

    public void OnClickCreateAccountButton()
    {
        print("click");
        createAccountScreen.SetActive(true);
    }

    public void OnValidateLogIn()
    {
        // On se connect au serveur
        DatabaseManager.instance.Open();

        // Quand on valide, on renseigne toutes les valeurs
        eMail = logInEmailInputField.text;
        password = logInPasswordInputField.text;

        bool checkEmail = false;
        bool checkPassword = false;

        // on check si l'email est dans notre base de donnée
        MySqlCommand cmdCheckEmail = new MySqlCommand("SELECT email FROM users WHERE email = '" + eMail + "';", DatabaseManager.instance.connection);
        MySqlDataReader reader = cmdCheckEmail.ExecuteReader();

        while (reader.Read())
        {
            if (reader["email"].ToString() != "")
            {
                print("check email");
                checkEmail = true;
            }
            else
            {
                Debug.Log($"<color=red> This email dosen't exist ! </color>");
            }
        }
        reader.Close();

        // On check si le password vas avec l'email
        MySqlCommand cmdCheckPassword = new MySqlCommand("SELECT password FROM users WHERE email = '" + eMail + "';", DatabaseManager.instance.connection);
        MySqlDataReader reader2 = cmdCheckPassword.ExecuteReader();

        while (reader2.Read())
        {
            if (reader2["password"].ToString() == password)
            {
                print("check password");
                checkPassword = true;
            }
            else
            {
                Debug.Log($"<color=red> The password is incorrect ! </color>");
            }
        }
        reader2.Close();

        // Si les deux paramètres si dessus ont été vaalidés on connect le client
        if (checkEmail == true && checkPassword == true)
        {
            Connect();
        }

        // On ferme la connection au serveur
        DatabaseManager.instance.connection.Close();
    }

    public void Connect()
    {
        Account.instance.email = eMail;

        print("connection");

        SceneManager.LoadScene("MainScene");
    }
}
