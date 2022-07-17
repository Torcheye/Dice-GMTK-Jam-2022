using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Transform gemHolder;
    public PrefabsScriptableObject prefabs;
    public int gemCount;
    public bool isDestination;
    public MenuOption menuOption = MenuOption.None;
    public enum MenuOption
    {
        None,
        Quit,
        Credit,
        Mute
    }

    private float _pitch = 1;

    private void Start()
    {
        SetGemCount();

        _pitch = 1 + 2 *
            Mathf.InverseLerp(0, GameObject.FindGameObjectsWithTag("Floor").Length, transform.GetSiblingIndex());
    }

    public void PlaySound()
    {
        GameManager.Instance.PlaySoundFloor(_pitch);
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
            PlaySound();
            transform.DOJump(transform.position, -.12f, 1, .7f).SetEase(Ease.Linear);

            if (menuOption != MenuOption.None)
            {
                switch (menuOption)
                {
                    case MenuOption.Quit:
                        Application.Quit();
                        return;
                    case MenuOption.Mute:
                        FindObjectOfType<BGMPlayer>().ToggleMute();
                        FindObjectOfType<GameManager>().ToggleMute();
                        return;
                    case MenuOption.Credit:
                        //
                        return;
                    default:
                        throw new Exception("Invalid menu option");
                }
            }

            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (isDestination)
                playerMovement.StepOnGround(gemCount, transform);
            else
                playerMovement.StepOnGround(gemCount);

            if (playerMovement.isMenu)
                return;
            gemCount = 0;
            SetGemCount();
        }
    }
}