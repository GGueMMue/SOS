using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public                              float force = 10000.0f;
    /*[SerializeField]*/                      private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        rb.AddForce(transform.forward * force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Wall"))
        {
            if(other.gameObject.CompareTag("Player"))
            {
                Debug.Log("플레이어 사망 상태");
            }
            Destroy(this.gameObject);
        }
    }
}
