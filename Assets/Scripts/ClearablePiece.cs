using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
{
    // Start is called before the first frame update
    public AnimationClip clearanimation;
    protected GamePiece piece;

    private bool isbeingcleared = false;
    public bool isBeingCleared
    {
        get { return isbeingcleared; }
    }
    void Awake()
    {   
        piece = GetComponent<GamePiece>();
          
    }

    // Update is called once per frame
    public virtual void Clear()
    {
        isbeingcleared = true;
        StartCoroutine(ClearPiece());
    }

    IEnumerator ClearPiece()
    {
        Animator animator = GetComponent<Animator>();
        
        
        if (animator)
        {
            animator.Play(clearanimation.name);
            yield return new WaitForSeconds(clearanimation.length);
            Destroy(gameObject);
        }
       
    }

    public void ClearTest()
    {
        isbeingcleared = true;
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(clearanimation.name);
            Destroy(gameObject);
        }
    }
}
