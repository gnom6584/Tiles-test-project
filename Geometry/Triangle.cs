using UnityEngine;
using System.Collections.Generic;

public struct Triangle : Primitive
{
    public readonly Vector2 A;

    public readonly Vector2 B;

    public readonly Vector2 C;

    public float Area => Mathf.Abs(A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y)) / 2f;

    public Triangle(Vector2 a, Vector2 b, Vector2 c) => (A, B, C) = (a, b, c);

    public bool Contains(Vector2 point)
    {
        var area = Area;

        if (Mathf.Approximately(area, 0f))
        {
            if (point == A)
                return true;
            return false;
        }

        var area1 = new Triangle(point, B, C).Area;
        var area2 = new Triangle(point, A, C).Area;
        var area3 = new Triangle(point, A, B).Area;

        return Mathf.Approximately(area, area1 + area2 + area3);
    }

    public IEnumerable<Edge> Edges
    {
        get
        {
            yield return new Edge(A, B);
            yield return new Edge(A, C);
            yield return new Edge(B, C);
        }
    }

    public IEnumerable<Vector2> Vertices
    {
        get
        {
            yield return A;
            yield return B;
            yield return C;
        }
    }


}

