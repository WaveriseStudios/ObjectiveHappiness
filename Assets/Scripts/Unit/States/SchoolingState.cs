// Fichier : Assets/Scripts/Units/States/SchoolingState.cs
using UnityEngine;

public class SchoolingState : IUnitState
{
	private float timeSchooling = 0f;
	private float schoolingDuration = 15f;
	private Job nextJob;

	public SchoolingState(Job jobToLearn)
	{
		this.nextJob = jobToLearn;
	}

	public void OnEnter(Unit unit)
	{
		timeSchooling = 0f;
		// TODO: Déplacement vers l'École
	}

	public void OnExecute(Unit unit)
	{
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