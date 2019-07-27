using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invincibleDuration;
    private float hatPickupTime;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    private int playersInGame;

    //instance
    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        // is all players in the scene?
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    // spawns a player and initilizes them
    void SpawnPlayer()
    {
        // Instantiates the player across the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        // add PlayerControllor script to each player
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        // initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    [PunRPC]
    public void GiveHat(int playerId, bool initialGive = false)
    {
        // remove the hat from the currently hatted player
        if (!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        // give the hat to the new player
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupTime = Time.time;
    }

    // player able to take hat at this time?
    public bool CanGetHat()
    {
        if (Time.time > hatPickupTime + invincibleDuration) return true;
        else return false;
    }

    [PunRPC]
    void GameWon(int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        //show who won

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu ()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
