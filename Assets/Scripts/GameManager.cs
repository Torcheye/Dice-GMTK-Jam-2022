using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MMFeedbacks cameraShake;
    public GameObject player;
    public List<AudioClip> footstepList;
    public AudioClip shatterSound;
    public AudioClip wooshSound;
    public AudioClip floorSound;

    private int _lastFootstepI = -1;
    private AudioSource _audioSource;

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (FindObjectOfType<BGMPlayer>().isMuted && _audioSource.volume > 0)
        {
            ToggleMute();
        }

        StartCoroutine(LoadGame());
    }

    public void ToggleMute()
    {
        _audioSource.volume = 1 - _audioSource.volume;
    }

    public IEnumerator LoadGame()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        for (int i = 0; i < floors.Length; i++)
        {
            floors[i].transform.Translate(Vector3.down * .7f, Space.World);
            floors[i].SetActive(false);
        }

        foreach (GameObject t in floors)
        {
            yield return new WaitForSeconds(.1f);
            t.SetActive(true);
            t.transform.DOMoveY(0, 1).SetEase(Ease.InOutQuad);
            t.GetComponent<Floor>().PlaySound();
        }

        yield return new WaitForSeconds(1);

        player.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShakeScreen()
    {
        cameraShake.PlayFeedbacks();
    }

    public void PlaySoundFootstep()
    {
        _audioSource.pitch = 1;
        int i = -1;
        while (i == _lastFootstepI)
            i = Random.Range(0, footstepList.Count);

        _audioSource.PlayOneShot(footstepList[i]);
    }

    public void PlaySoundWoosh()
    {
        _audioSource.pitch = 1;
        _audioSource.PlayOneShot(wooshSound);
    }

    public void PlaySoundShatter()
    {
        _audioSource.pitch = 1;
        _audioSource.PlayOneShot(shatterSound);
    }

    public void PlaySoundFloor(float pitch)
    {
        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(floorSound);
    }
}