using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiece : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject piece;
    public enum ColorType
    {
        RED,
        GREEN,
        PINK,
        BLUE,
        MAGENTA
    }
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;

    }

    public ColorSprite[] colorSprite;
    private Dictionary<ColorType, Sprite> ColorSpriteDict;
    private ColorType color;
    private SpriteRenderer spriter;
    public ColorType Color { get { return color; } set { SetColor(value); } }

    public int NumCols {  get { return colorSprite.Length; } }

    void Awake()
    {
        spriter = piece.GetComponent<SpriteRenderer>();
        ColorSpriteDict = new Dictionary<ColorType, Sprite>();
        for (int i = 0; i < colorSprite.Length; i++)
        {
            if (!ColorSpriteDict.ContainsKey(colorSprite[i].color))
            {
                ColorSpriteDict.Add(colorSprite[i].color, colorSprite[i].sprite);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetColor(ColorType newcolor)
    {
        color = newcolor;
        if (ColorSpriteDict.ContainsKey(newcolor))
        {
            spriter.sprite = ColorSpriteDict[newcolor];
        }
    }

    
}
