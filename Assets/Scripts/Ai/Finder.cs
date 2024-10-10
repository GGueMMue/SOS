using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder : MonoBehaviour
{
    public GameObject parent;
    public Enemy_Seacher es;


    // Start is called before the first frame update
    void Start()
    {
       // es = GetComponentInParent<Enemy_Seacher>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        //eS.SearchUser(other.transform);
        es.OnSerchUser();
        //Debug.Log(other.gameObject.tag);
    }
    private void OnTriggerExit(Collider other)
    {
        es.OffSerchUser();
    }

}
