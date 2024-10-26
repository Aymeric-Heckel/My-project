using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MySql.Data.MySqlClient;

public class VideoButton : MonoBehaviour
{
    public Image videoImage;
    public TextMeshProUGUI videoNameText;

    private int videoId;
    private GameObject videoVizualizer;

    public void DestroyVideoButton()
    {
        gameObject.transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
        Destroy(gameObject);
        gameObject.transform.parent.GetComponent<GridLayoutGroup>().enabled = true;
    }

    public void SetVideo(int _videoId, GameObject _videoVisualizer)
    {
        videoId = _videoId;
        videoVizualizer = _videoVisualizer;

        StartCoroutine(LoadVideoDataCoroutine());
    }

    private IEnumerator LoadVideoDataCoroutine()
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT video, video_title FROM videos WHERE video_id = @videoId;", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@videoId", videoId);

        MySqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            videoNameText.text = reader["video_title"].ToString();

            if (reader["video"].ToString() != "")
            {
                byte[] imageBytes = (byte[])reader["video"];
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                videoImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("Une erreur est survenue lors de l'import de l'image");
            }

            reader.Close();
        }

        DatabaseManager.instance.connection.Close();

        // Attend la fin de la frame avant de terminer la coroutine
        yield return null;
    }

    public void OnClickVideoButton()
    {
        videoVizualizer.GetComponent<VideoVisualizer>().SetVideo(videoId);
    }
}
