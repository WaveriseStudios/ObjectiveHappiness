using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType;
    public int baseGatherAmount = 1;
    public float gatherInterval = 2f;

    private float timeSinceLastGather = 0f;


    // Functions to make a unit with a resources gathering job work
    public bool TryGather(Unit gatheringUnit)
    {
        timeSinceLastGather += Time.deltaTime;

        if (timeSinceLastGather >= gatherInterval)
        {
            timeSinceLastGather = 0f;

            int amountGathered = baseGatherAmount;

            GameManager gm = GameObject.FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.AddResource(resourceType, amountGathered);
            }
            return true;
        }
        return false;
    }
}