using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon;
using Photon.Pun;
using Photon.Realtime;

public class TimerController : MonoBehaviourPunCallbacks
{
    public float startingTime;
    private float currentTime;

    public bool timerStarted = false;
    public Text countDownText;
    public Slider timeSlider;

    SoundManager sound;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startingTime;
         sound = SoundManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countDownText.text = currentTime.ToString("0");
        timeSlider.value = currentTime;

        if(currentTime <= 0)
        {
            currentTime = 0;
            timeSlider.value = 0;
            if (timerStarted)
            {
                sound._tick.Pause();
                sound._tick.PlayOneShot(sound._timerClips[1], 1f);
                timerStarted = false;
                GetComponent<ScoreManager>().TimeUp();                
            }
        }
    }

    [PunRPC]
    public void SetTimer(int time)
    {
        timerStarted = true;
        startingTime = time;
        currentTime = startingTime;
        timeSlider.maxValue = time;    
        sound._tick.Play();
        sound._tick.PlayOneShot(sound._timerClips[2], 1f);
    }
}
