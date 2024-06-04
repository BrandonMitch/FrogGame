using System.Collections.Generic;
using UnityEngine;
public class CoolDownBasic 
{
    public struct TimeInterval
    {
        public readonly float start;
        public readonly float end;
        public TimeInterval(float start, float end)
        {
            this.start = start;
            this.end = end;
        }
        public float Duration { get => end - start; }
    }

    private List<TimeInterval> intervals;
    private float duration;
    private float startTime;
    private bool paused; 
    public float Duration { get => duration;}
    public float StartTime { get => startTime;}

    /// <summary>
    /// Constructor for a new cooldown.
    /// </summary>
    /// <param name="duration">How long the cooldown is to last</param>
    /// <param name="startOnCreation">Optional parameter that will start the timer on creation </param>
    /// <param name="startTime"> Optional parameter to set the start time</param>
    public CoolDownBasic(float duration, bool startOnCreation = false)
    {
        this.duration = duration;
        if (startOnCreation)
        {
            startTime = Time.time;
        }
    }
    public bool IsOnCooldown()
    {
        float elapsedTime = getElapsedTime();
        if(elapsedTime > duration)
        {
            return false;
        } else
        {
            return true;
        } 
    }
    /// <summary>
    /// Get the progress of the cooldown on a percent basis.
    /// </summary>
    /// <param name="clamped"> If clamped is on, then the output will range from 0-1, otherwise it allow the percentage to go above 100%</param>
    /// <returns>1 if the cooldown is 100% complete, 0 if it just started.</returns>
    public float getCooldownProgress(bool clamped = true)
    {
        float elapsedTime = getElapsedTime() / duration;
        if (!clamped)
        {
            return elapsedTime;
        }
        else
        {
            if(elapsedTime > 1)
            {
                return 1;
            }
            else
            {
                return elapsedTime;
            }
        }
    }
    public float getElapsedTime()
    {
        float t = 0;
        if (!paused)
        {
            t += startTime - Time.time;
        }
        foreach (TimeInterval interval in intervals)
        {
            t += interval.Duration;
        }
        return t; 
    }
    public void StartCooldown()
    {
        StartCooldown(Time.time);
    }
    public void StartCooldown(float time)
    {
        startTime = time;
        intervals = new();
        paused = false;
    }
    public void Pause()
    {
        if (!paused)
        {
            intervals.Add(new TimeInterval(startTime, Time.time));
            paused = true;
        }
    }
    public void Unpause()
    {
        if (paused)
        {
            startTime = Time.time;
            paused = false;
        }
    }
}
