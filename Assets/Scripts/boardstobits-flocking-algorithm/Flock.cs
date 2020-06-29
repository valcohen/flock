using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;

    [Range(10, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    private float stayInsideRadius = 0f;
    private Vector3 stayInsideCenter = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed          = maxSpeed * maxSpeed;
        squareNeighborRadius    = neighborRadius * neighborRadius;
        squareAvoidanceRadius   = squareNeighborRadius
                                * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitSphere * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.one * Random.Range(0f, 360f)),
                transform
            );
            newAgent.name = "Agent " + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }

        if (behavior is CompositeBehavior ) {
            var cbs = ((CompositeBehavior)behavior).behaviors;
            foreach (var b in cbs) {
                if ( b is StayInRadiusBehavior) {
                    this.stayInsideRadius = ((StayInRadiusBehavior)b).radius;
                    this.stayInsideCenter = ((StayInRadiusBehavior)b).center;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            //FOR DEMO ONLY
            // agent.GetComponentInChildren<MeshRenderer>().material.color =
            //    Color.Lerp(Color.white, Color.red, context.Count / 6f);

            Vector3 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }

    void OnDrawGizmos() {
        // visualize agents
        Gizmos.color = Color.gray;
        foreach( var agent in agents) {
            Gizmos.DrawRay(
                agent.transform.position,
                agent.transform.forward * avoidanceRadiusMultiplier
            );
            var bb = agent.transform.GetComponent<Renderer>().bounds;
            Gizmos.DrawWireSphere(bb.center, bb.extents.magnitude);
            
        }


        // visualize StayInRadius 
        if (stayInsideRadius > 0) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(stayInsideCenter, stayInsideRadius);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
        foreach (Collider c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

}
