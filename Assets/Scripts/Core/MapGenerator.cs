using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 지뢰찾기 맵 그리드를 생성하고 TileSystem에 등록합니다.
/// 게임 시작 시 카메라 화면을 꽉 채우는 크기로 생성되며, 플레이어 스폰 지점(중앙)에는 지뢰를 배치하지 않습니다.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TileSystem tileSystem;

    [Header("Grid Settings")]
    [SerializeField] [Range(0f, 1f)] private float mineDensity = 0.15f;
    [SerializeField] private int padding = 2; // 화면 경계 밖으로 여유 타일 수

    // 생성된 그리드의 중앙 타일 좌표 (플레이어 스폰 지점)
    public Vector2Int SpawnTile { get; private set; }

    
    private void Start()
    {
        GenerateMap();
    }

    /// <summary>
    /// 카메라 화면을 꽉 채우는 맵을 생성합니다.
    /// 중앙이 플레이어 스폰 지점이며, 해당 칸은 지뢰가 없습니다.
    /// </summary>
    public void GenerateMap()
    {
        var cam = Camera.main;
        float orthoSize = cam != null ? cam.orthographicSize : 5f;
        float aspect = cam != null ? cam.aspect : (16f / 9f);

        int halfH = Mathf.CeilToInt(orthoSize) + padding;
        int halfW = Mathf.CeilToInt(orthoSize * aspect) + padding;

        int width  = halfW * 2 + 1;
        int height = halfH * 2 + 1;

        // 그리드를 (0,0) 중심으로 배치
        var originOffset = new Vector2Int(-halfW, -halfH);
        SpawnTile = Vector2Int.zero;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = new Vector2Int(originOffset.x + x, originOffset.y + y);
                bool isMine = Random.value < mineDensity && pos != SpawnTile;

                tileSystem.RegisterTile(new TileData
                {
                    position = pos,
                    isMine = isMine,
                    adjacentMines = 0,
                    state = TileState.Hidden
                });
            }
        }

        CalculateAdjacentMines(originOffset, width, height);
    }

    private void CalculateAdjacentMines(Vector2Int originOffset, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = new Vector2Int(originOffset.x + x, originOffset.y + y);

                if (!tileSystem.TryGetTile(pos, out TileData data) || data.isMine)
                    continue;

                int count = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        var neighbor = new Vector2Int(pos.x + dx, pos.y + dy);
                        if (tileSystem.TryGetTile(neighbor, out TileData neighborData) && neighborData.isMine)
                            count++;
                    }
                }

                data.adjacentMines = count;
                tileSystem.RegisterTile(data);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug: Validate Mine Density")]
    private void DebugValidateMineDensity()
    {
        var allTiles = tileSystem.GetAllTiles();
        if (allTiles.Count == 0)
        {
            Debug.LogWarning("[MapGenerator] 타일이 없습니다. GenerateMap()을 먼저 호출하세요.");
            return;
        }

        int total = allTiles.Count;
        int mineCount = 0;
        foreach (var tile in allTiles.Values)
            if (tile.isMine) mineCount++;

        float actual = (float)mineCount / total;
        Debug.Log($"[MapGenerator] 총 타일: {total} | 지뢰: {mineCount} | 실제 밀도: {actual:P1}");
    }
#endif
}
