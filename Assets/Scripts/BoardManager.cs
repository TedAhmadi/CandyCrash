using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour {
    public static BoardManager instance;
    public List<Sprite> objects = new List<Sprite>();
    public GameObject tile;
    private GameObject[,] tiles;
    private Vector2 offset;

    private float timer = 59.59f;

    public int xSize, ySize;

    private bool flag = false;
    private bool onPauseFlag = false;
    private float lastSwap;
    private float startPauseTime = 0f;
    private float stoptPauseTime = 0f;

    public bool onGameStart { get; set; }
    public bool IsShifting { get; set; }
    public bool gamewithTimer { get; set; }
    void Awake()
    {
        onGameStart = false;
    }
    // Use this for initialization
    void Start() {
        instance = GetComponent<BoardManager>();

        offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

    public void StartGamewithMove()
    {
        Clear();
        CreateBoard(offset.x, offset.y);
        GUIManager.instance.Score = 0;
        lastSwap = Time.time;
        onGameStart = true;
        GUIManager.instance.MoveCounter = 20;

        gamewithTimer = false;
    }

    public void StartGameWithTimer()
    {
        Clear();
        CreateBoard(offset.x, offset.y);
        GUIManager.instance.Score = 0;
        lastSwap = Time.time;
        onGameStart = true;
        timer = 59.59f;

        gamewithTimer = true;
    }

    private void CreateBoard(float xOffset, float yOffset) // Creat Board
    {
        tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
                tiles[x, y] = newTile;

                newTile.SetActive(true);
                newTile.transform.parent = transform;

                List<Sprite> possibleObjects = new List<Sprite>();  // */make sure there isnt any ready row/column 
                possibleObjects.AddRange(objects);

                possibleObjects.Remove(previousLeft[y]);
                possibleObjects.Remove(previousBelow);

                Sprite newSprite = possibleObjects[Random.Range(0, possibleObjects.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[y] = newSprite;
                previousBelow = newSprite;   // make sure there isnt any ready row/column*/

            }
        }
    }

    public void OnPause()
    {
        startPauseTime =  Time.time;
        onPauseFlag = true;
        //Debug.Log("startPauseTime: " + startPauseTime);
    }

    public void unPause()
    {
        stoptPauseTime = Time.time;
        onPauseFlag = false;
    }

    private void ShiftTilesDown(int x, int yStart) //shifting the Tails Down when is found and clear matches
    {
        IsShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;
        lastSwap = Time.time;
        startPauseTime = 0f;
        stoptPauseTime = 0f;
        flag = false;

        for (int y = yStart; y < ySize; y++)
        {
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            {
                nullCount++;
            }
            renders.Add(render);
        }

        if(renders.Count == 1)
        {
            List<Sprite> possibleObjects = new List<Sprite>();
            possibleObjects.AddRange(objects);

            renders[0].sprite = possibleObjects[Random.Range(0, possibleObjects.Count)];
        }

        for (int k = 0; k < renders.Count - 1; k++)
        {
            List<Sprite> possibleObjects = new List<Sprite>();
            possibleObjects.AddRange(objects);

            renders[k].sprite = renders[k + 1].sprite;
            // make sure is not go go out of bounds
            if (x > 0) { possibleObjects.Remove(tiles[x - 1, ySize - 1].GetComponent<SpriteRenderer>().sprite); }
            if (x < xSize - 1) { possibleObjects.Remove(tiles[x + 1, ySize - 1].GetComponent<SpriteRenderer>().sprite); }
            if (ySize - 1 > 0) { possibleObjects.Remove(tiles[x, ySize - 2].GetComponent<SpriteRenderer>().sprite); }

            renders[k + 1].sprite = possibleObjects[Random.Range(0, possibleObjects.Count)];
        }
        IsShifting = false;
    }

    private void Clear()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<SpriteRenderer>().sprite = null;
            }
        }
    }
    public void Hint()
    {
        for (int x = 0; x < xSize; x++) //nested loop to find all matches
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<Tile>().PossibleRow();
            }
        }
    }

    public void FindNull() //find the position of null Tail to shift the column down
    {
        for (int x = 0; x < xSize; x++) //nested loop to find all matches
        {
            for (int y = 0; y < ySize; y++)
            {
                //Debug.Log("time: " + ((Time.time - (stoptPauseTime - startPauseTime)) - lastSwap));
                if (!flag && ((Time.time - (stoptPauseTime - startPauseTime)) - lastSwap) >= 20f && !onPauseFlag && onGameStart)
                {
                    flag = tiles[x, y].GetComponent<Tile>().PossibleRow();
                }
                tiles[x, y].GetComponent<Tile>().FindallMatch();
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    ShiftTilesDown(x, y);
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (gamewithTimer)
        {
            if (!onPauseFlag)
            {
                timer -= Time.deltaTime;
                GUIManager.instance.Timer = timer;
            }
        }
        if (onGameStart)
        {
            FindNull();
        }
    }
}
