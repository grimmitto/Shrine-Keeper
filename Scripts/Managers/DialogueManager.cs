using UnityEngine;
using Ink.Runtime;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextAsset inkJSON;

    private Story story;

    // Events for UI
    public event Action<string> OnLineReady;
    public event Action<string> OnSpeakerChanged;
    public event Action OnDialogueEnd;
    public event Action<Choice[]> OnChoicesReady;

    // NEW: Scene request event
    public event Action<string> OnSceneRequest;
    public string LastRequestedScene;

    void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(string knotName)
    {
        // Enable Dialogue UI root
        DialogueUI ui = FindObjectOfType<DialogueUI>(true);
        if (ui != null && ui.dialogueUIRoot != null)
            ui.dialogueUIRoot.SetActive(true);

        story = new Story(inkJSON.text);

        // Inject Player Name if present
        var save = SaveManager.Instance?.saveInstance;
        if (save != null && !string.IsNullOrEmpty(save.playerName))
            story.variablesState["User"] = save.playerName;

        // BIND EXTERNAL FUNCTION FOR SCENE LOADING
        story.BindExternalFunction("RequestScene", (string sceneName) =>
        {
            Debug.Log("<color=orange>[DialogueManager]</color> Ink requested scene: " + sceneName);

            LastRequestedScene = sceneName;
            OnSceneRequest?.Invoke(sceneName);
        });

        story.ChoosePathString(knotName);

        ContinueStory();
    }

    public void ContinueStory()
    {
        if (!story.canContinue)
        {
            // Choices?
            if (story.currentChoices.Count > 0)
            {
                OnChoicesReady?.Invoke(story.currentChoices.ToArray());
                return;
            }

            // End of story
            OnDialogueEnd?.Invoke();
            return;
        }

        string rawLine = story.Continue().Trim();

        string speaker = "";
        string text = rawLine;

        int idx = rawLine.IndexOf(':');
        if (idx != -1)
        {
            speaker = rawLine.Substring(0, idx).Trim();
            text = rawLine.Substring(idx + 1).Trim();
            OnSpeakerChanged?.Invoke(speaker);
        }

        OnLineReady?.Invoke(text);
    }

    public void ChooseChoiceIndex(int index)
    {
        story.ChooseChoiceIndex(index);

        ContinueStory();
    }
}
