using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public const int Rows = 5;
    public const int Columns = 5;

    public const float gridWidth = 50;
    public const float gridHeight = 50;

    public RenderLine lineRenderer;

    public Color[] cellColors;
    public Color[] lineColors;
}
