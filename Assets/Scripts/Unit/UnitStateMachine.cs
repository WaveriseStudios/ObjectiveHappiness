using UnityEngine;

public class UnitStateMachine : MonoBehaviour
{
    private Unit unit;
    public IUnitState currentState;

    void Start()
    {
        unit = GetComponent<Unit>();

        if (unit.currentJob == Job.Vagabond)
        {
            ChangeState(new WanderState());
        }
        else
        {
            ChangeState(new WorkingState());
        }
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnExecute(unit);
        }
    }


    // Change state 
    public void ChangeState(IUnitState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(unit);
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter(unit);
        }
    }
}