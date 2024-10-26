using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoEditing : MonoBehaviour
{
    public ClientAccount account;
    public UploadManager uploadManager;

    public Image imageVizualizer;
    public TMP_InputField titleInputField;

    public Sprite uploadVideo;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetVideoEditor(Sprite _image)
    {
        gameObject.SetActive(true);
        titleInputField.text = null;
        imageVizualizer.sprite = _image;
        uploadVideo = _image;
    }

    public void OnClickPublishButton()
    {
        if(titleInputField.text == "")
        {
            Debug.Log("La video publie n'a pas de titre");
            return;
        }

        VideoManager.instance.AddVideo(uploadVideo, titleInputField.text, account.name, account);
        
        uploadManager.ResetUploadManger();

        gameObject.SetActive(false);
    }
}
