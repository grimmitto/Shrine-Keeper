using UnityEngine;

public class CutsceneCameraDirector : MonoBehaviour
{
    public GameObject priestCam;
    public GameObject playerCam;

    void OnEnable()
    {
        DialogueManager.Instance.OnSpeakerChanged += SwapCam;
    }

    void OnDisable()
    {
        DialogueManager.Instance.OnSpeakerChanged -= SwapCam;
    }

    void SwapCam(string speaker)
    {
        if (speaker.Contains("PRIEST"))
        {
            priestCam.SetActive(true);
            playerCam.SetActive(false);
        }
        else
        {
            priestCam.SetActive(false);
            playerCam.SetActive(true);
        }
    }
}
