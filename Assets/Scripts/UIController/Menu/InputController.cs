using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public KeyCode keyViewInventory = KeyCode.R;
    public KeyCode keyRestartEnemy = KeyCode.F;

    private GameObject inventory;

    private HandController handController;
    private HitReceiver hitReceiver;
    private void Start()
    {
        handController = GameManager.Instance.GetHandController();
        hitReceiver = GameManager.Instance.GetHitReceiver();
        inventory = UIManager.Instance.inventory;
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyViewInventory))
        {
            ToggleInventory(true);
        }
        else if (Input.GetKeyUp(keyViewInventory))
        {
            ToggleInventory(false);
        }

        if (Input.GetKeyDown(keyRestartEnemy))
        {
            hitReceiver.ResetToOriginalState();
        }
    }
    private void ToggleInventory(bool open)
    {
        inventory.SetActive(open);
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        handController.isLeftAttacking = open;
        handController.isRightAttacking = open;
    }
}
