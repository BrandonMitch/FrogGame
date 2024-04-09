using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class PlayerStatManager : MonoBehaviour
{
    public PlayerBaseStatsSO playerBaseStats;
    public PlayerBaseStatsSO playerCurrentStats;
    public PlayerBaseStatsSO playerAddStats;
    public PlayerBaseStatsSO playerMultiplyStats;
    private FieldInfo[] allFields;
    [SerializeField] private List<GenericStat> stats = new();

/*    [Header("|-----Movement Variables-----|")]
    [SerializeField] private float playerSpeed                          *//*= 1.0f;     *//*;
    [SerializeField] private float playerMaxSpeed                       *//*= 0.8f;     *//*;
    [SerializeField] private float playerRestingDrag                    *//*= 100f;     *//*;
    [SerializeField] private float playerRunningDrag                    *//*= 5f;       *//*;
    [SerializeField] private float playerDragSlowDownTime               *//*= 0.2f;     *//*;
    [SerializeField] private float playerRunForceModifier               *//*= 40f;      *//*;
    [Space]                                                             *//*            *//*
    [Header("|-----Lunge Variables-----|")]                             *//*            *//*
    [SerializeField] private float lateralForceModifer                  *//*= 150f;     *//*;
    [SerializeField] private float minimumLateralDuration               *//*= 0.3f;     *//*;
    [SerializeField] private float lateralDragCoefficient               *//*= 0f;       *//*;
    [SerializeField] private float dampingCoefficient                   *//*= 0.99f;    *//*;
    [SerializeField] private float minimumDistanceToSpawnANewPoint      *//*= 0.1f;     *//*;
    [SerializeField] private float minimumTimeToSpawnANewPoint          *//*= 1f;       *//*;
    [SerializeField] private float forwardLungeCoefficient              *//*= 0.1f;     *//*;
    [SerializeField] private float forwardLungeForceModifer             *//*= 150f;     *//*;
    [Space]                                                             *//*            *//*
    [SerializeField] private float lateralLungeEaseInFrames             *//*= 5;        *//*;
    [SerializeField] private float lateralLungeEaseOutFrames            *//*= 12;       *//*;
    [SerializeField] private float lateralLungeDesiredVEL               *//*= 3;        *//*;*/

    /// <summary>
    /// Inside the method, baseValue is obtained by calling the selector function with playerBaseStats, retrieving the current value of the stat.
    /// <br>newValue is calculated by adding amount to baseValue.</br>
    /// <br>Finally, the new value newValue is assigned to the variable, effectively modifying the base stat.</br>
    ///<br>This method allows you to modify a float variable directly by passing it by reference, and it retrieves the current value of the stat using the provided selector function. </br>
    ///<br>It then calculates the new value by adding the specified amount and assigns it to the variable. This approach provides flexibility and allows for easy modification of base stats.</br>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="variable"> variable is passed by reference, allowing the method to modify its value directly. </param>
    /// <param name="selector">selector is a function that takes a PlayerBaseStatsSO object and returns a float. This function is used to get the current value of the stat from the playerBaseStats object.</param>
    /// <param name="amount">amount is the value to add to the base stat.</param>
    /*
    public void AddToCurrentStat(ref float variable, Func<PlayerBaseStatsSO, float> selector, float amount) 
    {
        // Get the value using the selector function
        float baseValue = selector(playerBaseStats);

        // Add the amount to the value
        float newValue = baseValue + amount;

        // Assign the new value to the variable
        variable = newValue;
    }
    */
   #region depricated
    /*
    public void AddToCurrentStat(ref float variable, Func<PlayerBaseStatsSO, float> selector, float amount)
    {
        // Get the current value using the selector function
        float currrentValue = selector(playerCurrentStats);
        // find what the new value would be
        float newValue = currrentValue + amount;

        // set the reference to this value
        variable = newValue;
        // now update the value stored in the SO to the new value we calculated
        SetValueInPlayerStatsSO(selector, newValue);
    }
    
    // Helper method to update the value stored in the PlayerBaseStatsSO object using the provided selector function
    private void SetValueInPlayerStatsSO(Func<PlayerBaseStatsSO, float> selector, float newValue)
    {
        // Get the field info for the field that needs to be updated
        FieldInfo field = typeof(PlayerBaseStatsSO).GetField(selector.Method.Name, BindingFlags.Public | BindingFlags.Instance);
        // Update the value of the field in the PlayerBaseStatsSO object
        if (field != null)
        {
            field.SetValue(playerCurrentStats, newValue);
        }
    }
    // this one sucks
    private void AddValueInPlayerStatsSO(Func<PlayerBaseStatsSO, float> selector, float newValue)
    {
        var constantExpression = (ConstantExpression)((MemberExpression)selector.Target).Expression;
        var playerstats = (PlayerBaseStatsSO)constantExpression.Value;

        FieldInfo field = typeof(PlayerBaseStatsSO).GetField(selector.Method.Name, BindingFlags.Public | BindingFlags.Instance);

        // Get the current value
        float currentValue = (float)field.GetValue(playerstats);

        // Add the newValue to the current value and update the field
        field.SetValue(playerstats, currentValue + newValue);
    }
    private void AddValueInPlayerStatsSO(ref float field, float newValue)
    {
        field += newValue;
    }
    public void AddToAddModifier<T>(Expression<Func<PlayerBaseStatsSO, T>> propertySelector, float newValue)
    {
        var memberExpression = (MemberExpression)propertySelector.Body;
        var property = (PropertyInfo)memberExpression.Member;

        var stats = playerAddStats;
        var currentValue = (float)property.GetValue(stats);
        property.SetValue(stats, Convert.ChangeType(currentValue + newValue, typeof(T)));
    }
    [ContextMenu("test")]
    public void test()
    {
        //AddValueInPlayerStatsSO(ref playerAddStats.playerMaxSpeed, 10);
        AddValueInPlayerStatsSO(playerAddStats => playerAddStats.playerMaxSpeed, 10);
    }
    */
    #endregion



    public void Start()
    {
        GetFields();
        //UpdateIndividualStat(playerBaseStats => playerBaseStats.playerMaxSpeed);
        foreach (GenericStat stat in stats)
        {
            stat.CompleteUpdateValue();
        }
    }
    #region getters
    /*
    public float PlayerSpeed { get =>                               playerSpeed;
    }
    public float PlayerMaxSpeed { get =>                            playerMaxSpeed; 
        set => playerMaxSpeed = value; }
    public float PlayerRestingDrag { get =>                         playerRestingDrag; 
        set => playerRestingDrag = value; }
    public float PlayerRunningDrag { get =>                         playerRunningDrag; 
        set => playerRunningDrag = value; }
    public float PlayerDragSlowDownTime { get =>                    playerDragSlowDownTime; 
        set => playerDragSlowDownTime = value; }
    public float PlayerRunForceModifier { get =>                    playerRunForceModifier; 
        set => playerRunForceModifier = value; }
    public float LateralForceModifer { get =>                       lateralForceModifer; 
        set => lateralForceModifer = value; }
    public float MinimumLateralDuration { get =>                    minimumLateralDuration; 
        set => minimumLateralDuration = value; }
    public float LateralDragCoefficient { get =>                    lateralDragCoefficient; 
        set => lateralDragCoefficient = value; }
    public float DampingCoefficient { get =>                        dampingCoefficient; 
        set => dampingCoefficient = value; }
    public float MinimumDistanceToSpawnANewPoint { get =>           minimumDistanceToSpawnANewPoint; 
        set => minimumDistanceToSpawnANewPoint = value; }
    public float MinimumTimeToSpawnANewPoint { get =>               minimumTimeToSpawnANewPoint; 
        set => minimumTimeToSpawnANewPoint = value; }
    public float ForwardLungeCoefficient { get =>                   forwardLungeCoefficient; 
        set => forwardLungeCoefficient = value; }
    public float ForwardLungeForceModifer { get =>                  forwardLungeForceModifer; 
        set => forwardLungeForceModifer = value; }
    public float LateralLungeEaseInFrames { get =>                  lateralLungeEaseInFrames; 
        set => lateralLungeEaseInFrames = value; }
    public float LateralLungeEaseOutFrames { get =>                 lateralLungeEaseOutFrames; 
        set => lateralLungeEaseOutFrames = value; }
    public float LateralLungeDesiredVEL { get =>                    lateralLungeDesiredVEL; 
        set => lateralLungeDesiredVEL = value; }
    */
    #endregion
    [ContextMenu("Get Fields")]
    public void GetFields()
    {
        allFields = typeof(PlayerBaseStatsSO).GetFields();
    }
    public void UpdateIndividualStat(string fieldName)
    {
        FieldInfo field = typeof(PlayerBaseStatsSO).GetField(fieldName);

        if (field != null)
        {
            // Get the value of the field from both addStats and multiplyStats
            float addValue = (float)field.GetValue(playerAddStats);
            float multiplyValue = (float)field.GetValue(playerMultiplyStats);
            float baseValue = (float)field.GetValue(playerBaseStats);
            // Calculate the updated value and update the currentStats
            field.SetValue(playerCurrentStats, (baseValue + addValue) * multiplyValue);
        }
        else
        {
            Debug.LogError("Invalid field name: " + fieldName);
        }
    }

    public void UpdateIndividualStat(Func<PlayerBaseStatsSO, float> selector)
    {
        FieldInfo field = typeof(PlayerBaseStatsSO).GetField(selector.Method.Name, BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            // Get the value of the field from both addStats and multiplyStats
            float addValue = (float)field.GetValue(playerAddStats);
            float multiplyValue = (float)field.GetValue(playerMultiplyStats);
            float baseValue = (float)field.GetValue(playerBaseStats);
            // Calculate the updated value and update the currentStats
            field.SetValue(playerCurrentStats, (baseValue + addValue) * multiplyValue);
        }
        else
        {
            Debug.LogError("Error in update individual stats");
        }
    }
    [ContextMenu("UpdateStats")]
    private void UpdateStats()
    {
        if (allFields == null) { Debug.LogError("all fields is null"); return; }
        // Get all fields from the class
        foreach (FieldInfo field in allFields)
        {
            // Get the value of the field from both addStats and multiplyStats
            float addValue = (float)field.GetValue(playerAddStats);
            float multiplyValue = (float)field.GetValue(playerMultiplyStats);
            float baseValue = (float)field.GetValue(playerBaseStats);
            // Calculate the updated value and set it to the new instance
            field.SetValue(playerCurrentStats, (baseValue + addValue) * multiplyValue);
        }
    }
    [ContextMenu("Reset Current Field To BaseField")]
    private void ResetCurrentFieldToBaseField()
    {
        if (allFields == null) { Debug.LogError("all fields is null"); return; }
        foreach (FieldInfo field in allFields)
        {
            // Get the base value 
            float baseValue = (float)field.GetValue(playerBaseStats);
            // sset the current value to the base value
            field.SetValue(playerCurrentStats, baseValue);
        }
    }

    [ContextMenu("Reset Add Field to Zero")]
    private void ResetAddFieldToZero()
    {
        if (allFields == null) { Debug.LogError("all fields is null"); return; }
        foreach (FieldInfo field in allFields)
        {
            // set every field in add to 0
            field.SetValue(playerAddStats, 0);
        }
    }

    [ContextMenu("Reset Multiply Field to 1")]
    private void ResetMultiplyFieldTo1()
    {
        if (allFields == null) { Debug.LogError("all fields is null"); return; }
        foreach (FieldInfo field in allFields)
        {
            // set every field in add to 1
            field.SetValue(playerMultiplyStats, 1.0f);
        }
    }
}
