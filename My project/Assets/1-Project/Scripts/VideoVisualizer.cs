using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoVisualizer : MonoBehaviour
{
    public RectTransform referenceRect;
    public ProfileVideoDisplay display;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetVideo(int _videoId)
    {
        print("SetVideo");

        gameObject.SetActive(true);

        // Copiez les propri�t�s de position (left, right, top, bottom)
        gameObject.GetComponent<RectTransform>().offsetMin = referenceRect.offsetMin;
        gameObject.GetComponent<RectTransform>().offsetMax = referenceRect.offsetMax;

        display.ResetVideoDisplay(_videoId);
    }

    public void OnClickCloseVideoVizualizer()
    {
        gameObject.SetActive(false);  
    }

    #region Swipe system
    // ----------- Systeme de swipe ----------

    [Header("Swipe Systeme")]
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    public float swipeThreshold = 300f;
    public float swipeSpeed = 0.1f; // Vitesse de d�placement du RectTransform lors du swipe

    public RectTransform swipeArea; // La zone dans laquelle le swipe est autoris�

    void Update()
    {
        // V�rifie si le clic ou le toucher se produit dans la zone autoris�e
        if (IsInsideSwipeArea())
        {
            // Gestion du swipe sur l'�cran tactile
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

            // Gestion du swipe avec la souris (pour le d�veloppement)
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
        // D�place le RectTransform vers le haut ou vers le bas en fonction du mouvement du swipe
        float swipeDeltaY = fingerUpPosition.y - fingerDownPosition.y;

        if (swipeDeltaY < 0) // V�rifie si le swipe est vers le bas
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, swipeDeltaY * swipeSpeed);
        }
    }

    bool IsInsideSwipeArea()
    {
        // Convertit les coins du RectTransform en coordonn�es de l'�cran
        Vector3[] corners = new Vector3[4];
        swipeArea.GetWorldCorners(corners);

        Rect swipeRect = new Rect(corners[0], corners[2] - corners[0]);

        // V�rifie si le toucher ou le clic de la souris se produit � l'int�rieur de la zone rectangulaire
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
