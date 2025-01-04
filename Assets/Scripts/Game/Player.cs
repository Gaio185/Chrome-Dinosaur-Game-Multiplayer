using Mirror;
using Mirror.BouncyCastle.Asn1.Crmf;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private CharacterController characterController;

    private Vector3 previousInput;

    [SerializeField] private float gravity = 9.81f * 2f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float moveSpeed = 5f;

    public override void OnStartAuthority()
    {
        enabled = true;

        InputManager.Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        InputManager.Controls.Player.Move.canceled += ctx => ResetMovement();
    }
   
    [ClientCallback]
    void Update() => Move();

    [Client]
    private void SetMovement(Vector2 movement) => previousInput = movement;

    [Client]
    private void ResetMovement() => previousInput = Vector2.zero;

    [Client]
    private void Move()
    {
        //if (characterController.isGrounded)
        //{
        //    direction = new Vector3(horizontalInput * moveSpeed, -1f, 0f);

        //    if (Input.GetButton("Jump"))
        //    {
        //        direction = Vector3.up * jumpForce;
        //    }
        //}
        //else
        //{
        //    direction = new Vector3(horizontalInput * moveSpeed, direction.y, 0f);
        //    direction += (Vector3.down * gravity * Time.deltaTime);
        //}

        //characterController.Move(direction * Time.deltaTime);
        Vector3 movement = Vector3.zero;
        Vector3 right = characterController.transform.right;
        Vector3 up = characterController.transform.up;

        movement = (right.normalized * previousInput.x);
     

        characterController.Move(movement * moveSpeed * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            gameObject.SetActive(false);
        }
    }
}
