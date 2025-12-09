// Fichier : Assets/Scripts/Buildings/Building.cs
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingType type;

    public int maxRestOccupancy = 1;
    [HideInInspector] public int currentRestOccupancy = 0;


    public bool TryAcquireSlot()
    {
        if (currentRestOccupancy < maxRestOccupancy)
        {
            currentRestOccupancy++;
            return true;
        }
        return false;
    }

    public void ReleaseSlot()
    {
        currentRestOccupancy = Mathf.Max(0, currentRestOccupancy - 1);
    }
}