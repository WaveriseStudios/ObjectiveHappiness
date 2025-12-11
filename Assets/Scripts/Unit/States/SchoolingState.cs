using UnityEngine;
using System.Linq;

public class SchoolingState : IUnitState
{
	private float timeSchooling = 0f;
	private float schoolingDuration = 15f;
	private Job nextJob;
	private GameObject schoolObject;
	private GameManager gm;

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
		gm = GameObject.FindObjectOfType<GameManager>();
    }

	// Execute job switch in 15 sec
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
				unit.StateMachine.ChangeState(new WanderState());
			}
		}
	}

	public void OnExit(Unit unit)
	{
		GameObject.Destroy(unit.currentSkin);
		unit.SetNewSkin();
    }
}