using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootInstance : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("predator"))
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
