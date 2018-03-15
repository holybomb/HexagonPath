using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

public class AStarPathfinding
{
    public delegate void FindPathCallBack(List<HexCell> path);
    public IEnumerator FindPath(Dictionary<HexCell, Dictionary<HexCell, int>> edges, HexCell originNode, HexCell destinationNode,FindPathCallBack findPathCallBack=null)
    {
        //记录一个目标路径队列
        var frontier = new HeapPriorityQueue();
        frontier.Enqueue(originNode, 0);
        //记录路径图
        Dictionary<HexCell, HexCell> cameFrom = new Dictionary<HexCell, HexCell>();
        cameFrom.Add(originNode, default(HexCell));
        //记录到某个临时目标的路程花费
        Dictionary<HexCell, int> costSoFar = new Dictionary<HexCell, int>();
        costSoFar.Add(originNode, 0);
        var midNode = new Vector2(Mathf.Abs(destinationNode.OffsetCoord.x - originNode.OffsetCoord.x) / 2, Mathf.Abs(destinationNode.OffsetCoord.y - originNode.OffsetCoord.y) / 2);
        while (frontier.Count != 0)
        {
            var current = frontier.Dequeue();
            //寻路到目标点跳出
            if (current.Equals(destinationNode)) break;

            var neighbours = GetNeigbours(edges, current);
//            neighbours = neighbours.OrderBy(c => c.GetDistance(destinationNode)).ToList();
            if (neighbours.Contains(destinationNode))
                neighbours.RemoveAll(c => c != destinationNode);
            foreach (var neighbour in neighbours)
            {
                var newCost = costSoFar[current] + edges[current][neighbour];
                if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                {
                    //找到一个更近的邻居节点
                    costSoFar[neighbour] = newCost;
                    cameFrom[neighbour] = current;
                    //根据到目标点的坐标距离设置启发值
                    var heuristicDes = Heuristic(destinationNode, neighbour);
                    var priority = newCost + heuristicDes;// + Vector2.Distance(neighbour.OffsetCoord, midNode);
                    //将此邻居节点压入目标路径队列
                    frontier.Enqueue(neighbour, priority);
                }
            }
        }

        List<HexCell> path = new List<HexCell>();
        if (!cameFrom.ContainsKey(destinationNode))
        {
            //未找到路径
            if (findPathCallBack != null)
            {
                findPathCallBack.Invoke(path);
            }
            yield break;
        }
        path.Add(destinationNode);
        var temp = destinationNode;
        while (!cameFrom[temp].Equals(originNode))
        {
            var currentPathElement = cameFrom[temp];
            path.Add(currentPathElement);
            temp = currentPathElement;
        }
        if (findPathCallBack != null)
        {
            findPathCallBack.Invoke(path);
        }
    }
    private int Heuristic(HexCell a, HexCell b)
    {
        return a.GetDistance(b);
	}
    protected List<HexCell> GetNeigbours(Dictionary<HexCell, Dictionary<HexCell, int>> edges, HexCell node)
    {
        if (!edges.ContainsKey(node))
        {
            return new List<HexCell>();
        }
        return edges[node].Keys.ToList();
    }
}



