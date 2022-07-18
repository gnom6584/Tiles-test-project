using UnityEngine;
using System.Collections.Generic;

public struct Rectangle : Primitive
{
    public readonly Vector2 A;
    
    public readonly Vector2 B;

    public Rectangle(Vector2 a, Vector2 b) => (A, B) = (a, b);

    public float Width => Mathf.Abs(B.x - A.x);

    public float Height => Mathf.Abs(B.y - A.y);

    public float Area => Width * Height;

    public bool Contains(Vector2 point) 
        => point.x >= Mathf.Min(A.x, B.x) 
        && point.x <= Mathf.Max(A.x, B.x)
        && point.y >= Mathf.Min(A.y, B.y)
        && point.y <= Mathf.Max(A.y, B.y);

    public IEnumerable<Edge> Edges
    {
        get
        {
            var c = new Vector2(B.x, A.y);
            var d = new Vector2(A.x, B.y);

            yield return new Edge(A, c);
            yield return new Edge(c, B);
            yield return new Edge(B, d);
            yield return new Edge(d, A);
        }
    }

    public IEnumerable<Vector2> Vertices
    {
        get
        {
            yield return new Vector2(A.x, A.y);
            yield return new Vector2(B.x, A.y);
            yield return new Vector2(A.x, B.y);
            yield return new Vector2(B.x, B.y);
        }
    }
}
