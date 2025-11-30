using System.Collections;
using UnityEngine;
using TMPro;

public class GunController : MonoBehaviour
{
    [SerializeField] private Gun currentGun;
    private AudioSource playerAudioSource;

    // 🚀 Trail Renderer 프리팹
    [Header("Bullet")]
    [SerializeField] private TrailRenderer bulletTrailPrefab; // GameObject -> TrailRenderer로 타입 변경
    private const float BULLET_SPEED = 200f; // 총알이 날아가는 속도 (기존 로직 유지)

    // 상태 변수
    private float currentFireRate;
    private bool isReload = false;

    // 컴포넌트 및 위치
    [SerializeField] private Vector3 originPos; // 총의 기본 위치 (반동 복귀 위치)
    private AudioSource audioSource;
    [SerializeField] private Transform bulletSpawnPos;       // 총구 위치

    [Header("Impact Effect")]
    [SerializeField] private GameObject impactEffectPrefab; // 충돌 파티클과 총알 구멍을 포함하는 통합 프리팹
    [SerializeField] private TextMeshProUGUI text_BulletCount;

    // ✨ 추가: 카메라 참조 (조준점 발사에 필수)
    private Camera mainCam;
    private WeaponManager weaponManager;

    private Coroutine retroCoroutine;
    private Coroutine reloadCoroutine;

    private Animator armsAnimator;
    private FpsController fpsController;

    void Awake()
    {
        text_BulletCount = GameObject.Find("Bulletcount")?.GetComponent<TextMeshProUGUI>();
        armsAnimator = GameObject.Find("Arms")?.GetComponent<Animator>();
        fpsController = GameObject.Find("PlayerMesh")?.GetComponent<FpsController>();
        playerAudioSource = GameObject.Find("FpsPlayer")?.GetComponent<AudioSource>();
        weaponManager = GameObject.Find("ArmsCamera")?.GetComponent<WeaponManager>();
        audioSource = playerAudioSource;
    }

    void Start()
    {
        // ✨ 메인 카메라 할당 (Start에서 한 번만 호출)
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("씬에 태그가 MainCamera인 카메라가 없습니다!");
        }

        audioSource = playerAudioSource;
        UpdateBulletUI();
    }

    void Update()
    {
        GunFireRateCalc();
        TryFire();
        TryReload();
    }

    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime;
    }

    private void TryFire()
    {
        if (currentGun.gunName == "pistol") // 피스톨일 때는 버튼을 꾹누르는 걸로 총알이 발사 안되고 눌렀을때만 작동되게 하기 위해 이렇게 코딩함.
        {
            if (Input.GetButtonDown("Fire1") && currentFireRate <= 0 && !isReload && !fpsController.isRun)
                Fire();
            return;
        }
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload && !fpsController.isRun)
            Fire();
    }

    private void Fire()
    {
        if (currentGun.currentBulletCount <= 0)
        {
            if (reloadCoroutine == null)
                reloadCoroutine = StartCoroutine(ReloadCoroutine());
            return;
        }

        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate;
        UpdateBulletUI();

        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();

        armsAnimator.CrossFadeInFixedTime(Animator.StringToHash("PistolShoot"), 0.05f, 0, 0f);

        if (retroCoroutine != null)
            StopCoroutine(retroCoroutine);
        retroCoroutine = StartCoroutine(RetroActionCoroutine());

        DoRaycastShoot();
    }

    private void DoRaycastShoot()
    {
        if (mainCam == null) return;

        RaycastHit hit;
        Vector3 hitPoint;
        Vector3 hitNormal = Vector3.zero;
        bool madeImpact = false;

        // ✨ 1. 화면 중앙 좌표를 계산하여 레이 생성
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = mainCam.ScreenPointToRay(screenCenter);

        // 🔥 2. 카메라 레이로 충돌 지점 찾기 (총알의 목표 지점)
        if (Physics.Raycast(ray, out hit, currentGun.range))
        {
            // Ray가 오브젝트에 맞았을 때: 총알의 목표 지점과 충돌 정보 설정
            madeImpact = true;
            hitPoint = hit.point;
            hitNormal = hit.normal;

            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
                target.TakeDamage(currentGun.damage);

            // =========================================================
            // ✨ 통합된 충돌 효과 및 구멍 생성 로직 (impactEffectPrefab 사용)
            if ((impactEffectPrefab != null) && (hit.collider.gameObject.layer != LayerMask.NameToLayer("Player") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Monster")))
            {
                // impactEffectPrefab을 충돌 지점에 생성하고 노말 방향을 바라보게 설정
                GameObject impact = Instantiate(
                    impactEffectPrefab,
                    hit.point + hit.normal * 0.01f,
                    Quaternion.LookRotation(hit.normal));

                Destroy(impact, 5f);
            }
            // =========================================================
        }
        else
        {
            // Ray가 아무것도 맞추지 못했을 때:
            // 목표 지점을 카메라 레이의 사거리 끝으로 설정
            hitPoint = ray.origin + ray.direction * currentGun.range;
        }

        // 🚀 3. Trail Renderer (총알 궤적) 생성 및 이동 로직 호출
        // 총알은 총구 위치에서 hitPoint를 향해 날아갑니다.
        SpawnTrail(hitPoint, hitNormal, madeImpact);
    }

    // 🚀 새로운 Trail Renderer를 위한 총알 생성 및 이동 로직 (기존 로직 유지)
    private void SpawnTrail(Vector3 hitPoint, Vector3 hitNormal, bool madeImpact)
    {
        if (bulletTrailPrefab == null)
        {
            Debug.LogError("Bullet Trail Prefab이 할당되지 않았습니다. 인스펙터를 확인하세요!");
            return;
        }

        TrailRenderer trail = Instantiate(bulletTrailPrefab, bulletSpawnPos.position, Quaternion.identity);

        StartCoroutine(MoveTrailCoroutine(trail, hitPoint, hitNormal, madeImpact));
    }

    // 🚀 총알 오브젝트를 목표 지점까지 이동시키는 코루틴 (기존 로직 유지)
    private IEnumerator MoveTrailCoroutine(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, bool madeImpact)
    {
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(startPosition, hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BULLET_SPEED * Time.deltaTime;

            yield return null;
        }

        trail.transform.position = hitPoint;
        Destroy(trail.gameObject, trail.time);
    }


    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) &&
            !isReload &&
            currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            if (reloadCoroutine == null)
            {
                reloadCoroutine = StartCoroutine(ReloadCoroutine());
            }
        }
    }

    IEnumerator ReloadCoroutine()
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;
            armsAnimator.SetBool("isReload", isReload);
            PlaySE(currentGun.reload_Sound);

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;
            UpdateBulletUI();
            weaponManager.canChangeGun = false;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            UpdateBulletUI();
            isReload = false;
            weaponManager.canChangeGun = true;
            armsAnimator.SetBool("isReload", isReload);
        }

        reloadCoroutine = null;
    }


    // ✨ 반동 기능 (Position 조정 방식 - 기존 로직 유지)
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);

        // 1. 초기 위치를 originPos로 설정합니다.
        currentGun.transform.localPosition = originPos;

        // 2. 반동 적용 (뒤로 밀림)
        while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
        {
            currentGun.transform.localPosition =
                Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
            yield return null;
        }

        // 3. 원위치로 복귀
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition =
                Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
            yield return null;
        }

        retroCoroutine = null;
    }

    private void PlaySE(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void UpdateBulletUI()
    {
        if (text_BulletCount != null)
            text_BulletCount.text = currentGun.currentBulletCount + "|" + currentGun.carryBulletCount;
    }
}