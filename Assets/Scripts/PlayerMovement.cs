using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public MovementController controller;

    [SerializeField] 
    private float _movementSpeed = 40f;
    
    private float _horizontalMove = 0f;
    private bool _jump = false;
    private bool _crouch = false;
    private bool _slide = false;
    private bool _isSliding = false;
    private bool _roll = false; 
    private bool _isRolling = false; 
    [SerializeField] private float _timeSlide;

    void Update()
    {
        _horizontalMove = Input.GetAxisRaw("Horizontal") * _movementSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            _jump = true; 
        }

        if (Input.GetButtonDown("Slide"))
        {
            _slide = true; 
        }

        if (Input.GetButtonDown("Roll"))
        {
            _roll = true; 
        }
        
        if (Input.GetButtonDown("Crouch"))
        {
            _crouch = true; 
        } 
        else if(Input.GetButtonUp("Crouch"))
        {
            _crouch = false; 
        }
    }

    private void FixedUpdate()
    {
        bool canSlide = (controller._action == MovementController.Action.IDLE ||
                         controller._action == MovementController.Action.CROUCH ||
                         controller._action == MovementController.Action.MANDATORY_CROUNCH ||
                         controller._action == MovementController.Action.MOVEMENT);

        bool canRoll = _horizontalMove != 0 && controller._action == MovementController.Action.IDLE; 
        
        
        if (_roll && !_isRolling)
        {
            if (canRoll)
            {
                Debug.Log("in");
                _isRolling = true;
                controller.Roll(_horizontalMove * Time.fixedDeltaTime); 
            }
            else
                Debug.Log("false");
        }
        else if (_slide && !_isSliding)
        {
           if (canSlide)
            {
                _isSliding = true; 
                controller.Slide();
                StartCoroutine("StopSlideCall", _timeSlide);
            }
        }
        else if (_isSliding)
        {
            // Do nothing, just sliding  
        }
        else
        {
            controller.Move(_horizontalMove * Time.fixedDeltaTime, _crouch, _jump);
        }
        
        // Reseting parameters 
        _jump = false;
        _slide = false;
        _roll = false; 
    }

    private IEnumerator StopSlideCall()
    {
        yield return new WaitForSeconds(_timeSlide);
        _isSliding = false; 
        controller.StopSlide();
    }

    public void StopRollCall()
    {
        _isRolling = false;
        controller.StopRoll(); 
    }


}
