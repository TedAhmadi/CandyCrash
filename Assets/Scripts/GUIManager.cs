using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {
    public static GUIManager instance;

    public GameObject startGamePanel;
    public GameObject gameOverPanel;
    public GameObject moveCounterPanel;
    public GameObject TimerPanel;


    public Text scoreTxt;
    public Text moveCounterTxt;
    public Text timerText;
    public Text muteText;
    public Text yourScoreTxt;

    private int score, moveCounter;
    private float timer;
    void Awake()
    {
        instance = GetComponent<GUIManager>();
        moveCounter = 20;
    }

    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
            scoreTxt.text = score.ToString();
        }
    }

    public float Timer
    {
        get
        {
            return timer;
        }

        set
        {
            timer = value;
            if (timer <= 0)
            {
                timer = 0;
                BoardManager.instance.gamewithTimer = false;
                StartCoroutine(WaitForShifting());
            }
            timerText.text = timer.ToString("F");
        }
    }

    public int MoveCounter
    {
        get
        {
            return moveCounter;
        }

        set
        {
            moveCounter = value;
            if (moveCounter <= 0)
            {
                moveCounter = 0;
                StartCoroutine(WaitForShifting());
            }
            moveCounterTxt.text = moveCounter.ToString();
        }
    }

    public void MuteText(string MuteText) //change the Mute Button Tag when is pressed Mute/UnMute
    {
        muteText.text = MuteText;
    }

    public void GameOver()
    {
        moveCounterPanel.SetActive(false);
        TimerPanel.SetActive(false);
        BoardManager.instance.onGameStart = false;
        gameOverPanel.SetActive(true);
        yourScoreTxt.text = score.ToString();
    }

    private IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
        yield return new WaitForSeconds(.25f);
        GameOver();
    }

    public void PlayAgain()
    {
        gameOverPanel.SetActive(false);
        startGamePanel.SetActive(true);
    }


    public void ExitGame()
    {
        // If we are running in a standalone build of the game
        // Quit the application
        Application.Quit();

        // If we are running in the editor
        #if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
