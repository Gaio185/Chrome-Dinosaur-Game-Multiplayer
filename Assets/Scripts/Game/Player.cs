using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private CharacterController characterController;
    private Vector3 direction;

    private Vector3 previousInput;

    [SerializeField] private float gravity = 9.81f * 2f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float moveSpeed = 5f;

    private float score;

    public TextMeshProUGUI scoreText;      

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    [ClientCallback]
    private void OnEnable()
    {
        direction = Vector3.zero;
    }

    [ClientCallback]
    void Update() => Move();

    [Client]
    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (characterController.isGrounded)
        {
            direction = new Vector3(horizontalInput * moveSpeed, -1f, 0f);

            if (Input.GetButton("Jump"))
            {
                direction = Vector3.up * jumpForce;
            }
        }
        else
        {
            direction = new Vector3(horizontalInput * moveSpeed, direction.y, 0f);
            direction += (Vector3.down * gravity * Time.deltaTime);
        }

        characterController.Move(direction * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
