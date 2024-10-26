using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Video : MonoBehaviour
{
    private VideoDisplay display;

    public int likes;

    public int videoID;

    [Header("Publisher Infos")]
    public string publisherName;
    public Image publisherProfile;

    [Header("Video Info")]
    public string videoName;
    public Sprite image;
    public List<string> comments = new List<string>();

    internal void DestroyVdieo()
    {
        Destroy(gameObject);
    }

    public void SetVideo()
    {
        display = gameObject.transform.parent.GetComponent<VideoDisplay>();
        //display.ResetVideoDisplay(this);
    }
}
