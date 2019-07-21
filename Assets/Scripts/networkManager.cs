using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    //instance
    public static NetworkManager instance;

    void Awake()
    {
        // if an instance already exists and it's not this one - destroy us
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            // set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
}