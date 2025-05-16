using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [Serializable]
    public struct HandWeaponButton
    {
        public HandController.WeaponType weaponType;
        public Button button;
    }

    private HandController handController;

    public List<HandWeaponButton> leftHandButtons = new List<HandWeaponButton>();
    public List<HandWeaponButton> rightHandButtons = new List<HandWeaponButton>();

    void Start()
    {
        handController = GameManager.Instance.GetHandController();

        InitializationnButtonInventory(leftHandButtons, true);
        InitializationnButtonInventory(rightHandButtons, false);
    }

    void InitializationnButtonInventory(List<HandWeaponButton> buttonInventory, bool typeHand)
    {
        foreach (var wb in buttonInventory)
        {
            var type = wb.weaponType;
            var btn = wb.button;

            if (btn == null)
            {
                Debug.Log($"button not found {btn}");
                continue;
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                handController.ChangeWeapon(typeHand, type);
            });
        }
    }
}
