using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUtil
{
    public static Sprite GetSprite(string spritePath)
    {
       return Resources.Load<Sprite>(spritePath);
    }
}
