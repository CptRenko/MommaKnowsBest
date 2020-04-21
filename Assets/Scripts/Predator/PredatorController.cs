using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorController : MonoBehaviour
{

    [SerializeField]
    private PrefabDB m_prefabDb;

    [SerializeField]
    private GameObject[] m_spawnPoints;

    private GameObject m_pInstance;
    private GameController m_gController;
    private bool canStart; //We need a lag of 5 seconds at start of the game.

    public PrefabDB PrefabDb { get => m_prefabDb; set => m_prefabDb = value; }
    public GameObject[] SpawnPoints { get => m_spawnPoints; set => m_spawnPoints = value; }
    public GameObject PInstance { get => m_pInstance; set => m_pInstance = value; }
    public bool CanStart { get => canStart; set => canStart = value; }
    public GameController GController { get => m_gController; set => m_gController = value; }


    // Start is called before the first frame update
    void Start()
    {
        CanStart = false;
        GController = GameObject.FindGameObjectWithTag("controller").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time;

        if(PInstance == null && CanStart)
        {
            GeneratePredator();
        }

        if(time > 5 && !CanStart)
        {
            CanStart = true;
        }
    }

    void GeneratePredator()
    {
        //Without this, launch random errors like : THIS OBJECT NOT EXISTS IDIOT.
        if(GController.GStatus != GameStatus.GAME_OVER)
        { 
            int rd = Random.Range(0, SpawnPoints.Length);
            PInstance = Instantiate(PrefabDb.m_prefabs.m_predator, SpawnPoints[rd].transform.position, SpawnPoints[rd].transform.rotation);
            PInstance.AddComponent<PredatorInstance>();
            PInstance.AddComponent<BoxCollider2D>();
            PInstance.tag = "predator";
        }
    }

    
}
