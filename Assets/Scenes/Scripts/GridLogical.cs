using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLogical 
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    private Vector3 originPosition;
    private TextMesh[,] debugTextArray;
    public GridLogical(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        int xLength = gridArray.GetLength(0);
        int yLength = gridArray.GetLength(1);
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                //string gridText = "(" + x + "," + y + ")";
                string gridText = x + "." + y;
                //debugTextArray[x, y] = Tracer.CreateWorldText(gridText, null, GetWordPositionCenterPoint(x, y), 10, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.red, 100f);// left side vertical line
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.red, 100f); // bottom line
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.red, 100f); // horizontal outside line
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.red, 100f); // vertical outside line (left)
        }
    }
    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }


    // Takes in vector3 and gives grid position
    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x/ cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    private Vector3 GetWordPositionCenterPoint(int x, int y)
    {
        return GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
    }

    public void SetValue(int x, int y, int value)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (debugTextArray[x, y] != null)
            {
                debugTextArray[x, y].text = gridArray[x, y].ToString();
            }
        }

    }
    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public int getValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return -1;
        }

    }
    public void test()
    {

    }
    /*    public void GetInfo(out int width, out int height, out int[,] values)
        {
            width = this.width;
            height = this.height;

            // Create a new array and copy the values
            values = new int[this.width, this.height];
            Array.Copy(gridArray, values, gridArray.Length);
        }*/
    public void GetInfo(out int width, out int height, out int[][] values)
    {
        width = this.width;
        height = this.height;

        // Create a new jagged array and copy the values
        values = new int[this.width][];
        for (int i = 0; i < this.width; i++)
        {
            values[i] = new int[this.height];
            for (int j = 0; j < this.height; j++)
            {
                values[i][j] = gridArray[i, j]; // Assuming gridArray is a 2D array
            }
        }
    }

}
