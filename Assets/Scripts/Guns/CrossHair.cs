using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public LayerMask mask;
    public SpriteRenderer sr;
    [SerializeField] public Color originalColor;
    public Color hightLightColor;
    public Vector3 maxSize;
    [SerializeField] bool fireChecker = false;
    public float translateSpeed = 0.2f;

    
    

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        Cursor.visible = false;
        originalColor = sr.color;
        hightLightColor = Color.red;
        mask = LayerMask.GetMask("Enemy", "Wall");
    }



    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        this.transform.position = new Vector3(ray.origin.x, -10, ray.origin.z);

        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);

        //if (fireChecker)
        //{
        //    this.transform.localScale = Vector3.Lerp(this.transform.localScale, maxSize, Time.deltaTime * translateSpeed);
        //}
        //else
        //{
        //    this.transform.localScale = Vector3.Lerp(this.transform.localScale, originalSize, Time.deltaTime * translateSpeed);
        //}

    }

    public IEnumerator SetFire(float rpm)
    {
        if(fireChecker)
        {
            this.sr.color = hightLightColor;
            fireChecker = false;

            yield return new WaitForSeconds(rpm);

            fireChecker = true;

            this.sr.color = originalColor;

        }

        yield return null;
    }

    //public void SetNowFireTrue()
    //{
    //    fireChecker = true;
    //    this.sr.color = hightLightColor;

    //}

    //public void SetNowFireFalse()
    //{
    //    fireChecker = false;
    //    this.sr.color = originalColor;
    //}
}
