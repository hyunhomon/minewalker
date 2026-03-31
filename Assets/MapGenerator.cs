using UnityEngine;

/// <summary>
/// 지뢰찾기 맵 그리드를 생성하고 TileSystem에 등록합니다.
/// 거리 기반 지뢰 밀도 스케일링을 적용합니다.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TileSystem tileSystem;

    [Header("Grid Settings")]
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;

    private void Awake()
    {
        if (tileSystem == null)
        {
            Debug.LogError("TileSystem이 연결되지 않았습니다.");
            return;
        }

        GenerateMap(0, Vector2Int.zero);
    }

    /// <summary>
    /// 기본 시작 좌표(0,0) 기준으로 생성
    /// </summary>
    public void GenerateMap(int originDistance = 0)
    {
        GenerateMap(originDistance, Vector2Int.zero);
    }

    /// <summary>
    /// 지정한 거리 기준으로 그리드를 생성합니다.
    /// </summary>
    /// <param name="originDistance">현재 플레이어 거리 (밀도 계산 기준)</param>
    /// <param name="originOffset">그리드 시작 좌표 (Tilemap 기준)</param>
    public void GenerateMap(int originDistance, Vector2Int originOffset)
    {
        float density = GetMineDensity(originDistance);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(originOffset.x + x, originOffset.y + y);
                bool isMine = Random.value < density;

                // 시작 위치는 안전하게
                if (pos == Vector2Int.zero)
                    isMine = false;

                tileSystem.RegisterTile(new TileData
                {
                    position = pos,
                    isMine = isMine,
                    adjacentMines = 0,
                    state = TileState.Hidden
                });
            }
        }

        CalculateAdjacentMines(originOffset);
    }

    private void CalculateAdjacentMines(Vector2Int originOffset)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(originOffset.x + x, originOffset.y + y);

                TileData data;
                if (!tileSystem.TryGetTile(pos, out data) || data.isMine)
                    continue;

                int count = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                            continue;

                        Vector2Int neighbor = new Vector2Int(pos.x + dx, pos.y + dy);
                        TileData neighborData;

                        if (tileSystem.TryGetTile(neighbor, out neighborData) && neighborData.isMine)
                            count++;
                    }
                }

                data.adjacentMines = count;
                tileSystem.RegisterTile(data);
            }
        }
    }

    /// <summary>
    /// 거리에 따른 지뢰 밀도를 반환합니다.
    /// </summary>
    public static float GetMineDensity(int distance)
    {
        if (distance <= 50) return 0.15f;
        if (distance <= 150) return 0.22f;
        if (distance <= 300) return 0.28f;
        return 0.33f;
    }
}