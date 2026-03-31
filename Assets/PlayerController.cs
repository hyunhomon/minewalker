using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TileSystem tileSystem;

    [Header("Start Settings")]
    [SerializeField] private Vector2Int startGridPosition = Vector2Int.zero;

    [Header("Move Settings")]
    [SerializeField] private float moveCooldown = 0.15f;

    private Vector2Int _currentGridPosition;
    private float _lastMoveTime;
    private bool _isDead;

    private void Start()
    {
        if (tileSystem == null)
        {
            Debug.LogError("TileSystem이 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        _currentGridPosition = startGridPosition;
        UpdateWorldPosition();

        // 시작 위치가 유효하면 시작 타일 공개
        if (tileSystem.HasTile(_currentGridPosition))
        {
            bool hitMine = tileSystem.RevealTile(_currentGridPosition);

            if (hitMine)
            {
                Die();
            }
        }
        else
        {
            Debug.LogWarning("시작 위치에 타일이 없습니다: " + _currentGridPosition);
        }
    }

    private void Update()
    {
        if (_isDead)
            return;

        if (Time.time - _lastMoveTime < moveCooldown)
            return;

        Vector2Int input = GetMoveInput();

        if (input == Vector2Int.zero)
            return;

        TryMove(input);
    }

    private Vector2Int GetMoveInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            return Vector2Int.up;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            return Vector2Int.down;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            return Vector2Int.left;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            return Vector2Int.right;

        return Vector2Int.zero;
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int nextPosition = _currentGridPosition + direction;

        // 타일이 없는 위치는 이동 불가
        if (!tileSystem.HasTile(nextPosition))
        {
            Debug.Log("이동 불가: 타일이 없는 위치 " + nextPosition);
            return;
        }

        _currentGridPosition = nextPosition;
        UpdateWorldPosition();
        _lastMoveTime = Time.time;

        bool hitMine = tileSystem.RevealTile(_currentGridPosition);

        if (hitMine)
        {
            Die();
        }
    }

    private void UpdateWorldPosition()
    {
        transform.position = tileSystem.GetWorldPosition(_currentGridPosition);
    }

    private void Die()
    {
        _isDead = true;
        Debug.Log("지뢰를 밟았습니다! 플레이어 사망");
    }

    public Vector2Int GetCurrentGridPosition()
    {
        return _currentGridPosition;
    }
}