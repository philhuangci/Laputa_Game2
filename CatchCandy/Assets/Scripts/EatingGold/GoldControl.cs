using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EatingGold
{

    public class GoldControl : MonoBehaviour
    {
        public int[][] RoundGold ={new int[] { 1, 0, 2, 0, 3, 0, 2, 0, 1 },
        new int[] { 1, 0, 2, 0, 3, 0, 2, 0, 1 },
        new int[] { 3, 0, 2, 0, 5, 0, 2, 0, 3 },
        new int[] { 2, 0, 5, 0, 3, 0, 5, 0, 2 },
        new int[]{ 5, 0, 3, 0, 10, 0, 3, 0, 5} };

        public SceneControl SceneControl;

        public GameObject gold1;
        public GameObject gold2;
        public GameObject gold3;
        public GameObject gold5;
        public GameObject gold10;

        Dictionary<int, GameObject> GoldObjectsOnCloud = new Dictionary<int, GameObject>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GenerateGold(int round)
        {
            int[] golds = RoundGold[round];

            for (int i = 0; i < golds.Length; i++)
            {
                if (golds[i] > 0)
                {
                    GoldObjectsOnCloud.Add(i, GenerateGold(i, golds[i]));
                }
            }
        }

        public GameObject GenerateGold(int cloudNum, int goldNum)
        {
            GameObject gold = null;
            Vector3 pos = new Vector3();
            Vector3 delta = new Vector3(0f, 0.2f, 0f);
            pos = SceneControl.Clouds[cloudNum].transform.position + delta;

            switch (goldNum)
            {
                case 1:
                    gold = GameObject.Instantiate(gold1, pos, Quaternion.identity);
                    break;
                case 2:
                    gold = GameObject.Instantiate(gold2, pos, Quaternion.identity);
                    break;
                case 3:
                    gold = GameObject.Instantiate(gold3, pos, Quaternion.identity);
                    break;
                case 5:
                    gold = GameObject.Instantiate(gold5, pos, Quaternion.identity);
                    break;
                case 10:
                    gold = GameObject.Instantiate(gold10, pos, Quaternion.identity);
                    break;
            }

            gold.transform.position = pos;
            gold.SetActive(true);
            return gold;
        }

        public int GetGoldNum(int round, int cloud)
        {
            return RoundGold[round][cloud];
        }

        public void DismissGold(int cloudNum)
        {
            GoldObjectsOnCloud[cloudNum].SetActive(false);
        }

        public void DestroyAllGolds()
        {
            foreach (var goldObj in GoldObjectsOnCloud.Values)
            {
                GameObject.Destroy(goldObj);
            }
            GoldObjectsOnCloud.Clear();
        }
    }
}
