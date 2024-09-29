using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform turretTransform;
    [Header("Settings")]
    [Tooltip("Higher numbers allow for faster turret rotation.  Zero or negative numbers means no maximum, snaps instantly")]
    [SerializeField] float aimSpeed = 225f;

    Camera playerCam;

    void Start() //public override void OnNetworkSpawn()
    {
        playerCam = GetComponentInChildren<Camera>();
        if (playerCam == null)
        {
            playerCam = FindObjectOfType<Camera>();
        }
    }

    void LateUpdate() // Use LateUpdate instead of Update to solve jitter
    {
        // Authority Check
        if (!IsOwner) { return; }

        if (playerCam == null) { playerCam = FindObjectOfType<Camera>(); }

        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = playerCam.ScreenToWorldPoint(aimScreenPosition);

        if (aimSpeed <= 0f) // Setting Transfrom.up
        {
            // Set rotation
            turretTransform.up = new Vector2(aimWorldPosition.x - turretTransform.position.x, aimWorldPosition.y - turretTransform.position.y);
        }
        else // Using angles
        {
            Vector2 targetRelativePosition = new Vector2(aimWorldPosition.x - turretTransform.position.x, aimWorldPosition.y - turretTransform.position.y);
            float targetAngle = (Mathf.Atan2(targetRelativePosition.y, targetRelativePosition.x) * Mathf.Rad2Deg) - 90f;
            //turretTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle)); // Sets to snap instantly to target

            // Calculating and Clamping Angle
            if (targetAngle < 0) { targetAngle += 360; }
            float currentAngle = turretTransform.rotation.eulerAngles.z;
            float angleDifference = targetAngle - currentAngle;
            if (angleDifference > 180) { angleDifference -= 360; }
            else if (angleDifference < -180) { angleDifference += 360; }
            float rotateAmount = Mathf.Sign(angleDifference) * Mathf.Clamp(Mathf.Abs(angleDifference), 0f, aimSpeed * Time.deltaTime);
            
            // Set rotation
            turretTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, currentAngle + rotateAmount));
        }
    }
}
