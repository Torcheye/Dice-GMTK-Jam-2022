using System;
using DG.Tweening;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Transform gemHolder;
    public PrefabsScriptableObject prefabs;

    public int gemCount;

    private void Start()
    {
        SetGemCount();
    }

    public void SetGemCount()
    {
        if (gemCount == 0)
        {
            if (gemHolder.childCount > 0)
                Destroy(gemHolder.GetChild(0).gameObject);
        }
        else
        {
            if (gemHolder.childCount > 0)
            {
                Destroy(gemHolder.GetChild(0).gameObject);
            }

            Instantiate(prefabs.gems[gemCount], gemHolder);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("step");
            transform.DOJump(transform.position, -.08f, 1, .8f).SetEase(Ease.Linear);

            other.GetComponent<PlayerMovement>().StepOnGround(gemCount);
            gemCount = 0;
            SetGemCount();
        }
    }
}