using UnityEngine;

/// <summary>
/// 마우스 클릭 입력을 받아 TileSystem에 타일 공개를 요청합니다.
/// </summary>
public class InputSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TileSystem tileSystem;
    [SerializeField] private Camera gameCamera;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var cam = gameCamera != null ? gameCamera : Camera.main;
        Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        var tilePos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));

        if (!tileSystem.HasTile(tilePos)) return;

        tileSystem.RevealConnectedEmpty(tilePos);
    }
}
