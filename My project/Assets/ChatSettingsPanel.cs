using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSettingsPanel : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void CloseSettingPanel()
    {
        gameObject.SetActive(false);
    }
}
