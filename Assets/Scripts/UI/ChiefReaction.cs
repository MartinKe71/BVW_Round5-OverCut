using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChiefReaction : MonoBehaviour
{
    public Image image;
    public List<Sprite> sprites;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = null;
    }

    public void ChangeReaction(int idx)
    {
        if (idx < sprites.Count)
        {
            image.sprite = sprites[idx];
        }
    }
}
