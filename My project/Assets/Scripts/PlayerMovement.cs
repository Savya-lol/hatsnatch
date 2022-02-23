using System;
using System.Collections;
using System.Collections.Generic;
using DitzeGames.Effects;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerMovement : MonoBehaviour,IPunObservable
{
    #region publicVariables

    [Header("General Setup")] 
    public SkinnedMeshRenderer[] remoteBodies;
    public GameObject bodyTodisable;
    public GameObject localBody;
    public CharacterController characterController;
    public GameObject playerCamera;
    public Animator _animator;
    public TMP_Text nameTag;
    public GameObject hatObject;
    public CameraEffects CameraEffects;
    public GameObject miniMapcamera;
    public GameObject miniMapimage;
    public GameObject speedIcon;
    public GameObject hatIcon;
    
    [Header("Values Setup")] 
    public float walkingSpeed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float punchRange;
    public float curHatTime;
    public bool ready;
    
    //hide in Inspector
    [HideInInspector]
    public bool canMove = true;
    [HideInInspector] 
    public int id;
    [HideInInspector]
    public Player photonPlayer;
    [HideInInspector]
    public PhotonView _photonView;
    #endregion

    #region privateVariables
    private float _xRot;
    private Vector3 _movementInput;
    
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    #endregion

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            DisplayName();
        }
    }

    void Update()
    {
        if (_photonView.IsMine)
        {
            Move();
            CameraMove();
            Animation();
            if (Input.GetMouseButtonDown(0))
            {
                Punch();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                GameManager.instance.view.RPC("GiveHat",RpcTarget.AllBufferedViaServer,this.id);
            }
            hatIcon.SetActive(hatObject.activeSelf);
            if (hatObject.activeInHierarchy)
            {
                curHatTime += Time.deltaTime;
            }
        }
        else
        {
            if(curHatTime >= GameManager.instance.timeToWin)
            {
                GameManager.instance.view.RPC("WinGame", RpcTarget.All, GameManager.instance.playerWithhat);
            }
        }
    }

    void Move()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? walkingSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
 
        if (Input.GetButtonDown("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
 
        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
 
        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }


    void CameraMove()
    {  
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void Punch()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position,playerCamera.transform.forward, out hit,punchRange))
        {
            if (hit.collider.tag == "Player")
            {
                if (GameManager.instance.GetPlayer(hit.collider.gameObject).id == GameManager.instance.playerWithhat)
                {
                    if (GameManager.instance.CanGetHat())
                    {
                        GameManager.instance.view.RPC("GiveHat",RpcTarget.All,id);
                        _photonView.RPC("Punched",RpcTarget.Others);
                    }
                }
            }

            if (hit.collider.tag == "ready")
            {
                if (!ready)
                {
                    ready = true;
                    int cnt = 0;
                    foreach (PlayerMovement pm in GameManager.instance.players)
                    {
                        if (pm.ready)
                        {
                            cnt += 1;
                        }
                    }

                    if (cnt == 2)
                    {
                        print("Started");
                    }
                }
            }
        }
    }
    
    void Animation()
    {
        if (canMove)
        {
            _animator.SetFloat("hor", Input.GetAxis("Horizontal"));
            _animator.SetFloat("vert", Input.GetAxis("Vertical"));
        }

        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger("punch");
        }
    }

    void DisplayName()
    {
        nameTag.text = _photonView.Owner.NickName;
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players.Add(this);
            print("done");
    }
    
    public void SetHat (bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "hat")
        {
            Destroy(other.gameObject);

            if (_photonView.IsMine)
            {
                GameManager.instance.view.RPC("GiveHat",RpcTarget.All,id);
            }
        }
    }
    
    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
    {
        // we want to sync the 'curHatTime' between all clients
        if(stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if(stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void Punched()
    {
        CameraEffects.ShakeOnce();
    }
}
