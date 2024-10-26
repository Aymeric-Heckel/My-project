using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileLabel : MonoBehaviour
{
    private int user_id;
    private string username;
    private string name;
    private Sprite profileImageSprite;

    [Header("UI")]
    public Image profileImage;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI nameText;

    [Header("Profile Visualizer UI")]
    private GameObject profileVisualizer;

    public Sprite emptyProfileImageSprite;

    public void SetProfileLabel(int _user_id, string _username, string _name, Sprite _profileImageSprite, GameObject _profileVisualizer)
    {
        user_id = _user_id;
        username = _username;
        name = _name;
        profileImageSprite = _profileImageSprite;

        profileVisualizer = _profileVisualizer;

        // On assigne le photo de profile
        if (profileImageSprite != null)
        {
            profileImage.sprite = profileImageSprite;
        }
        else
        {
            profileImage.sprite = emptyProfileImageSprite;
        }

        // On assigne le num d'utilisateur et le nom
        usernameText.text = username;
        nameText.text = name;
    }

    public void SeeProfile()
    {
        profileVisualizer.gameObject.SetActive(true);
        profileVisualizer.GetComponent<ProfileVisualizer>().SetProfile(user_id);
    }
}

