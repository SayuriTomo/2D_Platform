using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDogController : MonoBehaviour
{
    private Rigidbody2D rb;

    private Transform CurrentPoint;

    public GameObject PointA;
    public GameObject PointB;

    public float MoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentPoint = PointA.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 Point = CurrentPoint.position - transform.position;
        if(CurrentPoint == PointB.transform)
        {
            rb.velocity = new Vector2(MoveSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-MoveSpeed, 0);
        }

        if(Vector2.Distance(transform.position, CurrentPoint.position)<0.5f
            && CurrentPoint == PointB.transform)
        {
            Flip();
            CurrentPoint = PointA.transform;
        }
        
        if (Vector2.Distance(transform.position, CurrentPoint.position) < 0.5f
           && CurrentPoint == PointA.transform)
        {
            Flip();
            CurrentPoint = PointB.transform;
        }
    }

    private void Flip()
    {
        Vector3 LocalScale = transform.localScale;
        LocalScale.x *= -1;
        transform.localScale = LocalScale;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(PointB.transform.position, 0.5f);
        Gizmos.DrawLine(PointA.transform.position, PointB.transform.position);
    }


}
