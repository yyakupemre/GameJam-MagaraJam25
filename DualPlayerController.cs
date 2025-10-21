using UnityEngine;

public class DualPlayerController : MonoBehaviour
{
    [Header("Players")]
    public Rigidbody2D topPlayer;
    public Rigidbody2D bottomPlayer;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    [Range(0.05f, 2f)] public float rayLength = 0.6f;
    [Range(2, 7)]      public int rayCount   = 3;
    [Range(0f, 0.3f)]  public float skinWidth = 0.05f;
    public Color rayHitColor   = Color.green;
    public Color rayMissColor  = Color.yellow;

    private bool topGrounded, bottomGrounded;
    private float moveInput;

    /* -------------------------------------------------------------- */
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (topGrounded)    Jump(topPlayer);
            if (bottomGrounded) Jump(bottomPlayer);
        }
    }

    void FixedUpdate()
    {
        topGrounded    = IsGrounded(topPlayer);
        bottomGrounded = IsGrounded(bottomPlayer);

        Vector2 v = new Vector2(moveInput * moveSpeed, 0f);
        if (topPlayer != null)
            topPlayer.linearVelocity = new Vector2(v.x, topPlayer.linearVelocity.y);
        if (bottomPlayer != null)
            bottomPlayer.linearVelocity = new Vector2(v.x, bottomPlayer.linearVelocity.y);
    }

    /* -------------------------------------------------------------- */
    bool IsGrounded(Rigidbody2D rb)
    {
        if (rb == null) return false;
        Collider2D col = rb.GetComponent<Collider2D>();
        if (col == null) return false;

        Vector2 dir = rb.gravityScale > 0 ? Vector2.down : Vector2.up;
        Bounds b = col.bounds;

        float width = b.extents.x * 2 - skinWidth * 2;
        float step  = rayCount <= 1 ? 0 : width / (rayCount - 1);
        Vector2 start = (Vector2)b.center + dir * (b.extents.y + skinWidth);

        for (int i = 0; i < rayCount; i++)
        {
            Vector2 origin = start + Vector2.left * (width * 0.5f - i * step);
            if (Physics2D.Raycast(origin, dir, rayLength, groundLayer))
                return true;
        }
        return false;
    }

    void Jump(Rigidbody2D rb)
    {
        if (rb == null) return;
        Vector2 jumpDir = -(rb.gravityScale * Physics2D.gravity).normalized;
        rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);
    }

    /* -------------------------------------------------------------- */
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        DrawRays(topPlayer, topGrounded);
        DrawRays(bottomPlayer, bottomGrounded);
    }

    void DrawRays(Rigidbody2D rb, bool grounded)
    {
        if (rb == null) return;
        Collider2D col = rb.GetComponent<Collider2D>();
        if (col == null) return;

        Vector2 dir = rb.gravityScale > 0 ? Vector2.down : Vector2.up;
        Bounds b = col.bounds;

        float width = b.extents.x * 2 - skinWidth * 2;
        float step  = rayCount <= 1 ? 0 : width / (rayCount - 1);
        Vector2 start = (Vector2)b.center + dir * (b.extents.y + skinWidth);

        Gizmos.color = grounded ? rayHitColor : rayMissColor;
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 origin = start + Vector2.left * (width * 0.5f - i * step);
            Gizmos.DrawLine(origin, origin + dir * rayLength);
        }
    }
}
