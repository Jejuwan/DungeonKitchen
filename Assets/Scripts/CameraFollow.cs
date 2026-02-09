using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 10f, -10f);
    public float followSpeed = 10f;
    public Vector3 fixedEuler = new Vector3(45f, 45f, 0f);

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // 쿼터뷰 각도를 고정해 좌우 이동 시 기울어짐 방지
        transform.rotation = Quaternion.Euler(fixedEuler);
    }
}



