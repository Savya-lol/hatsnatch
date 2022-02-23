using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class booster : MonoBehaviour
{
    void Start()
    {
        Invoke("Despawn",GameManager.instance.boosterSpawnTime);
    }

    void Despawn()
    {
        GameManager.instance.boosterSpawned = false;
        Destroy(gameObject);
    }
}
