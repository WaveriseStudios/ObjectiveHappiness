// Fichier : Assets/Scripts/Units/States/WorkingState.cs
using UnityEngine;

public class WorkingState : IUnitState
{
    private float timeWorked = 0f;
    private float fatigueThreshold = 50f;
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
        else
        {
            timeWorked = 0f;
            unit.Movement.StopMoving();

            if (unit.currentJob == Job.Mason)
            {
                // NOUVEAU : Cible un BuildingSite
                currentSite = GameObject.FindObjectOfType<BuildingSite>();

                if (currentSite != null)
                {
                    targetLocation = currentSite.transform.position;
                    unit.Movement.MoveTo(targetLocation);
                    Debug.Log($"{unit.gameObject.name} (Maçon) se dirige vers le chantier.");
                    targetNode = null; // Important pour éviter les conflits avec la récolte
                    return;
                }
                else
                {
                    Debug.Log($"{unit.gameObject.name}: Pas de chantier actif. Errance.");
                    unit.StateMachine.ChangeState(new ErranceState());
                    return;
                }
            }
        }

        // 2. Trouver le ResourceNode le plus proche du type requis
        ResourceNode[] allNodes = GameObject.FindObjectsOfType<ResourceNode>();
        ResourceNode closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (ResourceNode node in allNodes)
        {
            // FILTRAGE CRUCIAL : Ne sélectionner que les nœuds du bon type
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
            Debug.Log($"{unit.gameObject.name} (Job: {unit.currentJob}) se dirige vers {targetNode.name} pour {requiredType}.");
        }
        else
        {
            Debug.LogError($"Pas de ResourceNode de type {requiredType} trouvé pour {unit.gameObject.name}. Passage en errance.");
            unit.StateMachine.ChangeState(new ErranceState());
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
                // Le maçon contribue à la construction
                bool end = currentSite.Contribute(unit);
                if(end) { currentSite = null; }
            }
            else
            {
                unit.StateMachine.ChangeState(new WorkingState()); // Force le retour à OnEnter pour relancer la recherche
                return;
            }
        }
        else if (targetNode != null)
        {
            // Récolte (logique des autres métiers)
            targetNode.TryGather(unit);
        }
    }

    public void OnExit(Unit unit)
    {
        unit.Movement.StopMoving();
    }
}