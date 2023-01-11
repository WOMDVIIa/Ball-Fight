using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isBoss;
    public int hitPoints;
    public float speed = 300.0f;
    public int bossHP = 3;
    
    private Rigidbody enemyRb;
    private GameObject playerClone;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerClone = GameObject.Find("Player(Clone)");
        if (isBoss)
        {
            hitPoints = bossHP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = (playerClone.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * speed * Time.deltaTime);

        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
