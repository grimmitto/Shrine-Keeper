using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;

public class DialogueUI : MonoBehaviour
{
    [Header("Roots")]
    public GameObject dialogueUIRoot;   // panel with speaker + text + next button
    public GameObject choiceUIRoot;     // panel that holds the 1–3 choice buttons

    [Header("Text")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;

    [Header("Preset Choice Buttons (1–3)")]
    public Button choiceButton1;
    public Button choiceButton2;
    public Button choiceButton3;

    public TextMeshProUGUI choiceText1;
    public TextMeshProUGUI choiceText2;
    public TextMeshProUGUI choiceText3;

    private int choiceIndex1 = -1;
    private int choiceIndex2 = -1;
    private int choiceIndex3 = -1;

    void OnEnable()
    {
        DialogueManager.Instance.OnLineReady     += DisplayLine;
        DialogueManager.Instance.OnSpeakerChanged += DisplaySpeaker;
        DialogueManager.Instance.OnDialogueEnd   += HideUI;
        DialogueManager.Instance.OnChoicesReady  += ShowChoices;
    }

    void OnDisable()
    {
        if (DialogueManager.Instance == null) return;

        DialogueManager.Instance.OnLineReady     -= DisplayLine;
        DialogueManager.Instance.OnSpeakerChanged -= DisplaySpeaker;
        DialogueManager.Instance.OnDialogueEnd   -= HideUI;
        DialogueManager.Instance.OnChoicesReady  -= ShowChoices;
    }

    // Called by your "Next" button via UnityEvent
    public void OnNextPressed()
    {
        // If we’re showing a normal line, just continue
        ClearChoices();
        DialogueManager.Instance.ContinueStory();
    }

    void DisplaySpeaker(string s)
    {
        if (speakerText != null)
            speakerText.text = s;
    }

    void DisplayLine(string line)
    {
        if (dialogueText != null)
            dialogueText.text = line;

        // When a new line shows, hide any old choices
        ClearChoices();

        // Make sure main dialogue root is visible
        if (dialogueUIRoot != null)
            dialogueUIRoot.SetActive(true);
    }

    void HideUI()
    {
        // Called on -> END
        if (dialogueUIRoot != null)
            dialogueUIRoot.SetActive(false);

        if (choiceUIRoot != null)
            choiceUIRoot.SetActive(false);

        ClearChoices();
    }

    // ----------------------
    // CHOICES
    // ----------------------
    void ShowChoices(Choice[] choices)
    {
        // Reset existing indices
        choiceIndex1 = -1;
        choiceIndex2 = -1;
        choiceIndex3 = -1;

        // Make sure choice root is visible
        if (choiceUIRoot != null)
            choiceUIRoot.SetActive(true);

        // Hide all buttons first
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);

        // One choice → Button 1
        if (choices.Length == 1)
        {
            choiceButton1.gameObject.SetActive(true);
            choiceText1.text = choices[0].text;

            choiceIndex1 = choices[0].index;
        }
        // Two choices → Buttons 2 & 3
        else if (choices.Length == 2)
        {
            choiceButton2.gameObject.SetActive(true);
            choiceButton3.gameObject.SetActive(true);

            choiceText2.text = choices[0].text;
            choiceText3.text = choices[1].text;

            choiceIndex2 = choices[0].index;
            choiceIndex3 = choices[1].index;
        }
        // Three choices → Buttons 1, 2, 3
        else if (choices.Length == 3)
        {
            choiceButton1.gameObject.SetActive(true);
            choiceButton2.gameObject.SetActive(true);
            choiceButton3.gameObject.SetActive(true);

            choiceText1.text = choices[0].text;
            choiceText2.text = choices[1].text;
            choiceText3.text = choices[2].text;

            choiceIndex1 = choices[0].index;
            choiceIndex2 = choices[1].index;
            choiceIndex3 = choices[2].index;
        }
    }

    void ClearChoices()
    {
        // Hide choice root & buttons
        if (choiceUIRoot != null)
            choiceUIRoot.SetActive(false);

        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
        choiceButton3.gameObject.SetActive(false);

        choiceIndex1 = -1;
        choiceIndex2 = -1;
        choiceIndex3 = -1;
    }

    // ---------- Button events wired in Inspector ----------
    public void OnChoiceButton1()
    {
        if (choiceIndex1 >= 0)
            Choose(choiceIndex1);
    }

    public void OnChoiceButton2()
    {
        if (choiceIndex2 >= 0)
            Choose(choiceIndex2);
    }

    public void OnChoiceButton3()
    {
        if (choiceIndex3 >= 0)
            Choose(choiceIndex3);
    }

    void Choose(int index)
    {
        ClearChoices(); // hide choice UI after selection
        DialogueManager.Instance.ChooseChoiceIndex(index);
    }
}
