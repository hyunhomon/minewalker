using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileState
{
    Hidden,
    Revealed,
    Exploded
}

public struct TileData
{
    public Vector2Int position;
    public bool isMine;
    public int adjacentMines;
    public TileState state;
}

public class TileSystem : MonoBehaviour
{
    [Header("Tilemap References")]
    [SerializeField] private Tilemap hiddenTilemap;
    [SerializeField] private Tilemap revealedTilemap;

    [Header("Tile Assets")]
    [SerializeField] private TileBase hiddenTile;
    [SerializeField] private TileBase revealedTile;
    [SerializeField] private TileBase explodedTile;
    [SerializeField] private TileBase[] numberTiles; // index 0~8

    private readonly Dictionary<Vector2Int, TileData> _tiles = new Dictionary<Vector2Int, TileData>();

    public void RegisterTile(TileData data)
    {
        _tiles[data.position] = data;
        RefreshVisual(data.position);
    }

    public bool TryGetTile(Vector2Int position, out TileData data)
    {
        return _tiles.TryGetValue(position, out data);
    }

    public bool HasTile(Vector2Int position)
    {
        return _tiles.ContainsKey(position);
    }

    public bool RevealTile(Vector2Int position)
    {
        TileData data;

        if (!_tiles.TryGetValue(position, out data))
            return false;

        if (data.state != TileState.Hidden)
            return data.isMine && data.state == TileState.Exploded;

        data.state = data.isMine ? TileState.Exploded : TileState.Revealed;
        _tiles[position] = data;
        RefreshVisual(position);

        return data.isMine;
    }

    public void SetState(Vector2Int position, TileState state)
    {
        TileData data;

        if (!_tiles.TryGetValue(position, out data))
            return;

        data.state = state;
        _tiles[position] = data;
        RefreshVisual(position);
    }

    public void RemoveTile(Vector2Int position)
    {
        if (!_tiles.ContainsKey(position))
            return;

        _tiles.Remove(position);
        hiddenTilemap.SetTile(new Vector3Int(position.x, position.y, 0), null);
        revealedTilemap.SetTile(new Vector3Int(position.x, position.y, 0), null);
    }

    public IReadOnlyDictionary<Vector2Int, TileData> GetAllTiles()
    {
        return _tiles;
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        Vector3Int cell = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        return hiddenTilemap.GetCellCenterWorld(cell);
    }

    private void RefreshVisual(Vector2Int position)
    {
        TileData data;

        if (!_tiles.TryGetValue(position, out data))
            return;

        Vector3Int pos3 = new Vector3Int(position.x, position.y, 0);

        switch (data.state)
        {
            case TileState.Hidden:
                hiddenTilemap.SetTile(pos3, hiddenTile);
                revealedTilemap.SetTile(pos3, null);
                break;

            case TileState.Revealed:
                hiddenTilemap.SetTile(pos3, null);

                int index = Mathf.Clamp(data.adjacentMines, 0, 8);

                if (numberTiles != null && numberTiles.Length > index && numberTiles[index] != null)
                    revealedTilemap.SetTile(pos3, numberTiles[index]);
                else
                    revealedTilemap.SetTile(pos3, revealedTile);

                break;

            case TileState.Exploded:
                hiddenTilemap.SetTile(pos3, null);
                revealedTilemap.SetTile(pos3, explodedTile);
                break;
        }
    }
}