using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class RenderLine : MonoBehaviour
{
    int startIndex;
    int endIndex;
    LineRenderer lineRenderer;
    List<int> gridsIndex;
    List<Vector3> verticesPositions;
    Color lineColor;
    bool isLineEnded;

    // Start is called before the first frame update
    void Awake()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        gridsIndex = new List<int>();
        verticesPositions = new List<Vector3>();

        //Vector3 pos = transform.position;
        //pos.z = -1;
        //verticesPositions.Add(pos);

        //lineRenderer.positionCount = verticesPositions.Count;
        //lineRenderer.SetPositions(verticesPositions.ToArray());
    }

    public void SetStartIndex(int index, Vector3 position)
    {
        if (!IsLineEnded)
        {
            startIndex = index;

            AddPoint(position, index);
        }
    }

    public int GetStartIndex()
    {
        return startIndex;
    }

    public void SetEndIndex(int index)
    {
        endIndex = index;
        IsLineEnded = true;
    }

    public Color LineColor
    {
        set
        {
            lineColor = value;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }
    }

    public bool IsLineEnded
    { 
        get => isLineEnded; 
        set => isLineEnded = value; 
    }

    public void AddPoint(Vector2 point, int cellIndex)
    {
        Debug.Log("Current grid index is  : " + cellIndex);
        // if the visited grid is already in stack, then erase the line till the grid.
        if (gridsIndex.Contains(cellIndex))
        {
            CancelLine(cellIndex);
        }
        else
        {
            gridsIndex.Add(cellIndex);
            Vector3 pos =  point;// GridsHandler.GetGridPosition(gridIndex);
            pos.z = -1;
            verticesPositions.Add(pos);
        }
        lineRenderer.positionCount = verticesPositions.Count;
        lineRenderer.SetPositions(verticesPositions.ToArray());
    }

    public void ReversePointsList()
    {
        gridsIndex.Reverse();
        verticesPositions.Reverse();

        int temp = startIndex;
        startIndex = endIndex;
        endIndex = temp;
    }

    public List<int>    GetGridIndexes()
    {
        return gridsIndex;
    }

    // cancels the line..pass -1 to completly remove the line..
    public void CancelLine(int cellIndex = -1)
    {
        for (int i = gridsIndex.Count - 1; i >= 0 ; i--)
        {
            if (gridsIndex[i] == cellIndex)
                break;
            else
            {
                gridsIndex.RemoveAt(i);
                verticesPositions.RemoveAt(i);
            }
        }

        lineRenderer.positionCount = verticesPositions.Count;
        lineRenderer.SetPositions(verticesPositions.ToArray());
    }
}
