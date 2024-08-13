using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Material _highlightMaterial;

    protected MeshRenderer _meshRenderer;
    protected Material _defaultMaterial;
    protected PlayerWeaponController _weaponController;

    private void Start()
    {
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        _defaultMaterial = _meshRenderer.sharedMaterial;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        _meshRenderer = newMesh;
        _defaultMaterial = newMesh.sharedMaterial;
    }

    public virtual void Interaction()
    {

    }

    public void HighlightActive(bool active)
    {
        if (active)
        {
            _meshRenderer.material = _highlightMaterial;
        }
        else
        {
            _meshRenderer.material = _defaultMaterial;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (_weaponController == null && other.TryGetComponent(out PlayerWeaponController weaponController))
        {
            _weaponController = weaponController;
        }

        if (other.TryGetComponent(out PlayerInteraction playerInteraction))
        {
            playerInteraction.GetInteractables().Add(this);
            playerInteraction.UpdateClosestInteractable();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteraction playerInteraction))
        {
            playerInteraction.GetInteractables().Remove(this);
            playerInteraction.UpdateClosestInteractable();
        }
    }
}
