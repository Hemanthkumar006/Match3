using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using static AnimationManager;

public class AnimationManager : MonoBehaviour
{
    public struct ClearanimSprite
    {
        public int idx;
        public SpriteRenderer renderer;
    }
    public int clearanimval;
    public AnimationClip[] clearanims;
    private Animator animator;
    public ClearanimSprite[] animsprite;
    public AnimationClip ClearAnimation;
    public AnimationClip TouchAnime;
    public GamePiece piece;
    private SpriteRenderer render;
    // Start is called before the first frame update

    void Awake()
    {
        animator = GetComponent<Animator>();
        piece = GetComponent<GamePiece>();
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayTouchAnim()
    {
        //Debug.Log("Current Animator State: " + animator.GetCurrentAnimatorStateInfo(0).IsName("TouchAnimation"));
        StartCoroutine(TouchPiece());
    }

    IEnumerator TouchPiece()
    {
        if (animator)
        {
            //animator.Rebind();
            animator.Play(TouchAnime.name,-1,0f);
            yield return new WaitForSeconds(TouchAnime.length);
        }

    }
    /*void ColorValidator(ColorPiece.ColorType color)
    {
        if (color == ColorPiece.ColorType.RED)
        {
            animator.Play(ClearAnimationClipRed.name);
        }
        else if (color == ColorPiece.ColorType.BLUE)
        {
            animator.Play(ClearAnimationClipBlue.name);
        }
        else if (color == ColorPiece.ColorType.GREEN)
        {
            animator.Play(ClearAnimationClipGreen.name);
        }
        else if(color == ColorPiece.ColorType.PINK) 
        {
            animator.Play(ClearAnimationClipPink.name);
        }
        else if(color == ColorPiece.ColorType.MAGENTA)
        {
            animator.Play(ClearAnimationClipMagenta.name);
        }*/

    public void Playconsecanims()
    {
        StartCoroutine(playconsecanimrotine());
    }

    IEnumerator playconsecanimrotine()
    {
        int X = clearanimval - piece.Value;
        animator.Play(clearanims[X].name);
        yield return new WaitForSeconds(clearanims[X].length);

    }

    public void PutSpriteatEnd()
    {
        piece.Value -= 1;
        int i = piece.Value;
        render.sprite = animsprite[i].renderer.sprite;
    }

    private void AnimEventTest()
    {
        Debug.Log("I'm at one frame before to last.");
    }

    private void GetResult()
    {
        
    }
}
    

