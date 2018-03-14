using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGrid : MonoBehaviour
{
    public GameObject cellRoot;
    [HideInInspector]
    public List<HexCell> Cells = new List<HexCell>();
    // Use this for initialization
    void Awake()
    {
        if (cellRoot)
        {
            Cells.AddRange(cellRoot.transform.GetComponentsInChildren<HexCell>());
            Cells.ForEach(c =>
                {
                    c.CellClicked += ((object sender, System.EventArgs e) => {
                        var cell = sender as HexCell;
                        if(isSelectStart)               
                        {
                            ClearPath();
                            if(startCell)
                                startCell.UnMark();
                            startCell = cell;
                            if(endCell!=null)
                                endCell.UnMark();
                            endCell = null;
                        }
                        else
                        {
                            if(cell==startCell)
                            {
                                cell.UnMark();
                                startCell=null;
                                return;
                            }
                            else
                                endCell = cell;
                        }
                        isUpdatePath = true;
                        isSelectStart= !isSelectStart;
                    });
                });
        }
    }
    protected virtual Dictionary<HexCell, Dictionary<HexCell, int>> GetGraphEdges(List<HexCell> cells)
    {
        Dictionary<HexCell, Dictionary<HexCell, int>> ret = new Dictionary<HexCell, Dictionary<HexCell, int>>();
        foreach (var cell in cells)
        {
                ret[cell] = new Dictionary<HexCell, int>();
                var neighbors = cell.GetNeighbours(cells);
                foreach (var neighbour in neighbors)
                {
                    ret[cell][neighbour] = neighbour.MovementCost;
                }
        }
        return ret;
    }
    bool isSelectStart = true;
    HexCell startCell,endCell;
    List<HexCell> path;
    bool isUpdatePath;
    private static IPathfinding _pathfinder = new AStarPathfinding();

    void ClearPath()
    {
        if (path != null)
        {
            path.ForEach(c => c.UnMark());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startCell && endCell && startCell != endCell && isUpdatePath)
        {
            path = _pathfinder.FindPath<HexCell>(GetGraphEdges(Cells), startCell, endCell);
            if (path != null && path.Count > 0)
            {
                path.ForEach(c =>
                    {
                        if (c != startCell && c != endCell)
                            c.MarkAsPath();
                    });
            }
            isUpdatePath = false;
        }
    }
}

