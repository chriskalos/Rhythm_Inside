using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
        // If running in the Unity editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Quit the game
#endif
    }
}