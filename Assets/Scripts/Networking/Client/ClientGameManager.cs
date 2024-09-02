using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    public async Task<bool> InitAsync()
    {
        // Authenticate Player
        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        // Load next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
