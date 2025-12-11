using UnityEngine;

public class RestingState : IUnitState
{
    private float timeResting = 0f;
    private float restDuration = 15f;
    private GameObject house;

    public RestingState(GameObject dest) { house = dest; }

    public void OnEnter(Unit unit)
    {
        timeResting = 0f;
        unit.currentHouse = house;
        Vector3 targetLocation = house.transform.position;
        unit.Movement.MoveTo(targetLocation);
    }


    // Rest for 15s
    public void OnExecute(Unit unit)
    {
        if (!unit.isArrivedToDestination) return;

        timeResting += Time.deltaTime;

        if (timeResting >= restDuration)
        {
            unit.Rest();
        }
    }

    public void OnExit(Unit unit)
    {
    }
}