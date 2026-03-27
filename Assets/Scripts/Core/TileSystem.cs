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

    private readonly Dictionary<Vector2Int, TileData> _tiles = new();

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

    /// <summary>
    /// 타일을 공개 상태로 전환합니다.
    /// 지뢰면 Exploded, 아니면 Revealed로 변경됩니다.
    /// </summary>
    /// <returns>지뢰 여부</returns>
    public bool RevealTile(Vector2Int position)
    {
        if (!_tiles.TryGetValue(position, out TileData data))
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
        if (!_tiles.TryGetValue(position, out TileData data))
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
        hiddenTilemap.SetTile((Vector3Int)position, null);
        revealedTilemap.SetTile((Vector3Int)position, null);
    }

    public IReadOnlyDictionary<Vector2Int, TileData> GetAllTiles() => _tiles;

    /// <summary>
    /// 타일을 공개하고, adjacentMines가 0이면 인접 빈 타일을 BFS로 연쇄 공개합니다.
    /// </summary>
    /// <returns>지뢰를 밟았으면 true</returns>
    public bool RevealConnectedEmpty(Vector2Int startPosition)
    {
        if (!_tiles.TryGetValue(startPosition, out TileData startData))
            return false;

        if (startData.isMine)
        {
            RevealTile(startPosition);
            return true;
        }

        var queue = new Queue<Vector2Int>();
        var visited = new HashSet<Vector2Int>();

        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();

            if (!_tiles.TryGetValue(pos, out TileData data) || data.state != TileState.Hidden)
                continue;

            RevealTile(pos);

            if (data.adjacentMines != 0)
                continue;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    var neighbor = new Vector2Int(pos.x + dx, pos.y + dy);
                    if (!visited.Contains(neighbor) && _tiles.ContainsKey(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        return false;
    }

    private void RefreshVisual(Vector2Int position)
    {
        if (!_tiles.TryGetValue(position, out TileData data))
            return;

        var pos3 = (Vector3Int)position;

        switch (data.state)
        {
            case TileState.Hidden:
                hiddenTilemap.SetTile(pos3, hiddenTile);
                revealedTilemap.SetTile(pos3, null);
                break;

            case TileState.Revealed:
                hiddenTilemap.SetTile(pos3, null);
                int index = Mathf.Clamp(data.adjacentMines, 0, 8);
                revealedTilemap.SetTile(pos3, numberTiles != null && numberTiles.Length > index
                    ? numberTiles[index]
                    : revealedTile);
                break;

            case TileState.Exploded:
                hiddenTilemap.SetTile(pos3, null);
                revealedTilemap.SetTile(pos3, explodedTile);
                break;
        }
    }
}
