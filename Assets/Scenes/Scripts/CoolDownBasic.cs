using UnityEngine;
public class CoolDownBasic 
{
    private float duration;
    private float startTime;
    public float Duration { get => duration;}
    public float StartTime { get => startTime;}

    /// <summary>
    /// Constructor for a new cooldown.
    /// </summary>
    /// <param name="duration">How long the cooldown is to last</param>
    /// <param name="startOnCreation">Optional parameter that will start the timer on creation </param>
    public CoolDownBasic(float duration, bool startOnCreation = false)
    {
        this.duration = duration;
        if (startOnCreation)
        {
            startTime = Time.time;
        }
    }
    public bool isOnCooldown()
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
        return duration - startTime; 
    }
    public void StartCooldown()
    {
        StartCooldown(Time.time);
    }
    public void StartCooldown(float time)
    {
        startTime = time;
    }
}
