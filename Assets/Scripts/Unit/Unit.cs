// Fichier : Assets/Scripts/Units/Unit.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

[RequireComponent(typeof(UnitStateMachine), typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    // Refs
    public UnitStateMachine StateMachine;
    public UnitMovement Movement;
    private BuildingManager buildingManager;

    // Object refs
    public GameObject currentHouse;
    public GameObject focusPoint;
    public GameObject meshParent;
    public GameObject currentSkin;
    public bool isArrivedToDestination = false;

    // Unit stats
    public Job currentJob = Job.Vagabond;
    public List<SkinToJob> jobs;
    public int age = 0;
    public bool isTired = false;
    public bool isUnhappy = false;

    public GameObject tiredIcon;

    public int maxAge = 80;

    void Awake()
    {
        StateMachine = GetComponent<UnitStateMachine>();
        Movement = GetComponent<UnitMovement>();
        buildingManager = FindObjectOfType<BuildingManager>();
    }

    private void Update()
    {
        tiredIcon.SetActive(isTired);
    }


    private void Start()
    {
        SetNewSkin();
    }

    public void SetNewSkin()
    {
        currentSkin = GameObject.Instantiate(jobs.FirstOrDefault(i => i.job == currentJob).associatedModel, meshParent.transform);
    }

    void OnEnable()
    {
        TimeManager.OnDayEnd += OnDayEndHandler;
        BuildingManager.OnNewBuildingSiteCreated += OnNewBuildingSiteCreatedHandler;
    }

    void OnDisable()
    {
        TimeManager.OnDayEnd -= OnDayEndHandler;
        BuildingManager.OnNewBuildingSiteCreated -= OnNewBuildingSiteCreatedHandler;
    }

    private void OnNewBuildingSiteCreatedHandler(BuildingSite site)
    {
        if (currentJob == Job.Mason)
        {
            if (StateMachine.currentState.GetType() != typeof(WorkingState) && !isTired)
            {
                StateMachine.ChangeState(new WorkingState());
            }
            else
            {
                StateMachine.ChangeState(new SeekingRestState());
            }
        }
    }


    // On day end event
    private void OnDayEndHandler()
    {
        age++;

        if (age > maxAge)
        {
            Die("Old");
            return;
        }

        if (currentJob != Job.Vagabond && !isTired)
        {
            isTired = true;
            isUnhappy = true;
            StateMachine.ChangeState(new SeekingRestState());
        }
    }


    // Explicit i guess
    public void Die(string cause)
    {
        Debug.Log($"{gameObject.name} died of {cause}.");
        CameraController cam = FindObjectOfType<Camera>().GetComponent<CameraController>();
        if (cam.targetParent = this.transform)
        {
            cam.ExitFocus();
        }
        Destroy(gameObject);
    }

    // Called in UI for jobs
    public void SetJob(Job newJob)
    {
        if (!buildingManager.IsSchoolBuilt())
        {
            return;
        }

        if (currentJob == newJob) return;

        StateMachine.ChangeState(new SchoolingState(newJob, buildingManager.GetSchool().gameObject));
    }


    // Release the current house
    public void Rest()
    {
        isTired = false;
        isUnhappy = false;
        buildingManager.ReleaseRestSlot(currentHouse.GetComponent<Building>());

        if (currentJob != Job.Vagabond)
        {
            StateMachine.ChangeState(new WorkingState());
        }
        else
        {
            StateMachine.ChangeState(new WanderState());
        }
    }
}