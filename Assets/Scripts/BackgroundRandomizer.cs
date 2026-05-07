using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BackgroundRandomizer : MonoBehaviour
{
    public SpriteRenderer BackgroundObj;
    public List<Sprite> BackgroundSprites;
    private void Awake()
    {
        BackgroundObj.sprite = BackgroundSprites[Random.Range(0,BackgroundSprites.Count)];
    }
}
