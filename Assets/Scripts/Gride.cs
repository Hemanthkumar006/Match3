using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Random = UnityEngine.Random;
using UnityEditor;
using System;
using System.Net;
using System.Threading.Tasks;
using static Gride;
using UnityEngine.AdaptivePerformance.Samsung.Android;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using TMPro;
using static UnityEditor.Progress;

public class Gride : MonoBehaviour
{
    // Start is called before the first frame update

    public enum PieceType
    {
        EMPTY,
        NORMAL,
        MULTI,
        BUBBLE,
        NULL,
        SPECIAL
    }

    public enum PieceName
    {
        GENERAL,SQUARE2X2,SQUARE3X3,
        RECTANGLE1X2,RECTANGLE2X1,RECTANGLE1X3,RECTANGLE3X1,RECTANGLE1X4,RECTANGLE4X1,
        RECTANGLE1X5,RECTANGLE5X1,RECTANGLE1X6,RECTANGLE6X1,RECTANGLE1X7,RECTANGLE7X1,
        RECTANGLE1X8,RECTANGLE8X1,RECTANGLE1X9,RECTANGLE9X1,RECTANGLE1X10,RECTANGLE10X1,
        RECTANGLE2X3,RECTANGLE2X4,RECTANGLE2X5,RECTANGLE3X2,RECTANGLE3X4,RECTANGLE3X5,
        RECTANGLE4X2,RECTANGLE4X3,RECTANGLE4X5,RECTANGLE5X2,RECTANGLE5X3,RECTANGLE5X4
    }

    public enum Visiblity
    {
        RED,
        GREEN,
        BLUE,
        ORANGE,
        MAGENTA,
        NULL
    }

    public TextMeshProUGUI movesText;
    public Levels level;
    public ClearablePiece clearablepiece;
    public int xDim;
    public int yDim;
    public GameObject panel;
    private CanvasGroup cangroup;
    public int moves;
    public int score = 0;
    public float Bordersize;
    public Dictionary<int, int> positionDict;
    private HashSet<(int, int)> busyCoords = new HashSet<(int, int)>();
    private List<GamePiece> adjacentattackpieces = new List<GamePiece>();
    private List<GamePiece> Specialpieces = new List<GamePiece>();
    private List<GamePiece> UpMovingpieces = new List<GamePiece>();
    private List<GamePiece> OpenClosepieces = new List<GamePiece>();
    public List<GameObject> Gps = new List<GameObject>();

