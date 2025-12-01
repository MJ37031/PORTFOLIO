using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Joystick joystick;

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        // 이동
        rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);

        // 회전
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.fixedDeltaTime);
        }

        // 애니메이션
        animator.SetFloat("Speed", direction.magnitude);
    }
}
