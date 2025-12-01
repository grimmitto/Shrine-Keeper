using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NewGameUI : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject nameCanvas;
    public GameObject genderCanvas;

    [Header("Input")]
    public TMP_InputField nameInput;


    [Header("Preview Models")]
    public GameObject malePreview;
    public GameObject femalePreview;

    [Header("Scene After Character Creation")]
    public string OpeningCinematicScene = "FirstScene";
    private bool hasCompleted = false;

    private PlayerGender chosenGender = PlayerGender.Female;

    void Start()
    {
        nameCanvas.SetActive(true);
        genderCanvas.SetActive(false);

        malePreview.SetActive(false);
        femalePreview.SetActive(true);
    }

    // ----------------------------------------------------------
    // NAME SUBMISSION
    // ----------------------------------------------------------
    public void OnSubmitName()
    {
        var save = SaveManager.Instance.saveInstance;

        string entered = nameInput.text.Trim();
        save.playerName = string.IsNullOrWhiteSpace(entered) ? "Unnamed" : entered;

        nameCanvas.SetActive(false);
        genderCanvas.SetActive(true);
    }

    // ----------------------------------------------------------
    // GENDER SELECTION
    // ----------------------------------------------------------
    public void OnMaleSelect()
    {
        chosenGender = PlayerGender.Male;

        malePreview.SetActive(true);
        femalePreview.SetActive(false);
    }

    public void OnFemaleSelect()
    {
        chosenGender = PlayerGender.Female;

        malePreview.SetActive(false);
        femalePreview.SetActive(true);
    }

    // ----------------------------------------------------------
    // FINISH CREATION → LOAD FIRST SCENE
    // ----------------------------------------------------------


    public void OnCompleteCharacterCreation()
    {
        if (hasCompleted) return; // prevent double-trigger
        hasCompleted = true;

        var save = SaveManager.Instance.saveInstance;

        // Save chosen gender into SaveData
        save.playerGender = chosenGender;

        GameManager.Instance.playerGender = chosenGender;

        // Update lastScene so future loads know where to go
        save.lastScene = OpeningCinematicScene;

        // Optionally hide this UI so it can't be clicked again
        // gameObject.SetActive(false);

        // Ask GameManager to swap to the opening cinematic scene
        GameManager.Instance.StartCoroutine(
            GameManager.Instance.SwapScene(OpeningCinematicScene)
        );
    }

}
