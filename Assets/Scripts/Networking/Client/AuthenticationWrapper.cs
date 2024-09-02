using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        AuthState = AuthState.Authenticating;
        int tries = 0;
        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Sign in types are available, using anon to not require sign in

            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized) // Sign in was successful
            {
                AuthState = AuthState.Authenticated;
                break;
            }

            tries++;
            await Task.Delay(2000); // Wait one second before retrying
        }

        return AuthState;
    }
}
