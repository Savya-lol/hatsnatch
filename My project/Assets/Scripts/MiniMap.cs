using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform player;
    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
    }
}
