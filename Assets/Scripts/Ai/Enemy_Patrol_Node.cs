using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Enemy_Patrol_Node : MonoBehaviour
{
    public Enemy_Patrol_Node next_Node;

    public Vector3 Get_Now_Node()
    {
        return this.gameObject.transform.position;
    }

    public Vector3 Get_Next_Node()
    {
        return next_Node.gameObject.transform.position;
    }
}
