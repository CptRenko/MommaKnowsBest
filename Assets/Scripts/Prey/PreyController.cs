using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreyController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_children;

    [SerializeField]
    private List<Transform> m_preySpawnPoint;

    [SerializeField]
    private PrefabDB m_prefabDatabase;

    [SerializeField]
    private SpritesDB m_spriteDatabase;

    [SerializeField]
    private Image m_uiFoodCarrying;
    
    [SerializeField]
    private GameObject m_pCharacter;

    private GameObject m_preyInstance;
    private int m_parts;
    private PreyInstance m_pInstance;
    private MotherController m_mController;
    private Animator m_uiAnimController;

    public PrefabDB PrefabDatabase { get => m_prefabDatabase; set => m_prefabDatabase = value; }
    public GameObject PreyInstance { get => m_preyInstance; set => m_preyInstance = value; }
    public SpritesDB SpriteDatabase { get => m_spriteDatabase; set => m_spriteDatabase = value; }
    public int Parts { get => m_parts; set => m_parts = value; }
    public PreyInstance PInstance { get => m_pInstance; set => m_pInstance = value; }
    public GameObject PCharacter { get => m_pCharacter; set => m_pCharacter = value; }
    public MotherController MController { get => m_mController; set => m_mController = value; }
    public List<Transform> PreySpawnPoint { get => m_preySpawnPoint; set => m_preySpawnPoint = value; }
    public GameObject Children { get => m_children; set => m_children = value; }
    public Image UiFoodCarrying { get => m_uiFoodCarrying; set => m_uiFoodCarrying = value; }
    public Animator UiAnimController { get => m_uiAnimController; set => m_uiAnimController = value; }

    // Start is called before the first frame update
    void Start()
    {
        GeneratePrey();
        MController = PCharacter.GetComponent<MotherController>();
        UiAnimController = UiFoodCarrying.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PreyInstance == null)
        {
            GeneratePrey();
        }
    }

    private void FixedUpdate()
    {
        if(Parts == 0 && PInstance != null)
        {
            PInstance.Stop();
        }
        else 

        if (PInstance.Status == FoodStatus.FOLLOW)
        {
            UiFoodCarrying.gameObject.SetActive(true);
            if (MController.IsShooting)
            {
                if (Parts > 0)
                {
                    Parts -= 1;
                }
            }

            UiAnimController.SetInteger("count", Parts);
        }
    }

    public void GeneratePrey()
    {
        int rndSpawnPoint = 0;
        int rndSprite = 0;

        do
        {
            rndSpawnPoint = Random.Range(0, PreySpawnPoint.Count);
        }
        while (NestNearest(PreySpawnPoint[rndSpawnPoint].transform.position));

        rndSprite = Random.Range(0, SpriteDatabase.m_spritePrey.Length);

        PreyInstance = Instantiate(PrefabDatabase.m_prefabs.m_prey, PreySpawnPoint[rndSpawnPoint].transform.position, PreySpawnPoint[rndSpawnPoint].transform.rotation);
        PreyInstance.GetComponent<SpriteRenderer>().sprite = SpriteDatabase.m_spritePrey[rndSprite].m_sprite;
        PreyInstance.tag = "food";
        PInstance = PreyInstance.GetComponent<PreyInstance>();

        //Bee needs a fix in his scale
        if (SpriteDatabase.m_spritePrey[rndSprite].m_name.Equals("bee"))
        {
            PInstance.Anim.runtimeAnimatorController = (RuntimeAnimatorController)RuntimeAnimatorController.Instantiate(Resources.Load("Animation/PreyBeeController", typeof(RuntimeAnimatorController)));
            PreyInstance.transform.localScale = new Vector3(3.0f, 3.0f);
        }
        else
        {
            PInstance.Anim.runtimeAnimatorController = (RuntimeAnimatorController)RuntimeAnimatorController.Instantiate(Resources.Load("Animation/Prey", typeof(RuntimeAnimatorController)));
        }

        PreyInstance.SetActive(true);
        Parts = 5;
    }
    
    //Check if the actual PreySpawnPosition, it's the point nearest to Children
    public bool NestNearest(Vector3 spawnPointSelected)
    {
        //Vector3 spawnPointSelected = PreySpawnPoint[spawnPosIndex].position;
        float closestDistance = Mathf.Infinity;
        Transform spawnClosest = null;

        //Check what spawpoint it's the nearest to the Children
        foreach(Transform spawnPoint in PreySpawnPoint)
        {
            Vector3 distance = spawnPoint.position - Children.transform.position;
            float distSquared = distance.sqrMagnitude;

            if(distSquared < closestDistance)
            {
                closestDistance = distSquared;
                spawnClosest = spawnPoint;
            }
        }

        if(spawnClosest.position == spawnPointSelected)
        {
            return true;
        }

        return false;
    }

    /*private void DisablePreyGUI()
    {
        UiFoodCarrying.gameObject.SetActive(false);
    }*/
}
