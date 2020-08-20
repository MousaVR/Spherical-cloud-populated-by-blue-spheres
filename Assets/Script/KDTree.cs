﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTree<T> : IEnumerable<T>, IEnumerable where T : Component
{
    protected KdNode _root;
    protected KdNode _last;
    protected int _count;
    protected float _LastUpdate = -1f;
    protected KdNode[] _open;

    public int Count { get { return _count; } }
    public bool IsReadOnly { get { return false; } }
    public float AverageSearchLength { protected set; get; }
    public float AverageSearchDeep { protected set; get; }


    public T this[int key]
    {
        get
        {
            if (key >= _count)
                throw new ArgumentOutOfRangeException();
            var current = _root;
            for (var i = 0; i < key; i++)
                current = current.next;
            return current.component;
        }
    }

    /// <summary>
    /// Method to enable foreach-loops
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Method to enable foreach-loops
    /// </summary>
    /// <returns>Enumberator</returns>
    public IEnumerator<T> GetEnumerator()
    {
        var current = _root;
        while (current != null)
        {
            yield return current.component;
            current = current.next;
        }
    }

    /// <summary>
    /// add item
    /// </summary>
    /// <param name="item">item</param>
    public void Add(T item)
    {
        _add(new KdNode() { component = item });
    }

    /// <summary>
    /// batch add items
    /// </summary>
    /// <param name="items">items</param>
    public void AddAll(List<T> items)
    {
        foreach (var item in items)
            Add(item);
    }

    /// <summary>
    /// find first object that matches the given predicate
    /// </summary>
    /// <param name="match">lamda expression</param>
    public T Find(Predicate<T> match)
    {
        var current = _root;
        while (current != null)
        {
            if (match(current.component))
                return current.component;
            current = current.next;
        }
        return null;
    }

    /// <summary>
    /// remove all objects that matches the given predicate
    /// </summary>
    /// <param name="match">lamda expression</param>
    public void RemoveAll(Predicate<T> match)
    {
        var list = new List<KdNode>(_getNodes());
        list.RemoveAll(n => match(n.component));
        Clear();
        foreach (var node in list)
        {
            node._oldRef = null;
            node.next = null;
        }
        foreach (var node in list)
            _add(node);
    }

    public void Clear()
    {
        //rest for the garbage collection
        _root = null;
        _last = null;
        _count = 0;
    }


    /// <summary>
    /// Update positions (if objects moved)
    /// </summary>
    public void UpdatePositions()
    {
        //save old traverse
        var current = _root;
        while (current != null)
        {
            current._oldRef = current.next;
            current = current.next;
        }

        //save root
        current = _root;

        //reset values
        Clear();

        //readd
        while (current != null)
        {
            _add(current);
            current = current._oldRef;
        }
    }

    protected float _distance(Vector3 a, Vector3 b)
    {
            return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
    }

    protected float _getSplitValue(int level, Vector3 position)
    {
            return (level % 3 == 0) ? position.x : (level % 3 == 1) ? position.y : position.z;
    }

    private void _add(KdNode newNode)
    {
        _count++;
        newNode.left = null;
        newNode.right = null;
        newNode.level = 0;
        var parent = _findParent(newNode.component.transform.position);

        //set last
        if (_last != null)
            _last.next = newNode;
        _last = newNode;

        //set root
        if (parent == null)
        {
            _root = newNode;
            return;
        }

        var splitParent = _getSplitValue(parent);
        var splitNew = _getSplitValue(parent.level, newNode.component.transform.position);

        newNode.level = parent.level + 1;

        if (splitNew < splitParent)
            parent.left = newNode; //go left
        else
            parent.right = newNode; //go right
    }

    private KdNode _findParent(Vector3 position)
    {
        //travers from root to bottom and check every node
        var current = _root;
        var parent = _root;
        while (current != null)
        {
            var splitCurrent = _getSplitValue(current);
            var splitSearch = _getSplitValue(current.level, position);

            parent = current;
            if (splitSearch < splitCurrent)
                current = current.left; //go left
            else
                current = current.right; //go right

        }
        return parent;
    }

    /// <summary>
    /// Find closest object to given position
    /// </summary>
    /// <param name="position">position</param>
    /// <returns>closest object</returns>
    public T FindClosest(Vector3 position)
    {
        return _findClosest(position);
    }


    protected T _findClosest(Vector3 position, List<T> traversed = null)
    {
        if (_root == null)
            return null;

        var nearestDist = float.MaxValue;
        KdNode nearest = null;

        if (_open == null || _open.Length < Count)
            _open = new KdNode[Count];
        for (int i = 0; i < _open.Length; i++)
            _open[i] = null;

        var openAdd = 0;
        var openCur = 0;

        if (_root != null)
            _open[openAdd++] = _root;

        while (openCur < _open.Length && _open[openCur] != null)
        {
            var current = _open[openCur++];
            if (traversed != null)
                traversed.Add(current.component);

            var nodeDist = _distance(position, current.component.transform.position);
            if (nodeDist < nearestDist)
            {
                nearestDist = nodeDist;
                nearest = current;
            }

            var splitCurrent = _getSplitValue(current);
            var splitSearch = _getSplitValue(current.level, position);

            if (splitSearch < splitCurrent)
            {
                if (current.left != null)
                    _open[openAdd++] = current.left; //go left
                if (Mathf.Abs(splitCurrent - splitSearch) * Mathf.Abs(splitCurrent - splitSearch) < nearestDist && current.right != null)
                    _open[openAdd++] = current.right; //go right
            }
            else
            {
                if (current.right != null)
                    _open[openAdd++] = current.right; //go right
                if (Mathf.Abs(splitCurrent - splitSearch) * Mathf.Abs(splitCurrent - splitSearch) < nearestDist && current.left != null)
                    _open[openAdd++] = current.left; //go left
            }
        }

        AverageSearchLength = (99f * AverageSearchLength + openCur) / 100f;
        AverageSearchDeep = (99f * AverageSearchDeep + nearest.level) / 100f;

        return nearest.component;
    }

    private float _getSplitValue(KdNode node)
    {
        return _getSplitValue(node.level, node.component.transform.position);
    }

    private IEnumerable<KdNode> _getNodes()
    {
        var current = _root;
        while (current != null)
        {
            yield return current;
            current = current.next;
        }
    }

    protected class KdNode
    {
        internal T component;
        internal int level;
        internal KdNode left;
        internal KdNode right;
        internal KdNode next;
        internal KdNode _oldRef;
    }

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
    void test(int name)
    {
        FindClosest(Vector3.up);
        UpdatePositions();
    }
}
