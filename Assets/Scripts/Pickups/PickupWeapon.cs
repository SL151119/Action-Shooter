using UnityEngine;

public class PickupWeapon : Interactable
{
    [SerializeField] private Weapon_Data _weaponData;
    [SerializeField] private Weapon _weapon;

    [SerializeField] private BackupWeaponModel[] _models;

    private bool _isOldWeapon;

    private void Start()
    {
        if (!_isOldWeapon)
        {
            _weapon = new Weapon(_weaponData);
        }

        SetupGameObject();
    }

    public void SetupPickupWeapon(Weapon weapon, Transform transform)
    {
        _isOldWeapon = true;

        this._weapon = weapon;
        _weaponData = weapon.WeaponData;

        this.transform.position = transform.position + new Vector3(0, 0.75f, 0);
    }

    [ContextMenu("Update Weapon Model")]
    public void SetupGameObject()
    {
        gameObject.name = "PickupWeapon " + _weaponData.weaponType.ToString();
        SetupWeaponModel();
    }

    private void SetupWeaponModel()
    {
        foreach (BackupWeaponModel model in _models)
        {
            model.gameObject.SetActive(false);

            if (model._weaponType == _weaponData.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        _weaponController.PickupWeapon(_weapon);

        ObjectPool.instance.ReturnObject(gameObject);
    }
}
