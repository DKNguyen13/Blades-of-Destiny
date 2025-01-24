using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{

    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public AudioSource backgroundMusic;

    public Button musicButton;
    private bool isMusicOn = true;
    // Start is called before the first frame update
    void Start()
    {
        musicButton.onClick.AddListener(ToggleMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        if (isMusicOn)
        {
            backgroundMusic.loop = true;
            backgroundMusic.Play();
            musicButton.image.sprite = musicOnSprite;
        }
        else
        {
            backgroundMusic.Pause();
            musicButton.image.sprite = musicOffSprite;
        }
    }
}
