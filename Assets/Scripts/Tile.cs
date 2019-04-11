using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;    
    private SpriteRenderer render;

    private bool isSelected = false;
    private bool matchFound = false;

    private Vector2[] pathDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    public void Select()
    {
        isSelected = true;
        render.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
        SoundManager.instance.PlaySound(Clip.Select);
    }

    public void Deselect()
    {
        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }

    // Use this for initialization
    void Start () {
        
    }

    public void SwapSprite(SpriteRenderer render2) //swap Tail
    {
        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
        if(!BoardManager.instance.gamewithTimer)
        {
            GUIManager.instance.MoveCounter--;
        }
        SoundManager.instance.PlaySound(Clip.Select);
    }

    private bool InRange() //used Raycast to see if the second selected Tail is in the range of first selected Tail 
    {
        bool flag = false;
        for (int i = 0; i < pathDirections.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, pathDirections[i]);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject == previousSelected.gameObject)
                {
                    flag = true;
                }
            }
        }
        return flag;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void OnMouseDown()
    {
        if (!IsPointerOverUIObject() )
        {
            if (isSelected) // Is it already selected?
            {
                Deselect();
            }
            else
            {
                if (previousSelected == null) // Is it the first tile selected?
                {
                        Select();
                }
                else // Is it the second tile
                {
                    if (InRange()) //If is in the range
                    {
                        SwapSprite(previousSelected.render); //Swap 
                        previousSelected.Deselect();
                    }
                    else //if is not in the range decelect the prev and select new
                    {
                        previousSelected.GetComponent<Tile>().Deselect();
                        Select();
                    }
                }
            }
            
        }
    }
    
    private void ClearMatch(List<GameObject> matchingTiles)
    {
        for (int i = 0; i < matchingTiles.Count; i++)
        {
            matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
        }
        SoundManager.instance.PlaySound(Clip.Clear);
        matchFound = true;
    }

    private List<GameObject> FindMatch(Vector2 castDir) //using Raycast to find matching Tails (keep going in same direction till last 2 tail not equal)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles;
    }

    private void SearchMatch(Vector2[] paths) //search on Horizontal/vertical
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }

        if (matchingTiles.Count >= 2) // if there is more than 2 same Tail exist in the list
        {
            ClearMatch(matchingTiles); // set the Tails on the list to a null for all matchs on the list (ClearMatch)
        }
        else
        {
            return;
        }
    }

    public void FindallMatch() 
    {
        Vector2[] HorizontalPath = new Vector2[2] { Vector2.left, Vector2.right };
        Vector2[] VerticalPath = new Vector2[2] { Vector2.up, Vector2.down };
        SearchMatch(HorizontalPath);
        SearchMatch(VerticalPath);

        if (matchFound)
        {
            render.sprite = null;
            GUIManager.instance.Score += 10;
            matchFound = false;
        }
    }

    Vector2 topRight = new Vector2(2, 2);
    Vector2 topLeft = new Vector2(-2, 2);
    Vector2 downLeft = new Vector2(-2, -2);
    Vector2 downRight = new Vector2(2, -2);

    List<GameObject> matchingTiles = new List<GameObject>();

    public bool PossibleRow() //using RaycastHit find the possible row/column 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, topRight);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position, topLeft);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, downLeft);
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position, downRight);

        

        if ((hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)&&
            (hit1.collider != null && hit1.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
            )
        {
            hit.collider.GetComponent<SpriteRenderer>().color = selectedColor;
            hit1.collider.GetComponent<SpriteRenderer>().color = selectedColor;
            render.color = selectedColor;
            matchingTiles.Add(hit.collider.gameObject);
            matchingTiles.Add(hit1.collider.gameObject);
            
            StartCoroutine(WaitFor());
            return true;
        }
        if ((hit2.collider != null && hit2.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) &&
            (hit3.collider != null && hit3.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
             )
        {
            hit2.collider.GetComponent<SpriteRenderer>().color = selectedColor;
            hit3.collider.GetComponent<SpriteRenderer>().color = selectedColor;
            render.color = selectedColor;
            matchingTiles.Add(hit2.collider.gameObject);
            matchingTiles.Add(hit3.collider.gameObject);

            StartCoroutine(WaitFor());
            return true;
        }
        if ((hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) &&
            (hit3.collider != null && hit3.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
             )
        {
            hit.collider.GetComponent<SpriteRenderer>().color = selectedColor;
            hit3.collider.GetComponent<SpriteRenderer>().color = selectedColor;
            render.color = selectedColor;
            matchingTiles.Add(hit.collider.gameObject);
            matchingTiles.Add(hit3.collider.gameObject);

            StartCoroutine(WaitFor());
            return true;
        }
        if ((hit1.collider != null && hit1.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) &&
            (hit2.collider != null && hit2.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
             )
        {
            hit1.collider.GetComponent<SpriteRenderer>().color = selectedColor;
            hit2.collider.GetComponent<SpriteRenderer>().color = selectedColor;
            render.color = selectedColor;
            matchingTiles.Add(hit1.collider.gameObject);
            matchingTiles.Add(hit2.collider.gameObject);

            StartCoroutine(WaitFor());
            return true;
        }
        else
        {
            return false;
        }

    }
    private IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(0.50f);
        for (int i = 0; i < matchingTiles.Count; i++)
        {
            matchingTiles[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        render.color = Color.white;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
}
