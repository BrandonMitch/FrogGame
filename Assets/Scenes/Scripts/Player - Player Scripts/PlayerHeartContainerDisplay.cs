using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHeartContainerDisplay : MonoBehaviour
{
    public PlayerHealthSO playerHealthSO;
    public List<GameObject> UI_Heart_SpotsGO;
    private List<UI_Heart_Spot> UI_Heart_Spots = new List<UI_Heart_Spot>();

    public List<GameObject> UI_Vertical_HeartDisplay = new List<GameObject>();

    public HeartContainer baseHeartReference;
    public int MAX_COLLUMNS = 2;
    public int MAX_ROWS = 5;
    [SerializeField] float baseUISpotHeight = 100f;
    [SerializeField] int padding = 10;

    private bool intialized = false;
    private void Awake()
    {
        GetUIScripts();
    }
    private void GetUIScripts()
    {
        int length = UI_Heart_SpotsGO.Count;
        UI_Heart_Spots = new List<UI_Heart_Spot>();
        for (int i = 0; i < length; i++)
        {
            UI_Heart_Spots.Add(UI_Heart_SpotsGO[i].GetComponent<UI_Heart_Spot>());
        }
        intialized = true;
    }
    private void Start()
    {
        UpdateUI();
    }

    [SerializeField] bool debug1 = false;
    private float CalculateTotalHeight(List<HeartContainer> heartContainersSubList)
    {
        int totalPadding = 0;
        totalPadding += padding; // Padding for top
        float height = 0;
        int nContainers = 0;
        foreach (HeartContainer container in heartContainersSubList)
        {
            height += CalculateHeartContainerUIHeight(container);
            nContainers++;
        }
        totalPadding += nContainers * padding; // Padding below each UI element

        if (debug1) {
            Debug.Log("Total Padding = " + totalPadding + ", Total Height = " + (height + totalPadding));
        }

        height += totalPadding; // add padding to the height
        return height;
    }

    private float CalculateHeartContainerUIHeight(HeartContainer container)
    {
        float baseMaxHealth = baseHeartReference.GetMaxHealthValue();
        float containerMaxHealth = container.GetMaxHealthValue();
        return baseUISpotHeight * (containerMaxHealth / baseMaxHealth);
    }

    public void OnPlayerHealthChange()
    {
        if (intialized) {
            UpdateUI();
        }
        else
        {
            StartCoroutine(WaitForIntialization());
        }
    }
    public void OnPlayerHeartBreak()
    {
        
    }
    [ContextMenu("Update UI")]
    public void UpdateUI()
    {
        int totalContainerCount = playerHealthSO.GetCurrentHeartContainersCount();
        List<List<HeartContainer>> subLists = SubdivideList(playerHealthSO.heartContainers, MAX_ROWS);
        int i = -1;
        int col = -1;
        foreach (var subList in subLists)
        {
            col++;


            if (col < MAX_COLLUMNS)
            {

                UI_Vertical_HeartDisplay[col].SetActive(true); // enable vertical list

                { // Update height of vertical container
                    /*Debug.Log("sublist" + subList.Count);*/
                    RectTransform rect = UI_Vertical_HeartDisplay[col].GetComponent<RectTransform>();
                    float sublistHeight = CalculateTotalHeight(subList);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sublistHeight);
                }
                foreach (HeartContainer container in subList)
                {
                    i++;
                    EnableUIHeartSpot(i, true); // true for enable
                    UpdateIndividualHeart(i, container);
                }
            }
        }
        if (i < 0) { i = 0; }
        // Disable the rest of the hearts
        for (int j = i + 1; j < MAX_COLLUMNS*MAX_ROWS; j++)
        {
            EnableUIHeartSpot(j, false); // false for disable
        }
        for(int j = col + 1; j < MAX_COLLUMNS; j++)
        {
            UI_Vertical_HeartDisplay[j].SetActive(false);
        }
    }

    public void UpdateIndividualHeart(int i, HeartContainer container)
    {
        UI_Heart_Spots[i].SetAll(container.UIBackgroundFillColor, container.PercentFull(), container.image, CalculateHeartContainerUIHeight(container));
    }
    public void EnableUIHeartSpot(int i, bool enabled)
    {
        if(i>MAX_COLLUMNS*MAX_ROWS || i < 0) { Debug.Log("Out of bounds"); return; }
        UI_Heart_SpotsGO[i].SetActive(enabled);
    }
    /*    static List<List<HeartContainer>> SubdivideList(List<HeartContainer> originalList, int n)
        {
            List<List<HeartContainer>> subLists = new List<List<HeartContainer>>();

            int index = 0;
            while (index < originalList.Count)
            {
                subLists.Add(originalList.GetRange(index, Math.Min(n, originalList.Count - index)));
                index += n;
            }

            return subLists;
        }*/

    /*    static List<List<HeartContainer>> SubdivideList(List<HeartContainer> originalList, int n)
        {
            List<List<HeartContainer>> subLists = new List<List<HeartContainer>>();

            int index = 0;
            while (index < originalList.Count)
            {
                subLists.Add(originalList.Skip(index).Take(Math.Min(n, originalList.Count - index)).ToList());
                index += n;
            }

            return subLists;
        }*/

    static List<List<HeartContainer>> SubdivideList(List<HeartContainer> orginalList, int n)
    {

        List<List<HeartContainer>> subLists = new List<List<HeartContainer>>();

        int totalElements = orginalList.Count; 


        int remainder = totalElements % n;
        int nSublists = (totalElements / n) + ((remainder > 0)? 1 : 0);

        bool debug1 = false;
        if (debug1) Debug.Log("total elements:" + totalElements);
        if (debug1) Debug.Log("nSublists:" + nSublists);

        int i = 0;
        for (int j = 0; j < nSublists; j++) // iterates to different sublists
        {
            List<HeartContainer> newSubList = new List<HeartContainer>();
            for (int sublistIndex = 0; sublistIndex < n; sublistIndex++) // iterates through the sub list
            {
                if (i < totalElements) {
                    newSubList.Add(orginalList[i]);
                } 
                i++;
            }
            subLists.Add(newSubList);
        }
        return subLists;
    }
    IEnumerator WaitForIntialization()
    {
        while (!intialized) // while we aren't initialized
        {
            if (debug1) { Debug.Log("Called intialization corotuine"); }
            GetUIScripts();
            if (intialized == false) { yield return null; }
            else { break; }
        }
        UpdateUI();
        StopAllCoroutines();
    }
}
