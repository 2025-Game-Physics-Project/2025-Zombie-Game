using UnityEngine;

public class Gun : MonoBehaviour
{
    public Camera cam;          // 레이 쏠 카메라 (플레이어 카메라)
    public float range = 50f;   // 사거리
    public int damage = 1;      // 한 발 데미지

    public KeyCode fireKey = KeyCode.Mouse0; // 왼쪽 클릭

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(fireKey))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (cam == null)
        {
            Debug.LogWarning("Gun: Camera not assigned.");
            return;
        }

        // 화면 중앙에서 레이 쏘기
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hitInfo, range))
        {
            Debug.Log($"Hit: {hitInfo.collider.name}");

            // 맞은 오브젝트에서 ZombieHealth 찾기 (자식/부모까지)
            ZombieHealth zh = hitInfo.collider.GetComponent<ZombieHealth>();
            if (zh == null)
                zh = hitInfo.collider.GetComponentInParent<ZombieHealth>();

            if (zh != null)
            {
                zh.TakeDamage(damage);
            }

            // TODO: 나중에 피 이펙트, 총구 섬광, 사운드 등 추가
        }
    }
}
