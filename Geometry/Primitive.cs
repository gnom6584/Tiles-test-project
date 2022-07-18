using UnityEngine;
using System.Collections.Generic;

public interface Primitive
{
    bool Contains(Vector2 position);

    float Area
    {
        get;
    }

    IEnumerable<Vector2> Vertices
    {
        get;
    }

    IEnumerable<Edge> Edges
    {
        get;
    }

    static Polygon? Intersection(Primitive first, Primitive second)
    {
        var vertices = new List<Vector2>();

        foreach (var t_edge in first.Edges)
            foreach (var r_edge in second.Edges)
            {
                var pt = t_edge.Intersection(r_edge);
                if (pt.HasValue)
                    vertices.Add(pt.Value);
            }

        foreach (var vertex in second.Vertices)
            if (first.Contains(vertex))
                vertices.Add(vertex);

        foreach (var vertex in first.Vertices)
            if (second.Contains(vertex))
                vertices.Add(vertex);

        return vertices.Count < 3 ? null : new Polygon(vertices);
    }
}
