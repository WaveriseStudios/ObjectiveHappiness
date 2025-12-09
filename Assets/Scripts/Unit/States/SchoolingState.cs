// Fichier : Assets/Scripts/Units/States/SchoolingState.cs
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class SchoolingState : IUnitState
{
	private float timeSchooling = 0f;
	private float schoolingDuration = 15f;
	private Job nextJob;
	private GameObject schoolObject;

	public SchoolingState(Job jobToLearn, GameObject dest)
	{
		this.nextJob = jobToLearn;
		this.schoolObject = dest;
	}

	public void OnEnter(Unit unit)
	{
		timeSchooling = 0f;
        Vector3 targetLocation = schoolObject.transform.position;
        unit.Movement.MoveTo(targetLocation);
    }

	public void OnExecute(Unit unit)
	{
		if (!unit.isArrivedToDestination) return;

		timeSchooling += Time.deltaTime;

		if (timeSchooling >= schoolingDuration)
		{
			unit.currentJob = nextJob;

			if (nextJob != Job.Vagabond)
			{
				unit.StateMachine.ChangeState(new WorkingState());
			}
			else
			{
				unit.StateMachine.ChangeState(new ErranceState());
			}
		}
	}

	public void OnExit(Unit unit)
	{
		// L'individu quitte l'École
	}
}