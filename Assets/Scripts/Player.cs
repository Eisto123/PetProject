using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Pet")]
    [SerializeField] private PetAI _pet;


    [Header("Hand Interactors")]
    [SerializeField] private HandGrabInteractor _handGrabInteractorLeft;
    [SerializeField] private HandGrabInteractor _handGrabInteractorRight;

    private bool _isGrabbing;
    private bool _wasGrabbingLastFrame;

    private GameObject _grabbedObject;

    private void Update()
    {
        if (_handGrabInteractorLeft.IsGrabbing || _handGrabInteractorRight.IsGrabbing)
        {
            _grabbedObject = _handGrabInteractorLeft.SelectedInteractable != null ? _handGrabInteractorLeft.SelectedInteractable.transform.gameObject : null;
            if (_grabbedObject == null)
            {
                _grabbedObject = _handGrabInteractorRight.SelectedInteractable.transform.gameObject;
            }

            _isGrabbing = true;
            _wasGrabbingLastFrame = true;

            _pet.OnBallPickedUpByPlayer();
        }
        else
        {
            _isGrabbing = false;
            if (_wasGrabbingLastFrame)
            {
                Debug.Log("Player threw object");
                _wasGrabbingLastFrame = false;

                _pet.OnBallThrown(_grabbedObject.transform);
            }
        }
    }
}
