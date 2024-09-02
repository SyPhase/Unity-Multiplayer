using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

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
        if (AuthState == AuthState.Authenticated) // Already Authenticated successfully
        {
            return AuthState;
        }

        if (AuthState == AuthState.Authenticating) // Aleady started Authenticating...
        {
            Debug.LogWarning("Aleady authenticating, waiting for authentication...");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxTries); // Begin Authentication

        return AuthState;
    }

    static async Task<AuthState> Authenticating() // Wait for authentication to finish
    {
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(1000); // Wait a second
        }

        return AuthState;
    }

    static async Task SignInAnonymouslyAsync(int maxTries)
    {
        AuthState = AuthState.Authenticating;

        int tries = 0;
        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Sign in types are available, using anon to not require sign in

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized) // Sign in was successful
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authenticationException)
            {
                Debug.LogError(authenticationException);
                AuthState = AuthState.Error;
            }
            catch (RequestFailedException requestException)
            {
                Debug.LogError(requestException);
                AuthState = AuthState.Error;
            }

            tries++;
            await Task.Delay(2000); // Wait one second before retrying
        }

        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in after {maxTries} tries...");
            AuthState = AuthState.TimeOut;
        }
    }
}
