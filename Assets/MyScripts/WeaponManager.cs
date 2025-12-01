using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponMount;       // 손 위치 (WeaponMount)
    public Animator armsAnimator;
    public GameObject[] weaponPrefabs;  // 여러 무기 프리팹들
    private GameObject currentWeapon;   // 현재 장착 중 무기
    private float pistolReloadTime;
    private float ripleReloadTime;
    private int currentIndex = -1;
    public bool canChangeGun = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {

        pistolReloadTime = weaponPrefabs[0].GetComponent<Gun>().reloadTime;
        ripleReloadTime = weaponPrefabs[1].GetComponent<Gun>().reloadTime;
    }

    // Update is called once per frame
    void Update()
    {   
        if (armsAnimator.GetBool("isRun") || !canChangeGun) return; // 뛰고 있을 때는 총 전환을 할 수 없도록 함.

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 권총
        {
            armsAnimator.SetBool("isHoldPistol", true);
            armsAnimator.SetBool("isIdle", false);
            int gun_index = 0;
            if (currentIndex != gun_index)
                armsAnimator.Play("Idle", 0, 0f);
            EquipWeapon(gun_index);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            armsAnimator.SetBool("isHoldPistol", true);
            armsAnimator.SetBool("isIdle", false);
            armsAnimator.Play("Idle", 0, 0f);
            int gun_index = 1;
            if (currentIndex != gun_index)
                armsAnimator.Play("Idle", 0, 0f);
            EquipWeapon(gun_index);
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
