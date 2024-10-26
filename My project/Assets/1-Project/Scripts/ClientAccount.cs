using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using SimpleFileBrowser;
using MySql.Data.MySqlClient;
using System.IO;
using Mysqlx.Session;

public class ClientAccount : MonoBehaviour
{
    public static ClientAccount instance;

    [Header("user infos")]
    public int user_id;

    public string eMail;
    public string username;
    public string name;
    public string password;
    public string biographie;

    public List<Video> videosUplaodes = new List<Video>();

    public bool already = false;

    public int publicationsCount;
    public int followersCount;
    public int likesCount;

    [Header("UI References")]
    public Sprite emptyProfileImageSprite;
    public Sprite profileImageSprite;

    [Header("PrefabsR References")]
    public GameObject videoButtonPrefab;

    [Header("GameObject References")]
    public GameObject mainMenu;
    public GameObject createAccountPanel;
    public GameObject logInPanel;
    public GameObject videoScrollViewContent;
    public GameObject videoVisualizer;

    [Header("CreateAccountPanel UI")]
    public InputField registerEmailInputField;
    public InputField registerUsernameInputField;
    public InputField registerNameInputField;
    public InputField registerPasswordInputField;

    [Header("LogInPanel UI")]
    public InputField logInEmailInputField;
    public InputField logInPasswordInputField;

    [Header("Proflie Menu UI")]
    public Image profileImage;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI bioText;
    public TextMeshProUGUI publicationsCountText;
    public TextMeshProUGUI followersCountText;
    public TextMeshProUGUI likesCountText;

    [Header("Scripts References")]
    public ChatMenu chatMenu;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainMenu.SetActive(true);

