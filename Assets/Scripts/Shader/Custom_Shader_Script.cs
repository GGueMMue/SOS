using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Custom_Shader_Script : MonoBehaviour
{
    // Start is called before the first frame update

    public Shader shader;
    void Start()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach(Material material in renderer.materials)
            {
                if (renderer.gameObject.CompareTag("Crosshair")) continue;

                material.shader = shader;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
