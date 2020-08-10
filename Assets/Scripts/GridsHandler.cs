using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public class GridsHandler : MonoBehaviour
{
    struct GridItemData
    {
        public GameEnums.LineColors cellColor;
        public bool isEditable;

        public GridItemData(GameEnums.LineColors cellColor = GameEnums.LineColors.None, bool editable = true)
        {
            this.cellColor = cellColor;
            isEditable = editable;
        }
    }

    // GridLayoutGroup gridLayoutGroup;
    public SpriteRenderer gridPrefab;

    public float spaceInX;
    public float spaceInY;

    public Vector2 startPosition;
    public GameData gameData;

    bool canDrawLine;
    GridItemData[] gridItemData;
    SpriteRenderer[] grids;
    LinesManager linesManager;
    int currentGridIndex;
    bool isMouseMoved;
    GameEnums.LineColors pickedColor = GameEnums.LineColors.None;

    private void Awake()
    {
        // gridLayoutGroup = transform.GetComponent<GridLayoutGroup>();
        InputHandler.OnMouseMoved += OnMouseMoved;
        InputHandler.OnMouseDown += OnMouseDown; 
        InputHandler.OnMouseUp += OnMouseUp;
    }

    private void OnDisable()
    {
        InputHandler.OnMouseMoved -= OnMouseMoved;
        InputHandler.OnMouseDown -= OnMouseDown;
        InputHandler.OnMouseUp -= OnMouseUp;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Grids handler start called");
        JSONNode levelNode = GameManager.Instance.GetCurrentLevelJsonNode();

        isMouseMoved = false;
        currentGridIndex = -1;
        grids = new SpriteRenderer[GameData.Columns * GameData.Rows];
        gridItemData = new GridItemData[GameData.Columns * GameData.Rows];

        // Instantiate grids..
        for (int i = 0; i < GameData.Columns * GameData.Rows; i++)
        {
            Vector3 pos = new Vector3(startPosition.x + (i % GameData.Columns) * spaceInX,
                                      startPosition.y - (i / GameData.Rows) * spaceInY,
                                      0);

            grids[i] = Instantiate(gridPrefab, pos, Quaternion.identity);
            gridItemData[i] = new GridItemData(GameEnums.LineColors.None, true);
        }

        // Setting the default color in grids
        SetDefaultColorsInGrid(levelNode["Red"], GameEnums.LineColors.Red);
        SetDefaultColorsInGrid(levelNode["Blue"], GameEnums.LineColors.Blue);
        SetDefaultColorsInGrid(levelNode["Green"], GameEnums.LineColors.Green);
        SetDefaultColorsInGrid(levelNode["Orange"], GameEnums.LineColors.Orange);
        SetDefaultColorsInGrid(levelNode["Purple"], GameEnums.LineColors.Purple);

        linesManager = new LinesManager();
        linesManager.Init(gameData);

        //Reseting variables shownn in ingameUI
        GameManager.Instance.noOfMoves = 0;
        GameManager.Instance.pipesConnected = 0;
        GameManager.Instance.gridsFilledPercent = 0;
    }

    private void OnDestroy()
    {
        Debug.Log("GridsHandler OnDestroy is called");
    }
    // set the specified color to the cells .. color is picked from json..
    void SetDefaultColorsInGrid(string levelNode, GameEnums.LineColors color)
    {
        int col = (int)color;
        string rIndexes = levelNode;
        string[] splittedIndex = rIndexes.Trim().Split(',');
        for (int j = 0; j < splittedIndex.Length; j++)
        {
            int id = 0;
            if (int.TryParse(splittedIndex[j], out id))
            {
                // setting the grid data..
                gridItemData[id].cellColor = color;
                gridItemData[id].isEditable = false;

                // Enable the circle sprite and set the color
                Transform node = grids[id].transform.GetChild(0);
                node.gameObject.SetActive(true);
                node.GetComponent<SpriteRenderer>().color = gameData.lineColors[col - 1]; // as color index of red is 0 in array , whereas color of red is 1 in enum
            }
        }
    }

    // Calculate the lines connected and puzzle fill percent and reset the line drawed if left in middle
    void OnMouseUp(params object[] args)
    {
        if (gridItemData[currentGridIndex].cellColor != GameEnums.LineColors.None && !linesManager.GetLineRenderer(pickedColor).IsLineEnded)
        {
            List<int> lineIndices = linesManager.GetLineIndexes(pickedColor);

            for (int i = 0; i < lineIndices.Count; i++)
            {
                if (gridItemData[lineIndices[i]].isEditable)
                    gridItemData[lineIndices[i]].cellColor = GameEnums.LineColors.None;
            }

            linesManager.CancelLine(pickedColor, -1);
        }

        if(isMouseMoved)
            GameManager.Instance.noOfMoves++;

        CalculateFilledPercent();
        GameManager.Instance.pipesConnected = linesManager.GetLinesConnectedCount();
        if (IsPuzzleSolved())
        {
            Debug.Log("Game over");
            ScreenManager.Instance.ShowUIScreen(ScreenManager.UIScreens.Gameover);
        }
        isMouseMoved = false;
    }

    // Start drawing line , set the start point to line
    void OnMouseDown(params object[] args)
    {
        Vector3 pos = (Vector3)args[0];
        pos.z = 10; // sprite distance from camera
        Vector3 mousePointInWorld = Camera.main.ScreenToWorldPoint(pos);

        if (IsPointerLiesInGridLayout(mousePointInWorld))
        {
            currentGridIndex = GetCellByPointerPosition(mousePointInWorld);

            pickedColor = gridItemData[currentGridIndex].cellColor;
            // check if pointer started from empty cell
            if (gridItemData[currentGridIndex].isEditable == false)
            {
                canDrawLine = true;
                linesManager.StartLine(gridItemData[currentGridIndex].cellColor, grids[currentGridIndex].bounds.center, currentGridIndex);
            }
            else
                canDrawLine = false;
        }
    }

    // Draw the line over the grids on mouse moved..
    private void OnMouseMoved(params object[] args)
    {
        if (!canDrawLine)
            return;

        Vector3 pos = (Vector3)args[0];
        pos.z = 10; // sprite distance from camera
        Vector3 mousePointInWorld = Camera.main.ScreenToWorldPoint(pos);

        // check if pointer is out of grid layout
        if (IsPointerLiesInGridLayout(mousePointInWorld))
        {
            isMouseMoved = true;

            int gridIndex = GetCellByPointerPosition(mousePointInWorld);

            int currentRow = gridIndex / GameData.Rows;
            int currentCol = gridIndex % GameData.Columns;

            int prevRow = currentGridIndex / GameData.Rows;
            int prevCol = currentGridIndex % GameData.Columns;

            // chech if the prev grid and current grid is same, and if both are in same row or column.. This will ignore any cell that is diagnol to the prev cell..
            if (currentGridIndex != gridIndex &&
                (currentRow == prevRow || currentCol == prevCol))
            {
                int prevGridIndex = currentGridIndex;
                currentGridIndex = gridIndex;

                // if the new cell is already having some color, then break the connection
                if (gridItemData[gridIndex].cellColor != GameEnums.LineColors.None)
                {
                    RenderLine renderLine = linesManager.GetLineRenderer(pickedColor);
                    if (gridItemData[gridIndex].cellColor == pickedColor)
                    {
                        if (renderLine.IsLineEnded)
                        {
                            renderLine.IsLineEnded = false;
                            //HighlightCellsPassedByLine(pickedColor, Color.black);
                            if (renderLine.GetStartIndex() == prevGridIndex)
                            {
                                renderLine.ReversePointsList();
                            }
                        }

                        if (!gridItemData[gridIndex].isEditable && renderLine.GetStartIndex() != gridIndex)
                        {
                            linesManager.LineComplete(pickedColor, gridIndex);

                            //int col = (int)pickedColor;
                            //Color colorToHighlight = gameData.cellColors[col - 1];
                            //HighlightCellsPassedByLine(pickedColor, colorToHighlight);

                            gridItemData[gridIndex].cellColor = pickedColor;
                            linesManager.DrawLineToPoint(pickedColor, grids[gridIndex].bounds.center, gridIndex);
                        }
                        else
                        {
                            ResetLineIndexesColor(pickedColor, gridIndex);
                            linesManager.CancelLine(pickedColor, gridIndex);
                        }
                    }
                    else
                    {
                        if (!renderLine.IsLineEnded)
                        {
                            ResetLineIndexesColor(pickedColor, -1);
                            linesManager.CancelLine(pickedColor, -1);
                            canDrawLine = false;
                        }
                        
                    }
                    
                    return;
                }

                if (!linesManager.GetLineRenderer(pickedColor).IsLineEnded)
                {
                    gridItemData[gridIndex].cellColor = pickedColor;
                    linesManager.DrawLineToPoint(pickedColor, grids[gridIndex].bounds.center, gridIndex);
                }
            }
        }
       
    }

    //// set the color of tiles passed by the line..
    //void HighlightCellsPassedByLine(GameEnums.LineColors color, Color colorToSet)
    //{
    //    List<int> cells = linesManager.GetLineIndexes(color);
    //    for(int i = 0; i < cells.Count; i++)
    //    {
    //        grids[cells[i]].color = colorToSet;
    //    }
    //}
    // Calculates the percent of the grids filled with some color..
    void CalculateFilledPercent()
    {
        int filledCount = 0;
        for (int i = 0; i < GameData.Columns * GameData.Rows; i++)
        {
            if (gridItemData[i].cellColor != GameEnums.LineColors.None)
                filledCount++;
        }

        float totalGrids = GameData.Columns * GameData.Rows;

        GameManager.Instance.gridsFilledPercent = (int)(((float)filledCount / totalGrids) * 100);
    }

    // check if the puzzle is solved
    bool IsPuzzleSolved()
    {
        bool isPuzzleSolved = true;
        if (linesManager.IsLinesComplete())
        {
            for (int i = 0; i < GameData.Columns * GameData.Rows; i++)
            {
                if (gridItemData[i].cellColor == GameEnums.LineColors.None)
                {
                    isPuzzleSolved = false;
                    break;
                }
            }
        }
        else
            isPuzzleSolved = false;

        return isPuzzleSolved;
    }

    // resets the cells color with the default color
    void ResetLineIndexesColor(GameEnums.LineColors col, int index)
    {
        List<int> lineIndices = linesManager.GetLineIndexes(col);

        for (int i = lineIndices.Count - 1; i >= 0 ; i--)
        {
            if (gridItemData[lineIndices[i]].isEditable)
            {
                if (lineIndices[i] == index)
                    break;
                else
                {
                    Debug.Log("ResetLineIndexesColor  is : " + lineIndices[i]);
                    gridItemData[lineIndices[i]].cellColor = GameEnums.LineColors.None;
                }
            }
        }
    }

    // returns the cell index if the pointer is on top of the cell
    int GetCellByPointerPosition(Vector3 mousePointInWorld)
    {
        int gridIndex = -1;
        float extentLength = grids[0].bounds.extents.magnitude;
        //loop through all the cells to find out the pointer position on cell
        for (int i = 0; i < GameData.Columns * GameData.Rows; i++)
        {
            float distance = Vector3.Distance(mousePointInWorld, grids[i].bounds.center);
            // if the distance between the pointer position and the center of the sprite is less than the extent.. 
            //Then consider the pointer is in that cell..
            if (distance <= extentLength)
            {
                gridIndex = i;
                break;
            }
        }
        return gridIndex;
    }

    /// <summary>
    /// check if pointer lies in Grid layout
    /// </summary>
    /// <param name="pointerPosition"></param>
    /// <returns></returns>
    bool IsPointerLiesInGridLayout( Vector3 pointerPosition)
    {
        int lastIndex = (GameData.Columns * GameData.Rows) - 1;

        if (pointerPosition.x > grids[0].bounds.min.x &&
           pointerPosition.y < grids[0].bounds.max.y &&
           pointerPosition.x < grids[lastIndex].bounds.max.x &&
           pointerPosition.y > grids[lastIndex].bounds.min.y)
        return true;

        return false;
    }

}
