using UnityEngine;

public class SeekingRestState : IUnitState
{

    // Try to find a house to rest in
    public void OnEnter(Unit unit)
    {
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
            unit.StateMachine.ChangeState(new WanderState());
        }
    }

    public void OnExecute(Unit unit) { }

    public void OnExit(Unit unit) {}
}