using UnityEngine;
using System.Collections.Generic;

public struct Edge
{
    public readonly Vector2 A;
    
    public readonly Vector2 B;

    public Vector2 Normal => (B - A).normalized;

    public Edge(Vector2 a, Vector2 b) => (A, B) = (a, b);

    public IEnumerable<Vector2> Vertices
    {
        get
        {
            yield return A;
            yield return B;
        }
    }

    public bool Contains(Vector2 point, bool exactly = true)
    {
        var pointNormal = point - A;
        var normal = B - A;

        var pnm = pointNormal.magnitude;
        var nm = normal.magnitude;

        if (exactly && A == point)
            return true;

        var dot = Vector2.Dot(pointNormal / pnm, normal / nm);

        if (!Mathf.Approximately(dot, 1f))
            return false;

        if (exactly && Mathf.Approximately(pnm, nm))
            return true;

        return pnm < nm;

    }

    public Vector2? Intersection(Edge other, bool exactly = true) 
    {
        float n;
        if (Mathf.Abs(B.y - A.y) > float.Epsilon)
        {  
            float q = (B.x - A.x) / (A.y - B.y);
            float sn = (other.A.x - other.B.x) + (other.A.y - other.B.y) * q;
            if (Mathf.Abs(sn) < float.Epsilon)
                return null;

            float fn = (other.A.x - A.x) + (other.A.y - A.y) * q;
            n = fn / sn;
        }
        else
        {
            if (Mathf.Abs(other.A.y - other.B.y) < float.Epsilon)
                return null;
            
            n = (other.A.y - A.y) / (other.A.y - other.B.y);
        }

        var c = new Vector2(other.A.x + (other.B.x - other.A.x) * n, other.A.y + (other.B.y - other.A.y) * n);

        if (!Contains(c, exactly) || !other.Contains(c, exactly))
            return null;

        return c;
    }
}

