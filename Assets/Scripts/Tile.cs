using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update

    /*private int x; public int X { get { return x; } set { x = value; } }
    private int y; public int Y { get { return y; } set { y = value; } }
    public Gride gride;*/

    public int X;
    public int Y;

    public bool isNull = false;

    private bool isGenerable = false;
    public bool IsGenerable {  get { return isGenerable; } }

    private bool right = false; public bool Right { get { return right; } }
    private bool left = false; public bool Left { get { return left; } }
    private bool top = false; public bool Top { get { return top; } }
    private bool bottom = false; public bool Bottom { get { return bottom; } }  

    public Gride grid;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setcoord(int _x, int _y, Gride _grid)
    {
        X = _x;
        Y = _y;
        grid = _grid;
    }

    public void Mover(int x, int y)
    {
        this.transform.localPosition = new Vector3(x, y,0);
    }

}
