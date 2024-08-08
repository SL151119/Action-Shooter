using UnityEngine;

public enum EquipWeaponType
{
    SideEquip = 0,
    BackEquip = 1
}

public enum HoldType
{
    CommonHold = 1,
    LowHold = 2,
    HighHold = 3,
}

public class WeaponModel : MonoBehaviour
{
    public WeaponType _weaponType;
    public EquipWeaponType _equipType;
    public HoldType _holdType;

    public Transform _weaponPoint;
    public Transform _leftHandHoldPoint;
    
}
