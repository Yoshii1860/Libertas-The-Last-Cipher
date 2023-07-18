using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectHandler : MonoBehaviour
{
    // [SerializeField] private InputActionReference input;
    [SerializeField] private float pickupDistance = 5f;
    ItemActions action;

    void Start() 
    {
        action = GetComponent<ItemActions>();
    }

    public void Execute()
    {
        DetectObjects();
    }

    private void DetectObjects()
    {
        Debug.Log("Detecting objects");
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupDistance))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponent<ItemController>() != null)
            {
                Debug.Log("Item found");
                // Use PickUp() method from ItemActions class
                action.PickUp(hit);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupDistance);
    }
}