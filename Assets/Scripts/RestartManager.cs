using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using Photon;
using Photon.Pun;
using Photon.Realtime;

public class RestartManager : MonoBehaviour
{
    [Header("Grade Statue Check")]
    public List<bool> GradeCheck = new List<bool>();
    
    [Header("Complete Text")]
    public List<Text> CompleteText = new List<Text>();

    [Header("Sound Effects")]
    public AudioSource _audioSource;
    public AudioClip score;
    public AudioClip stamp;
    /*
    public Text UnitedSteaksComplete;
    public Text CarrotComplete;
    public Text PotatoComplete;
    public Text OnionComplete;
    public Text BabyCornComplete;
    public Text GarlicComplete;
    */

    public List<GameObject> ChiefFace = new List<GameObject>();

    public int CompletedFood = 0;
    public int NotCompletedFood = 0;
    public int ChiefFaceStatus;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickRestartButton()
    {
        PhotonNetwork.LoadLevel(4);
    }

    [PunRPC]
    public void SetGradeComplete(int inx)
    {
        GradeCheck[inx] = true;
    }

    //[PunRPC]
    public void CountFinalScore()
    {
        int i = 0;
        foreach (bool isComplete in GradeCheck)
        {
            if(isComplete) 
            {
                CompletedFood ++;
                CompleteText[i].text = "Finished!";
            }
            else
            {
                NotCompletedFood ++;
                CompleteText[i].text = "Unfinished!";
            }
            i ++;
        }

        foreach (Text status in CompleteText)
        {
            status.gameObject.SetActive(false);
        }
    }

    //[PunRPC]
    public void SetFinalScoreUI()
    {
        _audioSource = GetComponent<AudioSource>();
        if( (CompletedFood == 6) || (CompletedFood == 5) )
        {
            ChiefFaceStatus = 0; // great
        } 
        else if( (CompletedFood == 4) || (CompletedFood == 3) )
        {
            ChiefFaceStatus = 1; // ok
        }
        else if( (CompletedFood == 2) || (CompletedFood == 1) )
        {
            ChiefFaceStatus = 2; // bad
        }
        else
        {
            ChiefFaceStatus = 3; // awful
        }

        foreach(GameObject face in ChiefFace)
        {
            face.SetActive(false);
        }

        StartCoroutine(ShowFinalScore());

        
    }

    IEnumerator ShowFinalScore()
    {
        for (int i = 0; i < 6; i++)
        {
            CompleteText[i].gameObject.SetActive(true);
            _audioSource.PlayOneShot(score, 1f);
            yield return new WaitForSeconds(0.6f);
        }
        yield return new WaitForSeconds(2f);
        _audioSource.PlayOneShot(stamp, 1f);
        ChiefFace[ChiefFaceStatus].SetActive(true);
    }
}
