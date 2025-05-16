using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] HandController handController;
    [SerializeField] HitReceiver hitReceiver;
    public HandController GetHandController()
    {
        return handController;
    }
    public HitReceiver GetHitReceiver()
    {
        return hitReceiver;
    }

}
