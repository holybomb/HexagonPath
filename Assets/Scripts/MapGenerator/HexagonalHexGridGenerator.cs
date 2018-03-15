using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates hexagonal shaped grid of hexagons.
/// </summary>
[ExecuteInEditMode()]
public class HexagonalHexGridGenerator:MonoBehaviour
{
    public Transform CellsParent;
    public GameObject HexagonPrefab;
    public int Column = 5;
    public int Row = 5;
    public List<HexCell> GenerateGrid()
    {
        var hexagons = new List<HexCell>();
        
        if (HexagonPrefab.GetComponent<HexCell>() == null)
        {
            Debug.LogError("Invalid HexCell prefab provided");
            return hexagons;
        }
        for (int i = 0; i < Column; i++)
        {
            for (int j = 0; j < Row; j++)
            {
                GameObject HexCellObj = Instantiate(HexagonPrefab,CellsParent);
                var HexCell = HexCellObj.GetComponent<HexCell>();
                var hexSize = HexCell.GetCellDimensions();
                HexCellObj.SetActive(true);
                HexCellObj.transform.localPosition = new Vector3((i * hexSize.x * 0.75f), 0, (hexSize.z * 0.25f * (i % 2 == 0 ? -1 : 1)) + (j * hexSize.z));
                HexCell.OffsetCoord = new Vector2(i, Row - j);
                HexCell.name = "dige(" + HexCell.GetComponent<HexCell>().OffsetCoord.x + "," + HexCell.GetComponent<HexCell>().OffsetCoord.y+")";
                HexCell.MovementCost = 1;
                hexagons.Add(HexCell);
            }
        }
        UpdateInfo();
        return hexagons;
    }
    public void UpdateInfo()
    {
        foreach (Transform child in CellsParent)
        {
            var hexCell = child.GetComponent<HexCell>();
            var textNode = child.transform.Find("PosTag");
            if (textNode && hexCell)
            {
                var text = textNode.GetComponent<TextMesh>();
//                text.text = hexCell.CubeCoord.x + ",\n" + hexCell.CubeCoord.y + "," + hexCell.CubeCoord.z;
                text.text = hexCell.OffsetCoord.ToString("0,0");
            }
        }
    }
    public void ToggleCellPos()
    {
        foreach(Transform HexCell in CellsParent)
        {
            var text = HexCell.transform.Find("PosTag");
            if (text)
            {
                text.gameObject.SetActive(!text.gameObject.activeSelf);
            }
        }
    }
}

