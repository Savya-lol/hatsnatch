using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject player;
    public void spawn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            player = PhotonNetwork.Instantiate("Player", spawnPoints[0].position, Quaternion.identity);
        }
        else
        {
            player = PhotonNetwork.Instantiate("Player", spawnPoints[1].position, Quaternion.identity);
        }
       
        
        player.GetComponent<PlayerMovement>().localBody.SetActive(true);
        player.GetComponent<PlayerMovement>().bodyTodisable.SetActive(false);
        player.GetComponent<PlayerMovement>().miniMapcamera.SetActive(true);
        player.GetComponent<PlayerMovement>().miniMapimage.SetActive(true);
        player.GetComponent<PlayerMovement>().audio.SetActive(true);
        player.GetComponent<PlayerMovement>().canvas.SetActive(true);
        foreach (SkinnedMeshRenderer sr in player.GetComponent<PlayerMovement>().remoteBodies)
        {
            sr.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }

        player.GetComponent<PlayerMovement>().hatObject.GetComponent<MeshRenderer>().shadowCastingMode =
            ShadowCastingMode.ShadowsOnly;
        player.GetComponent<PlayerMovement>()._photonView.RPC("Initialize", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer);
            print("Initialized");
    }

}
