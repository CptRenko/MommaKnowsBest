using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Yeah... practically copy paste from ChildrenControlller... ^_^
//I could use a Interface but... meh, IT'S JUST WORKS.

public enum FoodStatus
{
    IDLE,
    FOLLOW
}

public class PreyInstance : MonoBehaviour
{

    private int m_parts;
    private FoodStatus m_status;
    private GameObject m_pCharacter;
    private Animator m_anim;

    public FoodStatus Status { get => m_status; set => m_status = value; }
    public GameObject PCharacter { get => m_pCharacter; set => m_pCharacter = value; }
    public int Parts { get => m_parts; set => m_parts = value; }
    public Animator Anim { get => m_anim; set => m_anim = value; }

    private void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PCharacter = GameObject.FindGameObjectWithTag("Player");
        Status = FoodStatus.IDLE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Status == FoodStatus.FOLLOW)
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(PCharacter.transform.position.x - 0.6f, PCharacter.transform.position.y - 0.4f), Time.time);

            transform.localScale = PCharacter.transform.localScale;
        }
    }

    public void Grab()
    {
        this.Status = FoodStatus.FOLLOW;
       
    }

    public void Stop()
    {
        Destroy(gameObject);
    }

}

