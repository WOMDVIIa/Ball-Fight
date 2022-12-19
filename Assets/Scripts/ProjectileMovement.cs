using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public GameObject target;

    private float speed = 15.0f;
    private float collisionForce = 13.0f;
    private Vector3 trajectory;

    // Start is called before the first frame update
    void Start()
    {
        //trajectory = (target.gameObject.transform.position - transform.position).normalized;
        //transform.rotation = Quaternion.LookRotation(trajectory);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            trajectory = (target.gameObject.transform.position - transform.position).normalized;
            //transform.rotation = Quaternion.LookRotation(trajectory);
            transform.Translate(trajectory * speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody targetRb = collision.gameObject.GetComponent<Rigidbody>();
            targetRb.AddForce(trajectory * collisionForce * targetRb.mass, ForceMode.Impulse);
            Destroy(gameObject);
        }        
    }
}