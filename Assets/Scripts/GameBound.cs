using System;
using UnityEngine;

public class GameBound : MonoBehaviour
{
    public float respawnHeight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().StartRespawn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().Respawn(respawnHeight);
        }
    }
}