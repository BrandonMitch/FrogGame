using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class GridSOValues : ScriptableObject
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] public int[][] values;
    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            values[x][y] = value;
        }
    }
    public int GetValue(int x , int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return values[x][y];
        }
        else
        {
            return -1;
        }
    }

    public void FromGridLogicalToSO(GridLogical grid)
    {
        string s = "";
        int[][] blankArray;
        grid.GetInfo(out width, out height, out blankArray);

        // Initialize the jagged array
        values = new int[width][];

        for (int i = 0; i < width; i++)
        {
            values[i] = new int[height];
            for (int j = 0; j < height; j++)
            {
                s += blankArray[i][j] + " ";
                values[i][j] = blankArray[i][j]; // Accessing elements of jagged array
            }
            s += "\n";
        }
        Debug.Log(s);
    }

    public void printArray()
    {
        string s = "";
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                s += values[i][j] + " ";
            }
            s += "\n";
        }
        Debug.Log(s);
    }
}
