using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLine : ClearablePiece
{
    public bool isRow;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void Clear()
    {
        base.Clear();
        if (isRow)
        {
            piece.GridRef.ClearRow(piece.Y);
        }
        else
        {
            piece.GridRef.ClearColumn(piece.X);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
