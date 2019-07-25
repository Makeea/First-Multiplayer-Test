using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
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

   
    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected to master Server");
    //    CreateRoom("testroom");

    //}

    //Attempts to Create a room
    public void CreateRoom (string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    //public override void OnCreatedRoom()
    //{
    //    Debug.Log("Created Room:  " + PhotonNetwork.CurrentRoom.Name);

    //}


    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}