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

    void Update()
    {
        _horizontalMove = Input.GetAxisRaw("Horizontal") * _movementSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            _jump = true; 
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
        controller.Move(_horizontalMove * Time.fixedDeltaTime, _crouch, _jump);
        _jump = false; 
    }


}
