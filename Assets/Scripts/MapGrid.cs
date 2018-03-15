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
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.blue;
		//设置宽度  
        lineRenderer.startWidth =0.1f;
        lineRenderer.endWidth = 0.1f; 
        #if UNITY_5
        lineRenderer.numPositions = 0;
        #else
		lineRenderer.positionCount = 0;
        #endif
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
        #if UNITY_5
            lineRenderer.numPositions = 0;
        #else
			lineRenderer.positionCount = 0;
        #endif
	}
    bool isSelectStart = true;
    HexCell startCell,endCell;
    List<HexCell> path;
    bool isUpdatePath;
    private static AStarPathfinding _pathfinder = new AStarPathfinding();

    void ClearPath()
    {
        if (path != null)
        {
            path.ForEach(c => c.UnMark());
			path.Clear ();
        }
    }
    void Update()
    {
        if (startCell && endCell && startCell != endCell && isUpdatePath)
        {
            StartCoroutine(_pathfinder.FindPath(GetGraphEdges(Cells), startCell, endCell, ((p) =>
                    {
                        path = p;
                        if (path != null && path.Count > 0)
                        {
                            path.ForEach(c =>
                                {
                                    if (c != startCell && c != endCell)
                                        c.MarkAsPath();
                                });
                        }
                        DrawPath();
                    })));
            isUpdatePath = false;
        }
    }
    bool isDrawLineThroughCenter = false;
	LineRenderer lineRenderer;
	private void DrawPath()
	{
		if (lineRenderer && path != null) {
			int LengthOfLineRenderer = path.Count+1;
            #if UNITY_5
            lineRenderer.numPositions = LengthOfLineRenderer;
            #else
			lineRenderer.positionCount=LengthOfLineRenderer;
            #endif
            if (isDrawLineThroughCenter)
            {
                int index = LengthOfLineRenderer;
                path.ForEach(p =>
                    {
                        lineRenderer.SetPosition(--index, p.transform.position + new Vector3(0.0f, 0.5f, 0.0f));  
                    });
                lineRenderer.SetPosition(--index, startCell.transform.position + new Vector3(0.0f, 0.5f, 0.0f));  
            }
            else
            {
                path.Add(startCell);
                for (int i = 0; i < LengthOfLineRenderer; i++)
                {
                    var p = path[i];
//                    float lerpDt = (float)(i) / LengthOfLineRenderer;
//                    var p = Vector3.Lerp(startCell.transform.position, endCell.transform.position, lerpDt);
//                    p.y = 0;
                    lineRenderer.SetPosition(i, p.transform.position - new Vector3(0.0f, 0.5f, 0.0f));
                }
            }
		}
	}
}

