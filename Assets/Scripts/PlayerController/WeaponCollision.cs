using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    public bool isLeftHand = true;
    public LayerMask validHitLayers;

    private HandController controller;
    private bool hasHitThisContact = false;

    void Start()
    {
        controller = GameManager.Instance.GetHandController();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & validHitLayers) == 0)
            return;

        Collider weaponCollider = GetComponent<Collider>();
        Collider targetCollider = collision.collider;

        Vector3 direction;
        float distance;

        Physics.ComputePenetration(
            weaponCollider, transform.position, transform.rotation,
            targetCollider, targetCollider.transform.position, targetCollider.transform.rotation,
            out direction, out distance
        );

            Vector3 hitPoint = transform.position + direction * distance;
            Vector3 normal = -direction;

            controller.DoHit(isLeftHand, targetCollider, hitPoint, normal);
            controller.TriggerReturn(isLeftHand);
            hasHitThisContact = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (hasHitThisContact)
        {
            controller.TriggerReturn(isLeftHand);
            hasHitThisContact = false;
        }
    }
}
