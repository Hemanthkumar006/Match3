using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class MovablePiece : MonoBehaviour
{
    public float filltime = 0.15f;
    private GamePiece piece;
    private IEnumerator moveCoroutine;
    // Start is called before the first frame update

    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(int newX, int newY,float time)
    {
        piece.X = newX;
        piece.Y = newY;

        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int x, int y, float time)
    {
        Vector3 EndPos = piece.GridRef.GetWorldPos(x,y);

        piece.transform.DOMove(EndPos, time).SetEase(Ease.Linear);
        yield return new WaitForSeconds(time);
    }


    public void Mover(int x,int y)
    {
        piece.X = x;
        piece.Y = y;

        piece.transform.localPosition = piece.GridRef.GetWorldPos(x, y);
    }

    public void MoveTest(int x, int y , float time)
    {
        piece.X = x;
        piece.Y = y;

        Vector3 StartPos = transform.position;
        Vector3 EndPos = piece.GridRef.GetWorldPos(x, y);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            piece.transform.position = Vector3.Lerp(StartPos, EndPos, t / time);
        }

        piece.transform.position = EndPos;

    }
}
