using UnityEngine;
using UnityEngine.Tilemaps;

public class InfinityMapLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TileSystem tileSystem;
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mineDensity = 0.15f;
    [SerializeField] private int viewWidth = 30;  
    [SerializeField] private int viewHeight = 40;

    private int _leftBoundary;
    private int _rightBoundary;

    void Start()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        int currentCamPos = Mathf.RoundToInt(cameraTransform.position.x);
        _leftBoundary = currentCamPos - (viewWidth / 2);
        _rightBoundary = currentCamPos + (viewWidth / 2);
    }

    void Update()
    {
        HandleInput();
        CheckBoundaries();
    }

    // 카메라 이동
    void HandleInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            cameraTransform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }
    
    // 카메라 좌표 계산 및 줄 추가
    void CheckBoundaries()
    {
        float offset = 5f;
        float removeBuffer = 10f;
        float camRightEdge = cameraTransform.position.x + (viewWidth / 2f) + offset;
        float camLeftEdge = cameraTransform.position.x - (viewWidth / 2f) - removeBuffer;

        if (camRightEdge > _rightBoundary)
        {
            _rightBoundary++;
            GenerateColumn(_rightBoundary);
        }

        if (_leftBoundary < camLeftEdge)
        {
            RemoveColumn(_leftBoundary);
            _leftBoundary++;
        }
    }
    
    // 새로 타일 한 줄씩 생성 및 지뢰 배치
    void GenerateColumn(int x)
    {
        for (int y = -viewHeight / 2; y <= viewHeight / 2; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (tileSystem.HasTile(pos)) continue;

            bool isMine = Random.value < mineDensity;
            tileSystem.RegisterTile(new TileData
            {
                position = pos,
                isMine = isMine,
                state = TileState.Hidden
            });
        }
        UpdateColumnNumbers(x);
        UpdateColumnNumbers(x - 1);
    }
    
    // 화면에서 벗어난 타일 삭제
    void RemoveColumn(int x)
    {
        for (int y = -viewHeight / 2; y <= viewHeight / 2; y++)
        {
            tileSystem.RemoveTile(new Vector2Int(x, y));
        }
    }
    
    // 주변 지뢰 개수 재계산 및 업데이트
    void UpdateColumnNumbers(int x)
    {
        for (int y = -viewHeight / 2; y <= viewHeight / 2; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (!tileSystem.TryGetTile(pos, out TileData data) || data.isMine) continue;

            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    if (tileSystem.TryGetTile(new Vector2Int(pos.x + dx, pos.y + dy), out TileData n) && n.isMine)
                        count++;
                }
            }
            // 지뢰 계산 결과 데이터에 반영 및 재등록
            data.adjacentMines = count;
            tileSystem.RegisterTile(data);
        }
    }
    
}