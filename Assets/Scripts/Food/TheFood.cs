using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using Photon;
using Photon.Pun;
using Photon.Realtime;

public class TheFood : MonoBehaviourPunCallbacks, IPunObservable
{

    public FoodType _foodType;
    public string _foodName;
    public List<MeshRenderer> _meshRenders;

    private GameObject Plate;
    private GameObject Pot;
    public int piece = 0;
    public bool _grabbed = false;
    private GameObject _soundManager;
    private GameObject _gameManager;

    public List<GameObject> CutFoodList = new List<GameObject>();
    public float AfterFinalCutFoodLength = 0;
    private float _MaxBeforeCut;
    private float _MaxAfterCut;
    private float _MinAfterCut;

    float _minWidth;
    float _fillMin;
    float _fillMax;
    bool canCut = true;

    private void Start()
    {
        _soundManager = GameObject.Find("SoundManager");
        _foodName = ((FoodType)_foodType).ToString();
        _gameManager = GameObject.Find("GameManager");

        Plate = GameObject.Find("Plate");
        Pot = GameObject.Find("Pot5");

        _minWidth = ScoreManager.instance._curFoodStat.minWidth;
    }


    [PunRPC]
    public void OnCut(Vector3 knifePos)
    {
        if (PhotonNetwork.IsMasterClient && canCut)
        {
            _soundManager.GetComponent<SoundManager>().GetComponent<PhotonView>().RPC("PlayCutFoodClip", RpcTarget.AllViaServer);
            float fillAmount;
            piece++;
            

            // Plate = GameObject.Find("Plate(Clone)");
            // Pot = GameObject.Find("Pot5");

            Vector3 _platePos = Plate.transform.position;

            // Instantiae a copy of current food
            GameObject newFood = PhotonNetwork.InstantiateRoomObject(_foodName, _platePos, transform.rotation, 0);

            //            newFood.transform.parent = Plate.transform;
            newFood.transform.position += new Vector3( -piece * 0.1f, 2, -1f);
            newFood.GetComponent<TheFood>().enabled = false;
            newFood.GetComponent<BoxCollider>().enabled = false;

            List<MeshRenderer> newFoodRenderers = newFood.GetComponent<TheFood>()._meshRenders;
            
            _MaxBeforeCut = _meshRenders[0].material.GetFloat("_FillMax");
            
            int bloodState;
            bloodState = _gameManager.GetComponent<BloodController>().bloodyState;

            for (int i = 0; i < _meshRenders.Count; i++)
            {
                // Set the fill max for the new food material
                newFoodRenderers[i].material.SetFloat("_FillMax", _meshRenders[i].material.GetFloat("_FillMax"));
                // Compute the fill Amount
                fillAmount = knifePos.x - _meshRenders[i].transform.position.x;
                if (newFoodRenderers[i].material.GetFloat("_FillMin") < fillAmount)
                {
                    newFoodRenderers[i].material.SetFloat("_FillMin", fillAmount);
                }
                if (_meshRenders[i].material.GetFloat("_FillMax") > fillAmount)
                {
                    _meshRenders[i].material.SetFloat("_FillMax", fillAmount);
                }

                switch (bloodState)
                    {
                        case 1:
                            newFoodRenderers[i].material.SetFloat("_BloodIntensity", 0.4f);
                            break;
                        case 2:
                            newFoodRenderers[i].material.SetFloat("_BloodIntensity", 0.6f);
                            break;
                        case 3:
                            newFoodRenderers[i].material.SetFloat("_BloodIntensity", 0.8f);
                            break;
                        case 4:
                            newFoodRenderers[i].material.SetFloat("_BloodIntensity", 0.9f);
                            break;
                        case 5:
                            newFoodRenderers[i].material.SetFloat("_BloodIntensity", 1.0f);
                            break;    
                        default:
                            newFoodRenderers[i].material.SetFloat("_BloodIntensity", 0.0f);
                            break;
                    }
            }

            _MaxAfterCut = _meshRenders[0].material.GetFloat("_FillMax");
            _MinAfterCut = _meshRenders[0].material.GetFloat("_FillMin");

            CutFoodList.Add(newFood);
            photonView.RPC("AddPiece", RpcTarget.AllViaServer);
            //_gameManager.GetComponent<ScoreManager>().AddToCutFoodLength( (_MaxBeforeCut - _MaxAfterCut), (_MaxAfterCut - _MinAfterCut), (_MaxBeforeCut - _MinAfterCut));
        }
    }

