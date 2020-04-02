using UnityEngine;

/*
 * This script shakes the camera based on the player object's distance to the object this script is attached to.
 */
public class ShakeByDistance : MonoBehaviour
{
    //Our player object.
    public GameObject Player;

    //The maximum distance. If the player is farther away than this distance then no shaking will be visible.
    public float Distance = 10f;

    //Our saved shake instance.
    private CameraShakeInstance _shakeInstance;

    void Start()
    {
        //Create the shake instance. We will modify its properties in Update()
        _shakeInstance = CameraShakerManager.GetCameraShaker("Main Camera").StartShake(2, 14, 0);
    }

	void Update ()
    {
        //Get the distance from the player to this object.
        float currentDistance = Vector3.Distance(Player.transform.position, this.transform.position);

        //Scale the magnitude of our saved shake, so that the scale is higher the closer we get to the object.
        _shakeInstance.ScaleMagnitude = 1 - Mathf.Clamp01(currentDistance / Distance);
	}
}
