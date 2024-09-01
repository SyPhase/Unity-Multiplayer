using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
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

    ClientGameManager gameManager;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public async Task CreateClient() //CreateClientAsync()
    {
        gameManager = new ClientGameManager();

        await gameManager.InitAsync();
    }
}
