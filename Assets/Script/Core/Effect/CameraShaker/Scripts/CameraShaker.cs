using UnityEngine;
using System.Collections.Generic;


public class CameraShaker : MonoBehaviour
{

    /// <summary>
    /// The default position influcence of all shakes created by this shaker.
    /// </summary>
    public Vector3 DefaultPosInfluence = new Vector3(0.15f, 0.15f, 0.15f);
    /// <summary>
    /// The default rotation influcence of all shakes created by this shaker.
    /// </summary>
    public Vector3 DefaultRotInfluence = new Vector3(1, 1, 1);


    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    Vector3 posAddShake, rotAddShake;

    List<CameraShakeInstance> cameraShakeInstances = new List<CameraShakeInstance>();
    public string cameraTag = "";
    public bool isUICamera = false;

    private float UI_Z = 0;
    void Awake()
    {
        if (string.IsNullOrEmpty(cameraTag))
            cameraTag = gameObject.name;
        CameraShakerManager.AddCameraShaker(cameraTag, this);

        UI_Z = transform.localPosition.z;
    }

    private void OnEnable()
    {
        //CameraShakeInstance cameraShakeInstance = new CameraShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
        //cameraShakeInstance.PositionInfluence = DefaultPosInfluence;
        //cameraShakeInstance.RotationInfluence = DefaultRotInfluence;
        //Shake(cameraShakeInstance);

    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Shake(CameraShakePresets.Bump);
        //}

            posAddShake = Vector3.zero;
        rotAddShake = Vector3.zero;

        for (int i = 0; i < cameraShakeInstances.Count; i++)
        {
            if (i >= cameraShakeInstances.Count)
                break;

            CameraShakeInstance c = cameraShakeInstances[i];

            if (c.CurrentState == CameraShakeState.Inactive && c.DeleteOnInactive)
            {
                cameraShakeInstances.RemoveAt(i);
                i--;
            }
            else if (c.CurrentState != CameraShakeState.Inactive)
            {
                posAddShake += CameraUtilities.MultiplyVectors(c.UpdateShake(), c.PositionInfluence);
                rotAddShake += CameraUtilities.MultiplyVectors(c.UpdateShake(), c.RotationInfluence);
            }
        }
        if (isUICamera)
            posAddShake.z = UI_Z;
        transform.localPosition = posAddShake;
        transform.localEulerAngles = rotAddShake;
    }


    /// <summary>
    /// Starts a shake using the given preset.
    /// </summary>
    /// <param name="shake">The preset to use.</param>
    /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
    public CameraShakeInstance Shake(CameraShakeInstance shake)
    {
        cameraShakeInstances.Add(shake);
        return shake;
    }

    /// <summary>
    /// Shake the camera once, fading in and out  over a specified durations.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake.</param>
    /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    /// <param name="fadeInTime">How long to fade in the shake, in seconds.</param>
    /// <param name="fadeOutTime">How long to fade out the shake, in seconds.</param>
    /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
    public CameraShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
    {
        CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
        shake.PositionInfluence = DefaultPosInfluence;
        shake.RotationInfluence = DefaultRotInfluence;
        cameraShakeInstances.Add(shake);

        return shake;
    }

    /// <summary>
    /// Shake the camera once, fading in and out over a specified durations.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake.</param>
    /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    /// <param name="fadeInTime">How long to fade in the shake, in seconds.</param>
    /// <param name="fadeOutTime">How long to fade out the shake, in seconds.</param>
    /// <param name="posInfluence">How much this shake influences position.</param>
    /// <param name="rotInfluence">How much this shake influences rotation.</param>
    /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
    public CameraShakeInstance ShakeOnce(float magnitude, float roughness, float fadeInTime, float fadeOutTime, Vector3 posInfluence, Vector3 rotInfluence)
    {
        CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness, fadeInTime, fadeOutTime);
        shake.PositionInfluence = posInfluence;
        shake.RotationInfluence = rotInfluence;
        cameraShakeInstances.Add(shake);

        return shake;
    }

    /// <summary>
    /// Start shaking the camera.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake.</param>
    /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    /// <param name="fadeInTime">How long to fade in the shake, in seconds.</param>
    /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
    public CameraShakeInstance StartShake(float magnitude, float roughness, float fadeInTime)
    {
        CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness);
        shake.PositionInfluence = DefaultPosInfluence;
        shake.RotationInfluence = DefaultRotInfluence;
        shake.StartFadeIn(fadeInTime);
        cameraShakeInstances.Add(shake);
        return shake;
    }

    /// <summary>
    /// Start shaking the camera.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake.</param>
    /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    /// <param name="fadeInTime">How long to fade in the shake, in seconds.</param>
    /// <param name="posInfluence">How much this shake influences position.</param>
    /// <param name="rotInfluence">How much this shake influences rotation.</param>
    /// <returns>A CameraShakeInstance that can be used to alter the shake's properties.</returns>
    public CameraShakeInstance StartShake(float magnitude, float roughness, float fadeInTime, Vector3 posInfluence, Vector3 rotInfluence)
    {
        CameraShakeInstance shake = new CameraShakeInstance(magnitude, roughness);
        shake.PositionInfluence = posInfluence;
        shake.RotationInfluence = rotInfluence;
        shake.StartFadeIn(fadeInTime);
        cameraShakeInstances.Add(shake);
        return shake;
    }

    /// <summary>
    /// Gets a copy of the list of current camera shake instances.
    /// </summary>
    public List<CameraShakeInstance> ShakeInstances
    { get { return new List<CameraShakeInstance>(cameraShakeInstances); } }

    void OnDestroy()
    {
        CameraShakerManager.RemoveCameraShaker(cameraTag);
    }
}
