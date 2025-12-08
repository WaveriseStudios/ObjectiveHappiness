// Fichier : Assets/Scripts/Buildings/Building.cs
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingType type;

    // --- NOUVEAUX CHAMPS DE CAPACITÉ ---
    public int maxRestOccupancy = 1; // Capacité maximale de la maison
    [HideInInspector] public int currentRestOccupancy = 0; // Occupation actuelle

    // ... (Start, OnDestroy, OnTriggerEnter inchangés) ...

    // Nouvelles méthodes pour gérer la capacité par la maison
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