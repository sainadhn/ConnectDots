using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinesManager 
{
    Dictionary<GameEnums.LineColors, RenderLine> linesOnBoard;

    RenderLine lineRenderer;
    GameData gameData;

    // Start is called before the first frame update
    public void Init(GameData gameData)
    {
        linesOnBoard = new Dictionary<GameEnums.LineColors, RenderLine>();
        this.lineRenderer = gameData.lineRenderer;
        this.gameData = gameData;
    }

    public RenderLine  GetLineRenderer(GameEnums.LineColors lineColor)
    {
        if(!linesOnBoard.ContainsKey(lineColor))
            return null;

        return linesOnBoard[lineColor];
    }

    public void StartLine(GameEnums.LineColors lineColor, Vector2 toPoint, int cellIndex)
    {
        int lColor = (int)lineColor;
        if (!linesOnBoard.ContainsKey(lineColor))
        {
            RenderLine newLine = Object.Instantiate(lineRenderer, toPoint, Quaternion.identity);
            linesOnBoard.Add(lineColor, newLine);
        }

        linesOnBoard[lineColor].LineColor = gameData.lineColors[lColor - 1];
        linesOnBoard[lineColor].SetStartIndex(cellIndex, toPoint);
    }
    /// <summary>
    /// Pushes new point to render line..
    /// </summary>
    /// <param name="lineColor"></param>
    /// <param name="point"></param>
    public void DrawLineToPoint(GameEnums.LineColors lineColor, Vector2 toPoint, int cellIndex)
    {
        linesOnBoard[lineColor].AddPoint(toPoint, cellIndex);
    }

    public void CancelLine(GameEnums.LineColors lineColor, int index)
    {
        linesOnBoard[lineColor].CancelLine(index);
    }

    public void LineComplete(GameEnums.LineColors lineColor, int cellIndex)
    {
        linesOnBoard[lineColor].SetEndIndex(cellIndex);
    }

    public List<int> GetLineIndexes(GameEnums.LineColors lineColor)
    {
        return linesOnBoard[lineColor].GetGridIndexes();
    }

    public int GetLinesConnectedCount()
    {
        int count = 0;
        for (int i = 0; i < GameManager.Instance.ColorsInPuzzle; i++)
        {
            GameEnums.LineColors col = (GameEnums.LineColors)(i + 1);
            if (linesOnBoard.ContainsKey(col) && linesOnBoard[col].IsLineEnded)
            {
                count++;
            }
        }

        return count;
    }
    public bool IsLinesComplete()
    {
        bool isLinesComplete = true;
        if(linesOnBoard.Count == GameManager.Instance.ColorsInPuzzle)
        {
            for(int i = 0; i < GameManager.Instance.ColorsInPuzzle; i++)
            {
                if (!linesOnBoard[(GameEnums.LineColors)(i + 1)].IsLineEnded)
                {
                    isLinesComplete = false;
                    break;
                }
            }
        }
        else
            isLinesComplete = false;

        return isLinesComplete;
    }
}
