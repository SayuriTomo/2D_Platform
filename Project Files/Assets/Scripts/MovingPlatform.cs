using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float MoveSpeed;
    public float WaitTime;

    public Transform[] MovePos;

    private int PosIndex;
    private Transform PlayerDefTransform;
   
    // Start is called before the first frame update
    void Start()
    {
        PosIndex = 1;
        PlayerDefTransform = GameObject.FindGameObjectWithTag("Player").transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, 
            MovePos[PosIndex].position, MoveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, MovePos[PosIndex].position) < 0.1f)
        {
            if (WaitTime < 0.0f)
            {
                if (PosIndex == 0)
                {
                    PosIndex = 1;
                }
                else 
                { 
                    PosIndex = 0; 
                }
                WaitTime = 0.5f;
            }
            else
            {
                WaitTime -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if(Collision.tag == "Player")
        {
            Collision.gameObject.transform.parent = gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D Collision)
    {
        if (Collision.tag == "Player")
        {
            Collision.gameObject.transform.parent = PlayerDefTransform;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(MovePos[0].transform.position, 0.5f);
        Gizmos.DrawWireSphere(MovePos[1].transform.position, 0.5f);
        Gizmos.DrawLine(MovePos[0].transform.position, MovePos[1].transform.position);
    }

}
