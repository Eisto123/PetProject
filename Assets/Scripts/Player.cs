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
    [SerializeField] private DistanceHandGrabInteractor _distanceHandGrabInteractorLeft;
    [SerializeField] private DistanceHandGrabInteractor _distanceHandGrabInteractorRight;

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

            _grabbedObject.transform.parent.SetParent(null);
            _wasGrabbingLastFrame = true;
            _pet.OnBallPickedUpByPlayer();
        }
        else if (_distanceHandGrabInteractorLeft.IsGrabbing || _distanceHandGrabInteractorRight.IsGrabbing)
        {
            _grabbedObject = _distanceHandGrabInteractorLeft.SelectedInteractable != null ? _distanceHandGrabInteractorLeft.SelectedInteractable.transform.gameObject : null;
            if (_grabbedObject == null)
            {
                _grabbedObject = _distanceHandGrabInteractorRight.SelectedInteractable.transform.gameObject;
            }

            _grabbedObject.transform.parent.SetParent(null);
            _wasGrabbingLastFrame = true;
            _pet.OnBallPickedUpByPlayer();
        }
        else
        {
            if (_wasGrabbingLastFrame)
            {
                Debug.Log("Player threw object");
                _wasGrabbingLastFrame = false;

                _pet.OnBallThrown(_grabbedObject.transform);
            }
        }
    }
}
