using UnityEngine;

public class WanderState : IUnitState
{
    private float wanderTimer = 0f;
    private float wanderInterval = 5f;

    public void OnEnter(Unit unit)
    {
        unit.Movement.StopMoving();
        wanderTimer = 0f;
        unit.Movement.MoveToRandomLocation(10f);
    }


    // Random movement in the map
    public void OnExecute(Unit unit)
    {
        if (unit.isTired)
        {
            unit.StateMachine.ChangeState(new SeekingRestState());
            return;
        }

        if (unit.Movement.IsArrived())
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval)
            {
                unit.Movement.MoveToRandomLocation(10f);
                wanderTimer = 0f;
            }
        }
    }

    public void OnExit(Unit unit)
    {
        unit.Movement.StopMoving();
    }
}