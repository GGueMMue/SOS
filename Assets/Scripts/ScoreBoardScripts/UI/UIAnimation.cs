using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public GameObject fst_Info;
    public GameObject sec_Info;
    public GameObject trd_Info;

    public GameObject text_fst_Info;
    public GameObject text_sec_Info;
    public GameObject text_trd_Info;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimationCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AnimationCoroutine()
    {
        yield return new WaitForSeconds(1f);
        fst_Info.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        text_fst_Info.SetActive(true) ;

        yield return new WaitForSeconds(1f);
        sec_Info.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        text_sec_Info.SetActive(true);

        yield return new WaitForSeconds(1f);
        trd_Info.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        text_trd_Info.SetActive(true);

        yield return null;
    }
}
