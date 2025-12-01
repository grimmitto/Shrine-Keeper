using UnityEngine;

public class OpeningSceneDirector : MonoBehaviour
{
    public GameObject maleMesh;
    public GameObject femaleMesh;

    void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            Debug.Log("<color=cyan>[Director]</color> Subscribing to OnSceneRequest");
            DialogueManager.Instance.OnSceneRequest += HandleSceneRequest;
        }
        else
        {
            Debug.LogError("<color=red>[Director]</color> DialogueManager.Instance is NULL in OnEnable!");
        }
    }

    void OnDisable()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnSceneRequest -= HandleSceneRequest;
    }
    
    void Update()
    {
        if (maleMesh.activeInHierarchy == false &&
            femaleMesh.activeInHierarchy == false)
        {
            Debug.Log("<color=red>[Director]</color> No mesh active, trying to activate");
            bool isMale = GameManager.Instance.playerGender == PlayerGender.Male;
            HandleGender(isMale);
        }
    }

    void Start()
    {
        Debug.Log("<color=yellow>[Director]</color> OpeningSceneDirector Start()");

        bool isMale = GameManager.Instance.playerGender == PlayerGender.Male;

        Debug.Log("<color=yellow>[Director]</color> Gender: " + (isMale ? "Male" : "Female"));

        HandleGender(isMale);

        Debug.Log("<color=lime>[Director]</color> Starting dialogue: opening_cinematic");

        DialogueManager.Instance.StartDialogue("opening_cinematic");
    }

    public void HandleGender(bool isMale)
    {
        Debug.Log("<color=yellow>[Director]</color> Gender: " + (isMale ? "Male" : "Female"));
        if (isMale)
        {
            Debug.Log("<color=red>[Director]</color> Trying to enable male mesh");
            maleMesh.SetActive(true);
            femaleMesh.SetActive(false);
        }
        else
        {
            Debug.Log("<color=red>[Director]</color> Trying to enable female mesh");
            maleMesh.SetActive(false);
            femaleMesh.SetActive(true);
        }
    }

    void HandleSceneRequest(string sceneName)
    {
        Debug.Log("<color=green>[Director]</color> Received scene request → " + sceneName);
        GameManager.Instance.StartCoroutine(GameManager.Instance.SwapScene(sceneName));
    }
}
