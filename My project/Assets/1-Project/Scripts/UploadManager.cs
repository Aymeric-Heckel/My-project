using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UploadManager : MonoBehaviour
{
    public VideoEditing videoEditing;

    private Sprite uploadeImage;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnClickUploadImageButton()
    {
        DetectDevice();
    }

    public void DetectDevice()
    {
        // Verifier sur quelle systeme d'exploitation est utilise l'app 
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            Debug.Log("PC or Mac");

            UploadeImage();
        }
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Mobile (Android or iOS) / Tu n'as pas programer la prise en charge de ce device");
        }
    }

    public void ResetUploadManger()
    {
        uploadeImage = null;
    }

    // ------------- Uploade d'une image ---------------
    // Function to open the file browser 
    public void UploadeImage()
    {
        // Show a file picker dialog
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"));
        FileBrowser.SetDefaultFilter(".png");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Show the file browser and handle the result
        FileBrowser.ShowLoadDialog(
            (paths) => { StartCoroutine(LoadImage(paths[0])); },  // Callback for when a file is selected
            () => { Debug.Log("User canceled the dialog"); },     // Callback for when the dialog is canceled
            FileBrowser.PickMode.Files,                           // Pick mode: single file selection
            false,                                                // Allow multiple selection
            null,                                                 // Initial path (optional)
            "Select Image",                                       // Title of the dialog
            "Select"                                              // Confirmation button text
        );
    }

    // Coroutine to load the image from the selected file path
    private IEnumerator LoadImage(string path)
    {
        var www = new WWW("file:///" + path);
        yield return www;
        Texture2D texture = new Texture2D(2, 2);
        www.LoadImageIntoTexture(texture);

        // Convert texture to sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        uploadeImage = sprite;

        videoEditing.SetVideoEditor(sprite);
        gameObject.SetActive(false);
    }
}

