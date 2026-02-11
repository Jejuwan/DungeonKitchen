using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Quaternion desiredRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 물리 충돌을 자연스럽게 유지하기 위한 권장 설정
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(x, 0f, z).normalized;

        if (moveInput.sqrMagnitude > 0.0001f)
        {
            // 이동 방향을 즉시 바라보도록 회전값 계산
            desiredRotation = Quaternion.LookRotation(moveInput, Vector3.up);
        }
    }

    private void FixedUpdate()
    {
        Vector3 desiredVelocity = moveInput * moveSpeed;

        // 중력 등 Y축 속도는 유지하고, 수평 이동만 제어
        rb.velocity = new Vector3(desiredVelocity.x, rb.velocity.y, desiredVelocity.z);

        if (moveInput.sqrMagnitude > 0.0001f)
        {
            // 물리 업데이트에서 회전을 적용해 덜덜 떨림을 줄임
            rb.MoveRotation(desiredRotation);
        }
    }
}




