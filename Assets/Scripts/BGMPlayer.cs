using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public bool isMuted;

    public void ToggleMute()
    {
        isMuted = !isMuted;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}