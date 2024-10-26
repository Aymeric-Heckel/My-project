using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;

public class ProfileVisualizerActionButton : MonoBehaviour
{
    public ProfileVisualizer profileVisualizer;

    #region Follow system

    public void OnClickFollowButton()
    {
        CheckFollow();
    }

    private void CheckFollow()
    {
        DatabaseManager.instance.Open();

        // Vérifier si le follow n'existe pas déjà
        MySqlCommand cmd = new MySqlCommand("SELECT * FROM follows WHERE user = @userId AND userWhoIsFollowing = @followerId", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@userId", ClientAccount.instance.user_id);
        cmd.Parameters.AddWithValue("@followerId", profileVisualizer.userId);
    
        MySqlDataReader reader = cmd.ExecuteReader();

        print("1234");

        // Si on trouve au moins une ligne (le follow existe déjà)
        if (reader.Read())
        {
            print("follow already existing");
        }
        else
        {
            // Si aucune ligne n'est trouvée, créer le follow
            CreateFollow();
            print("createFollow");
        }

    reader.Close();
    DatabaseManager.instance.connection.Close();
    }

    private void CreateFollow()
    {
        DatabaseManager.instance.Open();

        // Ajouter un follow à la table
        MySqlCommand cmd = new MySqlCommand("INSERT INTO follows (user, userWhoIsFollowing) VALUES (@user, @userWhoIsFollowing)", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@user", ClientAccount.instance.user_id);
        cmd.Parameters.AddWithValue("@userWhoIsFollowing", profileVisualizer.userId);

        try
        {
            cmd.ExecuteNonQuery(); // Utilisez ExecuteNonQuery pour une insertion
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }

        if (DatabaseManager.instance.connection.State == System.Data.ConnectionState.Open)
        {
            DatabaseManager.instance.connection.Close();
        }

        UpdateFollowCount();
    }

    private void UpdateFollowCount()
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmdLocalUserUpdate = new MySqlCommand("UPDATE users SET follows_count = follows_count + 1 WHERE user_id = @userId;", DatabaseManager.instance.connection);
        cmdLocalUserUpdate.Parameters.AddWithValue("@userId", ClientAccount.instance.user_id);

        try
        {
            cmdLocalUserUpdate.ExecuteNonQuery();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }

        MySqlCommand cmdFollowUserUpdate = new MySqlCommand("UPDATE users SET followers_count = followers_count + 1 WHERE user_id = @user_id", DatabaseManager.instance.connection);
        cmdFollowUserUpdate.Parameters.AddWithValue("@user_id", profileVisualizer.userId);

        try
        {
            cmdFollowUserUpdate.ExecuteNonQuery();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }

        DatabaseManager.instance.connection.Close();
    }

    #endregion
}