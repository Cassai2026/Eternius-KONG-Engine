using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class TopdownPointandclick : MonoBehaviour
{
    private CamTrigger _camTrigger;
    [SerializeField] private float moveSpeed = 1;

    private Vector2 moveDir = Vector2.zero;
    private Vector3 targetPosition; // Position to move towards

    private bool canMove = true; // Control whether the player can move or not
    private bool isTeleporting = false; // Flag to indicate if the player is currently being teleported

    [SerializeField] private Tilemap walkableTilemap; // Reference to the walkable tilemap
    
    void Update()
    {
        if (canMove)
        {
            if (!isTeleporting)
            {
                // Check if the player clicked on the walkable tilemap
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    Vector2Int cellPos = GetClickedCell();
                    if (walkableTilemap.HasTile(new Vector3Int(cellPos.x, cellPos.y, 0)))
                    {
                        targetPosition = walkableTilemap.CellToWorld(new Vector3Int(cellPos.x, cellPos.y, 0));
                    }
                }

                if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
                {
                    Vector2 position = transform.position;
                    moveDir = new Vector3(targetPosition.x - position.x, targetPosition.y - position.y).normalized;
                    transform.position = position + moveDir * (moveSpeed * Time.deltaTime);
                }
            }
        }
    }

    // Method to stop the player from moving
    public void StopMoving()
    {
        canMove = false;
    }

    // Method to allow the player to move
    public void StartMoving()
    {
        canMove = true;
    }

    // Method to indicate that the player is being teleported
    public void StartTeleport()
    {
        isTeleporting = true;
        // Prevent movement during teleportation
        StopMoving();
        // Clear the target position to prevent moving to the old target after teleportation
        targetPosition = transform.position;
    }

    // Method to indicate that the teleportation is complete
    public void EndTeleport()
    {
        isTeleporting = false;
        // Allow movement after a short delay
        Invoke("StartMoving", 0.5f); // Adjust delay time as needed
    }

    // Get the cell position of the clicked point on the tilemap
    private Vector2Int GetClickedCell()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = walkableTilemap.WorldToCell(mousePos);
        return new Vector2Int(cellPos.x, cellPos.y);
    }
}
