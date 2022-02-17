using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fruit
{
    public class FruitFactory : MonoBehaviour
    {
        bool Gen = false;
        public GameObject[] FruitPrefabs;

        public GameObject Boom;
        public Transform Ground;

        public void StartGenFruit()
        {
            Gen = true;
            StartCoroutine(GenFruit());
        }

        public void StopGenFruit()
        {
            Gen = false;
        }

        IEnumerator GenFruit()
        {
            Random.InitState(Time.frameCount);
            while (Gen)
            {
                int i = Random.Range(0, FruitPrefabs.Length);
                GameObject go = Instantiate(FruitPrefabs[i], new Vector3(Random.Range(-3.0f, 4.5f), 15, Random.Range(-3.5f, 3.5f)),Quaternion.identity);

                //go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Rigidbody r = go.GetComponent<Rigidbody>();
                r.angularVelocity = new Vector3(Random.Range(0,10), 0, Random.Range(0, 10));

                var m = go.GetComponentInChildren<MeshFilter>();
                m.mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

                var p = go.GetComponent<PlaneShadowCaster>();
                p.reciever = Ground;

                yield return new WaitForSeconds(0.5f);
            }
        }


    }
}
