using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// from https://www.youtube.com/watch?v=eMpI1eCsIyM&t=1s
namespace holistic3D { 

    public class globalFlock : MonoBehaviour
    {
        public GameObject agentPrefab;
        public GameObject goalPrefab;
        private GameObject goalInstance;

        static int numAgents = 10;
        public static int volumeRadius = 5;

        public static GameObject[] allAgents = new GameObject[numAgents];

        public static Vector3 goalPos = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            goalInstance = Instantiate(goalPrefab, goalPos, Quaternion.identity, this.transform);

            for (int i = 0; i < numAgents; i++) {
                Vector3 pos = new Vector3(
                    Random.Range(-volumeRadius, volumeRadius),
                    Random.Range(-volumeRadius, volumeRadius),
                    Random.Range(-volumeRadius, volumeRadius)
                );
                allAgents[i] = (GameObject) Instantiate(agentPrefab, pos, Quaternion.identity, this.transform);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if ( Random.Range(0, 10000) < 50 ) {
                goalPos = new Vector3(
                    Random.Range(-volumeRadius, volumeRadius),
                    Random.Range(-volumeRadius, volumeRadius),
                    Random.Range(-volumeRadius, volumeRadius)
                );
                goalInstance.transform.position = goalPos;

                Debug.Log($"updating goal: {goalPos}");
            }
        }
    }
}