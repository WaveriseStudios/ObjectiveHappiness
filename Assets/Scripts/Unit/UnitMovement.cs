using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private float stopDistance = 0.5f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 3.5f;
        agent.stoppingDistance = stopDistance;
    }


    // Go to location with navmesh
    public void MoveTo(Vector3 targetPosition)
    {
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(targetPosition);
        }
    }


    // random movement for wanderer
    public void MoveToRandomLocation(float maxRange)
    {
        Vector3 randomDirection = Random.insideUnitSphere * maxRange;
        randomDirection += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, maxRange, NavMesh.AllAreas))
        {
            MoveTo(hit.position);
        }
    }

    public bool IsArrived()
    {
        if (!agent.pathPending && agent.remainingDistance <= stopDistance)
        {
            return !agent.hasPath || agent.velocity.sqrMagnitude == 0f;
        }
        return false;
    }

    public void StopMoving()
    {
        if (agent.isActiveAndEnabled && agent.hasPath)
        {
            agent.ResetPath();
        }
    }
}