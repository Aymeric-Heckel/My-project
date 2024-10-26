using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;

public class SearchVideo : MonoBehaviour
{
    public List<GameObject> loadedVideo = new List<GameObject>();

    public int videoToLoadInOneTime = 10;

    public GameObject videoDispaly;

    private void Start()
    {
        // LoadVideo();
    }

    public void LoadVideo()
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT video_id FROM videos ORDER BY rand() LIMIT 1;", DatabaseManager.instance.connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        int _video_id = -1;

        while (reader.Read())
        {
            _video_id = int.Parse(reader["video_id"].ToString());
        }

        DatabaseManager.instance.connection.Close();

        if(_video_id != -1)
        {
            ImportVideo(_video_id);
        }
        else
        {
            LoadVideo();
        }
    }

    private void ImportVideo(int _videoId)
    {
        int videoId = _videoId;
        Sprite videoData;
        string videoTitle = "";
        string publisherName = "";
        int publisherId;
        int views = 0;

        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT * FROM videos WHERE video_id = '" +  videoId + "';", DatabaseManager.instance.connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) 
        {
            videoTitle = reader["video_title"].ToString();
            publisherName = reader["publisher_name"].ToString();
            publisherId = int.Parse(reader["publisher_id"].ToString());
        }

        Sprite sprite = GetVideoData(videoId);
        videoData = sprite;

        if (videoTitle == "" || publisherName == "")
        {
            LoadVideo();
        }
        else
        {
            VideoDisplay.instance.ResetVideoDisplay(videoId, videoData, publisherName, videoTitle);
        }
    }

    public Sprite GetVideoData(int videoId)
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT video FROM videos WHERE video_id = @id;", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@id", videoId);
        MySqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            if (reader["video"].ToString() != "")
            {
                byte[] imageBytes = (byte[])reader["video"];
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


    #region Swipe system
    // ------------- Système de swipe ------------------
    [Header("Swipe Systeme")]
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    public float swipeThreshold = 300f;

    public RectTransform swipeArea; // La zone dans laquelle le swipe est autorisé

    private bool canSwipe = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            LoadVideo();
        }

        // Vérifie si le clic ou le toucher se produit dans la zone autorisée
        if (IsInsideSwipeArea())
        {
            // Gestion du swipe sur l'écran tactile
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    fingerDownPosition = touch.position;
                    fingerUpPosition = touch.position;
                    canSwipe = true;
                }

                if (touch.phase == TouchPhase.Moved && canSwipe == true)
                {
                    fingerUpPosition = touch.position;
                    CheckSwipe();
                }
            }

            // Gestion du swipe avec la souris (pour le développement)
            if (Input.GetMouseButtonDown(0))
            {
                fingerDownPosition = Input.mousePosition;
                fingerUpPosition = Input.mousePosition;
                canSwipe = true;
            }

            if (Input.GetMouseButton(0) && canSwipe == true)
            {
                fingerUpPosition = Input.mousePosition;
                CheckSwipe();
            }
        }
    }

    bool IsInsideSwipeArea()
    {
        // Convertit les coins du RectTransform en coordonnées de l'écran
        Vector3[] corners = new Vector3[4];
        swipeArea.GetWorldCorners(corners);

        Rect swipeRect = new Rect(corners[0], corners[2] - corners[0]);

        // Vérifie si le toucher ou le clic de la souris se produit à l'intérieur de la zone rectangulaire
        return swipeRect.Contains(Input.mousePosition);
    }

    void CheckSwipe()
    {
        if (SwipeDistance() > swipeThreshold)
        {
            canSwipe = false;
            LoadVideo();
        }
    }

    float SwipeDistance()
    {
        return Vector2.Distance(fingerDownPosition, fingerUpPosition);
    }
    #endregion
}
