using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MySql.Data.MySqlClient;
using Unity.VisualScripting;
using System.IO;

public class ModifyProfile : MonoBehaviour
{
    public ClientAccount account;

    public Sprite emptyProfileImageSprite;
    public Sprite profileImageSprite;

    [Header("UI")]
    public Image profileImage;
    public TMP_InputField changeUsernameInputfield;
    public TMP_InputField changeBioInputfield;
    public TMP_InputField changePasswordInputfield;

    // initialisation des stats
    public void OnOpenModifyProfilPanel()
    {
        gameObject.SetActive(true);

        int _user_id = account.user_id;

        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE user_id = '" + _user_id + "';", DatabaseManager.instance.connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            profileImageSprite = GetProfileImage(_user_id);

            // On renseigne toute la partie Text
            changeUsernameInputfield.text = reader["username"].ToString();
            changeBioInputfield.text = reader["biographie"].ToString();
            changePasswordInputfield.text = reader["password"].ToString();

            /*publicationsCountText.text = _account.pubicationCount.ToString();
            followersCountText.text = _account.followersCount.ToString();
            likesCountText.text = _account.likesCount.ToString();*/
        }

        reader.Close();
        DatabaseManager.instance.connection.Close();

        if (account.profileImageSprite != null)
        {
            profileImage.sprite = profileImageSprite;
        }
        else
        {
            profileImage.sprite = emptyProfileImageSprite;
        }
    }

    public Sprite GetProfileImage(int userId)
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT profile_image FROM users WHERE user_id = @id;", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@id", userId);
        MySqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            if (reader["profile_image"].ToString() != "")
            {
                byte[] imageBytes = (byte[])reader["profile_image"];
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                reader.Close();
                DatabaseManager.instance.connection.Close();
                return sprite;
            }
            else
            {
                return null;
            }
        }
        else
        {
            Debug.LogError("Profile image not found.");
            reader.Close();
            DatabaseManager.instance.connection.Close();
            return null;
        }
    }


    public void OnCloseModifyProfilPanel()
    {
        gameObject.SetActive(false);
    }

    public void ApplyModifications()
    {
        // faire les modifications en local
        account.username = changeUsernameInputfield.text;
        account.biographie = changeBioInputfield.text;
        account.password = changePasswordInputfield.text;

        // envoyer les modifications sur le serveur
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("UPDATE users SET username = @u, biographie = @b, password = @p WHERE user_id = @id; ", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@id", account.user_id);
        cmd.Parameters.AddWithValue("@u", changeUsernameInputfield.text);
        cmd.Parameters.AddWithValue("@b", changeBioInputfield.text);
        cmd.Parameters.AddWithValue("@p", changePasswordInputfield.text);

        try
        {
            cmd.ExecuteReader();
        }
        catch(IOException e) 
        {
            Debug.Log(e);
        }

        DatabaseManager.instance.connection.Close();

        account.SetProfile();
    }
}
