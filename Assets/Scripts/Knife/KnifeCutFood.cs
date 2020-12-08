using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum FoodType
//{
//    Carrot
//}

//[Serializable]
//public struct FoodStat
//{
//    public String prefabName;
//    public int maxTime;
//    public int pieceCount;
//    public AudioClip cutSound;
//    [ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
//    public Color surfaceColor;
//    [ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
//    public Color cutColor;
//}

public class KnifeCutFood : MonoBehaviour
{
    public List<FoodType> _foodTypes;
    public List<FoodStat> _foodStats;

    public bool _cuttingFood = false;
    public bool _cuttingHand = false;
    public TheFood _curFood;

    private FoodStat _curFoodStat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            if (other.GetComponent<TheFood>().IsCuttingFood(transform.position) && other.GetComponent<TheFood>()._grabbed)
            //if (other.GetComponent<TheFood>().IsCuttingFood(transform.position))
            {
                _cuttingFood = true;

                _curFood = other.GetComponent<TheFood>();
                //other.GetComponent<TheFood>().OnCut(transform.position);

                int typeIdx = _foodTypes.FindIndex(x => x == other.GetComponent<TheFood>()._foodType);
                //_curFoodStat = _foodStats[typeIdx];
                SoundManager.instance.SetCutFoodClip(typeIdx);
            }
        }
        else if (other.CompareTag("Left Hand"))
        {
            _cuttingHand = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            _cuttingFood = false;

            SoundManager.instance.ResetCutFoodClip();
        }
        else if (other.CompareTag("Left Hand"))
        {
            _cuttingHand = false;
        }
        // Commented out because we don't want hand get cut once it touches the knife
        //else if (other.CompareTag("Left Hand"))
        //{
        //    if (!other.GetComponent<LeftHandController>()._handInvincible)
        //    {
        //        other.GetComponent<LeftHandController>().OnCut();
        //        SoundManager.instance.PlayCutFingerClip();
        //    }
        //}
    }

    public void CutFoodDone()
    {
        _cuttingFood = false;
    }
}
