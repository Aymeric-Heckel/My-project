using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    [Header("Server infos")]
    [SerializeField] private string host;
    [SerializeField] private string database;
    [SerializeField] private string username;
    [SerializeField] private string password;

    public MySqlConnection connection;

    private void Awake()
    {
        instance = this;

        if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Open()
    {
        string constr = "Server=" + host + ";DATABASE=" + database + ";User ID=" + username + ";Password=" + password + ";Pooling=true;Charset=utf8;;Connection Timeout=30;";
        
        try
        {
            connection = new MySqlConnection(constr);
            connection.Open();
            Debug.Log($"<color=green>Open connection ! </color>");
        }
        catch (IOException ex)
        {
            Debug.LogException(ex);
        }
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Shutdown connection !");

        if(connection != null && connection.State.ToString() != "Closed")
        {
            connection.Close();
        }
    }
}
