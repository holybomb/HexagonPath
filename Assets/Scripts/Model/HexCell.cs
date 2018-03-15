using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Implementation of hexagonal cell.
/// </summary>
public class HexCell : MonoBehaviour
{
    public bool isSelected = false;
    [HideInInspector]
    public Vector2 OffsetCoord;
	public long SingleOffset { get { return (((long)OffsetCoord.x) << 32) | (long)((uint)(int)OffsetCoord.y); } }
    /// <summary>
    /// HexGrids comes in four types regarding the layout. This distinction is necessary to convert cube coordinates to offset and vice versa.
    /// </summary>

	private bool _IsCubeCoordInit;
	private Vector3 _CubeCoord;

    public int MovementCost;

    public event EventHandler CellClicked;

    /// <summary>
    /// 立方体坐标
    /// </summary>
    public Vector3 CubeCoord
    {
        get
        {
			if (_IsCubeCoordInit)
				return _CubeCoord;
			
            Vector3 ret = new Vector3();
            ret.x = OffsetCoord.x;
            ret.z = OffsetCoord.y - (OffsetCoord.x + (Mathf.Abs(OffsetCoord.x) % 2)) / 2; 
            ret.y = -ret.x - ret.z;

			_IsCubeCoordInit = true;
			_CubeCoord = ret;
            return ret;
        }
    }
    /// <summary>
    /// 立方体坐标转为偏移坐标
    /// </summary>
    /// <returns>The to offset coords.</returns>
    /// <param name="cubeCoords">Cube coords.</param>
    protected Vector2 CubeToOffsetCoords(Vector3 cubeCoords)
    {
        Vector2 ret = new Vector2();
        ret.x = cubeCoords.x;
        ret.y = cubeCoords.z + (cubeCoords.x + (Mathf.Abs(cubeCoords.x) % 2)) / 2;
        return ret;
    }
    /// <summary>
    /// 获取cell尺寸
    /// </summary>
    /// <returns>The cell dimensions.</returns>
    public Vector3 GetCellDimensions()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            return renderer.bounds.size * 1.03f;

        return new Vector3(1.0f, 1.0f, 1f);// * 1.1f;
    }
    /// <summary>
    /// 六个方向的立方体坐标偏移
    /// </summary>
    protected static readonly Vector3[] _directions =  {
        new Vector3(+1, -1, 0), new Vector3(+1, 0, -1), new Vector3(0, +1, -1),
        new Vector3(-1, +1, 0), new Vector3(-1, 0, +1), new Vector3(0, -1, +1)};

    public int GetDistance(HexCell other)
    {
		int dx = (int)(CubeCoord.x - other.CubeCoord.x);
		int dy = (int)(CubeCoord.y - other.CubeCoord.y);
		int dz = (int)(CubeCoord.z - other.CubeCoord.z);
		int distance = ((dx > 0 ? dx : -dx) + (dy > 0 ? dy : -dy) + (dz > 0 ? dz : -dz)) / 2;
        return distance;
    }

    //Distance is given using Manhattan Norm.
    public List<HexCell> GetNeighbours(List<HexCell> cells)
    {
        List<HexCell> ret = new List<HexCell>();

        for (var i = 0; i < _directions.Length / 2; i++)
        {
            var neighbour = cells.Find(c => c.OffsetCoord == CubeToOffsetCoords(CubeCoord + _directions[i]));
            if (neighbour == null) continue;
            ret.Add(neighbour);
        }

        for (var i = _directions.Length / 2; i < _directions.Length; i++)
        {
            var neighbour = cells.Find(c => c.OffsetCoord == CubeToOffsetCoords(CubeCoord + _directions[i]));
            if (neighbour == null) continue;
            ret.Add(neighbour);
        }
        return ret;
    }//Each square cell has six neighbors, which positions on grid relative to the cell are stored in _directions constant.

    public Color SelectColor = Color.green;
    public Color UnSelectColor = Color.white;
    public Color pathColor = Color.yellow;
    public void MarkAsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
        SetColor(isSelected ? SelectColor : UnSelectColor);
    }
    public void MarkAsPath()
    {
        SetColor(pathColor);
    }
    public void UnMark()
    {
        SetColor(UnSelectColor);
    }
    public void SetColor(Color color)
    {
        var render = transform.GetComponent<Renderer>();
        if (render && render.material)
        {
            render.material.SetColor("_Color", color);
        }
    }
    public void OnMouseDown()
    {
        MarkAsSelected(!isSelected);
        if (CellClicked != null)
        {
            CellClicked.Invoke(this, null);
        }
    }
}