using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public GameObject target;

    private float speed = 15.0f;
    private float collisionForce = 6.5f;
    private Vector3 trajectory;

    private Enemy enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        enemyScript = target.GetComponent<Enemy>();
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
            Destroy(gameObject);

            Rigidbody targetRb = collision.gameObject.GetComponent<Rigidbody>();
            if (!enemyScript.isBoss)
            {
                targetRb.AddForce(trajectory * collisionForce * targetRb.mass, ForceMode.Impulse);
            }
            else
            {
                enemyScript.hitPoints--;
                if (enemyScript.hitPoints <= 0)
                {
                    Destroy(collision.gameObject);
                }
            }
        }        
    }
}