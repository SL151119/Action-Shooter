using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Player _player;
    private List<Interactable> interactables = new List<Interactable>();

    private Interactable _closestInteractable;

    private void Start()
    {
        _player.Controls.Character.Interaction.performed += context => InteractWithClosest(); 
    }

    private void InteractWithClosest()
    {
        _closestInteractable?.Interaction();
        interactables.Remove(_closestInteractable);

        UpdateClosestInteractable();
    }

    public void UpdateClosestInteractable()
    {
        _closestInteractable?.HighlightActive(false);

        _closestInteractable = null;

        float closestDistance = float.MaxValue;

        foreach (Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                _closestInteractable = interactable;
            }
        }

        _closestInteractable?.HighlightActive(true);
    }

    public List<Interactable> GetInteractables() => interactables;
}
