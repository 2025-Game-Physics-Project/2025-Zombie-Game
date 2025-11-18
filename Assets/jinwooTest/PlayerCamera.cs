using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Inspector에서 조절 가능한 이동 속도 변수
    public float moveSpeed = 5.0f;

    void Update()
    {
        // 1. 키보드 입력 값 (Input) 가져오기

        // Horizontal: A(-1) 또는 D(1) 키 입력
        float horizontalInput = Input.GetAxis("Horizontal");

        // Vertical: W(1) 또는 S(-1) 키 입력
        float verticalInput = Input.GetAxis("Vertical");

        // 2. 이동 벡터 (Vector) 계산

        // 전후좌우 이동 벡터 계산. Vector3.forward는 카메라의 앞 방향, Vector3.right는 카메라의 오른쪽 방향을 나타냅니다.
        Vector3 movement = transform.forward * verticalInput + transform.right * horizontalInput;

        // 3. 시간 및 속도 적용하여 실제 이동

        // Time.deltaTime을 곱하여 프레임 속도에 관계없이 일정한 속도로 이동하게 합니다.
        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}
