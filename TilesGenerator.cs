using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TilesGenerator
{
    public static (Mesh TilesMesh, Mesh TilesBumpMesh) TriangulateTiles(IEnumerable<Vector2> tiles, Primitive clipPolygon, Vector3 tileSize)
    {
        var tilesMesh = new Mesh();
        var tilesBumpMesh = new Mesh();

        var tilesBumpVertices = new List<Vector3>();
        var tilesBumpTriangles = new List<int>();
        var tilesBumpUv = new List<Vector2>();

        var tilesVertices = new List<Vector3>();
        var tilesTriangles = new List<int>();
        var tilesUv = new List<Vector2>();

        int tilesBumpTrianglesIndex = 0;
        int tilesTrianglesIndex = 0;

        foreach (var tile in tiles)
        {

            var tilePrimitive = Primitive.Intersection(new Polygon(
                tile,
                tile + Vector2.right * tileSize.x,
                tile + Vector2.up * tileSize.y,
                tile + new Vector2(tileSize.x, tileSize.y)
            ), clipPolygon);

            if (tilePrimitive.HasValue)
            {
                var tileVertices = tilePrimitive.Value.Vertices;
                var tileVerticesCount = tileVertices.Count();

                var tileBumpVertices = new List<Vector3>();
                foreach (var v in tileVertices)
                {
                    tileBumpVertices.Add(v);
                    tileBumpVertices.Add(v);
                }

                tilesBumpVertices.AddRange(tileBumpVertices.Select(it => it + Vector3.forward * tileSize.z * -.5f));
                tilesBumpVertices.AddRange(tileBumpVertices.Select(it => it + Vector3.forward * tileSize.z * .5f));
                var tileBumpVerticesCount = tileBumpVertices.Count;

                tilesBumpUv.AddRange(tileBumpVertices.Select((it, index) => new Vector2(index % 2, 0)));
                tilesBumpUv.AddRange(tileBumpVertices.Select((it, index) => new Vector2(index % 2, 1)));

                for (int i = 0; i < tileBumpVerticesCount - 1; ++i)
                {
                    tilesBumpTriangles.Add(tilesBumpTrianglesIndex + i);
                    tilesBumpTriangles.Add(tilesBumpTrianglesIndex + 1 + i);
                    tilesBumpTriangles.Add(tilesBumpTrianglesIndex + tileBumpVerticesCount + i);

                    tilesBumpTriangles.Add(tilesBumpTrianglesIndex + tileBumpVerticesCount + 1 + i);
                    tilesBumpTriangles.Add(tilesBumpTrianglesIndex + tileBumpVerticesCount + i);
                    tilesBumpTriangles.Add(tilesBumpTrianglesIndex + 1 + i);
                }

                tilesBumpTriangles.Add(tilesBumpTrianglesIndex + tileBumpVerticesCount - 1);
                tilesBumpTriangles.Add(tilesBumpTrianglesIndex);
                tilesBumpTriangles.Add(tilesBumpTrianglesIndex + 2 * tileBumpVerticesCount - 1);

                tilesBumpTriangles.Add(tilesBumpTrianglesIndex + tileBumpVerticesCount);
                tilesBumpTriangles.Add(tilesBumpTrianglesIndex + 2 * tileBumpVerticesCount - 1);
                tilesBumpTriangles.Add(tilesBumpTrianglesIndex);

                tilesBumpTrianglesIndex += tileBumpVerticesCount * 2;

                tilesVertices.AddRange(tileVertices.Select(it => (Vector3) it + Vector3.forward * tileSize.z * -.5f));
                tilesUv.AddRange(tileVertices.Select(it => Vector2.Scale(it - tile, new Vector2(1f / tileSize.x, 1f / tileSize.y))));

                foreach (var triangle in tilePrimitive.Value.TrianglesIndices)
                    tilesTriangles.AddRange(new int[] { triangle.A + tilesTrianglesIndex, triangle.B + tilesTrianglesIndex, triangle.C + tilesTrianglesIndex });

                tilesTrianglesIndex += tileVerticesCount;
            }
        }

        tilesBumpMesh.vertices = tilesBumpVertices.ToArray();
        tilesBumpMesh.triangles = tilesBumpTriangles.ToArray();
        tilesBumpMesh.uv = tilesBumpUv.ToArray();
        tilesBumpMesh.RecalculateNormals();

        tilesMesh.vertices = tilesVertices.ToArray();
        tilesMesh.triangles = tilesTriangles.ToArray();
        tilesMesh.uv = tilesUv.ToArray();
        tilesMesh.RecalculateNormals();

        return (tilesMesh, tilesBumpMesh);
    }

    public static IEnumerable<float> FillLineWithTiles(float begin, float end, float tileSize, float spacing, float offset)
    {
        var length = end - begin;

        //Вычисление нулевой позиции плитки
        var step = tileSize + spacing;
        offset = step - offset % step;

        var position = -(offset % step);
        if (position + tileSize < 0 || Mathf.Approximately(position, -tileSize))
            position += step;

        //Заполнение линии
        for (; position < length; position += step)
            yield return position + begin;
    }


    public static IEnumerable<Vector2> FillPolygonWithTiles(Primitive polygon, Vector2 pivot, Vector2 tileSize, float offset, float spacingX, float spacingY)
    {
        if (tileSize.x <= 0f || tileSize.y <= 0f || spacingX < 0f || spacingY < 0f)
            throw new ArgumentException();

        var verticesY = polygon.Vertices.Select(it => it.y);

        var minY = verticesY.Min();
        var maxY = verticesY.Max();

        var lengthY = maxY - minY;

        var halfLengthY = lengthY * .5f;

        // Получение высоты рядов
        foreach (var h in FillLineWithTiles(minY, maxY, tileSize.y, spacingY, pivot.y - minY))
        {
            var bottom = h;
            var top = bottom + tileSize.y;

            static bool LessEqual(float x, float y) => x < y || Mathf.Approximately(x, y);
            static bool GreaterEqual(float x, float y) => x > y || Mathf.Approximately(x, y);

            // Вычисление x границ рядов

            var left = Mathf.Infinity;
            var right = Mathf.NegativeInfinity;

            foreach (var edge in polygon.Edges)
            {
                var edgeMinY = Mathf.Min(edge.A.y, edge.B.y);
                var edgeMaxY = Mathf.Max(edge.A.y, edge.B.y);

                bool InsideEdge(float y) => GreaterEqual(y, edgeMinY) && LessEqual(y, edgeMaxY);

                Vector2 Intersection(float y)
                {
                    var dir = edge.B - edge.A;
                    return edge.A + ((y - edge.A.y) / dir.y) * dir;
                }

                if (InsideEdge(bottom))
                {
                    var first = Intersection(bottom);
                    left = Mathf.Min(left, first.x);
                    right = Mathf.Max(right, first.x);
                }

                if (InsideEdge(top))
                {
                    var first = Intersection(top);
                    left = Mathf.Min(left, first.x);
                    right = Mathf.Max(right, first.x);
                }
            }

            foreach (var vertex in polygon.Vertices)
            {
                if (GreaterEqual(vertex.y, bottom) && LessEqual(vertex.y, top))
                {
                    left = Mathf.Min(left, vertex.x);
                    right = Mathf.Max(right, vertex.x);
                }
            }

            var length = right - left;

            var halfLength = length * .5f;

            var index = (bottom - pivot.y) / (tileSize.y + spacingY);

            //Заполнение ряда
            foreach (var tile in
               FillLineWithTiles(left, right, tileSize.x, spacingX, index * offset + (-left + pivot.x))
               .Select(x => new Vector2(x, bottom))
            )
                yield return tile;
        }
    }
}
