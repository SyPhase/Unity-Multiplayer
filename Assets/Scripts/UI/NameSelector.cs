using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    [SerializeField] Button connectButton;
    [SerializeField] int minNameLength = 1;
    [SerializeField] int maxNameLength = 16;

    public const string PlayerNameKey = "PlayerName";

    void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null) // is headless server
        {
            SceneManager.LoadScene(1 + SceneManager.GetActiveScene().buildIndex);
            return;
        }

        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        HandleNameChanged();
    }

    public void HandleNameChanged()
    {
        connectButton.interactable = 
            nameField.text.Length >= minNameLength && 
            nameField.text.Length <= maxNameLength;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);

        SceneManager.LoadScene(1 + SceneManager.GetActiveScene().buildIndex);
    }
}
