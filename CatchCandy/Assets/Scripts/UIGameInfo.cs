using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameInfo : MonoBehaviour
{
    Animator animator;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        text = GetComponent<Text>();
        //text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowQuickStart(string info)
    {
        StartCoroutine(QuickStart(info));
    }

    IEnumerator QuickStart(string info)
    {
        text.enabled = true;
        text.text = info;
        animator.SetBool("Show", true);
        yield return new WaitForSeconds(1.0f);
        text.enabled = false;
        animator.SetBool("Show", false);
    }

    
}
