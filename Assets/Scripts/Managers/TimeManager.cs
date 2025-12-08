// Fichier : Assets/Scripts/Managers/TimeManager.cs
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public float dayDurationInSeconds = 60f; // Durée d'une journée (Y unités de temps)

    // Événements
    public static event UnityAction OnDayEnd;
    public static event UnityAction<int> OnDayStart;

    private float timeSinceDayStart = 0f;
    private int currentDay = 1;
    private bool isPaused = false;
    private float timeScaleFactor = 1f;

    void Start()
    {
        SetTimeScale(1f);
        OnDayStart?.Invoke(currentDay);
    }

    void Update()
    {
        if (isPaused) return;

        timeSinceDayStart += Time.deltaTime * timeScaleFactor;

        if (timeSinceDayStart >= dayDurationInSeconds)
        {
            EndDay();
        }
    }

    private void EndDay()
    {
        OnDayEnd?.Invoke();
        timeSinceDayStart = 0f;
        currentDay++;
        OnDayStart?.Invoke(currentDay);

        Debug.Log($"Jour {currentDay} commencé.");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : timeScaleFactor; // Vraie pause de Unity
    }

    public void SetTimeScale(float scale)
    {
        if (scale == 1f || scale == 2f || scale == 3f)
        {
            timeScaleFactor = scale;
            if (!isPaused)
            {
                Time.timeScale = scale;
            }
        }
    }
}