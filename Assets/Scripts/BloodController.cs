using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon;
using Photon.Pun;
using Photon.Realtime;

public class BloodController : MonoBehaviourPunCallbacks
{
    public GameObject BloodStain;
    public List<GameObject> CutHand;
    public int bloodyState = 0; // 0~5

    // Start is called before the first frame update
    void Start()
    {
        HideAllCutHand();
        CutHand[0].SetActive(true);
    }

    [PunRPC]
    public void OpenBloodStainUI()
    {
        //BloodStain.SetActive(true);
        StartCoroutine(WaitToOpenBlood());
        StartCoroutine(WaitToCloseBlood());
    }

    [PunRPC]
    public void SetHandUI(int inx){
        HideAllCutHand();
        CutHand[inx].SetActive(true);
        bloodyState = inx;
    }

    [PunRPC]
    public int GetBloodState()
    {
        return bloodyState;
    }

    private void HideAllCutHand()
    {
        foreach (GameObject hands in CutHand)
        {
            hands.SetActive(false);
        }
    }

    private IEnumerator WaitToOpenBlood()
    {
        yield return new WaitForSeconds (0.25f);
        BloodStain.SetActive(true);
    }
    private IEnumerator WaitToCloseBlood()
    {
        yield return new WaitForSeconds (2);
        BloodStain.SetActive(false);
    }
}