    [PunRPC]
    public void AddPiece()
    {
        GameObject.Find("GameManager").GetComponent<ScoreManager>().AddToPieceCounter();
    }

    [PunRPC]
    public void MovePlateToPot()
    {
        /* DOMove
        Vector3 adjPos = new Vector3(-6f, 9.975f, 0f);
        transform.position = Plate.transform.position + new Vector3(-piece * 0.2f, 2, piece * 0.2f);
        transform.DOMove(Pot.transform.position + adjPos, 2f);
        foreach (GameObject food in CutFoodList)
        {
            food.transform.DOMove(Pot.transform.position + adjPos, 2f);
        }
        Debug.LogError(Plate.transform);
        Debug.LogError(Pot.transform);
        Plate.transform.DOMove(Pot.transform.position + new Vector3(0, 7.975f, 0), 2f)
            .OnComplete(()=> SelfDestory());
        */

        // DOJump
        if (PhotonNetwork.IsMasterClient && canCut)
        {
            canCut = false;
            Vector3 adjPos = new Vector3(3f, 0f, 0f);
            transform.position = Plate.transform.position + new Vector3(-piece * 0.1f, 2, -1f);
            StartCoroutine(FlyToPotInLine(adjPos));
        }
        SoundManager.instance._intoSoupSource.PlayDelayed(1f);

        //foreach (GameObject food in CutFoodList)
        //{
        //    //food.transform.DOMove(Pot.transform.position + adjPos, 2f);
        //    food.transform.DOJump(Pot.transform.position + adjPos, 5f, 1, 2f, false);
        //}
        //transform.DOJump(Pot.transform.position + adjPos, 5f, 1, 2f, false).OnComplete(() => SelfDestory());
        //Debug.LogError(Plate.transform);
        //Debug.LogError(Pot.transform);
        //Plate.transform
        //    .DOJump(Pot.transform.position + new Vector3(0, 7.975f, 0), 20f, 1, 2f, false);
    }

    IEnumerator FlyToPotInLine(Vector3 adjPos)
    {
        foreach (GameObject food in CutFoodList)
        {
            food.transform.DOJump(Pot.transform.position + adjPos, 10f, 1, 2f, false);
            food.transform.DOScale(food.transform.localScale * 0.2f , 2f);
            yield return new WaitForSeconds(3f/(CutFoodList.Count+1));
        }
        transform.DOScale(transform.localScale * 0.5f, 2f);
        transform.DOJump(Pot.transform.position + adjPos, 10f, 1, 2f, false).OnComplete(() => SelfDestory());
    }

    public void SelfDestory()
    {
        foreach(GameObject food in CutFoodList)
        {
            PhotonNetwork.Destroy(food);
        }
        //Plate.transform.DOMove(new Vector3(0.5f, -4.41f, 17.2f), 2f);
        // Plate.transform.DOJump(new Vector3(0.5f, -4.41f, 17.2f), 3f, 1, 2f, false);

        PhotonNetwork.Destroy(gameObject);
    }

    public bool IsCuttingFood(Vector3 knifePos)
    {
        return ((_meshRenders[0].material.GetFloat("_FillMax") - _minWidth) 
            > knifePos.x - transform.position.x);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Knife") && IsCuttingFood(other.transform.position))
    //    {
    //        if (_grabbed)
    //        {
    //            transform.position += new Vector3(-0.5f * Time.deltaTime, 0, 0.3f * Time.deltaTime);
    //        }
    //        else
    //        {
    //            transform.position += new Vector3(3f * Time.deltaTime, 0, -3f * Time.deltaTime);
    //        }
    //    }
    //}

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            foreach (MeshRenderer renderer in _meshRenders)
            {
                stream.SendNext(renderer.material.GetFloat("_FillMax"));
                stream.SendNext(renderer.material.GetFloat("_FillMin"));
                stream.SendNext(renderer.material.GetFloat("_BloodIntensity"));
            }
            stream.SendNext(_grabbed);
        }
        else
        {
            foreach (MeshRenderer renderer in _meshRenders)
            {
                float _fillMax = (float)stream.ReceiveNext();
                float _fillMin = (float)stream.ReceiveNext();
                float _bloodIntensity = (float)stream.ReceiveNext();
                renderer.material.SetFloat("_FillMax", _fillMax);
                renderer.material.SetFloat("_FillMin", _fillMin);
                renderer.material.SetFloat("_BloodIntensity", _bloodIntensity);
            }
            _grabbed = (bool)stream.ReceiveNext();
        }
    }
}
