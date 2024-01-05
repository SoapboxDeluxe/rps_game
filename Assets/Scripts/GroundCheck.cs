using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    // The object's collider component
    private PolygonCollider2D polyCol;

    // The layer mask for the ground
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    // A flag to indicate whether the object is grounded
    public bool isGrounded = false;
    public bool isOnPlayer = false; 

    void Start() {
        // Get the object's collider component
        polyCol = GetComponent<PolygonCollider2D>();
    }

    void Update() {
        // Calculate the bounds of the collider's bottom edge
        Vector3 bottomLeft = polyCol.bounds.min;
        Vector3 bottomRight = polyCol.bounds.max;
        Vector3 bottomCenter = polyCol.bounds.center; 
        bottomLeft.y = polyCol.bounds.min.y; 
        bottomCenter.y = polyCol.bounds.min.y;
        bottomRight.y = polyCol.bounds.min.y;

        // Cast two rays down to check for ground
        RaycastHit2D hitLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.1f, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.1f, groundLayer);

        // Cast three rays down to check for players
        RaycastHit2D hitPlayerLeft = Physics2D.Raycast(bottomLeft, Vector2.down, 0.5f, playerLayer);
        RaycastHit2D hitPlayerCenter = Physics2D.Raycast(bottomCenter, Vector2.down, 0.5f, playerLayer);
        RaycastHit2D hitPlayerRight = Physics2D.Raycast(bottomRight, Vector2.down, 0.5f, playerLayer);
        
        Debug.DrawRay(bottomLeft, Vector3.down * .5f, Color.green);
        Debug.DrawRay(bottomCenter, Vector3.down * .5f, Color.green);
        Debug.DrawRay(bottomRight, Vector3.down * .5f, Color.green);

        // Set the isGrounded flag based on the raycast results
        isGrounded = hitLeft.collider != null || hitRight.collider != null;
        isOnPlayer = hitPlayerLeft.collider != null || hitPlayerRight.collider != null || hitPlayerCenter.collider != null;
    }
}