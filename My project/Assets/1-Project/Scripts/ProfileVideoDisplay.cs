using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MySql.Data.MySqlClient;
using UnityEditor;
using System;
using Mysqlx.Crud;
using Unity.VisualScripting;

public class ProfileVideoDisplay : MonoBehaviour
{

    public CommentsPanel commentPanel;

    public Image image;
    public TextMeshProUGUI publisherNameText;
    public TextMeshProUGUI videoNameText;

    public Image likeButtonImage;

    [Header("Video Infos")]
    public int videoId;

    public Sprite videoData;
    public string publisherName;
    public string videoName;

    public bool isLiked;


    public void ResetVideoDisplay(int video_id/*Sprite _videoData, string _publisherName, string _videoName*/)
    {
        videoId = video_id;

        DatabaseManager.instance.Open();

        MySqlCommand mySqlCommand = new MySqlCommand("SELECT video, publisher_name, video_title FROM videos WHERE video_id = '" + videoId + "';", DatabaseManager.instance.connection);
        MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

        if (mySqlDataReader.Read())
        {
            if (mySqlDataReader["video"].ToString() != "")
            {
                byte[] imageBytes = (byte[])mySqlDataReader["video"];
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                videoData = sprite;
            }
            else
            {
                Debug.LogError("Une erruer est survenue lors de l'import de l'image, video id : " + videoId);
            }


            publisherName = mySqlDataReader["publisher_name"].ToString();
            videoName = mySqlDataReader["video_title"].ToString();
        }

        mySqlDataReader.Close();

        commentPanel.gameObject.SetActive(false);


        MySqlCommand cmd = new MySqlCommand("SELECT video_id FROM videos_likes WHERE video_id = '" + videoId + "' AND user_id = '" + ClientAccount.instance.user_id + "';", DatabaseManager.instance.connection); ;
        MySqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            likeButtonImage.color = Color.magenta;
            isLiked = true;
            reader.Close();
        }
        else
        {
            likeButtonImage.color = Color.white;
            isLiked = false;
            reader.Close();
        }

        DatabaseManager.instance.connection.Close();

        SetNewVideo();
    }

    private void SetNewVideo()
    {
        image.color = Color.white;
        image.sprite = videoData;
        publisherNameText.text = publisherName;
        videoNameText.text = videoName;
    }

    public void OpenCommentsPanel()
    {
        commentPanel.SetCommentsPanel(videoId);
    }

    public void LikeVideoButton()
    {
        if (isLiked == true)
        {
            DislikeVideo();
            return;
        }

        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("INSERT INTO videos_likes (video_id, user_id) VALUES (@video_id, @user_id)", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@video_id", videoId);
        cmd.Parameters.AddWithValue("@user_id", ClientAccount.instance.user_id);

        try
        {
            cmd.ExecuteNonQuery();
            isLiked = true;
            likeButtonImage.color = Color.magenta;
            Debug.Log("Like ajoutée avec succès à la base de données.");
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur lors de l'insertion du like : " + e);
        }

        DatabaseManager.instance.connection.Close();
    }

    public void DislikeVideo()
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("DELETE FROM videos_likes WHERE video_id = @video_id AND user_id = @user_id", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@video_id", videoId);
        cmd.Parameters.AddWithValue("@user_id", ClientAccount.instance.user_id);

        try
        {
            cmd.ExecuteNonQuery();
            isLiked = false;
            likeButtonImage.color = Color.white;
            Debug.Log("Like supprimée avec succès à la base de données.");
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur lors de la suppressoin du like : " + e);
        }

        DatabaseManager.instance.connection.Close();
    }
}
