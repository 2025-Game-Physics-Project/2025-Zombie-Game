using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponMount;       // 손 위치 (WeaponMount)
    public Animator armsAnimator;
    public GameObject[] weaponPrefabs;  // 여러 무기 프리팹들
    private GameObject currentWeapon;   // 현재 장착 중 무기
    private int currentIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnequipWeapon(); // 시작 시 아무 무기도 장착하지 않음
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 권총
        {
            armsAnimator.SetBool("isHoldPistol", true);
            armsAnimator.SetBool("isIdle", false);
            EquipWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //armsAnimator.SetBool("isHoldPistol", true);
            //armsAnimator.SetBool("isIdle", false);
            EquipWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            armsAnimator.SetBool("isIdle", true);
            armsAnimator.SetBool("isHoldPistol", false);
            UnequipWeapon();
        }
    }

    public void EquipWeapon(int index)
    {
        if (index == currentIndex) return; // 같은 무기면 무시

        UnequipWeapon();

        if (index < 0 || index >= weaponPrefabs.Length) return;

        // 무기 생성
        GameObject newWeapon = Instantiate(weaponPrefabs[index], weaponMount);
        
        newWeapon.transform.localPosition = Vector3.zero;
        // newWeapon.transform.localRotation = Quaternion.identity;

        currentWeapon = newWeapon;
        currentIndex = index;
    }
    
    public void UnequipWeapon()
    {
        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = null;
        currentIndex = -1;
    }
}
