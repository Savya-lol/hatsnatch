using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public bool gameStarted = false;
    public Animator crate_anim;
    public GameObject[] boosters;
    public Transform[] boosterSpawnpos;
    public float boosterSpawnTime;
    public float curTime = 0;
    public bool boosterSpawned;
    

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

    private void Update()
    {
        spawnBooster();
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
            GetPlayer(playerWithhat).walkingSpeed = 7f;
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
    
    [PunRPC]
    void Ready(int id)
    {
        GetPlayer(id).ready = true;
        int cnt = 0;
        foreach (PlayerMovement p in players)
        {
            if (p.ready)
            {
                cnt += 1;
                print("added");
            }
        }

        if (cnt >= 2)
        {
            gameStarted = true;
            foreach (PlayerMovement p in players)
            {
                p.canMove = true;
            }
            crate_anim.SetTrigger("open");
            print("opened");
        }
    }

    public void spawnBooster()
    {

        if (gameStarted)
        {
            if (!boosterSpawned)
            {
                curTime += Time.deltaTime;
                if (curTime > boosterSpawnTime)
                {
                    print("called");
                    int ran = Random.Range(0, boosters.Length - 1);
                    int ranSpawn = Random.Range(0, boosterSpawnpos.Length - 1);
                    PhotonNetwork.Instantiate(boosters[ran].name, boosterSpawnpos[ranSpawn].position,
                        Quaternion.identity);
                    curTime = 0;
                    print("spawnned");
                    boosterSpawned = true;
                }
            }
        }
    }
}
