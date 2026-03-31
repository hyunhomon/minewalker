using UnityEngine;

public class TileSystemTester : MonoBehaviour
{
    [SerializeField] private TileSystem tileSystem;
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;

    private void Start()
    {
        if (tileSystem == null)
        {
            Debug.LogError("TileSystem이 연결되지 않았습니다.");
            return;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileData data = new TileData
                {
                    position = new Vector2Int(x, y),
                    isMine = false,
                    adjacentMines = Random.Range(0, 4), // 테스트용 숫자
                    state = TileState.Hidden
                };

                tileSystem.RegisterTile(data);
            }
        }

        // 예시로 지뢰 하나 심기 (테스트)
        TileData mineData;
        if (tileSystem.TryGetTile(new Vector2Int(3, 3), out mineData))
        {
            mineData.isMine = true;
            tileSystem.RegisterTile(mineData);
        }
    }
}