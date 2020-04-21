using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//@TODO:
//- Squanking enemies
//- Melee attack (Claws and Pecking)
//- HP

public enum MotherStatus
{
    IDLE,
    LOAD_CHILDREN,
    SHOOTING,
    LOAD_FOOD
}

public class ShootData
{
    public Vector3 direction;
    public GameObject shootObject;
}

public class MotherController : MonoBehaviour
{
    [SerializeField]
    private SpritesDB m_spriteDatabase;

    [SerializeField]
    private PrefabDB m_prefabDatabase;

    [SerializeField]
    private float m_speed;

    private SpriteRenderer m_sRenderer;
    private Rigidbody2D m_rb;
    private Vector2 m_direction;
    private List<ShootData> m_poolSpawn;
    private MotherStatus m_mStatus;
    private PreyInstance m_preyObject;
    private bool m_isShooting;
    private int m_hp;
    private GameController m_gController;
    private Animator m_anim;

    public SpritesDB SpriteDatabase { get => m_spriteDatabase; set => m_spriteDatabase = value; }
    public SpriteRenderer SRenderer { get => m_sRenderer; set => m_sRenderer = value; }
    public float Speed { get => m_speed; set => m_speed = value; }
    public Rigidbody2D Rb { get => m_rb; set => m_rb = value; }
    public Vector2 Direction { get => m_direction; set => m_direction = value; }
    public PrefabDB PrefabDatabase { get => m_prefabDatabase; set => m_prefabDatabase = value; }
    public List<ShootData> PoolSpawn { get => m_poolSpawn; set => m_poolSpawn = value; }
    public MotherStatus MStatus { get => m_mStatus; set => m_mStatus = value; }
    public PreyInstance PreyObject { get => m_preyObject; set => m_preyObject = value; }
    public bool IsShooting { get => m_isShooting; set => m_isShooting = value; }
    public int Hp { get => m_hp; set => m_hp = value; }
    public GameController GController { get => m_gController; set => m_gController = value; }
    public Animator Anim { get => m_anim; set => m_anim = value; }

    // Start is called before the first frame update
    void Start()
    {
        //Set sprite on database for player at SpriteRenderer
        SRenderer = GetComponent<SpriteRenderer>();
        SRenderer.sprite = SpriteDatabase.m_spriteMother;
        Rb = GetComponent<Rigidbody2D>();
        PoolSpawn = new List<ShootData>();
        MStatus = MotherStatus.IDLE;
        IsShooting = false;
        Hp = 3;
        GController = GameObject.FindGameObjectWithTag("controller").GetComponent<GameController>();
        Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //While PreyObject it's != to null, the gameobject exists on scene.
        //So... i track the object until it disappears and I change the status
        if(PreyObject == null && this.MStatus == MotherStatus.LOAD_FOOD)
        {
            this.MStatus = MotherStatus.IDLE;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 shootDirection = Vector3.zero;

        if(GController.GStatus == GameStatus.PLAY)
        { 
            Direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Rb.MovePosition(Rb.position + Direction * Speed * Time.fixedDeltaTime);
            SRenderer.flipX = (Input.GetAxisRaw("Horizontal") > 0) ? true : false;
            
        }

        if ((Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0) || (Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0))
        {
            Anim.SetBool("isMove", true);
        }
        else
        {
            Anim.SetBool("isMove", false);
        }

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            shootDirection = Vector3.right;

        }
        else
        {
            shootDirection = Vector3.left;
        }

        //if (Input.GetKeyDown(KeyCode.Space) && (MStatus != MotherStatus.LOAD_CHILDREN && MStatus != MotherStatus.LOAD_FOOD))
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //GameObject shoot = Instantiate(PrefabDatabase.m_prefabs.m_shoot, this.transform.position, this.transform.rotation);
            if(PoolSpawn.Count < 5 && MStatus == MotherStatus.LOAD_FOOD)
            {
                IsShooting = true;
                GameObject shoot = Instantiate(PrefabDatabase.m_prefabs.m_shoot, (transform.position + shootDirection), transform.rotation);
                ShootData sData = new ShootData();
                sData.direction = shootDirection;
                sData.shootObject = shoot;
                PoolSpawn.Add(sData);
            }
        }
        else
        {
            IsShooting = false;
        }

        for (int i = 0; i < PoolSpawn.Count; i++)
        {
            if(PoolSpawn[i].shootObject != null)
            {
                if(PoolSpawn[i].direction == Vector3.right)
                {
                    PoolSpawn[i].shootObject.transform.position += Vector3.right * Time.fixedDeltaTime * 4.0f;
                }
                else
                {
                    PoolSpawn[i].shootObject.transform.position += Vector3.left * Time.fixedDeltaTime * 4.0f;
                }
            }
            else 
            {
                //If it's null it's because the gameobject was destroyed.
                PoolSpawn.RemoveAt(i);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("predator"))
        {
            if(Hp > 0)
            {
                Hp -= 1;
            }
            else
            {
                GController.GStatus = GameStatus.GAME_OVER;
                Destroy(gameObject);
            }

            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("children") && MStatus != MotherStatus.LOAD_FOOD)
        {
            ChildrenController cController = collision.gameObject.GetComponent<ChildrenController>();
            if (Input.GetKeyDown(KeyCode.X) && MStatus != MotherStatus.SHOOTING)
            {
                cController.Grab();
                MStatus = MotherStatus.LOAD_CHILDREN;
                Anim.SetBool("isGrab", true);
                //Anim.SetBool("isGrab", false);
            }

            //Player it's on position of a Nest
            if(Input.GetKeyDown(KeyCode.C) && MStatus == MotherStatus.LOAD_CHILDREN && cController.IsNestAvailable)
            {
                cController.Stop();
                MStatus = MotherStatus.IDLE;
            }
        }

        if(collision.CompareTag("food") && MStatus != MotherStatus.LOAD_CHILDREN)
        {
            collision.gameObject.GetComponent<PreyInstance>().Grab();
            MStatus = MotherStatus.LOAD_FOOD;

            //Track the instance of Food, because Stop() it's called in other script
            PreyObject = collision.GetComponent<PreyInstance>();
        }
    }
}
