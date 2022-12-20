using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 300.0f;
    
    private Rigidbody enemyRb;
    private GameObject playerClone;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerClone = GameObject.Find("Player(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = (playerClone.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * speed * Time.deltaTime);

        if (transform.position.y < -10)
        {
            Destroy(gameObject); //dss
        }
    }
}
