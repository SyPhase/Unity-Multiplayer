using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text playerNameText;
    TankPlayer player;

    void Start()
    {
        player = GetComponentInParent<TankPlayer>();

        HandlePlayerNameChanged(string.Empty, player.PlayerName.Value);

        player.PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();
    }

    void OnDestroy()
    {
        player.PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }
}
