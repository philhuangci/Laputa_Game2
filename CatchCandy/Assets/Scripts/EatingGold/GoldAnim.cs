using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EatingGold
{

    public class GoldAnim : MonoBehaviour
    {
        float time = 0;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            gameObject.transform.rotation = Quaternion.Euler(0, time * 100, 90);

        }
    }
}