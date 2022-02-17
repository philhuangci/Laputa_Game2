using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruit
{
    public class Fruit : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Terrain")
            {
                StartCoroutine(DisappearFruit(0.5f));
            }
        }

        IEnumerator DisappearFruit(float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(this.gameObject);
        }
    }
}


