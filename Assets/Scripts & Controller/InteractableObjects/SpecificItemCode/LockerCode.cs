using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerCode : InteractableObject
{
    [Space(10)]
    [Header("RUN ITEM CODE")]
    [SerializeField] Transform door;
    MeshRenderer doorMesh;
    bool insideCollider = false;
    bool open = false;
    bool locked = false;

    void Start()
    {
        doorMesh = door.GetComponent<MeshRenderer>();
    }

    // Override the base class method for specific implementation
    protected override void RunItemCode()
    {
        if (!locked) 
        {
            if (!open) StartCoroutine(Open());
            else if (open) StartCoroutine(Close());
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            insideCollider = true;
            ToggleMeshEmission(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            insideCollider = false;
            ToggleHidden(false);
            ToggleMeshEmission(false);
        }
    }

    void ToggleHidden(bool hidden)
    {
        foreach (AISensor sensor in GameManager.Instance.enemyPool.GetComponentsInChildren<AISensor>())
        {
            sensor.hidden = hidden;
        }
    }

    void ToggleMeshEmission(bool emission)
    {
        if (emission)
        {
            doorMesh.material.EnableKeyword("_EMISSION");
            doorMesh.material.SetColor("_EmissionColor", new Color(0.1f, 0.1f, 0.1f));
        }
        else
        {
            doorMesh.material.DisableKeyword("_EMISSION");
        }
    }

    public void CloseLocker()
    {
        if (open)
        {
            StartCoroutine(ShutDown());
        }
        else locked = true;
    }


    IEnumerator Open()
    {
        locked = true;

        // slowly open door of the locker
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            door.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, -100, 0), t);
            yield return null;
        }

        if (insideCollider)
        {
            ToggleHidden(false);
        }

        locked = false;
        open = true;
    }

    IEnumerator Close()
    {
        if (insideCollider)
        {
            ToggleHidden(true);
        }
        locked = true;
        
        // close the door of the locker
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            door.localRotation = Quaternion.Lerp(door.localRotation, Quaternion.Euler(0, 0, 0), t);
            yield return null;
        }

        locked = false;
        open = false;
    }

    IEnumerator ShutDown()
    {
        locked = true;
        
        // close the door of the locker
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            door.localRotation = Quaternion.Lerp(door.localRotation, Quaternion.Euler(0, 0, 0), t);
            yield return null;
        }
    }
}