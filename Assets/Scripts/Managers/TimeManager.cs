// Fichier : Assets/Scripts/Managers/TimeManager.cs
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public float dayDurationInSeconds = 60f;

    public float tickFrequency = 0.1f;

    public static event UnityAction OnDayEnd;
    public static event UnityAction<int> OnDayStart;
    public static event UnityAction OnGameTick;

    private float timeSinceLastTick = 0f;
    private float timeSinceDayStart = 0f;
    private int currentDay = 1;
    private float timeScaleFactor = 1f;

    public TextMeshProUGUI dayCounter;


    void Start()
    {
        
        Time.timeScale = 1f;
        SetTimeScale(1f);

        OnDayStart?.Invoke(currentDay);
    }

    void Update()
    {
        timeSinceLastTick += Time.deltaTime;

        dayCounter.text = "Day : "+currentDay.ToString();

        if (timeSinceLastTick >= tickFrequency)
        {
            int ticksToProcess = Mathf.FloorToInt(timeSinceLastTick / tickFrequency);
            timeSinceLastTick -= ticksToProcess * tickFrequency;

            float gameTimeAdvanced = (ticksToProcess * tickFrequency) * timeScaleFactor;
            timeSinceDayStart += gameTimeAdvanced;

            OnGameTick?.Invoke();

            if (timeSinceDayStart >= dayDurationInSeconds)
            {
                EndDay();
            }
        }
    }

    // End day time, pause , x3 x2 etc

    private void EndDay()
    {
        OnDayEnd?.Invoke();
        timeSinceDayStart = 0f;
        currentDay++;
        OnDayStart?.Invoke(currentDay);
    }

    public void TogglePause(bool value)
    {
        if (value)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = timeScaleFactor;
        }
    }

    public void SetTimeScale(float scale)
    {
        if (scale == 1f || scale == 2f || scale == 3f)
        {
            timeScaleFactor = scale;
            if (Time.timeScale > 0f)
            {
                Time.timeScale = scale;
            }
        }
    }
}