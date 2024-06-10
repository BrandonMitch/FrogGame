using System.Collections.Generic;
using UnityEngine;
public class CoolDownBasic 
{
    private struct TimeInterval
    {
        public readonly float start;
        public readonly float end;
        public TimeInterval(float start, float end)
        {
            this.start = start;
            this.end = end;
        }
        public float TimeIntervalDuration { get => end - start; }
    }

    private List<TimeInterval> intervals;
    private float duration;
    private float startTime;
    private bool paused = false; 


    /// <summary>
    /// Constructor for a new cooldown. Provide how long the cool down is
    /// </summary>
    /// <param name="duration">How long the cooldown is to last</param>
    /// <param name="startOnCreation">Optional parameter that will start the timer on creation </param>
    /// <param name="startTime"> Optional parameter to set the start time</param>
    public CoolDownBasic(float duration, bool startOnCreation = false)
    {
        this.duration = duration;
        if (startOnCreation)
        {
            StartCooldown();
        }
    }
    public bool IsCooldownComplete
    {
        get
        {
            if (ElapsedTime >= duration)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    /// <summary>
    /// Get the cool down progress on a percent basis, 0.0 being it just started, 1.0 if it is finished
    /// </summary>
    public float CoolDownProgressClamped => IsCooldownComplete ? 1 : CoolDownProgressUnclamped;
    /// <summary>
    /// Get the cool down progress on a perecent basis, 0.0 being it just started, 1.0 if it is just finished, will return greater than one if more time has passed.
    /// </summary>
    public float CoolDownProgressUnclamped
    {
        get => ElapsedTime / duration;
    }
    public float ElapsedTime
    {
        get
        {
            float t = 0;
            if (!paused)
            {
                t += Time.time - startTime;
            }
            foreach (TimeInterval interval in intervals)
            {
                t += interval.TimeIntervalDuration;
            }
            return t;
        }
    }
    public float Duration { get => duration; set => duration = value; }

    public void StartCooldown()
    {
        StartCooldown(Time.time);
    }
    public void StartCooldown(float time)
    {
        startTime = time;

        if(intervals != null)
        {
            intervals.Clear();
        }
        else
        {
            intervals = new();
        }
        paused = false;
    }
    public void Pause()
    {
        if (!paused)
        {
            intervals.Add(new TimeInterval(startTime, Time.time));
            paused = true;
        }
        else
        {
            Debug.LogWarning("Cool down is already paused and it is being paused again, make sure to Unpause()");
        }
    }
    public void Unpause()
    {
        if (paused)
        {
            startTime = Time.time;
            paused = false;
        }
        else
        {
            Debug.LogWarning("Cool down is already unpaused");
        }
    }

    public void SkipCooldown()
    {
        startTime -= 2 * duration;
    }
}
