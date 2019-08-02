using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    [HideInInspector]
    public bool gameEnded = false;              
    public float timeToWin;                              
    public float invincibleDuration;                   
    private float hatPickupTime;                       

    [Header("Players")]
    public string playerPrefabLocation;            
    public Transform[] spawnPoints;              
    [HideInInspector]
    public PlayerController[] players;          
    [HideInInspector]
    public int playerWithHat;                        
    private int playersInGame;                      

    [Header("Components")]
    public PhotonView photonView;

    public static GameManager instance;

    void Awake ()
    {
        instance = this;
    }

    void Start ()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame ()
    {
        playersInGame++;

        if(playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }

    void SpawnPlayer()
    {
        
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, 0);

        
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public bool CanGetHat ()
    {
        if(Time.time > hatPickupTime + invincibleDuration) return true;
        else return false;
    }

    [PunRPC]
    public void GiveHat (int playerId, bool initialGive = false)
    {
        if(!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickupTime = Time.time;
    }

    public PlayerController GetPlayer (int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer (GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }

    [PunRPC]
    void WinGame (int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu ()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}