using UnityEngine;

/*
 * This script begins shaking the camera when the player enters the trigger, and stops shaking when the player leaves.
 */
public class ShakeOnTrigger : MonoBehaviour
{
    //Our saved shake instance.
    private CameraShakeInstance _shakeInstance;

    void Start()
    {
        //We make a single shake instance that we will fade in and fade out when the player enters and leaves the trigger area.
        _shakeInstance = CameraShakerManager.GetCameraShaker("Main Camera").StartShake(2, 15, 2);

        //Immediately make the shake inactive.  
        _shakeInstance.StartFadeOut(0);

        //We don't want our shake to delete itself once it stops shaking.
        _shakeInstance.DeleteOnInactive = true;
    }

    //When the player enters the trigger, begin shaking.
    void OnTriggerEnter(Collider c)
    {
        //Check to make sure the object that entered the trigger was the player.
        if (c.CompareTag("Player"))
            _shakeInstance.StartFadeIn(1);
    }

    //When the player exits the trigger, stop shaking.
    void OnTriggerExit(Collider c)
    {
        //Check to make sure the object that exited the trigger was the player.
        if (c.CompareTag("Player"))
        {
            //Fade out the shake over 3 seconds.
            _shakeInstance.StartFadeOut(3f);        
        }
            
    }
}
