using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers;
    public TMP_Text winText;

    [Header("Components")]
    public PhotonView photonView;

    private float updateTimer;

    // instance
    public static GameUI instance;

    void Awake ()
    {
        // set the instance to this script
        instance = this;
    }

    void Start ()
    {
        photonView.RPC("InitializePlayerUI",RpcTarget.AllBufferedViaServer);
    }

    void Update ()
    {
        UpdatePlayerUI();
    }

    // initializes the player UI containers
    [PunRPC]
    void InitializePlayerUI ()
    {
        // loop through all of the containers
        for (int x = 0; x < playerContainers.Length; ++x)
        {
            PlayerUIContainer container = playerContainers[x];

            // only enable and modify the UI containers we need
            if(x < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[x].NickName;
                container.hatTimeSlider.maxValue = GameManager.instance.timeToWin;
            }
            else
                container.obj.SetActive(false);
        }
    }

    // updates the player UI sliders
    void UpdatePlayerUI ()
    {
        // loop through all of the players
        for(int x = 0; x < GameManager.instance.players.Count; ++x)
        {
            if(GameManager.instance.players[x] != null)
                playerContainers[x].hatTimeSlider.value = GameManager.instance.players[x].curHatTime;
        }
    }

    // called when a player has won the game
    public void SetWinText (string winnerName)
    {
        winText.transform.parent.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }
}

// class which holds info for each player's UI element
[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
}
