using UnityEngine;

public class ShakeDemo : MonoBehaviour 
{
    Vector3 posInf = new Vector3(0.25f, 0.25f, 0.25f);
    Vector3 rotInf = new Vector3(1, 1, 1);
    float magn = 1, rough = 1, fadeIn = 0.1f, fadeOut = 2f;

    bool modValues;
    bool showList;

    CameraShakeInstance shake;

    delegate float Slider(float val, string prefix, float min, float max, int pad);

	void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Application.LoadLevel(Application.loadedLevel);
        }

        Slider s = delegate(float val, string prefix, float min, float max, int pad)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(prefix, GUILayout.MaxWidth(pad));
            val = GUILayout.HorizontalSlider(val, min, max);
            GUILayout.Label(val.ToString("F2"), GUILayout.MaxWidth(50));
            GUILayout.EndHorizontal();
            return val;
        };

        GUI.Box(new Rect(10, 10, 250, Screen.height - 15), "Make-A-Shake");
        GUILayout.BeginArea(new Rect(29f, 40, 215, Screen.height - 40));

        GUILayout.Label("--Position Infleunce--");
        posInf.x = s(posInf.x, "X", 0, 4, 25);
        posInf.y = s(posInf.y, "Y", 0, 4, 25);
        posInf.z = s(posInf.z, "Z", 0, 4, 25);

        GUILayout.Label("--Rotation Infleunce--");
        rotInf.x = s(rotInf.x, "X", 0, 4, 25);
        rotInf.y = s(rotInf.y, "Y", 0, 4, 25);
        rotInf.z = s(rotInf.z, "Z", 0, 4, 25);

        GUILayout.Label("--Other Properties--");

        magn = s(magn, "Magnitude:", 0, 10, 75);
        rough = s(rough, "Roughness:", 0, 20, 75);

        fadeIn = s(fadeIn, "Fade In:", 0, 10, 75);
        fadeOut = s(fadeOut, "Fade Out:", 0, 10, 75);

        GUILayout.Label("--Saved Shake Instance--");
        GUILayout.Label("You can save shake instances and modify their properties at runtime.");

        if (shake == null && GUILayout.Button("Create Shake Instance"))
        {
            shake = CameraShakerManager.GetCameraShaker("Main Camera").StartShake(magn, rough, fadeIn);
            shake.DeleteOnInactive = false;
        }

        if (shake != null)
        {
            if (GUILayout.Button("Delete Shake Instance"))
            {
                shake.DeleteOnInactive = true;
                shake.StartFadeOut(fadeOut);
                shake = null;
                
            }

            if (shake != null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Fade Out"))
                {
                    shake.StartFadeOut(fadeOut);
                }
                if (GUILayout.Button("Fade In"))
                {
                    shake.StartFadeIn(fadeIn);
                }
                GUILayout.EndHorizontal();

                modValues = GUILayout.Toggle(modValues, "Modify Instance Values");

                if (modValues)
                {
                    shake.ScaleMagnitude = magn;
                    shake.ScaleRoughness = rough;
                    shake.PositionInfluence = posInf;
                    shake.RotationInfluence = rotInf;
                }
            }
        }

        GUILayout.Label("--Shake Once--");
        GUILayout.Label("You can simply have a shake that automatically starts and stops too.");

        if (GUILayout.Button("Shake!"))
        {
            CameraShakeInstance c = CameraShakerManager.GetCameraShaker("Main Camera").ShakeOnce(magn, rough, fadeIn, fadeOut);
            c.PositionInfluence = posInf;
            c.RotationInfluence = rotInf;
        }

        GUILayout.EndArea();

        float height;

        if (!showList)
            height = 120;
        else
            height = 120 + CameraShakerManager.GetCameraShaker("Main Camera").ShakeInstances.Count * 130f;

        GUI.Box(new Rect(Screen.width - 310, 10, 300, height), "Shake Instance List");
        GUILayout.BeginArea(new Rect(Screen.width - 285, 40, 255, Screen.height - 40));

        GUILayout.Label("All shake instances are saved and stacked as long as they are active.");

        showList = GUILayout.Toggle(showList, "Show List");

        if (showList)
        {
            int index = 1;
            foreach (CameraShakeInstance c in CameraShakerManager.GetCameraShaker("Main Camera").ShakeInstances)
            {
                string state = c.CurrentState.ToString();

                GUILayout.Label("#" + index + ": Magnitude: " + c.Magnitude.ToString("F2") + ", Roughness: " + c.Roughness.ToString("F2"));
                GUILayout.Label("      Position Influence: " + c.PositionInfluence);
                GUILayout.Label("      Rotation Influence: " + c.RotationInfluence);
                GUILayout.Label("      State: " + state);
                GUILayout.Label("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                index++;
            }
        }
        GUILayout.EndArea();
	}
}
