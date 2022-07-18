using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class App : MonoBehaviour
{
    public struct Parameters
    {
        public float Spacing;

        public float Rotation;

        public float Offset;

        public Parameters(float spacing, float rotation, float offset) => (Spacing, Rotation, Offset) = (spacing, rotation, offset);
    }

    const float RectWidth = 1.5f;

    const float RectHeight = 1;

    const float TileWidth = 0.15f;

    const float TileHeight = 0.074f;

    const float TileLength = 0.0069f;

    [SerializeField] MeshFilter _tilesMeshFilter;
    
    [SerializeField] MeshFilter _tilesBumpMeshFilter;

    public UnityEvent<float> TileArea;

    public void GenerateTiles(Parameters parameters)
    {
        var rotation = Quaternion.Euler(0f, 0f, parameters.Rotation);
        var inverseRotation = Quaternion.Inverse(rotation);


        var rectangle = new Polygon(
            rotation * new Vector2(RectWidth * -.5f, RectHeight * -.5f),
            rotation * new Vector2(RectWidth * .5f, RectHeight * -.5f),
            rotation * new Vector2(RectWidth * -.5f, RectHeight * .5f),
            rotation * new Vector2(RectWidth * .5f, RectHeight * .5f)
        );

        var tiles = TilesGenerator.FillPolygonWithTiles(rectangle, new Vector2(RectWidth * -.5f, RectHeight * -.5f), new Vector2(TileWidth, TileHeight), parameters.Offset, parameters.Spacing, parameters.Spacing);

        var (tilesMesh, tilesBumpMesh) = TilesGenerator.TriangulateTiles(tiles, rectangle, new Vector3(TileWidth, TileHeight, TileLength));

        tilesMesh.vertices = tilesMesh.vertices.Select(it => inverseRotation * it).ToArray();
        tilesBumpMesh.vertices = tilesBumpMesh.vertices.Select(it => inverseRotation * it).ToArray();

        (_tilesMeshFilter.mesh, _tilesBumpMeshFilter.mesh) = (tilesMesh, tilesBumpMesh);

        TileArea.Invoke(tiles.Count() * TileWidth * TileHeight);
    }
}
