using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public bool isOnSite = false;


    // Nothing special only trigger event to start gathering
    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit != null)
        {
            unit.isArrivedToDestination = true;
            isOnSite = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit != null)
        {
            unit.isArrivedToDestination = false;
            isOnSite = false;
        }
    }
}
