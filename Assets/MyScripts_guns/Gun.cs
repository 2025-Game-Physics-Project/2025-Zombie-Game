using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    /* 모든 종류의 총들이 공통적으로 갖고 있는 속성들(멤버 변수들) */

    public string gunName;  // 총의 이름.
    public float range;     // 총의 사정 거리.
    public float accuracy;  // 총의 정확도.
    public float fireRate;  // 연사 속도.
    public float reloadTime;// 재장전 속도.

    public int damage;      // 총의 공격력.

    public int reloadBulletCount;   // 총의 재장전 개수.
    public int currentBulletCount;  // 현재 탄창에 남아있는 총알의 개수.
    public int maxBulletCount;      // 총알을 최대 몇 개까지 소유할 수 있는지. 
    public int carryBulletCount;    // 현재 소유하고 있는 총알의 총 개수.

    public float retroActionForce;  // 반동 세기. (position 조정으로 사용)

    public ParticleSystem muzzleFlash;  // 화염구 이펙트 재생을 담당할 파티클 시스템 컴포넌트
    public AudioClip fire_Sound;    // 총 발사 소리 오디오 클립
    public AudioClip reload_Sound;    // 총 재장전 소리 오디오 클립
}