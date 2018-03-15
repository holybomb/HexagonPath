using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates hexagonal shaped grid of hexagons.
/// </summary>
[ExecuteInEditMode()]
class HexagonalHexGridGenerator : ICellGridGenerator
{
    public GameObject HexagonPrefab;
    public int Radius;

    public override List<HexCell> GenerateGrid()
    {
        var hexagons = new List<HexCell>();

        if (HexagonPrefab.GetComponent<HexCell>() == null)
        {
            Debug.LogError("Invalid HexCell prefab provided");
            return hexagons;
        }

        for (int i = 0; i < Radius; i++)
        {
            for (int j = 0; j < (Radius*2) - i - 1; j++)
            {
                GameObject HexCell = Instantiate(HexagonPrefab);
                var hexSize = HexCell.GetComponent<HexCell>().GetCellDimensions();
                HexCell.SetActive(true);
				HexCell.transform.SetParent(CellsParent,true);
				HexCell.transform.localPosition = new Vector3((i * hexSize.x * 0.75f), 0, (i * hexSize.z * 0.5f) + (j * hexSize.z));
                HexCell.GetComponent<HexCell>().OffsetCoord = new Vector2(i, Radius - j - 1 - (i/2));
                HexCell.GetComponent<HexCell>().MovementCost = 1;
				HexCell.name = "dige(" + HexCell.GetComponent<HexCell>().OffsetCoord.x + "," + HexCell.GetComponent<HexCell>().OffsetCoord.y+")";
                hexagons.Add(HexCell.GetComponent<HexCell>());

                if (i == 0) continue;

                GameObject hexagon2 = Instantiate(HexagonPrefab);
                hexagon2.SetActive(true);
				hexagon2.transform.SetParent(CellsParent,true);
				hexagon2.transform.localPosition = new Vector3((-i * hexSize.x * 0.75f), 0, (i * hexSize.z * 0.5f) + (j * hexSize.z));
                hexagon2.GetComponent<HexCell>().OffsetCoord = new Vector2(-i, Radius - j - 1 - (i/2));
                hexagon2.GetComponent<HexCell>().MovementCost = 1;
				hexagon2.name = "dige(" + HexCell.GetComponent<HexCell>().OffsetCoord.x + "," + HexCell.GetComponent<HexCell>().OffsetCoord.y+")";
                hexagons.Add(hexagon2.GetComponent<HexCell>());
            }
        }   
		return hexagons;
    }
	public override List<HexCell> UpdateGrid ()
	{
		var hexagons = new List<HexCell>();

		if (HexagonPrefab.GetComponent<HexCell>() == null)
		{
			Debug.LogError("Invalid HexCell prefab provided");
			return hexagons;
		}

		var children = new List<GameObject>();
		foreach(Transform HexCell in CellsParent)
		{
			children.Add(HexCell.gameObject);
		}

		for (int i = 0; i < children.Count; i++) {
			var origin = children [i].gameObject;

			GameObject HexCell = Instantiate(HexagonPrefab);

			HexCell.transform.SetParent(CellsParent,true);
			HexCell.transform.position = origin.transform.position;
			HexCell.GetComponent<HexCell>().OffsetCoord = origin.GetComponent<HexCell>().OffsetCoord;
			HexCell.GetComponent<HexCell>().MovementCost = origin.GetComponent<HexCell>().MovementCost;
			hexagons.Add(HexCell.GetComponent<HexCell>());
			HexCell.name = "dige(" + HexCell.GetComponent<HexCell>().OffsetCoord.x + "," + HexCell.GetComponent<HexCell>().OffsetCoord.y+")";

//            var text = (HexCell.transform.Find("PosTag")??GameUIUtil.setPrefebOnParent("3DUI/3DText", HexCell.transform).transform).GetComponent<TextMesh>();
//            text.name = "PosTag";
//            text.fontSize = 25;
//            text.transform.localRotation = new Quaternion(180, 0, 0, 0);
//            text.transform.localPosition = new Vector3(0, -0.5f, 0);
//            text.text = string.Format("{0},{1}", HexCell.GetComponent<HexCell>().OffsetCoord.x.ToString("0"), HexCell.GetComponent<HexCell>().OffsetCoord.y.ToString("0"));
//            text.gameObject.SetActive(false);
		}

		children.ForEach(c => DestroyImmediate(c));

		return hexagons;
    }
    public override void ToggleCellPos()
    {
        var children = new List<GameObject>();
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

