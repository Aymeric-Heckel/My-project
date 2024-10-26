using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMenu : MonoBehaviour
{
    public Sprite profileImageSprite;

    public MainMenu mainMenu;
    public GameObject chatSettingsPanel;
    public Image profileImage;

    public void SetChatMenu()
    {
        profileImage.sprite = profileImageSprite;
    }

    public void OnClickProfileImage()
    {
        gameObject.SetActive(false);
        mainMenu.OnClickProfileMenuButton();
    }

    public void OnClickChatSettingsButton()
    {
        chatSettingsPanel.SetActive(true);
    }
}