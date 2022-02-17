using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoundInfo : MonoBehaviour
{
    Animator animator;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowRound(string info)
    {
        StartCoroutine(QuickShow(info));
    }

    IEnumerator QuickShow(string info)
    {
        text.enabled = true;
        text.text = info;
        animator.SetBool("Show", true);
        yield return new WaitForSeconds(1.0f);
        text.enabled = false;
        animator.SetBool("Show", false);
    }
}
