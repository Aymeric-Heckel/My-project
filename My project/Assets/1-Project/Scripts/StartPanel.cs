using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : MonoBehaviour
{
    public GameObject registerPanel;
    public GameObject LogInPanel;

    private void Start()
    {
        gameObject.SetActive(true);
        registerPanel.SetActive(false);
        LogInPanel.SetActive(false);
    }

    public void RegisterButton()
    {
        gameObject.SetActive(false);
        registerPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void LogInButton()
    {
        gameObject.SetActive(false);
        LogInPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
