using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public                              float force = 1000.0f;
    /*[SerializeField]*/                      private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        rb.AddForce(transform.forward * force);

        Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) // 유저, 벽, 적을 만났을 때 총알 소모. 적끼리는 영향이 없음.
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Wall"))
        {
            if(other.gameObject.CompareTag("Player"))
            {
                Player_Controller playerController = other.gameObject.GetComponent<Player_Controller>();
                if (playerController != null)
                {
                    playerController.DeadEffect();
                }
                //other.GetComponent<Player_Controller>().DeadEffect();
            }
            Destroy(this.gameObject);
        }
    }
}
