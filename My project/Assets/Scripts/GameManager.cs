using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PhotonView view;
    public List<PlayerMovement> players;
    public int playerWithhat = -1;
    public float hatPickuptime;
    public float invincibleDuration;
    public bool initialGive = true;
    public float timeToWin;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
        }
    }

    public PlayerMovement GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    // returns the player of the requested GameObject
    public PlayerMovement GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }

    public bool CanGetHat()
    {
        if (Time.time > hatPickuptime + invincibleDuration) return true;
        else return false;
    }

    [PunRPC]
    public void GiveHat(int playerId)
    {
        if (!initialGive)
        {
            GetPlayer(playerWithhat).walkingSpeed = 7.5f;
            GetPlayer(playerWithhat).SetHat(false);
        }

        playerWithhat = playerId;
        GetPlayer(playerId).SetHat(true);
        GetPlayer(playerId).walkingSpeed = 6;
        hatPickuptime = Time.time;
        initialGive = false;
    }

    [PunRPC]
    void WinGame(int playerId)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        foreach (PlayerMovement p in players)
        {
            p.canMove = false;
        }
        PlayerMovement player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);
    }
    
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.MainMenu();
    }
    
    
}
