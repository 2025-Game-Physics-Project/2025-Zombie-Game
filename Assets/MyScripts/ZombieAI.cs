using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform player; //플레이어. 씬에서 드래그.

    public float detectionRange = 10f; //이 거리 안에 들어가면 추격 시작
    public float attackRange = 2f; //이 거리 안에 들어가면 공격

    public float moveSpeed = 5f;        // 걷는 속도
    public float rotationSpeed = 8f;    // 플레이어 방향으로 도는 속도

    private Animator animator;

    public int attackDamage = 1; //공격용 데미지. 플레이어 체력은 3. 공격 데미지 1.
    private PlayerHealth playerHealth; //플레이어 체력 변수.
    private ZombieHealth zombieHealth; //좀비 체력 변수. hp 0일시 로직 중단 위함.
    private AudioClip audioClip;



    private void Awake() //물체 생성되는 Awake때 애니메이터 할당
    {
        animator = GetComponent<Animator>();
        playerHealth = player.GetComponent<PlayerHealth>(); //플레이어 체력 가져오기. 다른 씬에서도 활용할 경우 null 체크 추가.
        zombieHealth = GetComponent<ZombieHealth>(); //좀비 체력 가져오기. hp 0일시 로직 중단 위함.
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
void Update()
{
    if (player == null) return;
    if (zombieHealth != null && zombieHealth.IsDead) return;

    float distance = Vector3.Distance(transform.position, player.position);

    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    bool isCurrentlyAttacking =
        stateInfo.IsName("zombie_attack") && stateInfo.normalizedTime < 1.0f;

    if (isCurrentlyAttacking)
        return;

    // === 1) 시야 없으면 무조건 Idle ===
    if (!HasLineOfSight())
    {
        SetIdle();
        return;
    }

    // === 2) 시야가 있을 때만 판단 ===
    if (distance <= attackRange)
    {
        SetAttack();
    }
    else if (distance <= detectionRange)
    {
        SetChase();
    }
    else
    {
        SetIdle();
    }
}

    private void SetIdle()
    {
        animator.SetBool("IsWalking", false);
        // 이동 X
    }

    private void SetChase()
    {
        animator.SetBool("IsWalking", true);

        // 플레이어 방향 계산
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        dir.Normalize();

        // 회전
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        // ==== 여기부터 레이캐스트로 벽뚫 방지 ====
        float step = moveSpeed * Time.deltaTime;
        float rayDistance = step + 0.2f; // 살짝 여유

        // 좀비 중심에서 약간 위쪽에서 레이 쏘기 (바닥 충돌 방지)
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, rayDistance))
        {
            // 벽, 장애물, 그 외 모든 콜라이더 막기
            if (!hit.collider.CompareTag("Player"))
            {
                // 충돌물이 있음 → 이동 금지
                Debug.DrawLine(rayOrigin, hit.point, Color.red);
                return;
            }
        }

        // 이동 가능 → 실제 이동
        transform.position += dir * step;

        Debug.DrawRay(rayOrigin, dir * rayDistance, Color.green);
        /*
        animator.SetBool("IsWalking", true);

        // 플레이어 방향 계산
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;               // 위/아래는 무시 (바닥에서만 움직이게)
        dir.Normalize();

        // 플레이어 쪽으로 천천히 회전
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );

        // 플레이어 방향으로 이동
        transform.position += dir * moveSpeed * Time.deltaTime;*/
    }

    public float attackCooldown = 1.5f; //공격 쿨타임.
    private float lastAttackTime = -999f; //마지막 공격 시간

    private void SetAttack()
    {
        animator.SetBool("IsWalking", false);

        // 1) 현재 공격 애니가 재생 중인지 확인
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isCurrentlyAttacking =
            stateInfo.IsName("zombie_attack") && stateInfo.normalizedTime < 1.0f;

        if (isCurrentlyAttacking)
        {
            // 이미 공격 중이면 다시 트리거를 쏘지 않음
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // 시야(Line of Sight)가 없으면 추격 중단
    bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 dir = (player.position - transform.position).normalized;

        float dist = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist))
        {
            // Player를 직접 보지 못하면 false
            return hit.collider.CompareTag("Player");
        }

        return true;
}

    // 애니메이션 이벤트에서 호출할 공격용 함수.
    public void OnAttackHit()
    {
        if (playerHealth == null) return;

        // 좀비의 가슴 높이에서 앞으로 레이 쏘기
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 dir = (player.position - transform.position).normalized;

        float rayDistance = attackRange; // 공격 범위만큼

        if (Physics.Raycast(origin, dir, out RaycastHit hit, rayDistance))
        {
            // Player에 맞았을 때만 데미지 처리
            if (hit.collider.CompareTag("Player"))
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("Zombie hit player! (Raycast)");
                PlayBite();
            }
            else
            {
                Debug.Log("Attack blocked by " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("Attack missed (no hit).");
        }

        // 디버그용 레이
        Debug.DrawRay(origin, dir * rayDistance, Color.red, 0.2f);
        /*
        if (playerHealth == null) return;

        // 아직 공격 거리 안에 있는지 확인 (플레이어가 뒤로 빠져서 회피 가능하므로)
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            // 너무 멀어졌으면 미스 처리 로그. 디버그용
            Debug.Log("Attack missed (player too far).");
            return;
        }

        playerHealth.TakeDamage(attackDamage);
        Debug.Log("Zombie hit player!");

        PlayBite();*/

    }

    public void PlayBite()
    {
        var audio = GetComponent<AudioSource>();
        audio.Play();
    }

}
