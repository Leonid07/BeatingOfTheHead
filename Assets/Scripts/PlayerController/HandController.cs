using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public enum WeaponType { Fist, FlashLight, PeanutButter, PipeWrench, Shovel}

    [System.Serializable]
    public struct HandData
    {
        public WeaponType weaponType;
        public GameObject weaponObject;
        public Animator handAnimator;
        public float powerMultiplier;
        public float radiusMultiplier;
        public int animatorLayerIndex;
    }

    public HandData[] leftHandData;
    public HandData[] rightHandData;

    public WeaponType leftEquipped = WeaponType.Fist;
    public WeaponType rightEquipped = WeaponType.Fist;

    public float blendSpeed = 5f;
    public float baseHandMass = 1f;

    public bool isLeftAttacking = false;
    public bool isRightAttacking = false;
    Coroutine leftCoroutine;
    Coroutine rightCoroutine;

    private void Start()
    {
        InitializeWeapons(leftHandData, leftEquipped);
        InitializeWeapons(rightHandData, rightEquipped);
    }

    void InitializeWeapons(HandData[] arr, WeaponType equipped)
    {
        foreach (var h in arr)
            h.weaponObject.SetActive(h.weaponType == equipped);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) TriggerAttack(true);
        if (Input.GetMouseButtonDown(1)) TriggerAttack(false);
    }

    void TriggerAttack(bool left)
    {
        if (left && isLeftAttacking || !left && isRightAttacking) return;

        if (left)
        {
            if (leftCoroutine != null) StopCoroutine(leftCoroutine);
            isLeftAttacking = true;
            var hand = GetHandData(true);
            leftCoroutine = StartCoroutine(AttackCoroutine(hand, () => isLeftAttacking = false));
        }
        else
        {
            if (rightCoroutine != null) StopCoroutine(rightCoroutine);
            isRightAttacking = true;
            var hand = GetHandData(false);
            rightCoroutine = StartCoroutine(AttackCoroutine(hand, () => isRightAttacking = false));
        }
    }

    IEnumerator AttackCoroutine(HandData hand, System.Action onFinish)
    {
        var anim = hand.handAnimator;
        for (int i = 0; i < anim.layerCount; i++)
            anim.SetLayerWeight(i, i == hand.animatorLayerIndex ? 1f : 0f);

        float blend = anim.GetFloat("attackBlend");
        while (blend < 1f)
        {
            blend = Mathf.MoveTowards(blend, 1f, blendSpeed * Time.deltaTime);
            anim.SetFloat("attackBlend", blend);
            yield return null;
        }
        anim.SetFloat("attackBlend", 1f);

        while (blend > 0f)
        {
            blend = Mathf.MoveTowards(blend, 0f, blendSpeed * Time.deltaTime);
            anim.SetFloat("attackBlend", blend);
            yield return null;
        }
        anim.SetFloat("attackBlend", 0f);

        onFinish();
    }

    public void TriggerReturn(bool left)
    {
        if (left)
        {
            if (!isLeftAttacking)
            {
                return;
            }

            isLeftAttacking = false;

            if (leftCoroutine != null)
            {
                StopCoroutine(leftCoroutine);
            }

            var hand = GetHandData(true);
            leftCoroutine = StartCoroutine(ReturnCoroutine(hand));
        }
        else
        {
            if (!isRightAttacking)
            {
                return;
            }

            isRightAttacking = false;

            if (rightCoroutine != null)
            {
                StopCoroutine(rightCoroutine);
            }

            var hand = GetHandData(false);
            rightCoroutine = StartCoroutine(ReturnCoroutine(hand));
        }
    }


    IEnumerator ReturnCoroutine(HandData hand)
    {
        var anim = hand.handAnimator;
        float blend = anim.GetFloat("attackBlend");
        while (blend > 0f)
        {
            blend = Mathf.MoveTowards(blend, 0f, blendSpeed * Time.deltaTime);
            anim.SetFloat("attackBlend", blend);
            yield return null;
        }
        anim.SetFloat("attackBlend", 0f);
    }
    public void ChangeWeapon(bool left, WeaponType newWeapon)
    {
        var arr = left ? leftHandData : rightHandData;
        var equip = left ? leftEquipped : rightEquipped;

        if (equip == newWeapon) return;

        var oldData = arr.FirstOrDefault(h => h.weaponType == equip);
        var newData = arr.FirstOrDefault(h => h.weaponType == newWeapon);

        if (oldData.weaponObject != null)
            oldData.weaponObject.SetActive(false);
        if (newData.weaponObject != null)
            newData.weaponObject.SetActive(true);

        if (left) leftEquipped = newWeapon;
        else rightEquipped = newWeapon;
    }
    public void DoHit(bool left, Collider target, Vector3 hitPoint, Vector3 normal)
    {
        var hand = GetHandData(left);
        float power = baseHandMass * hand.powerMultiplier;
        float radius = hand.radiusMultiplier;
        float mass = hand.weaponObject.TryGetComponent<Rigidbody>(out var rb) ? rb.mass : baseHandMass;

        if (target.TryGetComponent<HitReceiver>(out var receiver))
            receiver.ApplyHit(target, hitPoint, normal, power * mass, radius);
    }

HandData GetHandData(bool left)
    {
        var arr = left ? leftHandData : rightHandData;
        var equip = left ? leftEquipped : rightEquipped;
        return System.Array.Find(arr, h => h.weaponType == equip);
    }
}