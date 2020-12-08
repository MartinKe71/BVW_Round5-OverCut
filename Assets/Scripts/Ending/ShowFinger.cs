using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFinger : MonoBehaviour
{
    public List<GameObject> _fingers;
    public List<Sprite> _sprites;
    public Image _ending;
    public int _num = 0;

    public void ActivateFinger()
    {
        _ending.sprite = _sprites[_num];
        if (_num > 0)
        {
            GetComponent<AudioSource>().Play();
            StartCoroutine(Activating());
        }            
    }

    IEnumerator Activating()
    {
        for (int i = 0; i < _num; i++)
        {
            _fingers[i].SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
