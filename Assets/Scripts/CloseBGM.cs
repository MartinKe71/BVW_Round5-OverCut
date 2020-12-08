using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CloseBGM : MonoBehaviour
{
    public CloseBGM instance;
    public AudioMixerGroup BGM;

    private void Awake()
    {
        instance = this;
    }
    public void DeactivateBGM()
    {
        //GameObject.Find("BGM").SetActive(false);
        BGM.audioMixer.SetFloat("BGMVolume", -80.00f);
    }

    public void RecoverBGM()
    {
        GameObject.Find("BGM").SetActive(true);
    }
}
