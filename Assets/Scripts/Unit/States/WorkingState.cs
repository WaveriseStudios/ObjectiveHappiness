using UnityEngine;

public class WorkingState : IUnitState
{
    private float timeWorked = 0f;
    private float fatigueThreshold = 60f;
    private Vector3 targetLocation;
    private ResourceNode targetNode;
    private BuildingSite currentSite;

    public void OnEnter(Unit unit)
    {
        ResourceType requiredType = ResourceType.Food;

        timeWorked = 0f;
        unit.Movement.StopMoving();
        if (unit.currentJob == Job.FoodGatherer) requiredType = ResourceType.Food;
        else if (unit.currentJob == Job.Lumberjack) requiredType = ResourceType.Wood;
        else if (unit.currentJob == Job.Miner) requiredType = ResourceType.Stone;

        // Mason work
        else
        {
            unit.Movement.StopMoving();

            if (unit.currentJob == Job.Mason)
            {
                currentSite = GameObject.FindObjectOfType<BuildingSite>();

                if(unit.isTired)
                {
                    unit.StateMachine.ChangeState(new SeekingRestState());
                }

                else if (currentSite != null)
                {
                    targetLocation = currentSite.transform.position;
                    unit.Movement.MoveTo(targetLocation);
                    targetNode = null;
                    return;
                }
                else
                {
                    unit.StateMachine.ChangeState(new WanderState());
                    return;
                }
            }
        }

        ResourceNode[] allNodes = GameObject.FindObjectsOfType<ResourceNode>();
        ResourceNode closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (ResourceNode node in allNodes)
        {
            if (node.resourceType == requiredType)
            {
                float distance = Vector3.Distance(unit.transform.position, node.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }
        }

        if (closestNode != null)
        {
            targetNode = closestNode;
            targetLocation = targetNode.transform.position;
            unit.Movement.MoveTo(targetLocation);
        }
        else
        {
            unit.StateMachine.ChangeState(new WanderState());
        }
    }

    public void OnExecute(Unit unit)
    {
        if (!unit.isArrivedToDestination) return;

        timeWorked += Time.deltaTime;
        if (timeWorked >= fatigueThreshold)
        {
            unit.isTired = true;
            unit.StateMachine.ChangeState(new SeekingRestState());
            return;
        }

        if (unit.currentJob == Job.Mason)
        {

            if (currentSite != null)
            {
                bool end = currentSite.Contribute(unit);
                if(end) { currentSite = null;}
            }
            else
            {
                unit.StateMachine.ChangeState(new WorkingState());
                return;
            }
        }
        else if (targetNode != null)
        {
            targetNode.TryGather(unit);
        }
    }

    public void OnExit(Unit unit)
    {
        unit.Movement.StopMoving();
    }
}