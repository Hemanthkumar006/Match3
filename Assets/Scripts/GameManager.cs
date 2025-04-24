using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Gride gridComponent;
    public Gride GridComponent { get { return gridComponent; } }
    void Awake()
    {
        gridComponent = GetComponent<Gride>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
