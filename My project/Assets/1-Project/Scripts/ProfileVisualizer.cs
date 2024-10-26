using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using TMPro;
using System.Threading;

public class ProfileVisualizer : MonoBehaviour
{
    public RectTransform referenceRect;

    public int userId;

    public GameObject videoButtonPrefab;
    public GameObject videoVisualizer;
    public GameObject videoScrollViewContent;

    [Header("Profile UI")]
    public Image profileImage;
    public TextMeshProUGUI usernameText;

    public TextMeshProUGUI publicationsCountText;
    public TextMeshProUGUI followersCountText;
    public TextMeshProUGUI likesCountText;

    Sprite profileImageSprite;
    public Sprite emptyProfileImageSprite;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetProfile(int user_id)
    {
        userId = user_id;

        DatabaseManager.instance.Open();

        gameObject.SetActive(true);

        // Copiez les propriétés de position (left, right, top, bottom)
        gameObject.GetComponent<RectTransform>().offsetMin = referenceRect.offsetMin;
        gameObject.GetComponent<RectTransform>().offsetMax = referenceRect.offsetMax;

        MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE user_id = '" + user_id + "';",  DatabaseManager.instance.connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            profileImageSprite = GetProfileImage(user_id);

            // On renseigne toute la partie Text
            usernameText.text = reader["username"].ToString();

            /*publicationsCountText.text = _account.pubicationCount.ToString();
            followersCountText.text = _account.followersCount.ToString();
            likesCountText.text = _account.likesCount.ToString();*/
        }

        // On renseigne la photo de profile
        if (profileImageSprite != null)
        {
            profileImage.sprite = profileImageSprite;
        }
        else
        {
            profileImage.sprite = emptyProfileImageSprite;
        }

        // Supprimer les videos charges sur le ProfileVisualizer
        for (int i = 0; i < videoScrollViewContent.transform.childCount; i++)
        {
            videoScrollViewContent.transform.GetChild(i).GetComponent<VideoButton>().DestroyVideoButton();
        }

        SetVideo();
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

    public void SetVideo()
    {
        StartCoroutine(LoadVideosCoroutine());
    }

    private IEnumerator LoadVideosCoroutine()
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT video_id FROM videos WHERE publisher_id = @userId;", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@userId", userId);

        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int videoID = int.Parse(reader["video_id"].ToString());

            // Crée la partie visuelle
            GameObject ins = Instantiate(videoButtonPrefab);
            ins.transform.parent = videoScrollViewContent.transform;

            VideoButton insVideoButton = ins.GetComponent<VideoButton>();
            insVideoButton.SetVideo(videoID, videoVisualizer);

            ins.GetComponent<RectTransform>().localScale = Vector3.one;

            // Attend la fin de la frame avant de continuer (si tu as beaucoup de vidéos, cela évite de tout charger d'un coup)
            yield return null;
        }

        reader.Close();
        DatabaseManager.instance.connection.Close();
    }

#region Swipe System
// ------------- Système de swipe ------------------
[Header("Swipe Systeme")]
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    public float swipeThreshold = 300f;
    public float swipeSpeed = 0.1f; // Vitesse de déplacement du RectTransform lors du swipe

    public RectTransform swipeArea; // La zone dans laquelle le swipe est autorisé



    void Update()
    {
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
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    fingerUpPosition = touch.position;
                    MoveSwipeArea();
                    CheckSwipe();
                }
            }

            // Gestion du swipe avec la souris (pour le développement)
            if (Input.GetMouseButtonDown(0))
            {
                fingerDownPosition = Input.mousePosition;
                fingerUpPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                fingerUpPosition = Input.mousePosition;
                MoveSwipeArea();
                CheckSwipe();
            }
        }
    }

    void MoveSwipeArea()
    {
        // Déplace le RectTransform vers le haut ou vers le bas en fonction du mouvement du swipe
        float swipeDeltaY = fingerUpPosition.y - fingerDownPosition.y;

        if (swipeDeltaY < 0) // Vérifie si le swipe est vers le bas
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, swipeDeltaY * swipeSpeed);
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
            //fermer le profile visualizer
            gameObject.SetActive(false);
        }
    }

    float SwipeDistance()
    {
        return Vector2.Distance(fingerDownPosition, fingerUpPosition);
    }
    #endregion
}
