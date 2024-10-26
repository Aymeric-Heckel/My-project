using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Collections;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;

public class VideoManager : MonoBehaviour
{
    public GameObject videoPrefab;
    public int videosID;

    public static VideoManager instance;

    public byte[] videoData;
    public string videoName;
    public string publisherUsername;
    public int publisherId;

    private void Awake()
    {
        instance = this;
    }

    public void AddVideo(Sprite _image, string _videoName, string _publisherName, ClientAccount publisherAccount)
    {
        videoData = SpriteToBytes(_image);
        videoName = _videoName;
        publisherUsername = _publisherName;
        publisherId = publisherAccount.user_id;

        print(_videoName + " " + _publisherName);

        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("INSERT INTO videos (video, video_title, publisher_name, publisher_id) VALUES (@video, @video_title, @publisher_name, @publisher_id)", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@video", videoData);
        cmd.Parameters.AddWithValue("@video_title", videoName);
        cmd.Parameters.AddWithValue("@publisher_name", publisherUsername);
        cmd.Parameters.AddWithValue("@publisher_id", publisherId);

        try
        {
            cmd.ExecuteReader();
        }
        catch (IOException exeption) 
        {
            Debug.Log(exeption);
        }

        DatabaseManager.instance.connection.Close();

        UpdatePublishCount();
    }

    private byte[] SpriteToBytes(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        // Create a new Texture2D and copy the original texture into it
        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        newTexture.SetPixels(texture.GetPixels());
        newTexture.Apply();

        //CompressionTest(); // On vas tester la difference de compression (poid image)

        return newTexture.EncodeToJPG(50); // 75 is the quality parameter (0 to 100)
    }

    public void UpdatePublishCount()
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("UPDATE users SET publications_count = publications_count + 1 WHERE user_id = @userId;", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@userId", ClientAccount.instance.user_id);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }

        DatabaseManager.instance.connection.Close();
    }
}
