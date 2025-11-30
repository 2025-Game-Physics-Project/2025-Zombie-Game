using UnityEngine;
using DG.Tweening;

public class CameraRunBob : MonoBehaviour
{
    public Camera cam;

    [Header("Tilt Settings")]
    public float tiltSpeed = 10f;    // 흔들리는 속도
    public float tiltAmount = 2f;    // 좌우 기울기 각도

    [Header("FOV Settings")]
    public float normalFOV = 60f;
    public float runFOV = 70f;

    float defaultZRot;
    bool isRunning = false;

    void Start()
    {
        defaultZRot = transform.localEulerAngles.z;
        cam.fieldOfView = normalFOV;
    }

    void Update()
    {
        if (isRunning)
        {
            float tilt = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
            transform.localRotation = Quaternion.Euler(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                defaultZRot + tilt
            );
        }
        else
        {
            // 부드럽게 원래 회전으로 돌아오기
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                Quaternion.Euler(
                    transform.localEulerAngles.x,
                    transform.localEulerAngles.y,
                    defaultZRot
                ),
                Time.deltaTime * 10f
            );
        }
    }

    public void SetRunning(bool value)
    {
        isRunning = value;

        if (isRunning)
            cam.DOFieldOfView(runFOV, 0.5f);
        else
            cam.DOFieldOfView(normalFOV, 0.25f);
    }
}
