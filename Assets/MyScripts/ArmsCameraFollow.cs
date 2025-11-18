using UnityEngine;

public class ArmsCameraFollow : MonoBehaviour
{
    public Camera mainCamera;
    public float positionLag = 8f; // 위치 따라오는 속도
    public float rotationLag = 6f; // 회전 따라오는 속도

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // 위치 보간해서 살짝 늦게 따라오게 만들기
        transform.position = Vector3.Lerp(
            transform.position,
            mainCamera.transform.position,
            Time.deltaTime * positionLag
        );

        // 회전 보간해서 고개 돌릴 때 늦게 따라오게 만들기
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            mainCamera.transform.rotation,
            Time.deltaTime * rotationLag
        );
    }
}
