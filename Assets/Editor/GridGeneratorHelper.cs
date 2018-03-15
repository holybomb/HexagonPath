using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(HexagonalHexGridGenerator), true)]
public class GridGeneratorHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HexagonalHexGridGenerator gridGenerator = (HexagonalHexGridGenerator)target;
  
        if (GUILayout.Button("生成地图"))
        {
            gridGenerator.GenerateGrid();
        }
        if (GUILayout.Button("清空地图"))
        {
            var children = new List<GameObject>();
            foreach(Transform cell in gridGenerator.CellsParent)
            {
                children.Add(cell.gameObject);
            }

            children.ForEach(c => DestroyImmediate(c));
        }
        if (GUILayout.Button("显示坐标")) {
            gridGenerator.ToggleCellPos();
        }
    }
}
