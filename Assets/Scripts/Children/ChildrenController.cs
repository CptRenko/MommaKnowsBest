using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum ChildrenStatus
{
    IDLE,
    FOLLOW
}

public class ChildrenController : MonoBehaviour
{
    [SerializeField]
    private SpritesDB m_spriteDatabase;
    
    [SerializeField]
    private GameObject m_pCharacter;

    [SerializeField]
    private List<Transform> m_nestSpawnPoint;

    [SerializeField]
    private Slider m_countdown;

    [SerializeField]
    private GameObject m_controller;

    [SerializeField]
    private AudioSource m_aSource;


    private SpriteRenderer m_spriteRenderer;
    private ChildrenStatus m_status;
    private int trackIndexNest; //Track the actual nest to prevent player,  put in the same place for a time.
    private bool m_isNestAvailable;
    private GameController m_gController;
    private PreyController m_pController;
    private bool m_resetTime;

    public SpritesDB SpriteDatabase { get => m_spriteDatabase; set => m_spriteDatabase = value; }
    public SpriteRenderer SpriteRenderer { get => m_spriteRenderer; set => m_spriteRenderer = value; }
    public ChildrenStatus Status { get => m_status; set => m_status = value; }
    public GameObject PCharacter { get => m_pCharacter; set => m_pCharacter = value; }
    public List<Transform> NestSpawnPoint { get => m_nestSpawnPoint; set => m_nestSpawnPoint = value; }
    public int TrackIndexNest { get => trackIndexNest; set => trackIndexNest = value; }
    public bool IsNestAvailable { get => m_isNestAvailable; set => m_isNestAvailable = value; }
    public Slider Countdown { get => m_countdown; set => m_countdown = value; }
    public GameController GController { get => m_gController; set => m_gController = value; }
    public GameObject Controller { get => m_controller; set => m_controller = value; }
    public PreyController PController { get => m_pController; set => m_pController = value; }
    public AudioSource ASource { get => m_aSource; set => m_aSource = value; }
    public bool ResetTime { get => m_resetTime; set => m_resetTime = value; }


    // Start is called before the first frame update
    void Start()
    {
        //Set sprite on database for player at SpriteRenderer
        SpriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer.sprite = SpriteDatabase.m_spriteChildren;
        Status = ChildrenStatus.IDLE;

        IsNestAvailable = false;
        int rd = Random.Range(0, NestSpawnPoint.Count);
        TrackIndexNest = rd;
        this.transform.position = NestSpawnPoint[rd].transform.position;
        Countdown.maxValue = 100;
        GController = Controller.GetComponent<GameController>();
        PController = Controller.GetComponent<PreyController>();
        StartCoroutine(GetHungry());
        ResetTime = false;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if(this.Status == ChildrenStatus.FOLLOW)
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(PCharacter.transform.position.x - 0.3f, PCharacter.transform.position.y - 0.3f), Time.time);
        }
    }

    public void Grab()
    {
        this.Status = ChildrenStatus.FOLLOW;
    }

    public void Stop()
    {
        this.Status = ChildrenStatus.IDLE;
        TrackIndexNest = NestNearest();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("food") && PCharacter.GetComponent<MotherController>().MStatus != MotherStatus.LOAD_CHILDREN)
        {
            if(collision.gameObject != null)
            { 
                collision.GetComponent<PreyInstance>().Stop();
                PController.Parts = 0;
                PController.UiAnimController.SetInteger("count", 0);
                ResetTime = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("nest") && collision.transform.position != NestSpawnPoint[TrackIndexNest].position)
        {
            IsNestAvailable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IsNestAvailable = false;
    }

    //Check on what nest the player it's
    //Based on NestNearest from PreyController
    public int NestNearest()
    {
        int id = -1;

        //Vector3 spawnPointSelected = PreySpawnPoint[spawnPosIndex].position;
        float closestDistance = Mathf.Infinity;

        for(int i = 0; i < NestSpawnPoint.Count; i++)
        {
            Vector3 distance = NestSpawnPoint[i].position - transform.position;
            float distSquared = distance.sqrMagnitude;

            if(distSquared < closestDistance)
            {
                id = i;
                closestDistance = distSquared;
            }
        }
        return id;
    }

    private void OnDestroy()
    {
        GController.GStatus = GameStatus.GAME_OVER;
    }

    IEnumerator GetHungry()
    {
        float t = 0;
        while(t < 7)
        {
            Debug.Log(t);
            ASource.pitch += 0.1f;

            if(ResetTime)
            {
                ASource.pitch = 1;
                t = 0;
                ResetTime = false;
            }

            yield return new WaitForSeconds(1f);
            t++;
        }

        Destroy(gameObject);
    }

}
