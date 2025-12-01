using UnityEngine;

public class SoundDown : MonoBehaviour
{
    void Awake()
    {
        var obj = GameObject.Find("BgmManager");
        Destroy(obj);
    }
}
