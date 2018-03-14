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
						if(startCell && endCell)
						{
							cell.UnMark();
							Reset();
							return;
						}
						cell.MarkAsSelected(true);
                        if(isSelectStart)               
                        {
                            startCell = cell;
                        }
                        else
                        {
                            if(cell==startCell)
                            {
								Reset();
                                return;
                            }
                            else
							{
                                endCell = cell;
							}
                        }
                        isUpdatePath = true;
                        isSelectStart= !isSelectStart;
                    });
                });
        }
		var lineObject = new GameObject ();
		lineRenderer = lineObject.AddComponent<LineRenderer>();  
		//设置材质  
		lineRenderer.material = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));  
		//设置颜色  
		lineRenderer.SetColors(Color.red, Color.blue);  
		//设置宽度  
		lineRenderer.SetWidth(0.1f, 0.1f);  
		lineRenderer.positionCount = 0;

		if (AStarPathfinding.Distances == null)
		{
			AStarPathfinding.Distances = new Dictionary<long, Dictionary<long, int>>();
			Cells.ForEach(c =>
				{
					var dict = new Dictionary<long, int>();
					Cells.ForEach(c1 =>
						{
							dict[c1.SingleOffset] = c.GetDistance(c1);
						});
					AStarPathfinding.Distances[c.SingleOffset] = dict;
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
	/// <summary>
	/// 重置选择状态
	/// </summary>
	private void Reset()
	{
		isSelectStart = true;
		ClearPath ();
		if (startCell)
			startCell.UnMark ();
		startCell = null;
		if (endCell)
			endCell.UnMark ();
		endCell = null;
		if (lineRenderer)
			lineRenderer.positionCount = 0;
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
			path.Clear ();
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
			DrawPath ();
            isUpdatePath = false;
        }
    }
	LineRenderer lineRenderer;
	private void DrawPath()
	{
		if (lineRenderer && path != null) {
			int LengthOfLineRenderer = path.Count+1;
			lineRenderer.positionCount=LengthOfLineRenderer;
			int index = LengthOfLineRenderer;
			path.ForEach (p => {
				lineRenderer.SetPosition(--index, p.transform.position+new Vector3(0.0f,0.5f,0.0f));  
			});
			lineRenderer.SetPosition(--index, startCell.transform.position+new Vector3(0.0f,0.5f,0.0f));  

		}
	}
}

