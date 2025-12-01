using UnityEngine;

public class MainMenuSceneHelper : MonoBehaviour
{

    public GameObject MainCanvas;
    public GameObject SlotCanvas;


    [Header("Save Slots")]
    public GameObject Slot1;
    public GameObject Slot1Empty;
    public GameObject Slot2;
    public GameObject Slot2Empty;
    public GameObject Slot3;
    public GameObject Slot3Empty;

    private bool oneTaken;
    private bool twoTaken;
    private bool threeTaken;




    void Start()
    {
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        oneTaken = SaveUtility.SaveExists(1);
        twoTaken = SaveUtility.SaveExists(2);
        threeTaken = SaveUtility.SaveExists(3);

        if (!oneTaken)
        {
            Slot1Empty.SetActive(true);
        }
        else { Slot1Empty.SetActive(false); }

        if (!twoTaken)
        {
            Slot2Empty.SetActive(true);
        }
        else { Slot2Empty.SetActive(false); }

        if (!threeTaken)
        {
            Slot3Empty.SetActive(true);
        }
        else { Slot3Empty.SetActive(false); }
    }

    public void PlaySlot(int num)
    {
        if (num == 1)
        {
            PlaySlot1();
        }
        if (num == 2)
        {
            PlaySlot2();
        }
        if (num == 3)
        {
            PlaySlot3();
        }
    }

    public void PlaySlot1()
    {
        GameManager.Instance.SetSlot(1);
        if (!oneTaken)
        {
            GameManager.Instance.NewGame();
        }
        else
        {
            GameManager.Instance.LoadGame();
        }

    }

    public void PlaySlot2()
    {
        GameManager.Instance.SetSlot(2);
        if (!twoTaken)
        {
            GameManager.Instance.NewGame();
        }
        else
        {
            GameManager.Instance.LoadGame();
        }
    }

    public void PlaySlot3()
    {
        GameManager.Instance.SetSlot(3);
        if (!threeTaken)
        {
            GameManager.Instance.NewGame();
        }
        else
        {
            GameManager.Instance.LoadGame();
        }
    }

    public void PlayButtonClick()
    {
        SlotCanvas.SetActive(true);
        MainCanvas.SetActive(false);
    }
    public void GoBackClicked()
    {
        MainCanvas.SetActive(true);
        SlotCanvas.SetActive(false);
    }

    public void ExitButtonClick()
    {

        Application.Quit();


    }


}
