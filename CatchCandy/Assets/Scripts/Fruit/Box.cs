using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fruit
{
    public class Box : MonoBehaviour
    {
        PlayerFSM Player;
        // Start is called before the first frame update
        void Start()
        {
            Player = GetComponentInParent<PlayerFSM>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Fruit")
            {
                Player.ShowStars(other.gameObject.transform.position);
                Player.GetFruit();
                Destroy(other.gameObject);
            }
            else if(other.gameObject.tag == "Boom")
            {
                Player.FallDown();
                Player.GetBoom();
            }
        }
    }
}
