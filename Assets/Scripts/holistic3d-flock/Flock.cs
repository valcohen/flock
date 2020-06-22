using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace holistic3D { 

    public class Flock : MonoBehaviour
    {
        public float speed = 0.1f;

        float   rotationSpeed = 4.0f;
        Vector3 avgHeading;
        Vector3 avgPosition;
        float   maxNeighborDistance = 3.0f;
        float   minNeighborDistance = 1.0f;

        bool isTurning = false;

        // Start is called before the first frame update
        void Start()
        {
            speed = Random.Range(speed * 0.75f, speed * 1.25f);
        }

        // Update is called once per frame
        void Update()
        {
            if ( Vector3.Distance(transform.position, Vector3.zero /* volumne center */)
                >= globalFlock.volumeRadius
            ) {
                isTurning = true;
            } else {
                isTurning = false;
            }

            if (isTurning) {
                Vector3 direction = Vector3.zero - transform.position;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    rotationSpeed * Time.deltaTime
                );
                speed = Random.Range(0.5f, 1f);
            } else { 
                if (Random.Range(0,5) < 1) ApplyRules();

            }
            transform.Translate(0, 0, Time.deltaTime * speed);
        }

        void ApplyRules() {
            // TODO: manage global dependency
            GameObject[] gos = globalFlock.allAgents;

            Vector3 vCenter = Vector3.zero;
            Vector3 vAvoid = Vector3.zero;
            float gSpeed = 0.1f;

            Vector3 goalPos = globalFlock.goalPos;

            float dist;

            int groupSize = 0;
            foreach ( GameObject go in gos ) {
                if (go != this.gameObject) {
                    dist = Vector3.Distance(go.transform.position, this.transform.position);
                    if (dist <= maxNeighborDistance) {
                        vCenter += go.transform.position;
                        groupSize++;

                        if (dist < minNeighborDistance) {
                            vAvoid += (this.transform.position - go.transform.position);
                        }

                        Flock anotherFlock = go.GetComponent<Flock>();
                        gSpeed += anotherFlock.speed;
                    }
                }

                if ( groupSize > 0 ) {
                    Vector3 avgCenter   = vCenter / groupSize + (goalPos - this.transform.position);
                    float   avgSpeed    = gSpeed / groupSize;

                    Vector3 direction   = (avgCenter +  vAvoid) - transform.position;

                    if (direction != Vector3.zero) {
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            Quaternion.LookRotation(direction),
                            rotationSpeed * Time.deltaTime
                        );
                    }
                }
            }
        }
    }
}