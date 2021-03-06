﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGrid : MonoBehaviour
{
    public GameObject cellRoot;
    [HideInInspector]
    public List<HexCell> Cells = new List<HexCell>();
    void Awake()
    {
        if (cellRoot)
        {
			//初始化格子信息
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
        lineObject.name = "PathLine";
		lineObject.transform.SetParent (cellRoot.transform);
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
                    StartCoroutine(DrawPath());
                    })));
            isUpdatePath = false;
        }
    }
    public bool isDrawLineThroughCenter = false;
	LineRenderer lineRenderer;
    /// <summary>
    /// 绘制路径
    /// </summary>
    /// <returns>The path.</returns>
    private IEnumerator DrawPath()
	{
		if (lineRenderer && path != null) {
			int LengthOfLineRenderer = path.Count+1;

            path.Add(startCell);
            for (int i = 0; i < LengthOfLineRenderer; i++)
            {
                #if UNITY_5
                lineRenderer.numPositions++;
                #else
                lineRenderer.positionCount++;
                #endif
				var p = path[LengthOfLineRenderer-i-1];
                var pos = p.transform.position;
                if (!isDrawLineThroughCenter)
                {
                    float lerpDt = (float)(i) / (LengthOfLineRenderer - 1);

                    pos = Vector3.Lerp(startCell.transform.position, endCell.transform.position, lerpDt);
                    if (Vector3.Distance(pos, p.transform.position) > 0.25)
                    {
                        for (float t = 0; t < 1.0f; t += 0.1f)
                        {
                            var temp = Vector3.Lerp(pos, p.transform.position, t);
                            if (Vector3.Distance(temp, p.transform.position) <= 0.25)
                            {
                                pos = temp;
                                break;
                            }
                        }
                    }
                }
				lineRenderer.SetPosition(i, pos);
                yield return new WaitForSeconds(0.1f);
            }
		}
        yield return 0;
	}
}

