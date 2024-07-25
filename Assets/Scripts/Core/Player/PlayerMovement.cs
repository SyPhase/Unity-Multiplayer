using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody2D rb;
    [Header("Settings")]
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float turnSpeed = 135f;

    Vector2 previousMoveInput;

    public override void OnNetworkSpawn()
    {
        // Authority Check
        if (!IsOwner) { return; }

        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        // Authority Check
        if (!IsOwner) { return; }

        inputReader.MoveEvent -= HandleMove;
    }

    void HandleMove(Vector2 moveInput)
    {
        previousMoveInput = moveInput;
    }

    void Update()
    {
        // Authority Check
        if (!IsOwner) { return; }

        float zRotation = -turnSpeed * Time.deltaTime * previousMoveInput.x;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    void FixedUpdate()
    {
        // Authority Check
        if (!IsOwner) { return; }

        rb.velocity = moveSpeed * (Vector2)bodyTransform.up * previousMoveInput.y;
    }
}
