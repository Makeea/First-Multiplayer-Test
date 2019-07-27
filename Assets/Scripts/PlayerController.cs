using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpSpeed;
    public GameObject hatObject;

    [HideInInspector]
    public float curHatTime;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;

    [PunRPC]
    public void Initialize (Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        if (id == 1)
            GameManager.instance.GiveHat(id, true);


        if (!photonView.IsMine)
            rig.isKinematic = true;
    }

    private void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rig.velocity = new Vector3(x, rig.velocity.y, z);
    }

    // are we on the ground and if so jump
    void TryJump()
    {
        // raycase that shoots below us
        Ray ray = new Ray(transform.position, Vector3.down);

        // if we hit something then we are on ground - start jumping
        if(Physics.Raycast(ray, 0.7f))
        {
            rig.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }
    // could replace for something else. maybe not a hat but a bomb.
    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;

        // did we hit another player?
        if (collision.gameObject.CompareTag("Player"))
        {
            // do they have the hat?
            if(GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
                // can we get the hat?
                if(GameManager.instance.CanGetHat())
                {
                    // give us the hat
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }
}
