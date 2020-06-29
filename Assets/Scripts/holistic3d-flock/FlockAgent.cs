using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace holistic3D { 

    public class FlockAgent : MonoBehaviour
    {
        private float speed          = 0.1f;

        public float rotationSpeed  = 4.0f;
        public float minSpeed       = 0.8f;
        public float maxSpeed       = 2.0f;

        Vector3 avgHeading;
        Vector3 avgPosition;

        public float   maxNeighborDistance = 3.0f;
        public float   minNeighborDistance = 1.0f;

        public Material turnMaterial;
        private Renderer myRenderer;
        private Material originalMaterial;
        public Vector3  newGoalPos;
        public Flock myFlock   {get; set;}

        bool isTurning = false;

        AnimationState anim;

        // Start is called before the first frame update
        void Start() {
            speed = Random.Range(minSpeed, maxSpeed);

            myRenderer = this.GetComponent<Renderer>();
            originalMaterial = myRenderer.material;

            // anim = this.GetComponent<Animation>()["Motion"];
            setAnimSpeed();
        }

        private void setAnimSpeed() {
            // anim.speed = speed;
        }

        private void OnTriggerEnter(Collider other) {
            Debug.Log($"entering collider on {other.name}");
            if (!isTurning) {
                newGoalPos = this.transform.position - other.gameObject.transform.position;
                Debug.Log($"Turning away from '{other.name}'"); //, bearing {newGoalPos}");
            }
            myRenderer.material = turnMaterial;
            isTurning = true;
        }

        private void OnTriggerExit(Collider other) {
            myRenderer.material = originalMaterial;
            isTurning = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (isTurning) {
                Vector3 direction = newGoalPos - transform.position;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    rotationSpeed * Time.deltaTime
                );
                speed = Random.Range(minSpeed, maxSpeed);
                setAnimSpeed();
            } else { 
                if (Random.Range(0,10) < 1) ApplyRules();
            }
            transform.Translate(0, 0, Time.deltaTime * speed);
        }

        void ApplyRules() {
            // TODO: manage global dependency
            GameObject[] gos = myFlock.allAgents;

            Vector3 vCenter = Vector3.zero;
            Vector3 vAvoid = Vector3.zero;
            float gSpeed = 0.1f;

            Vector3 goalPos = myFlock.goalPos;

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

                        FlockAgent anotherFlock = go.GetComponent<FlockAgent>();
                        gSpeed += anotherFlock.speed;
                    }
                }

                if ( groupSize > 0 ) {
                    Vector3 avgCenter   = vCenter / groupSize + (goalPos - this.transform.position);
                    speed   = gSpeed / groupSize;
                    setAnimSpeed();

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