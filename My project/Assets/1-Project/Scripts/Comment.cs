using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Comment : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public RectTransform containerRect;
    public LayoutElement layoutElement;

    void Start()
    {
        // Appeler la fonction pour ajuster la taille de l'objet une fois que le texte est initialis�
        AdjustContainerSize();
    }

    void Update()
    {
        // Appeler la fonction � chaque mise � jour pour s'assurer que la taille est toujours correcte
        AdjustContainerSize();
    }

    void AdjustContainerSize()
    {
        // R�cup�rer la taille du texte
        Vector2 textSize = textMeshPro.GetPreferredValues();

        // Ajuster la taille de l'objet conteneur pour s'adapter au texte
        containerRect.sizeDelta = new Vector2(textSize.x, textSize.y);
    }
}
