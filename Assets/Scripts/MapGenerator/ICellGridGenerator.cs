using System.Collections.Generic;
using UnityEngine;

public abstract class ICellGridGenerator : MonoBehaviour
{
    public Transform CellsParent;
    public abstract List<HexCell> GenerateGrid();
    public abstract List<HexCell> UpdateGrid();
    public abstract void ToggleCellPos();
}

