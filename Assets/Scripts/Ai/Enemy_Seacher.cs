using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Seacher : MonoBehaviour
{
    public                                   bool searchUser = false;
    public                                   float raycastDistance_;

    [SerializeField]                         GameObject player;
    [SerializeField]                         Transform enTR;
    // rotationTime = 10f;
    [SerializeField]                         NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        enTR = GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    /*void Update()
    {
        
        RestSearch();

        if (searchUser)
        {
            SearchUser();
        }
        
    }*/

    //public void OnSerchUser()
    //{
    //    searchUser = true;
    //}

    //public void HeardShotSound()
    //{
    //    FollowUser();
    //}

    //public void OffSerchUser()
    //{
    //    searchUser = false ;
    //}

    public void FollowUser()
    {
        agent.isStopped = false;
        agent.speed = 13;
        agent.velocity = agent.desiredVelocity;

        //transform.LookAt(player.transform.position); // 움직임이 기괴함
        agent.SetDestination(player.transform.position);
    }

    public void RotateToUser()
    {
        Vector2 forward = new Vector2(enTR.position.z, enTR.position.x);
        Vector2 steeringTarget = new Vector2(agent.steeringTarget.z, agent.steeringTarget.x);

        Vector2 dir = steeringTarget - forward;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        enTR.eulerAngles = Vector3.up * angle;


        /*Vector3 dir = agent.desiredVelocity;
        Quaternion rotation = Quaternion.LookRotation(dir);
        enTR.rotation = Quaternion.Slerp(enTR.rotation, rotation, Time.deltaTime*rotationSpeed);*/
    }
    // 기괴한 NavMesh 특유의 회전 사용 X

    //public void RestSearch()
    //{
    //    if (find_)
    //    {
    //        time -= Time.deltaTime;

    //        if (time < 0)
    //        {
    //            find_ = false;
    //            UnFollowUser();
    //        }
    //        else
    //        {
    //            RotateToUser();
    //            FollowUser();
    //        }
    //    }
    //} // 범위에서 사라져도 3초간 더 추격

    public void UnFollowUser()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;

    } // 유저를 추격하지 않을 때
    public bool SearchUser()
    {
        RaycastHit hit;

        //int layerMask = (1 << 6) | (1 << 7);

        Vector3 playerPosition = (this.transform.position - player.transform.position).normalized;
        Debug.DrawRay(this.transform.position, -playerPosition, Color.red);

        int layerMask = LayerMask.GetMask("Player", "Wall");

        //Debug.Log(this.transform.position);
        if (Physics.Raycast(this.transform.position, -playerPosition, out hit, 100f, layerMask))
        {
            Debug.Log(layerMask);
            if (hit.collider.gameObject.CompareTag("Player"))
            {

                //time = 3f;
                //RotateToUser();
                //FollowUser();
                //find_ = true;

                raycastDistance_ = hit.distance;

                Debug.Log(hit.collider.gameObject.name);
                
                //FollowUser();

                return true;
            } 
        }
        return false;
    }
}
