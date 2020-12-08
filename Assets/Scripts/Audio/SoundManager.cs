using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("AudioSources")]
    public AudioSource _knifeSource;
    public AudioSource _fingerSource;
    public AudioSource _intoSoupSource;
    public AudioSource _tick;

    [Header("AudioClips")]
    public List<AudioClip> _cutFoodClips;
    public List<AudioClip> _fingerClips;
    public List<AudioClip> _cutOthers;
    public List<AudioClip> _timerClips;

    [Header("Current AudioClips")]
    private AudioClip _curCutFood = null;
    private AudioClip _curCutFinger = null;

    private void Awake()
    {
        instance = this;
    }

    #region Cut Food
    public void SetCutFoodClip(int idx)
    {
        _curCutFood = _cutFoodClips[idx];
    }

    [PunRPC]
    public void PlayCutFoodClip()
    {
        if (_curCutFood != null)
        {
            _knifeSource.PlayOneShot(_curCutFood, 0.6f);
        }
        else
        {
            // Play cut board sound
            _knifeSource.PlayOneShot(_cutOthers[0], 0.6f);
        }
    }

    public void ResetCutFoodClip()
    {
        _curCutFood = null;
    }

    #endregion Cut Food

    #region Cut Finger
    public int _nextFinToCut = 0;

    public void SetCutFingerClip()
    {
        if (_nextFinToCut < _fingerClips.Count)
        {
            _curCutFinger = _fingerClips[_nextFinToCut];
            _nextFinToCut++;
        }
        else
        {
        }
    }

    public void PlayCutFingerClip()
    {
        SetCutFingerClip();
        if (_curCutFinger != null)
        {
            _fingerSource.PlayOneShot(_curCutFinger, 0.6f);
        }
    }

    public void ResetCutFingerClip()
    {
        _nextFinToCut = 0;
    }

    #endregion Cut Finger
}
