using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;

public class SearchProfile : MonoBehaviour
{
    public List<Account> searchResponces = new List<Account>();

    public TMP_InputField searchInputField;

    public GameObject client;

    public TextMeshProUGUI notResultText;

    [Header("UI")]
    public GameObject responseOfChercheParent;
    public GameObject profileLabelPrefab;

    public GameObject profileVisualizer;

    public void OnClickSearchButton()
    {
        profileVisualizer.SetActive(false);
        Search(searchInputField.text);
    }

    public void Search(string text)
    {
        if (text == "")
        {
            notResultText.gameObject.SetActive(true);
            return;
        }

        // On retire toutes les étiquettes deja affiches
        if (responseOfChercheParent.transform.childCount != 0)
        {
            foreach (Transform child in responseOfChercheParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        Check(text);
    }

    private void Check(string text)
    {
        DatabaseManager.instance.Open();
        
        MySqlCommand cmd = new MySqlCommand("SELECT user_id, username, name, profile_image FROM users WHERE name LIKE '" + text + "%';", DatabaseManager.instance.connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int user_id = int.Parse(reader["user_id"].ToString());
            string username = reader["username"].ToString();
            string name = reader["name"].ToString();
            Sprite profileImageSprite;

            if (reader["profile_image"].ToString() != "")
            {
                byte[] imageBytes = (byte[])reader["profile_image"];
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                profileImageSprite = sprite;
            }
            else
            {
                profileImageSprite = null;
            }

            Display(user_id, username, name, profileImageSprite);
        }

        reader.Close();

        DatabaseManager.instance.connection.Close();
    }

    private void Display(int user_id, string username, string name, Sprite profileImage)
    {
        GameObject ins = Instantiate(profileLabelPrefab);
        ins.transform.parent = responseOfChercheParent.transform;
        ins.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        ins.GetComponent<ProfileLabel>().SetProfileLabel(user_id, username, name, profileImage, profileVisualizer);
    }
}
