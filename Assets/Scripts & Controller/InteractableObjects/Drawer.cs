using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Drawer : InteractableObject
{
    [Space(10)]
    [Header("OPEN DOOR/DRAWER")]
    [Tooltip("Is it a drawer (true) or a door (false)?")]
    [SerializeField] bool drawer;
    [Tooltip("The position (drawer) or rotation (door) to move to")]
    [SerializeField] Vector3 openPosOrRot;
    Vector3 closedPosOrRot;
    bool isOpen = false;
    bool isMoving = false;

    void Start()
    {
        closedPosOrRot = drawer ? transform.localPosition : transform.localEulerAngles;
        transform.GetComponent<AudioSource>().spatialBlend = 1;
        transform.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Custom;
    }

    protected override void RunItemCode()
    {
        if (isMoving) return;
        StartCoroutine(LerpToPosition());
    }

    IEnumerator LerpToPosition()
    {
        isMoving = true;
        Vector3 currentVector = isOpen ? openPosOrRot : closedPosOrRot;
        Vector3 newPosOrRot = isOpen ? closedPosOrRot : openPosOrRot;

        float t = 0f;
        float timeToMove = 1f;

        if (drawer) AudioManager.Instance.PlaySoundOneShot(gameObject.GetInstanceID(), "drawer", 1f, 1f);
        else AudioManager.Instance.PlaySoundOneShot(gameObject.GetInstanceID(), "closet door", 1f, 1f);

        yield return new WaitForSeconds(0.3f);

        isOpen = !isOpen;

        if (drawer)
        {
            while (t < 1)
            {
                t += Time.deltaTime / timeToMove;
                transform.localPosition = Vector3.Lerp(currentVector, newPosOrRot, t);
                yield return null;
            }
        }
        else
        {
            while (t < 1)
            {
                t += Time.deltaTime / timeToMove;
                transform.localEulerAngles = Vector3.Lerp(currentVector, newPosOrRot, t);
                yield return null;
            }
        }
        
        isMoving = false;
    }
}
