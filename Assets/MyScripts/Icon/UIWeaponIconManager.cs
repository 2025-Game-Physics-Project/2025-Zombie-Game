using UnityEngine;

public class UIWeaponIconManager : MonoBehaviour
{
    public GunIconAnimator pistolIcon;
    public GunIconAnimator rifleIcon;

    public void ShowPistol()
    {
        pistolIcon.Activate();
        rifleIcon.Deactivate();
    }

    public void ShowRifle()
    {
        pistolIcon.Deactivate();
        rifleIcon.Activate();
    }
    public void DeactivateAll()
    {
        pistolIcon.Deactivate();
        rifleIcon.Deactivate();
    }
}
