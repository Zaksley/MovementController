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
                         controller._action == MovementController.Action.MANDATORY_CROUNCH); 
        
        
        if (_slide && !_isSliding)
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
            Debug.Log("waiting");
        }
        else
        {
            controller.Move(_horizontalMove * Time.fixedDeltaTime, _crouch, _jump);
        }
        
        _jump = false;
        _slide = false; 
    }

    private IEnumerator StopSlideCall()
    {
        yield return new WaitForSeconds(_timeSlide);
        Debug.Log("stop slide");
        _isSliding = false; 
        controller.StopSlide();
    }


}
