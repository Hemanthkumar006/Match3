using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantiate : MonoBehaviour
{
    public GameObject InstantiatePrefab;
    private GamePiece[,] pieces;

    private int x = 0;
    private int y = 0;
    // Start is called before the first frame update
    void Start()
    {
        pieces = new GamePiece[4,4];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadOnClick()
    {
        StartCoroutine(LL());
    }

    IEnumerator LL()
    {
        GameObject gs = Instantiate(InstantiatePrefab, Vector3.zero, Quaternion.identity);
        gs.transform.SetParent(transform, true);
        gs.transform.localPosition = new Vector3(x,y,0);

        pieces[x,y] = gs.GetComponent<GamePiece>();
        x += 1;
        if (x > 4)
        {
            y += 1;
            x  =  0;
        }
        Animator animator = gs.GetComponent<Animator>();
        animator.Play("NormalCreate");
        yield return new WaitForSeconds(2);
    }
}
