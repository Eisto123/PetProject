using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private HandGrabInteractor _handGrabInteractorLeft;
    [SerializeField] private HandGrabInteractor _handGrabInteractorRight;

    private bool _isGrabbing;
    private bool _wasGrabbingLastFrame;

    private void Update()
    {
        if (_handGrabInteractorLeft.IsGrabbing || _handGrabInteractorRight.IsGrabbing)
        {
            Debug.Log("Player is grabbing");
            _isGrabbing = true;
        }
        else
        {
            _isGrabbing = false;
        }
    }
}
