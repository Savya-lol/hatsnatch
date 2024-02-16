using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //instance for singleton
    public static NetworkManager instance;
    public TMP_InputField nameInput;
    public TMP_InputField roomName_txt;
    public Button hostButton;
    public Button joinButton;
    public TMP_Text StateText;

    string roomName;
    
    private void Awake()
    {
        //creating a singleton
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void SetName()
    {
        PhotonNetwork.NickName = nameInput.text;
    }

    public void SetRoomName()
    {
        roomName = roomName_txt.text;
    }
    public void Join()
    {
        if(roomName == null)
        {
            StateText.text = "Please Enter Room Name";
            return;
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    public void Host()
    {
        if (roomName == null)
        {
            StateText.text = "Please Enter Room Name";
            return;
        }
        PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        StartCoroutine("LoadGame");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        StateText.text = "Connected";
        joinButton.interactable = true;
        hostButton.interactable = true;
        print("Connected");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        StateText.SetText("Connected");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        StateText.text = "Error: ("+returnCode+")"+" "+ message;
        joinButton.interactable = true;
        hostButton.interactable = true;
    }
    
    
    IEnumerator LoadGame()
    {
        PhotonNetwork.LoadLevel(1);
        hostButton.interactable = false;
        joinButton.interactable = false;
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            StateText.text = "Loading(" + (PhotonNetwork.LevelLoadingProgress/1)*100+"%)";
            yield return new WaitForEndOfFrame();
        }
        GameObject.FindObjectOfType<RoomManager>().spawn();
    }

    public void MainMenu()
    {
        StartCoroutine("LoadMenu");
    }
    IEnumerator LoadMenu()
    {
        PhotonNetwork.LoadLevel(0);

        joinButton = GameObject.Find("Join").GetComponent<Button>();
        hostButton = GameObject.Find("Host").GetComponent<Button>();
        StateText = GameObject.Find("State Text").GetComponent<TMP_Text>();
        nameInput = GameObject.Find("Name").GetComponent<TMP_InputField>();

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return new WaitForEndOfFrame();
        }


    }
}