        eMail = Account.instance.email;
        SetProfile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetProfile();
        }
    }

    public void SetProfile()
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE email = '" + eMail + "';", DatabaseManager.instance.connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            user_id = int.Parse(reader["user_id"].ToString());

            eMail = reader["email"].ToString();
            username = reader["username"].ToString();
            name = reader["name"].ToString();
            password = reader["password"].ToString();
            biographie = reader["biographie"].ToString();

            publicationsCount = int.Parse(reader["publications_count"].ToString());
            followersCount = int.Parse(reader["followers_count"].ToString());
            likesCount = int.Parse(reader["follows_count"].ToString());

            profileImageSprite = GetProfileImage(user_id);
        }

        reader.Close();
        DatabaseManager.instance.connection.Close();

        if (profileImageSprite == null)
        {
            profileImage.sprite = emptyProfileImageSprite;
            chatMenu.profileImageSprite = emptyProfileImageSprite;
        }
        else
        {
            profileImage.sprite = profileImageSprite;
            chatMenu.profileImageSprite = profileImageSprite;
        }

        // Profil menu instantiate
        usernameText.text = username;
        bioText.text = biographie;

        publicationsCountText.text = publicationsCount.ToString();
        followersCountText.text = followersCount.ToString();
        likesCountText.text = likesCount.ToString();
    }

    public Sprite GetProfileImage(int userId)
    {
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("SELECT profile_image FROM users WHERE user_id = @id;", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@id", userId);
        MySqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            if (reader["profile_image"].ToString() != "")
            {
                byte[] imageBytes = (byte[])reader["profile_image"];
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                reader.Close();
                DatabaseManager.instance.connection.Close();
                return sprite;
            }
            else
            {
                return null; 
            }
        }
        else
        {
            Debug.LogError("Profile image not found.");
            reader.Close();
            DatabaseManager.instance.connection.Close();
            return null;
        }
    }

    public IEnumerator SetVideo()
    {
        if(already != true)
        {
            already = true;

            DatabaseManager.instance.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT video_id FROM videos WHERE publisher_id = '" + user_id + "';", DatabaseManager.instance.connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // Cr�e la partie viduel
                GameObject ins = Instantiate(videoButtonPrefab);
                ins.transform.parent = videoScrollViewContent.transform;
                ins.GetComponent<VideoButton>().SetVideo(int.Parse(reader["video_id"].ToString()), videoVisualizer);
                ins.GetComponent<RectTransform>().localScale = Vector3.one;
            }

            DatabaseManager.instance.connection.Close();
        }

        // Attend la fin de la frame avant de terminer la coroutine
        yield return null;
    }

    public void Connect()
    {
        mainMenu.SetActive(true);
        createAccountPanel.SetActive(false);

        SetProfile();
    }

    #region Changer l'image de profil
    public void ChangeProfileImage()
    {
        // Verifier sur quelle systeme d'exploitation est utilise l'app 
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            Debug.Log("PC or Mac");


            PcChangeProfileImage();
        }
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Mobile (Android or iOS)");


            MobileChangeProfileImage();
        }
    }

    // S�l�ctionner une image depuis l'app photo sur l'appareil
    public void MobileChangeProfileImage()
    {
        // Appel d'une fonction native pour s�lectionner une image � partir de la galerie du t�l�phone
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null) // V�rifie si une image a �t� s�lectionn�e avec succ�s
            {
                // Charge l'image � partir du chemin d'acc�s dans une texture
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 512, false, false);

                if (texture == null)
                {
                    Debug.Log("Impossible de charger l'image !");
                    return;
                }

                // Convertit la texture en sprite pour l'afficher dans l'objet Image
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                // Affiche le sprite dans l'objet Image
                profileImageSprite = sprite;
                InsertProfileImage(user_id, sprite);

                SetProfile();
            }
        }, "S�lectionnez une image");

        // V�rifie si l'autorisation d'acc�s � la galerie a �t� refus�e
        if (permission == NativeGallery.Permission.Denied)
        {
            Debug.Log("Permission d'acc�s � la galerie refus�e !");
        }
        else
        {
            Debug.Log("L'application ne fonctionne pas sur une plateforme mobile !");
        }
    }


    // S�l�ctionner une image depuis l'explorateur de fichiers
    public void PcChangeProfileImage()
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

        profileImageSprite = sprite;
        InsertProfileImage(user_id, sprite);

        SetProfile();
    }

    public void InsertProfileImage(int userId, Sprite sprite)
    {
        byte[] spriteBytes = SpriteToBytes(sprite);
        DatabaseManager.instance.Open();

        MySqlCommand cmd = new MySqlCommand("UPDATE users SET profile_image = @profile_image WHERE user_id = @id;", DatabaseManager.instance.connection);
        cmd.Parameters.AddWithValue("@profile_image", spriteBytes);
        cmd.Parameters.AddWithValue("@id", userId);

        try
        {
            cmd.ExecuteNonQuery();
            Debug.Log("Profile image inserted successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to insert profile image: " + ex.Message);
        }
        finally
        {
            DatabaseManager.instance.connection.Close();
        }
    }
    private byte[] SpriteToBytes(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        // Create a new Texture2D and copy the original texture into it
        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        newTexture.SetPixels(texture.GetPixels());
        newTexture.Apply();

        //CompressionTest(); // On vas tester la difference de compression (poid image)

        return newTexture.EncodeToJPG(50); // 75 is the quality parameter (0 to 100)
    }
    #endregion

    #region Test de difference de compression
    /*
    void CompressionTest()
    {
        byte[] originalBytes = SpriteToBytesPNG(profileImageSprite);
        byte[] compressedBytes = SpriteToBytesJPEG(profileImageSprite);

        Debug.Log("Original Size (PNG): " + originalBytes.Length/8 + " octet");
        Debug.Log("Compressed Size (JPEG): " + compressedBytes.Length/8 + " octet");
        Debug.Log("Compression Ratio: " + (float)originalBytes.Length / compressedBytes.Length);
    }

    private byte[] SpriteToBytesPNG(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        return texture.EncodeToPNG();
    }

    private byte[] SpriteToBytesJPEG(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        newTexture.SetPixels(texture.GetPixels());
        newTexture.Apply();
        return newTexture.EncodeToJPG(50);
    }
    */
    #endregion
}

