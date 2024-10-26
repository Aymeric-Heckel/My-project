using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddContentMenu : MonoBehaviour
{
    WebCamTexture webcam;
    public RawImage img;

    public GameObject UploadPanel;

    private void Start()
    {
        webcam = new WebCamTexture();
        Debug.Log(webcam.deviceName);
        img.texture = webcam;
        webcam.Play();
    }

    public void OnClickUploadButton()
    {
        UploadPanel.SetActive(true);
    }
}
