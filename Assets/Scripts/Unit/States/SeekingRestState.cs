// Fichier : Assets/Scripts/Units/States/SeekingRestState.cs
using UnityEngine;

public class SeekingRestState : IUnitState
{
    public void OnEnter(Unit unit)
    {
        // 1. Tenter d'acquérir une place dans une Maison
        BuildingManager bm = GameObject.FindObjectOfType<BuildingManager>();

        if (bm != null && bm.TryAcquireRestSlot())
        {
            Building house = bm.FindAvailableHouseAndAcquireSlot(unit.transform.position);
            if (house != null)
            {
                unit.StateMachine.ChangeState(new RestingState(house.gameObject));
            }
        }
        else
        {
            // 2. Si aucune maison/place n'est disponible, l'unité erre
            unit.StateMachine.ChangeState(new ErranceState());
        }
    }

    public void OnExecute(Unit unit)
    {
    }

    public void OnExit(Unit unit)
    {
        // Transition immédiate
    }
}