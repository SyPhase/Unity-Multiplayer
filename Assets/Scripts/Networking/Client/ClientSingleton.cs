using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    public ClientGameManager GameManager { get; private set; }

    static ClientSingleton instance;
    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindObjectOfType<ClientSingleton>();

            if (instance == null)
            {
                Debug.Log("No Client Singleton in the scene!!!");
                return null;
            }

            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public async Task<bool> CreateClient() //CreateClientAsync()
    {
        GameManager = new ClientGameManager();

        return await GameManager.InitAsync();
    }

    void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