    private Vector3 presspos;
    private float DragThreshold = 0.25f;
    private bool PauseInput = false;
    private GamePiece swap1;
    private GamePiece swap2;

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
        public int value;

    }
    public struct GridAttributes
    {
        public int rows;
        public int columns;
        public int moves;
        public int[,] cells;

    }

    public PiecePrefab[] piecePrefabs;
    private Dictionary<PieceType, GameObject> piecePrefabDict;

    [SerializeField]

    public GameObject Background;
    public Tile[,] tiles;
    public Stack<GamePiece>[,] GpStack;
    public GamePiece[,] pieces;
    public GamePiece[,] Sppieces;
    public GamePiece[,] Backgroundpieces;
    public float FillTime = 0.1f;
    private bool inverse = false;

    private bool IsStackable;
    private bool currentlyfilling;

    private GamePiece pressedPiece;
    public GamePiece PressedPiece { get { return pressedPiece; } set { pressedPiece = value; } }
    private GamePiece enteredPiece;
    public GamePiece EnteredPiece { get { return enteredPiece; } set { enteredPiece = value; } }

    private GamePiece hconnectingpiece;
    private GamePiece vconnectingpiece;

    public int[] Temparray = { 0, 1, 2, 3, 4};


    public int[,] arr1 =
    {
        { 1,1,2,1,2,2,1,2,3},
        { 2,2,1,2,2,1,2,1,4},
        { 15,15,2,1,2,2,1,2,3},
        { 2,4,1,2,2,1,2,1,4},
        { 3,15,2,15,2,15,1,15,3},
        { 15,3,15,2,15,1,15,1,15},
        { 1,1,2,1,2,2,1,2,3},
        { 2,15,1,15,2,15,1,15,4},
        { 15,1,15,1,15,2,15,2,15}

    };

    public int[,] arr2;
    public int[,] movearray =
        {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        
        };
    public int[,] CustomPieceGeneration;

    private void Awake()
    {
        Levels level = GetComponent<Levels>();
        cangroup = panel.GetComponent<CanvasGroup>();
        Dictgetter();
        Shufflearray();
        UpdateMoves();
    }
    void FetchData()
    {
        Debug.Log("Fetching..");
    }


    void Shufflearray()
    {
        int n = Temparray.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = Temparray[i];
            Temparray[i] = Temparray[randomIndex];
            Temparray[randomIndex] = temp;
        }
    }
    void Dictgetter()
    {
        positionDict = new Dictionary<int, int>();
        for(int i = 0; i < Temparray.Length; i++)
        {
            positionDict.Add(Temparray[i], i);
        }
    }

    bool ColorInRange(int i)
    {
        return(i >= 0 && i < 7);
    }
    public bool NumberinRange(int i , int j)
    {
        return(i >= 0 && i <= j);
    }
    void Start()
    {
        SetupCamera();
        Debug.Log("Shuffle array is done");
        piecePrefabDict = new Dictionary<PieceType, GameObject>(); 
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        // Setup Background tiles
        tiles = new Tile[xDim, yDim];
        if(IsStackable) 
        {
            GpStack = new Stack<GamePiece>[xDim, yDim];
        }
        
        //Setup Gamepieces
        //y are rows and x are cols
        pieces = new GamePiece[xDim, yDim];
        Backgroundpieces = new GamePiece[xDim,yDim];
        arr2 = new int[xDim, yDim];
        for (int j = 0; j < yDim; j++)
        {
            for (int i = 0; i < xDim; i++)
            {
                GameObject background = (GameObject)Instantiate(Background, Vector3.zero, Quaternion.identity);
                background.name = "Background(" + i + ", " + j + ")";
                //background.transform.parent = transform;
                background.transform.SetParent(this.transform, true);

                tiles[i, j] = background.GetComponent<Tile>();
                tiles[i, j].setcoord(i, j, this);
                tiles[i,j].Mover(i, j);

                if (ColorInRange(arr1[j, i]))
                {
                    GameObject piecetype = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], Vector3.zero, Quaternion.identity);
                    piecetype.name = "Piece( " + i + " , " + j + " )";

                    //piecetype.transform.parent = transform;
                    piecetype.transform.SetParent(this.transform, true);
                    pieces[i, j] = piecetype.GetComponent<GamePiece>();
                    pieces[i, j].Initt(i, j, this, PieceType.NORMAL);
                    int randcol = arr1[j,i];
                    int index = positionDict[randcol];
                    int randomcolor = Temparray[index];
                    //int randomcolor = arr1[j,i];
                    pieces[i, j].ColorComponent.SetColor((ColorPiece.ColorType)randomcolor);
                    /*if (pieces[i, j].IsMovable())
                    {
                        pieces[i, j].MovableComponent.Mover(i, j);
                    }*/

                    pieces[i, j].transform.localPosition = new Vector3(i, j, 0);
                }
                
                else
                {
                    arr2[j, i] = arr1[j, i];
                }
                if (arr1[j,i] == 15)
                {
                    SpawnNewPiece(i, j, PieceType.BUBBLE);
                }

            }
            
        }
        float screenHeight = Camera.main.orthographicSize;
        float aboveScreenHeight = screenHeight + 10f;
        this.transform.position = new Vector3(transform.position.x, aboveScreenHeight, 0);
        this.transform.DOMove(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.Linear);
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3 ((float) (xDim - 1)/2f,(float) (yDim-1)/2f, -10f);
        if(xDim < 4 && yDim < 4)
        {
            Camera.main.orthographicSize = 0.5f;
        }
        float AspectRatio = (float) Screen.width / (float) Screen.height;
        float Verticalheight = (float)yDim / 2f +  Bordersize;
        float Horizontalheight = ((float)xDim / 2f +  Bordersize) / AspectRatio;
        Camera.main.orthographicSize = (Verticalheight > Horizontalheight) ? Verticalheight : Horizontalheight;
    }


    // Update is called once per frame

    

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y, 0);
    }

    void SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject) Instantiate(piecePrefabDict[type],Vector3.zero,Quaternion.identity);
        newPiece.transform.SetParent(this.transform);
        pieces[x,y] = newPiece.GetComponent<GamePiece>();
        pieces[x,y].Initt(x,y,this,type);
        pieces[x,y].transform.localPosition = new Vector3(x,y,0);
        
    }

    void SpawnMultiPiece(float x, float y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type],new Vector3(x,y,0), Quaternion.identity);
        newPiece.transform.parent = transform;
   
    }

    public IEnumerator Fill()
    {
        bool needsRefill = true;
        while (needsRefill)
        {
            yield return new WaitForSeconds(FillTime);
            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(FillTime);
            }
            needsRefill = ClearAllValidMatches();
            PauseInput = needsRefill;
        }       

    }

    public bool FillStep()
    {
        bool hasMoved = false;
        for(int y = 1; y < yDim; y++)
        {
            for(int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }
                GamePiece piece = pieces[x,y];
                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y - 1];
                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y - 1, FillTime);
                        pieces[x, y - 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        hasMoved = true;
                    }
                    else
                    {
                        for(int diag = -1; diag <=1; diag++)
                        {
                            if(diag != 0)
                            {
                                int diagX = x + diag;
                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y - 1];
                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;
                                        for (int AboveY = y; AboveY < yDim; AboveY++)
                                        {
                                            GamePiece Abovepiece = pieces[diagX, AboveY];
                                            if (Abovepiece.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!Abovepiece.IsMovable() && Abovepiece.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }
                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y - 1, FillTime);
                                            pieces[diagX, y - 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            hasMoved = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }    
            }
        }

        for (int x = 0; x < xDim; x++)
        {

            GamePiece pieceTop = pieces[x, yDim - 1];
            if (pieceTop.Type == PieceType.EMPTY)
            {
                Destroy(pieceTop.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], Vector3.zero, Quaternion.identity);

                newPiece.transform.SetParent(transform, true);
                newPiece.transform.localPosition = new Vector3(x, yDim, 0);


                pieces[x, yDim - 1] = newPiece.GetComponent<GamePiece>();
                Animator animator = newPiece.GetComponent<Animator>();
                animator.Play("NormalCreate");
                pieces[x, yDim - 1].Initt(x, yDim - 1, this, PieceType.NORMAL);
                pieces[x, yDim - 1].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, yDim - 1].ColorComponent.NumCols));
                pieces[x, yDim - 1].MovableComponent.Move(x, yDim - 1, FillTime);
                hasMoved = true;
            }
        }

        //Debug.Log("One Fill Loop Cycle Completed.");
        //Debug.Log("The value of inverse is " + inverse);

        return hasMoved;
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return ((piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1));
    }

    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
        presspos = Input.mousePosition;
    }
        

    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    public void ReleasePiece()
    {
        float dist = Vector3.Distance(Input.mousePosition, presspos);
        Debug.Log(presspos);
        if (dist < DragThreshold)
        {
            Debug.Log("Pressed but not dragged.");
            if (pressedPiece != null && pressedPiece.AnimationComponent != null && !PauseInput)
            {
                pressedPiece.AnimationComponent.PlayTouchAnim();
                Debug.Log("Touch animation played.");
            }
            else
            {
                Debug.LogWarning("Pressed piece or AnimationComponent is null.");
            }
        }
        else
        {
            if (IsAdjacent(pressedPiece, enteredPiece))
            {
                SwapPieces(pressedPiece, enteredPiece);
                swap1 = pressedPiece;
                swap2 = enteredPiece;
            }
        }
        pressedPiece = null;
        enteredPiece = null;
        //presspos = Vector3.zero;
    }
    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if(moves >= 1 && !PauseInput)
        {
            StartCoroutine(Swappieceroutine(piece1, piece2));
        }
        else
        {
            Debug.Log("Out of Moves.");
        }
        

    }

    public IEnumerator Swappieceroutine(GamePiece piece1,GamePiece piece2)
    {
        //bool result = false;
        List<GamePiece> One = new List<GamePiece>();
        List<GamePiece> one = new List<GamePiece>();
        List<GamePiece> two = new List<GamePiece>();
        /*if(piece1.IsUpMovingPiece ||  piece2.IsUpMovingPiece)
        {
            result = true;
        }*/

        if (piece1.IsMovable() && !piece2.IsMovable())
        {
            if(piece2.Type == PieceType.EMPTY)
            {
                pieces[piece1.X, piece1.Y] = piece2;
                pieces[piece2.X, piece2.Y] = piece1;
                piece1.MovableComponent.Move(piece2.X,piece2.Y, FillTime);
            }
            One = GetMatch(piece2,piece2.X, piece2.Y);
            if(One == null)
            {
                //
            }
        }
        else if (piece2.IsMovable() && !piece1.IsMovable())
        {
            if (piece1.Type == PieceType.EMPTY)
            {              
                pieces[piece1.X,piece1.Y] = piece2;
                pieces[piece2.X,piece2.Y] = piece1;
                piece2.MovableComponent.Move(piece1.X,piece1.Y,FillTime);
            }
            One = GetMatch(piece1, piece1.X, piece1.Y);
            if(One == null)
            {
                //
            }
        }
        else if (piece1.IsMovable() && piece2.IsMovable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;
            int piece1X = piece1.X;
            int piece1Y = piece1.Y;
            Animator anim1 = piece1.GetComponent<Animator>();
            Animator anim2 = piece2.GetComponent<Animator>();
            anim1.Play("NormalSwapOne");
            anim2.Play("NormalSwapTwo");
            piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
            piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);
            yield return new WaitForSeconds(FillTime);
            one = GetMatch(piece1, piece1.X, piece1.Y);
            two = GetMatch(piece2, piece2.X, piece2.Y);
        }        

        if (one == null && two == null)
        {
            Debug.Log("Match not found.");
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;
            int piece1X = piece1.X;
            int piece1Y = piece1.Y;
            piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
            piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);
            yield return new WaitForSeconds(FillTime);
        }
        else if(one != null ||  two != null || One != null) 

        {
            Debug.Log("match found.");
            ClearAllValidMatches();
            pressedPiece = null;
            enteredPiece = null;
            swap1 = null;
            swap2 = null;
            StartCoroutine(Fill());
            AdjacentRemover();
            UpdateMoves();
            Debug.Log("Moves Left = " + moves);
            if(moves <= 0)
            {
                panel.SetActive(true);
                //cangroup.DOFade(1f, 5f);
                //fadeInButton.onClick.AddListener(OnFadeInButtonClick);
                PauseInput = true;
            }
        }
        if (one != null) { one.Clear(); }
        if (two != null) { two.Clear(); }
        if (one == null) { one = new List<GamePiece>(); }
        if (two == null) { two = new List<GamePiece>(); }
    }

    void UpdateMoves()
    {
        Animator anim = movesText.GetComponent<Animator>();
        moves--;
        movesText.text = moves.ToString();
        if (moves <= 4 && moves != 0)
        {  
            anim.Play("TextFlicker");
        }
        /*else if(moves == 0)
        {
            anim.speed = 0;
        }*/      
    }
    public bool InRange(int newX, int newY)
    {
        return(newX >= 0 && newX < xDim && newY >= 0 && newY < yDim);
    }


    public List<GamePiece> GetMatch(GamePiece piece , int x , int y) 
    {
        if (piece.IsMovable())
        {
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalpieces = new List<GamePiece>();
            List<GamePiece> hverticalpieces = new List<GamePiece>();
            List<GamePiece> verticalpieces = new List<GamePiece>();
            List<GamePiece> vhorizontalpieces = new List<GamePiece>();
            List<GamePiece> Matchingpieces = new List<GamePiece>();

            horizontalpieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xoffset = 1; xoffset < xDim; xoffset++)
                {
                    int X;
                    if (dir == 0)
                    {
                        X = x - xoffset;
                    }
                    else
                    {
                        X = x + xoffset;
                    }

                    if (X < 0 || X >= xDim)
                    {
                        break;
                    }

                    if (pieces[X, y].IsColored() && pieces[X, y].ColorComponent.Color == color)
                    {
                        horizontalpieces.Add(pieces[X, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (horizontalpieces.Count >= 3)
            {
                for (int i = 0; i < horizontalpieces.Count; i++)
                {
                    if (!Matchingpieces.Contains(horizontalpieces[i]))
                    {
                        Matchingpieces.Add(horizontalpieces[i]);
                    }                    
                    for(int dir = 0; dir <= 1; dir++)
                    {
                        for (int yoffset = 1; yoffset < yDim; yoffset++)
                        {
                            int Y;
                            if(dir == 0)
                            {
                                Y = horizontalpieces[i].Y - yoffset;
                            }
                            else
                            {
                                Y = horizontalpieces[i].Y + yoffset;
                            }
                            if(Y < 0 || Y >= yDim)
                            {
                                break;
                            }
                            if (pieces[horizontalpieces[i].X, Y].IsColored() && pieces[horizontalpieces[i].X, Y].ColorComponent.Color == color)
                            {
                                hverticalpieces.Add(pieces[horizontalpieces[i].X, Y]);
                                if(hverticalpieces.Count >= 2)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if(hverticalpieces.Count < 2)
                    {
                        hverticalpieces.Clear();
                    }
                }
                if(hverticalpieces.Count >= 2)
                {
                    for (int j = 0; j < hverticalpieces.Count; ++j)
                    {
                        if (!Matchingpieces.Contains(hverticalpieces[j]))
                        {
                            Matchingpieces.Add(hverticalpieces[j]);
                        }
                        
                    }
                }
                
            }


            verticalpieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yoffset = 1; yoffset < yDim; yoffset++)
                {
                    int Y;
                    if (dir == 0)
                    {
                        Y = y - yoffset;
                    }
                    else
                    {
                        Y = y + yoffset;
                    }

                    if (Y < 0 || Y >= yDim)
                    {
                        break;
                    }

                    if (pieces[x, Y].IsColored() && pieces[x, Y].ColorComponent.Color == color)
                    {
                        verticalpieces.Add(pieces[x, Y]);
                    }
                    else 
                    { 
                        break; 
                    }
                }
            }

            if (verticalpieces.Count >= 3)
            {
                for (int i = 0; i < verticalpieces.Count; i++)
                {
                    if (!Matchingpieces.Contains(verticalpieces[i]))
                    {
                        Matchingpieces.Add(verticalpieces[i]);
                    }                   
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xoffset = 1; xoffset < xDim; xoffset++)
                        {
                            int X;
                            if (dir == 0)
                            {
                                X = verticalpieces[i].X - xoffset;
                            }
                            else
                            {
                                X = verticalpieces[i].X + xoffset;
                            }
                            if (X < 0 || X >= xDim)
                            {
                                break;
                            }
                            if (pieces[X, verticalpieces[i].Y].IsColored() && pieces[X, verticalpieces[i].Y].ColorComponent.Color == color)
                            {
                                vhorizontalpieces.Add(pieces[X, verticalpieces[i].Y]);
                                if(vhorizontalpieces.Count >= 2)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if(vhorizontalpieces.Count < 2)
                    {
                        vhorizontalpieces.Clear();
                    }
                }
                if(vhorizontalpieces.Count >= 2)
                {
                    for(int i = 0; i< vhorizontalpieces.Count; i++)
                    {
                        if (!Matchingpieces.Contains(vhorizontalpieces[i]))
                        {
                            Matchingpieces.Add((vhorizontalpieces[i]));
                        }
                    }
                }
            }

            if(Matchingpieces.Count >= 3)
            {
                return Matchingpieces;
            }
        }
        return null;
    }

    

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;
        bool hasSppotential = false;
        GamePiece Sppiece;

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);
                    if (match != null)
                    {
                        Sppiece = match[Random.Range(0, match.Count)];
                        if(match.Count >= 4)
                        {
                            foreach (var item in match)
                            {
                                if (item == swap1 || item == swap2)
                                {
                                    Sppiece = item;
                                    break;
                                }
                            }
                            //StartCoroutine(sphandler(match,Sppiece));
                            //hasSppotential = true;
                            //needsRefill = true;
                        }

                        for (int i = 0; i < match.Count; i++)
                        {
                            if (Clearpiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                            }
                        }

                    }
                }
            }
        }
        return needsRefill;
    }

    IEnumerator HandleMultiAnim(List<GamePiece> match,GamePiece Sppiece)
    {
        List<(int x, int y)> coordinates = new List<(int, int)>();
        int SppieceX = Sppiece.X;
        int SppieceY = Sppiece.Y;

        for (int i = 0; i < match.Count; i++)
        {
            int x = match[i].X;  // Cast x to int
            int y = match[i].Y;  // Cast y to int

            // Add the tuple (x, y) to the list
            coordinates.Add((x, y));
        }
        foreach (GamePiece item in match)
        {
            if (item != Sppiece)
            {
                SpriteRenderer render = item.GetComponent<SpriteRenderer>();
                render.sortingOrder = 1;
                int itemx = item.X; int itemy = item.Y;
                item.MovableComponent.Move(Sppiece.X, Sppiece.Y, 0.24f);
                item.ClearableComponent.Clear();
                //SpawnNewPiece(itemx, itemy,PieceType.EMPTY);

                Debug.Log("Clearing " + item.name);
                //SpawnNewPiece(itemX, itemY, PieceType.EMPTY);
            }
            else if(item == Sppiece)
            {
                Debug.Log("This is sppiece and not destroyed.");
            }
        }
        yield return new WaitForSeconds(0.20f);

        /*foreach(GamePiece x in match)
        {
            if(x != Sppiece)
            {
                x.ClearableComponent.Clear();
            }
        }*/
        Sppiece.ClearableComponent.Clear();
        SpawnNewPiece(SppieceX, SppieceY, PieceType.BUBBLE);
        yield return new WaitForSeconds(.15f);

        for (int i = 0; i < coordinates.Count; i++)
        {
            int x = coordinates[i].x;  
            int y = coordinates[i].y;
            if (x == SppieceX && y == SppieceY)
                continue;
            SpawnNewPiece(x, y, PieceType.EMPTY);
        }
    }

    IEnumerator sphandler(List<GamePiece> m, GamePiece pie)
    {
        var coordKey = (pie.X, pie.Y);

        // Prevent duplicate bubble spawns
        if (busyCoords.Contains(coordKey))
        {
            yield break;
        }

        busyCoords.Add(coordKey);

        List<(int x, int y)> coordlist = new List<(int x, int y)>();
        int pieX = pie.X, pieY = pie.Y;

        foreach (GamePiece piece in m)
        {
            if (piece != pie)
            {
                coordlist.Add((piece.X, piece.Y));
                piece.MovableComponent.Move(pieX, pieY, .25f);
                piece.ClearableComponent.Clear();
            }
        }

        yield return new WaitForSeconds(0.25f);
        pie.ClearableComponent.Clear();
        yield return new WaitForSeconds(.15f);
        // Double-check if the piece is still clear before spawning bubble
        GamePiece currentPiece = pieces[pieX,pieY];
        if (currentPiece != null && currentPiece.Type != PieceType.BUBBLE)
        {
            SpawnNewPiece(pieX, pieY, PieceType.BUBBLE);
        }

        for (int i = 0; i < coordlist.Count; i++)
        {
            int x = coordlist[i].x;
            int y = coordlist[i].y;

            if (x == pieX && y == pieY)
                continue;

            SpawnNewPiece(x, y, PieceType.EMPTY);
        }

        busyCoords.Remove(coordKey);
    }

    IEnumerator SpSelector(int res,int x,int y)
    {
        yield return new WaitForSeconds(0.14f);
        if(res >= 4)
        {
            SpawnNewPiece(x, y, PieceType.BUBBLE);
        }
    }

    public bool Clearpiece(int x, int y,bool res = false)
    {
        if (pieces[x, y].IsClearable() && !pieces[x,y].ClearableComponent.isBeingCleared)
        {
            pieces[x,y].Value -= 1;
            //Debug.Log($"The Value of Piece at Index[{x},{y}] is {pieces[x, y].Value}");
            if (pieces[x, y].Value < 1)
            {
                pieces[x, y].ClearableComponent.Clear();
                score += 2;
                ClearBackgroundPiece(x, y);
                if (!res)
                {
                    ClearObstacles(x, y);
                }                
                SpawnNewPiece(x, y, PieceType.EMPTY);
                return true;
            }
        }        
        return false;
    }

    public bool ClearpiecebySp(int x, int y)
    {
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.isBeingCleared)
        {
            pieces[x, y].Value -= 1;
            //Debug.Log($"The Value of Piece at Index[{x},{y}] is {pieces[x, y].Value}");
            if (pieces[x, y].Value < 1)
            {
                pieces[x, y].ClearableComponent.Clear();
                score += 2;
                ClearBackgroundPiece(x, y);
                SpawnNewPiece(x, y, PieceType.EMPTY);
                return true;
            }
        }
        return false;
    }
    public void ClearObstacles(int x , int y)
    {
        for(int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (pieces[adjacentX,y].Sppieceholder && !pieces[adjacentX, y].LinkedPiece.AlreadyAttacked)
                {
                    pieces[adjacentX, y].LinkedPiece.Value -= 1;
                    if (pieces[adjacentX, y].LinkedPiece.Value < 1)
                    {
                        pieces[adjacentX, y].LinkedPiece.ClearableComponent.Clear();
                        score += 4;
                        SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                    }
                    else if (pieces[adjacentX, y].Value >= 1)
                    {
                        pieces[adjacentX, y].LinkedPiece.AlreadyAttacked = true;
                        adjacentattackpieces.Add(pieces[adjacentX, y].LinkedPiece);
                    }
                }
                
                else if (pieces[adjacentX, y].IsClearable() && pieces[adjacentX, y].IsAdjacentClearable && !pieces[adjacentX,y].AlreadyAttacked)
                {
                    pieces[adjacentX, y].Value -= 1;
                    
                    if (pieces[adjacentX, y].Value < 1)
                    {
                        pieces[adjacentX, y].ClearableComponent.Clear();
                        score += 4;
                        SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                    }
                    else if (pieces[adjacentX,y].Value >= 1)
                    {
                        //pieces[adjacentX, y].AnimationComponent.Playconsecanims();
                        pieces[adjacentX, y].AlreadyAttacked = true;
                        adjacentattackpieces.Add(pieces[adjacentX, y]);
                    }
                }                
            }
        }

        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if (pieces[x,adjacentY].Sppieceholder && !pieces[x, adjacentY].LinkedPiece.AlreadyAttacked)
                {
                    pieces[x, adjacentY].LinkedPiece.Value -= 1;

                    if (pieces[x, adjacentY].LinkedPiece.Value < 1)
                    {
                        pieces[x, adjacentY].LinkedPiece.ClearableComponent.Clear();
                        score += 4;
                        SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                    }
                    else if (pieces[x, adjacentY].Value >= 1)
                    {
                        pieces[x, adjacentY].LinkedPiece.AlreadyAttacked = true;
                        adjacentattackpieces.Add(pieces[x, adjacentY].LinkedPiece);
                    }
                }
                else if (pieces[x, adjacentY].IsClearable() && pieces[x,adjacentY].IsAdjacentClearable && !pieces[x,adjacentY].AlreadyAttacked)
                {
                    pieces[x, adjacentY].Value -= 1;
                    
                    if (pieces[x,adjacentY].Value < 1)
                    {
                        pieces[x, adjacentY].ClearableComponent.Clear();
                        score += 4;
                        SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                    }
                    else if (pieces[x, adjacentY].Value >= 1)
                    {
                        pieces[x, adjacentY].AlreadyAttacked = true;
                        adjacentattackpieces.Add(pieces[x, adjacentY]);
                    }
                }
                
            }
        }
    }

    public bool Filler()
    {
        bool hasMoved = false;
        for (int y = 0; y < yDim; y++)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }
                GamePiece piece = pieces[x, y];
                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y - 1];
                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y - 1, FillTime);
                        pieces[x, y - 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        hasMoved = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;
                                if (inverse)
                                {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y - 1];
                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;
                                        for (int AboveY = y; AboveY < yDim; AboveY++)
                                        {
                                            GamePiece Abovepiece = pieces[diagX, AboveY];
                                            if (Abovepiece.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!Abovepiece.IsMovable() && Abovepiece.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }

                                        }
                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y - 1, FillTime);
                                            pieces[diagX, y - 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            hasMoved = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
        return hasMoved;
    }
    public void ClearRow(int row)
    {
        for(int x = 0; x < xDim; x++)
        {
            Clearpiece(x, row);
        }
    }

    public void ClearColumn(int column)
    {
        for (int y = 0; y < yDim; y++)
        {
            Clearpiece(column, y);
        }
    }

    public void ClearAllPieces()
    {
        for(int y = 0; y < yDim; ++y)
        {
            for(int x = 0; x < xDim; ++x)
            {
                Clearpiece(x, y,true);
            }
        }

        StartCoroutine(Fill());
    }
    public void CustomRefill()
    {
        if(Isfavorable())
        {

        }
    }
    public void Startlessmoveswarning()
    {
        Debug.Log("Warning Started! Few Moves Left.");
    }

    public bool Isfavorable()
    {
        return true;
    }

    public bool HardtoWin()
    {
        bool hardertowin = false;
        int targetpieces = 5;
        if(moves <= 4)
        {
            if(targetpieces >= moves + 3)
            {
                hardertowin = true;
            }
        }
        return hardertowin;
    }

    public void ClearBackgroundPiece(int x, int y)
    {
        if (Backgroundpieces[x, y] != null && Backgroundpieces[x, y].IsClearable() && !Backgroundpieces[x,y].ClearableComponent.isBeingCleared)
        {
            Backgroundpieces[x, y].Value -= 1;
            if (Backgroundpieces[x,y].Value >= 1)
            {
                Console.WriteLine("Not yet");
            }
            else if(Backgroundpieces[x, y].Value < 1)
            {
                Backgroundpieces[x,y].ClearableComponent.Clear();
                score += 2;
            }           
        }
    }

    public int[,] GenerateCoordinates(float x, float y, PieceName name)
    {
        if(name == PieceName.SQUARE2X2)
        {
            int[,] coordinates = new int[4, 2]
            {
                { (int) (x + 0.5f), (int) (y - 0.5f) },
                { (int) (x - 0.5f), (int) (y - 0.5f) },
                { (int) (x - 0.5f), (int) (y + 0.5f) },
                { (int) (x + 0.5f), (int) (y + 0.5f) }
            };
            return coordinates;
        }
        else if (name == PieceName.SQUARE3X3)
        {
            int[,] coordinates = new int[9, 2]
            {
                { (int) (x - 1f), (int) (y + 1f) },
                { (int) (x - 1f), (int) y },
                { (int) (x - 1f), (int) (y - 1f) },
                { (int) x, (int) (y + 1f) },
                { (int) x, (int) y },
                { (int) x, (int) (y - 1f) },
                { (int) (x + 1f), (int) (y + 1f) },
                { (int) (x + 1f), (int) y },
                { (int) (x + 1f), (int) (y + 1f) }
            };
            return coordinates;
        }

        else if (name == PieceName.RECTANGLE1X2)
        {
            int[,] coordinates = new int[2, 2]
            {
                { (int) x, (int)(y + 0.5f) },
                { (int) x, (int)(y - 0.5f) }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE2X1)
        {
            int[,] coordinates = new int[2, 2]
            {
                { (int)(x + 0.5f), (int) y },
                { (int)(x - 0.5f), (int) y }
            };
            return coordinates;
        }

        else if (name == PieceName.RECTANGLE1X3)
        {
            int[,] coordinates = new int[3, 2]
            {
                { (int) x, (int) y },
                { (int) x, (int)(y + 1f) },
                { (int) x, (int)(y - 1f) } 
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE3X1)
        {
            int[,] coordinates = new int[3, 2]
            {
                { (int)(x), (int) y },
                { (int)(x + 1f), (int) y },
                { (int)(x - 1f), (int) y }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE1X4)
        {
            int[,] coordinates = new int[4, 2]
            {
                { (int) x, (int)(y - 0.5f) },
                { (int) x, (int)(y + 0.5f) },
                { (int) x, (int)(y - 1.5f) },
                { (int) x, (int)(y + 1.5f) }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE4X1)
        {
            int[,] coordinates = new int[4, 2]
            {
                { (int)(x - 0.5f), (int) y },
                { (int)(x + 0.5f), (int) y },
                { (int)(x - 1.5f), (int) y },
                { (int)(x + 1.5f), (int) y },
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE1X5)
        {
            int[,] coordinates = new int[5, 2]
            {
                { (int) x, (int) y },
                { (int) x, (int)(y - 1f) },
                { (int) x, (int)(y + 1f) },
                { (int) x, (int)(y - 2f) },
                { (int) x, (int)(y + 2f) }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE5X1)
        {
            int[,] coordinates = new int[5, 2]
            {
                { (int) x, (int) y },
                { (int)(x - 1f), (int) y },
                { (int)(x + 1f), (int) y },
                { (int)(x - 2f), (int) y },
                { (int)(x + 2f), (int) y }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE1X6)
        {
            int[,] coordinates = new int[6, 2]
            {
                { (int) x, (int)(y - 0.5f) },
                { (int) x, (int)(y + 0.5f) },
                { (int) x, (int)(y - 1.5f) },
                { (int) x, (int)(y + 1.5f) },
                { (int) x, (int)(y - 2.5f) },
                { (int) x, (int)(y + 2.5f) }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE6X1)
        {
            int[,] coordinates = new int[6, 2]
            {
                { (int)(x - 0.5f), (int) y },
                { (int)(x + 0.5f), (int) y },
                { (int)(x - 1.5f), (int) y },
                { (int)(x + 1.5f), (int) y },
                { (int)(x - 2.5f), (int) y },
                { (int)(x + 2.5f), (int) y }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE1X7)
        {
            int[,] coordinates = new int[7, 2]
            {
                { (int) x, (int) y },
                { (int) x, (int)(y - 1f) },
                { (int) x, (int)(y + 1f) },
                { (int) x, (int)(y - 2f) },
                { (int) x, (int)(y + 2f) },
                { (int) x, (int)(y - 3f) },
                { (int) x, (int)(y + 3f) }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE7X1)
        {
            int[,] coordinates = new int[7, 2]
            {
                { (int) x, (int) y },
                { (int)(x - 1f), (int) y },
                { (int)(x + 1f), (int) y },
                { (int)(x - 2f), (int) y },
                { (int)(x + 2f), (int) y },
                { (int)(x - 3f), (int) y },
                { (int)(x + 3f), (int) y }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE1X8)
        {
            int[,] coordinates = new int[8, 2]
            {
                { (int) x, (int)(y - 0.5f) },
                { (int) x, (int)(y + 0.5f) },
                { (int) x, (int)(y - 1.5f) },
                { (int) x, (int)(y + 1.5f) },
                { (int) x, (int)(y - 2.5f) },
                { (int) x, (int)(y + 2.5f) },
                { (int) x, (int)(y - 3.5f) },
                { (int) x, (int)(y + 3.5f) }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE8X1)
        {
            int[,] coordinates = new int[8, 2]
            {
                { (int)(x - 0.5f), (int) y },
                { (int)(x + 0.5f), (int) y },
                { (int)(x - 1.5f), (int) y },
                { (int)(x + 1.5f), (int) y },
                { (int)(x - 2.5f), (int) y },
                { (int)(x + 2.5f), (int) y },
                { (int)(x - 3.5f), (int) y },
                { (int)(x + 3.5f), (int) y }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE1X9)
        {
            int[,] coordinates = new int[9, 2]
            {
                { (int) x, (int) y },
                { (int) x, (int)(y - 1f) },
                { (int) x, (int)(y + 1f) },
                { (int) x, (int)(y - 2f) },
                { (int) x, (int)(y + 2f) },
                { (int) x, (int)(y - 3f) },
                { (int) x, (int)(y + 3f) },
                { (int) x, (int)(y - 4f) },
                { (int) x, (int)(y + 4f) }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE9X1)
        {
            int[,] coordinates = new int[9, 2]
            {
                { (int) x, (int) y },
                { (int)(x - 1f), (int) y },
                { (int)(x + 1f), (int) y },
                { (int)(x - 2f), (int) y },
                { (int)(x + 2f), (int) y },
                { (int)(x - 3f), (int) y },
                { (int)(x + 3f), (int) y },
                { (int)(x - 4f), (int) y },
                { (int)(x + 4f), (int) y }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE1X10)
        {
            int[,] coordinates = new int[10, 2]
            {
                { (int) x, (int)(y - 0.5f) },
                { (int) x, (int)(y + 0.5f) },
                { (int) x, (int)(y - 1.5f) },
                { (int) x, (int)(y + 1.5f) },
                { (int) x, (int)(y - 2.5f) },
                { (int) x, (int)(y + 2.5f) },
                { (int) x, (int)(y - 3.5f) },
                { (int) x, (int)(y + 3.5f) },
                { (int) x, (int)(y - 4.5f) },
                { (int) x, (int)(y + 4.5f) }
            };
            return coordinates;
        }
        else if (name == PieceName.RECTANGLE10X1)
        {
            int[,] coordinates = new int[10, 2]
            {
                { (int)(x - 0.5f), (int) y },
                { (int)(x + 0.5f), (int) y },
                { (int)(x - 1.5f), (int) y },
                { (int)(x + 1.5f), (int) y },
                { (int)(x - 2.5f), (int) y },
                { (int)(x + 2.5f), (int) y },
                { (int)(x - 3.5f), (int) y },
                { (int)(x + 3.5f), (int) y },
                { (int)(x - 4.5f), (int) y },
                { (int)(x + 4.5f), (int) y }
            };
            return coordinates;
        }
        return null;
    }


    void SetLinkedPieces(GamePiece piece)
    {
        int[,] piecesaray = GenerateCoordinates((float)piece.X, (float)piece.Y, piece.Piecename);
        int rows = piecesaray.GetLength(0);
        for(int i = 0; i < rows; i++)
        {
            int a = piecesaray[i, 0];
            int b = piecesaray[i, 1];
            if (pieces[a, b].Sppieceholder)
            {
                pieces[a, b].LinkedPiece = piece;
            }           
        }
    }
    void AdjacentRemover()
    {
        if(adjacentattackpieces.Count > 0)
        {
            foreach (var item in adjacentattackpieces)
            {
                item.AlreadyAttacked = false;
            }
            adjacentattackpieces.Clear();
        }
        upmover();
        
    }

    void upmover()
    {
        if(UpMovingpieces.Count > 0)
        {
            foreach(var item in UpMovingpieces)
            {
                GamePiece p = pieces[item.X, item.Y +1];
                SwapPieces(item, p);
            }
        }
    }

    void openclose()
    {
        if(OpenClosepieces.Count > 0)
        {
            foreach(var item in OpenClosepieces)
            {
                
            }
        }
    }

    public void spawnltsec(List<GamePiece> matchers , GamePiece xpiece)
    {
        foreach(GamePiece piece in matchers)
        {
            piece.MovableComponent.Move(xpiece.X, xpiece.Y,FillTime);
            piece.ClearableComponent.Clear();
        }
    }
    
    List<GamePiece> MoveRecomendation()
    {
        int highscore = 0, mediumscore = 0, lowscore = 0;
        //bool possiblemoves = true;
        List<GamePiece> matches = new List<GamePiece>();
        List<GamePiece> match11 = new List<GamePiece>();
        List<GamePiece> match10 = new List<GamePiece>();
        List<GamePiece> match01 = new List<GamePiece>();
        List<GamePiece> matchn11 = new List<GamePiece>();
        for(int y = 0; y < yDim; y++)
        {
            for(int x = 0; x < xDim; x++)
            {
                GamePiece piece = pieces[x, y];
                match11 = GetMatch(piece, piece.X , piece.Y + 1);
                match01 = GetMatch(piece, piece.X, piece.Y - 1);
                matchn11 = GetMatch(piece, piece.X -1, piece.Y);
                match10 = GetMatch(piece, piece.X + 1, piece.Y);
            }
        }
        if(match01.Count == 0 && match10.Count == 0 && match11.Count == 0 && matchn11.Count == 0)
        {
            Debug.Log("The Board needs to be reshuffled.");
        }
        match01.Clear();
        match10.Clear();
        match11.Clear();
        matchn11.Clear();
        return matches;
    }
}
