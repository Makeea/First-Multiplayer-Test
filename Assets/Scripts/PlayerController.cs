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

    public void Initialized( Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        // first player gets the hat

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
}
