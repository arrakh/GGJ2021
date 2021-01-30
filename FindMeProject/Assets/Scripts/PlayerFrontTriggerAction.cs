using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrontTriggerAction : MonoBehaviour
{
    public MovementController MovementController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MovementController.TargetAttack = other.gameObject;
        }
    }
}
