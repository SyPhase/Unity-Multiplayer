using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class StartupSceneLoader
{
    static StartupSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadStartUpScene;
    }

    static void LoadStartUpScene(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (EditorSceneManager.GetActiveScene().buildIndex != 0)
            {
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
