using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image videoButtonImage;
    public ClientAccount account;
    public SearchVideo searchVideo;
    public ChatMenu chatMenuScr;

    [Header("Sprite")]
    public Sprite loop;
    public Sprite messages;
    public Sprite profil;
    public Sprite video;
    public Sprite loopPlein;
    public Sprite messagesPlein;
    public Sprite profilPlein;
    public Sprite videoPlein;

    [Header("UI")]
    public GameObject proflieMenu;
    public GameObject chatMenu;
    public GameObject videoMenu;
    public GameObject addContnetMenu;
    public GameObject searchMenu;

    public GameObject profileVisualizer;

    [Header("Buttons")]
    public Image profilMenuButton;
    public Image chatMenuButton;
    public Image videoMenuButton;
    public Image addContnetMenuButton;
    public Image searchMenuButton;

    private void Start()
    {
        OnClickVideoMenuButton();
    }

    public void OnClickProfileMenuButton()
    {
        proflieMenu.SetActive(true);
        chatMenu.SetActive(false);
        videoMenu.SetActive(false);
        addContnetMenu.SetActive(false);
        searchMenu.SetActive(false);
        profileVisualizer.SetActive(false);

        profilMenuButton.sprite = profilPlein;
        chatMenuButton.sprite = messages;
        videoMenuButton.sprite = video;
        // addContnetMenuButton.sprite = ;
        searchMenuButton.sprite = loop;

        account.SetProfile();
        StartCoroutine(account.SetVideo());
    }

    public void OnClickChatMenuButton()
    {
        proflieMenu.SetActive(false);
        chatMenu.SetActive(true);
        videoMenu.SetActive(false);
        addContnetMenu.SetActive(false);
        searchMenu.SetActive(false);
        profileVisualizer.SetActive(false);

        profilMenuButton.sprite = profil;
        chatMenuButton.sprite = messagesPlein;
        videoMenuButton.sprite = video;
        // addContnetMenuButton.sprite = ;
        searchMenuButton.sprite = loop;

        chatMenuScr.SetChatMenu();
    }

    public void OnClickVideoMenuButton()
    {
        proflieMenu.SetActive(false);
        chatMenu.SetActive(false);
        videoMenu.SetActive(true);
        addContnetMenu.SetActive(false);
        searchMenu.SetActive(false);
        profileVisualizer.SetActive(false);

        profilMenuButton.sprite = profil;
        chatMenuButton.sprite = messages;
        videoMenuButton.sprite = videoPlein;
        // addContnetMenuButton.sprite = ;
        searchMenuButton.sprite = loop;
    }

    public void OnClickAddContentMenuButton()
    {
        print("Televerser");

        proflieMenu.SetActive(false);
        chatMenu.SetActive(false);
        videoMenu.SetActive(false);
        addContnetMenu.SetActive(true);
        searchMenu.SetActive(false);
        profileVisualizer.SetActive(false);

        profilMenuButton.sprite = profil;
        chatMenuButton.sprite = messages;
        videoMenuButton.sprite = video;
        // addContnetMenuButton.sprite = ;
        searchMenuButton.sprite = loop;
    }

    public void OnClickSearchMenuButton()
    {
        proflieMenu.SetActive(false);
        chatMenu.SetActive(false);
        videoMenu.SetActive(false);
        addContnetMenu.SetActive(false);
        searchMenu.SetActive(true);
        profileVisualizer.SetActive(false);
        
        profilMenuButton.sprite = profil;
        chatMenuButton.sprite = messages;
        videoMenuButton.sprite = video;
        // addContnetMenuButton.sprite = ;
        searchMenuButton.sprite = loopPlein;
    }
}
