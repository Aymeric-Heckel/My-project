using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;
using System;
using System.IO;

public class CommentsPanel : MonoBehaviour
{
    public RectTransform referenceRect;
    public GameObject commentPrefab;

    private int videoId;

    public GameObject commentsZone;

    public TMP_InputField commentText;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetCommentsPanel(int _video)
    {
        print("SetCommentsPanel");

        gameObject.SetActive(true);

        // Copiez les propriétés de position (left, right, top, bottom)
        gameObject.GetComponent<RectTransform>().offsetMin = referenceRect.offsetMin;
        gameObject.GetComponent<RectTransform>().offsetMax = referenceRect.offsetMax;
    
        videoId = _video;

        DisplayComments();

        commentText.text = "";
    }

    private void DisplayComments()
    {
        for(int i = 0; i < commentsZone.transform.childCount; i++)
        {
            GameObject commentTodestroy = commentsZone.transform.GetChild(i).gameObject;
            Destroy(commentTodestroy);
        }

        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT * FROM videos_comments WHERE video_id = " + videoId + " ORDER BY date DESC;", DatabaseManager.instance.connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            GameObject _commentPrefab = Instantiate(commentPrefab);
            _commentPrefab.GetComponent<TextMeshProUGUI>().text = reader["content"].ToString();
            _commentPrefab.transform.parent = commentsZone.transform;
            _commentPrefab.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        reader.Close();

        DatabaseManager.instance.connection.Close();
    }

    public void OnCkickAddCommentButton()
    {
        if(commentText.text != "")
        {
            int user_id = ClientAccount.instance.user_id;
            string content = commentText.text;

            DatabaseManager.instance.Open();

            MySqlCommand cmd = new MySqlCommand("INSERT INTO videos_comments (video_id, publisher_id, content) VALUES (@video_id, @publisher_id, @content)",DatabaseManager.instance.connection);
            cmd.Parameters.AddWithValue("@video_id", videoId);
            cmd.Parameters.AddWithValue("@publisher_id", user_id);
            cmd.Parameters.AddWithValue("@content", content);

            try
            {
                cmd.ExecuteNonQuery();
                Debug.Log("Vidéo ajoutée avec succès à la base de données.");
            }
            catch (Exception e)
            {
                Debug.LogError("Erreur lors de l'insertion du commentaire : " + e);
            }

            DatabaseManager.instance.connection.Close();

            commentText.text = "";
        }
    }

    #region Systeme de swipe
    // ----------- Systeme de swipe ----------
    [Header("Swipe System")]
    public float swipeThreshold = 300f;
    public float swipeSpeed = 0.1f; // Vitesse de déplacement du RectTransform lors du swipe

    public RectTransform swipeArea; // La zone dans laquelle le swipe est autorisé

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

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
