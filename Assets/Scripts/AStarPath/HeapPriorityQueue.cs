using System.Collections.Generic;
using System;


class PriorityQueueNode : IComparable
{
    public HexCell Item { get; private set; }
    public float Priority { get; private set; }

    public PriorityQueueNode(HexCell item, float priority)
    {
        Item = item;
        Priority = priority;
    }

    public int CompareTo(object obj)
    {
        return Priority.CompareTo((obj as PriorityQueueNode).Priority);
    }
}

class HeapPriorityQueue
{
    private List<PriorityQueueNode> _queue;
    
    public HeapPriorityQueue()
    {
        _queue = new List<PriorityQueueNode>();
    }

    public int Count
    {
        get { return _queue.Count; }
    }
    /// <summary>
    /// 压入队列
    /// </summary>
    /// <param name="item">Item.</param>
    /// <param name="priority">Priority.</param>
    public void Enqueue(HexCell item, int priority)
    {
        _queue.Add(new PriorityQueueNode(item, priority));
        int ci = _queue.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (_queue[ci].CompareTo(_queue[pi]) >= 0)
                break;
            var tmp = _queue[ci];
            _queue[ci] = _queue[pi];
            _queue[pi] = tmp;
            ci = pi;
        }
    }
    public List<HexCell> getQueueCell()
    {
        var result = new List<HexCell>();
        _queue.ForEach(c => result.Add(c.Item));
        return result;
    }
    /// <summary>
    /// 取出队列
    /// </summary>
    public HexCell Dequeue()
    {
        int li = _queue.Count - 1;
        var frontItem = _queue[0];
        _queue[0] = _queue[li];
        _queue.RemoveAt(li);

        li--;
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > li) break;
            int rc = ci + 1;
            if (rc <= li && _queue[rc].CompareTo(_queue[ci]) < 0)
                ci = rc;
            if (_queue[pi].CompareTo(_queue[ci]) <= 0) 
                break;
            var tmp = _queue[pi];
            _queue[pi] = _queue[ci]; 
            _queue[ci] = tmp;
            pi = ci;
        }
        return frontItem.Item;
    }
}

