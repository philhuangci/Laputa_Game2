using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlaneShadowCaster : MonoBehaviour
{
    public Transform reciever;   //阴影接收平面（通常是地面）
    void Update()
    {
        if(reciever)
        {
            GetComponentInChildren<Renderer>().sharedMaterial.SetMatrix("_World2Ground", reciever.GetComponent<Renderer>().worldToLocalMatrix);
            GetComponentInChildren<Renderer>().sharedMaterial.SetMatrix("_Ground2World", reciever.GetComponent<Renderer>().localToWorldMatrix);
        }
    }
}
