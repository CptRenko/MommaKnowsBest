using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus
{
    PLAY,
    GAME_OVER
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_panel;

    private GameStatus m_gStatus;

    public GameStatus GStatus { get => m_gStatus; set => m_gStatus = value; }
    public GameObject Panel { get => m_panel; set => m_panel = value; }


    // Start is called before the first frame update
    void Start()
    {
        GStatus = GameStatus.PLAY;
        
        //Needed if the game it's restarted
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(GStatus == GameStatus.GAME_OVER)
        {
            Panel.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
