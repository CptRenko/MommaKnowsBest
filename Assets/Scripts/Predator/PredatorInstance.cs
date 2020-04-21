using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PredatorInstance : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject m_player;
    private GameObject m_children;
    private GameObject m_prey;
    private GameObject m_game;
    private Transform m_toPos;
    private string trackTag; //What's the object to follow?
    private GameController m_gController;
    private SpriteRenderer m_sRenderer;

    public GameObject Player { get => m_player; set => m_player = value; }
    public GameObject Children { get => m_children; set => m_children = value; }
    public Transform ToPos { get => m_toPos; set => m_toPos = value; }
    public GameObject Prey { get => m_prey; set => m_prey = value; }
    public string TrackTag { get => trackTag; set => trackTag = value; }
    public GameObject Game { get => m_game; set => m_game = value; }
    public GameController GController { get => m_gController; set => m_gController = value; }
    public SpriteRenderer SRenderer { get => m_sRenderer; set => m_sRenderer = value; }

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Children = GameObject.FindGameObjectWithTag("children");
        Prey = GameObject.FindGameObjectWithTag("food");
        ToPos = SelectTarget(Player.transform, Children.transform, Prey.transform);
        GController = GameObject.FindGameObjectWithTag("controller").GetComponent<GameController>();
        SRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform != null && GController.GStatus != GameStatus.GAME_OVER)
        {
            if(ToPos != null)
            { 
                transform.position = Vector3.MoveTowards(transform.position,
                        ToPos.position, Time.fixedDeltaTime * 2.5f);

                SRenderer.flipX = Player.transform.position.x > transform.position.x;

                float distance = (transform.position - ToPos.position).sqrMagnitude;
                if (distance < 1)
                {
                    switch (TrackTag)
                    {
                        case "player":
                            Destroy(Player.gameObject);
                            break;

                        case "children":
                            Destroy(Children.gameObject);
                            break;

                        case "Prey":
                            Destroy(Prey.gameObject);
                            break;
                    }

                    Destroy(gameObject);
                }
            }
            else //Prey was destroyed, need search a new target and get a new instance of Prey
            {
                Prey = GameObject.FindGameObjectWithTag("food");
                ToPos = SelectTarget(Player.transform, Children.transform, Prey.transform);
            }
        }
    }

    Transform SelectTarget(Transform player, Transform children, Transform prey)
    {
        float distanceToPlayer = (transform.position - player.position).sqrMagnitude;
        float distanceToChildren = (transform.position - children.position).sqrMagnitude;
        float distanceToPrey = (transform.position - prey.position).sqrMagnitude;

        //Avoid losing unfairly
        if(distanceToPlayer < 25)
        {
            distanceToPlayer = Mathf.Infinity;
        }
        else if(distanceToChildren < 25)
        {
            distanceToChildren = Mathf.Infinity;
        }
        else if(distanceToPrey < 25)
        {
            distanceToPrey = Mathf.Infinity;
        }

        //0 = Player, 1 = Children, 2 = Prey
        List<float> data = new List<float>
        {
            distanceToPlayer,
            distanceToChildren,
            distanceToPrey
        };

        int index = data.IndexOf(data.Min());

        switch(index)
        {
            case 0:
                trackTag = "player";
                return player;
            case 1:
                trackTag = "children";
                return children;
            case 2:
                trackTag = "prey";
                return prey;
            default:
                return null;
        }
    }
}
