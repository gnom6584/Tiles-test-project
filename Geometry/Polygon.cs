using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public struct Polygon : Primitive
{
    public IEnumerable<Vector2> Vertices => _vertices;

    public readonly IReadOnlyList<Vector2> _vertices { get; }

    public float Area => Triangles.Sum(triangle => triangle.Area);

    public bool Contains(Vector2 point)
    {
        foreach (var triangle in Triangles)
            if (triangle.Contains(point))
                return true;
        return false;
    }

    public IEnumerable<Edge> Edges
    {
        get
        {
            for (var i = 0; i < _vertices.Count - 1; i++)
                yield return new Edge(_vertices[i], _vertices[i + 1]);
            yield return new Edge(_vertices[_vertices.Count - 1], _vertices[0]);
        }
    }

    public IEnumerable<Triangle> Triangles
    {
        get
        {
            for (var i = 1; i < _vertices.Count - 1; i++)
                yield return new Triangle(_vertices[i], _vertices[0], _vertices[i + 1]);
        }
    }

    public IEnumerable<(int A, int B, int C)> TrianglesIndices
    {
        get
        {
            for (var i = 1; i < _vertices.Count - 1; i++)
                yield return (i, 0, i + 1);
        }
    }

    public Polygon(params Vector2[] vertices) => _vertices = Init(vertices);

    public Polygon(IReadOnlyList<Vector2> vertices) => _vertices = Init(vertices);    

    static IReadOnlyList<Vector2> Init(IReadOnlyList<Vector2> initValue)
    {
        if (initValue.Count is 0)
            throw new System.ArgumentException("Empty vertices array");

        var list = initValue.ToList();
        var center = initValue.Aggregate((it, sum) => sum + it) / initValue.Count;
        list.Sort((lhs, rhs) => Mathf.Atan2(rhs.x - center.x, rhs.y - center.y).CompareTo(Mathf.Atan2(lhs.x - center.x, lhs.y - center.y)));

        list = RemoveDuplicatesInSortedArray(list);

        var listCount = initValue.Count;

        for (var i = 0; i < 3 - listCount; ++i)
            list.Add(list[0]);

        return list;
    }

    static List<Vector2> RemoveDuplicatesInSortedArray(List<Vector2> arr)
    {
        int n = arr.Count;

        if (n == 0 || n == 1)
            return arr;

        var temp = new Vector2[n];

        int j = 0;
        for (int i = 0; i < n - 1; i++)
            if (arr[i] != arr[i + 1])
                temp[j++] = arr[i];

        temp[j++] = arr[n - 1];

        for (int i = 0; i < j; i++)
            arr[i] = temp[i];

        return arr.Take(j).ToList();
    }
}
