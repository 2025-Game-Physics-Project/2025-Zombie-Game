using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform player; //플레이어. 씬에서 드래그.

    public float detectionRange = 10f; //이 거리 안에 들어가면 추격 시작
    public float attackRange = 2f; //이 거리 안에 들어가면 공격

    public float moveSpeed = 2f;        // 걷는 속도
    public float rotationSpeed = 5f;    // 플레이어 방향으로 도는 속도

    private Animator animator;

    public int attackDamage = 1; //공격용 데미지. 플레이어 체력은 3. 공격 데미지 1.
    private PlayerHealth playerHealth; //플레이어 체력 변수.
    private ZombieHealth zombieHealth; //좀비 체력 변수. hp 0일시 로직 중단 위함.

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
        if (player == null) return; // 혹시 플레이어 안 넣었으면 그냥 패스
        if (zombieHealth != null && zombieHealth.IsDead) return; //좀비가 죽었다면 패스. hp 0일시 로직 중단 위함.

        // 1. 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // 1) 지금 공격 애니메이션이 재생 중인지 먼저 체크
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isCurrentlyAttacking =
            stateInfo.IsName("zombie_attack") && stateInfo.normalizedTime < 1.0f;

        // 2) 공격 중이라면, 이동/상태 전환을 잠시 막는다
        if (isCurrentlyAttacking)
        {
            return;
        }

        if (distance <= attackRange)
        {
            // 3단계: 공격 범위 안
            SetAttack();
        }
        else if (distance <= detectionRange)
        {
            // 2단계: 인식 범위 안 (추격)
            SetChase();
        }
        else
        {
            // 1단계: 플레이어 못 봄 (Idle)
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
        transform.position += dir * moveSpeed * Time.deltaTime;
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

    // 애니메이션 이벤트에서 호출할 공격용 함수.
    public void OnAttackHit()
    {
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
    }
}
