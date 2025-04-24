using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    private int x;
    public int X { get { return x; } set { if (IsMovable() && IsNormalPiece) { x = value; } } }
    private int y;
    public int Y { get { return y; } set { if (IsMovable() && IsNormalPiece) { y = value; } } }

    private float a;
    public float A { get { return a; } set { if (IsSpecialPiece) {  a = value; } } }

    private float b;
    public float B { get { return a; } set { if (IsSpecialPiece) { b = value; } } }

    private Gride grid;
    public Gride GridRef { get { return grid; } }

    private Gride.PieceType type;
    public Gride.PieceType Type { get {  return type; } }
    // Start is called before the first frame update

    public Gride.Visiblity Visibility;
    public Gride.PieceName Piecename;

    private MovablePiece movableComponent;
    public MovablePiece MovableComponent { get {  return movableComponent; } }

    private ColorPiece colorComponent;
    public ColorPiece ColorComponent { get {  return colorComponent; } }

    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent { get { return clearableComponent;} }


    private AnimationManager animationcomponent;
    public AnimationManager AnimationComponent { get { return animationcomponent; } }

    private Levels levelComponent;
    public Levels LevelComponent { get {  return levelComponent; } }

    private GamePiece linkedpiece;
    public GamePiece LinkedPiece { get { return linkedpiece; } set { if (Sppieceholder) { linkedpiece = value; } } }

    private GamePiece blockedpiece;
    public GamePiece Blockedpiece { get { return blockedpiece;  } set { if (isBlocked) { blockedpiece = value; } } }


    
    public int Value;
    public bool IsAdjacentAttackable;
    public bool IsAdjacentClearable;
    public bool Sppieceholder;
    public bool IsNormalPiece;
    public bool IsSpecialPiece;
    public bool IsMatchable;
    public bool IssBlocked;
    public bool IsUpMovingPiece;
    private bool alreadyAttacked;


    private bool isBlocked;
    public bool IsBlocked { get { return isBlocked; } }
    public bool AlreadyAttacked {  get { return alreadyAttacked; } set { if (IsAdjacentClearable) { alreadyAttacked = value; } } }

    void Awake()
    {
        levelComponent = GetComponent<Levels>();
        movableComponent = GetComponent<MovablePiece>();
        colorComponent = GetComponent<ColorPiece>();
        clearableComponent = GetComponent<ClearablePiece>();
        animationcomponent = GetComponent<AnimationManager>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame

    public void Initt(int _x, int _y, Gride _grid, Gride.PieceType _type)
    {
        x = _x;
        y = _y;
        grid = _grid;
        type = _type;
    }

    public void InitMulti(float _x, float _y, Gride _grid, Gride.PieceType _type)
    {
        a = _x;
        b = _y;
        grid = _grid;
        type = _type;
    }

    public bool IsMovable()
    {
        if (IssBlocked)
        {
            return false;
        }
        return movableComponent != null;
    }

    public bool IsColored()
    {
        return colorComponent != null;
    }

    public bool IsClearable()
    {
        return clearableComponent != null;
    }


    public void OnMouseEnter()
    {
        grid.EnterPiece(this);
        //grid.EnteredPiece = this;
    }

    public void OnMouseDown()
    {
        grid.PressPiece(this);
        //grid.PressedPiece = this;
    }

    public void OnMouseUp()
    {
        grid.ReleasePiece();
    }
    //touch input
    public void OnTouchEnter()
    {
        grid.EnterPiece(this);
    }

    public void OnTouchDown()
    {
        grid.PressPiece(this);
    }

    public void OnTouchUp()
    {
        grid.ReleasePiece();
    }

    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {

            }
            else if(touch.phase == TouchPhase.Moved)
            {

            }
            else if(touch.phase == TouchPhase.Ended)
            {

            }
        }
    }
}
