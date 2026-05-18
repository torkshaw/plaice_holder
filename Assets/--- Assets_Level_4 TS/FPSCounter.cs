using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        float fps = 1.0f / deltaTime;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 12;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.LowerRight;

        GUI.Label(
            new Rect(Screen.width - 120, Screen.height - 40, 100, 20),
            $"FPS: {Mathf.Ceil(fps)}",
            style
        );
    }
}