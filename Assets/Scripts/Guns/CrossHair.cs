using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public LayerMask mask;
    public SpriteRenderer sr;
    [SerializeField] Color originalColor;
    public Color hightLightColor;
    [SerializeField] Vector3 originalSize;
    public Vector3 maxSize;
    [SerializeField] bool fireChecker = false;
    public float translateSpeed = 0.2f;

    
    

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        Cursor.visible = false;
        originalColor = sr.color;
        originalSize = this.transform.localScale;
        hightLightColor = Color.red;

        maxSize = originalSize + new Vector3(0.2f, 0.2f, 0.2f);
        mask = LayerMask.GetMask("Enemy", "Wall");
    }



    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);

        if (fireChecker)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, maxSize, Time.deltaTime * translateSpeed);
        }
        else
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, originalSize, Time.deltaTime * translateSpeed);
        }

    }

    public void SetNowFireTrue()
    {
        fireChecker = true;
        //sr.color = hightLightColor;

    }

    public void SetNowFireFalse()
    {
        fireChecker = false;
        //sr.color = originalColor;

    }
}
