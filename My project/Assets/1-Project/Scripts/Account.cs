using UnityEngine;

public class Account : MonoBehaviour 
{
    public static Account instance;

    public string email;

    private void Awake()
    {
        instance = this;

        if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
