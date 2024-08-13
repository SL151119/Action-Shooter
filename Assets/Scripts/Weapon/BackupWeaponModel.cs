using UnityEngine;

public enum HangType
{
    LowBackHang = 0,
    SideHang = 1,
    BackHag = 2,
}

public class BackupWeaponModel : MonoBehaviour
{
    public WeaponType _weaponType;
    [SerializeField] private HangType _hangType;

    public void Activate(bool activated) => gameObject.SetActive(activated);

    public bool HangTypeIs(HangType hangType)
    {
        return this._hangType == hangType;
    }
}
