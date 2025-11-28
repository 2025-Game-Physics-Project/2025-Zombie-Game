using System;
using System.Collections;
using NUnit.Framework;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;

public class FpsController : MonoBehaviour
{
    public Rigidbody bodyRb;        // PlayerBody에 붙은 RigidBody

    public GameObject rightArm;
    Quaternion rightArmBaseRot;
    public GameObject leftArm;

    public Transform viewRig;       // ViewRig (카메라/팔 들어있는 오브젝트)
    public Transform bodyPivot;
    public float moveSpeed = 0f;
    public float mouseSensitivity = 3f;


    float yaw;
    float pitch;

    public Animator armsAnimator;

    public float defaultSpeed = 5f;
    public float realRunSpeed = 8f;
    public float runSpeed = 8f;
    public float runAcceleration = 20f;
    public float runFriction = 18f;

    public float jumpForce = 7f;
    public LayerMask groundMask;
    private float groundCheckDistance = 0.15f;

    private bool isRun = false;
    private bool isGrounded = false;
    private bool isJumpDelay = false;
    private bool canShoot = true;

    Vector3 inputDir;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rightArmBaseRot = rightArm.transform.localRotation;
    }

    IEnumerator JumpDelay()
    {
        isJumpDelay = true;
        yield return new WaitForSeconds(0.5f);
        isJumpDelay = false;
        Console.WriteLine("점프 딜레이 끝남");
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        inputDir = new Vector3(h, 0, v).normalized;


        if (inputDir.Equals(Vector3.zero))
        {
            armsAnimator.SetBool("isMove", false); // 애니메이션 파라미터 isMove를 false로 줌.
            isRun = false;
        }
        else
        {
            armsAnimator.SetBool("isMove", true); // 애니메이션 파라미터 isMove를 true로 줌.
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) // 왼쪽 시프트를 누르면 달리기
        {
            isRun = !isRun;
        }

        if (isRun)
        {
            runSpeed = realRunSpeed;
            armsAnimator.speed = 1.8f;

            if (armsAnimator.GetBool("isHoldPistol") && !armsAnimator.IsInTransition(0)) // 피스톨을 들고 있을때 달리면 총을 든 채로 달림. 이때는 총을 못 쏘게 할 거 같음.
            {
                AnimatorStateInfo stateInfo = armsAnimator.GetCurrentAnimatorStateInfo(0);
                if (!stateInfo.IsName("Move"))
                    armsAnimator.CrossFade("Move", 1f);
            }
        }
        else
        {
            runSpeed = defaultSpeed;
            armsAnimator.speed = 1f;
        }

        // 총 쏘는 조건
        if (armsAnimator.GetBool("isHoldPistol"))
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }

        // 총 쏘기
        if (canShoot && Input.GetMouseButtonDown(0))
        {
            armsAnimator.CrossFadeInFixedTime(Animator.StringToHash("PistolShoot"), 0.05f, 0, 0f);
        }


        // 마우스 회전
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            bodyRb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isGrounded = false;
            StartCoroutine(JumpDelay());
        }

    }
    void FixedUpdate()
    {
        if (!isJumpDelay)
        {
            isGrounded = Physics.Raycast(bodyPivot.position + Vector3.up * 0.05f, Vector3.down,
                                        groundCheckDistance, groundMask);
        }

        Vector3 worldInput = Quaternion.Euler(0, yaw, 0) * inputDir;

        Vector3 targetVel = Vector3.zero;

        if (worldInput.magnitude > 0)
            targetVel = worldInput * runSpeed;

        Vector3 currentVel = bodyRb.linearVelocity;
        Vector3 horizontalVel = new Vector3(currentVel.x, 0, currentVel.z);

        // 부드럽게 가감속
        horizontalVel = Vector3.MoveTowards(horizontalVel, targetVel, runAcceleration * Time.fixedDeltaTime);

        float gravity = Physics.gravity.y;
        if (!isGrounded)
        {
            currentVel.y += gravity * Time.fixedDeltaTime; // 프레임당 중력 적용
        }

        bodyRb.linearVelocity = new Vector3(horizontalVel.x, currentVel.y, horizontalVel.z);

        if (isGrounded && bodyRb.linearVelocity.y < 0)
            bodyRb.linearVelocity = new Vector3(bodyRb.linearVelocity.x, -0.2f, bodyRb.linearVelocity.z);

    }

    void LateUpdate()
    {
        // 카메라 리그를 몸 위치에 붙이기
        viewRig.position = bodyPivot.position;
        viewRig.rotation = Quaternion.Euler(0, yaw, 0);

        // 상하 회전은 카메라 팔쪽에만 적용
        foreach (var cam in viewRig.GetComponentsInChildren<Camera>())
        {
            cam.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }

        // 팔 에임 보정
        if (armsAnimator.GetBool("isHoldPistol") && !isRun)
        {
            Quaternion pitchRot = Quaternion.AngleAxis(pitch -90f + rightArm.transform.localEulerAngles.x, Vector3.right);

            Quaternion target = pitchRot * rightArmBaseRot;
            rightArm.transform.localRotation = target;
        }
    }

    void OnGUI()
    {
        //GUI.TextField(new Rect(0, 0, 100, 30), isGrounded.ToString()); //디버그용. 지금은 지움.
    }
}
