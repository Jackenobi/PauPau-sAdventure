using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class playerdialogueScreen : MonoBehaviour
{
    private DialogueLine currentLine;
    private string currentSpeaker;
    public string playerName = "PaoPao";



    public event System.Action<string> onChoiceSelected;

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text nameTMP;
    public TMP_Text dialogueTextTMP;
    public GameObject[] choiceButtons;
    public GameObject continueButton;
    public CinemachineInputAxisController cinemachineController;

    [Header("Portraits")]
    public Image npcPortrait;         
    public Image characterPortrait;   

    [Header("Dialogue Screens")]
    public DialogueScreen npcDialogueScreen;

    [Header("Input")]
    public PlayerInput input;


    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }


    public void ShowDialogue(DialogueLine dialogue, string speakerName)
    {
        currentLine = dialogue;
        currentSpeaker = "PaoPao";

        nameTMP.text = speakerName;
        dialogueTextTMP.text = dialogue.text;

        //Portraits aktualisieren
        if (npcPortrait != null)
        {
            if (dialogue.npcPortrait != null)
            {
                npcPortrait.sprite = dialogue.npcPortrait;
                npcPortrait.gameObject.SetActive(true);
            }
            else
            {
                npcPortrait.gameObject.SetActive(false);
            }
        }

        if (characterPortrait != null)
        {
            if (dialogue.playerPortrait != null)
            {
                characterPortrait.sprite = dialogue.playerPortrait;
                characterPortrait.gameObject.SetActive(true);
            }
            else
            {
                characterPortrait.gameObject.SetActive(false);
            }
        }

        //Choices anzeigen
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < dialogue.choices.Length)
            {
                choiceButtons[i].SetActive(true);
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text = dialogue.choices[i].text;
            }
            else
            {
                choiceButtons[i].SetActive(false);
            }
        }

        //Wenn keine Choices Continue-Button anzeigen
        if (dialogue.choices.Length == 0)
        {
            continueButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(continueButton);
        }
        else
        {
            continueButton.SetActive(false);
            EventSystem.current.SetSelectedGameObject(choiceButtons[0]);
        }

        input.SwitchCurrentActionMap("UI");
        panel.SetActive(true);
        cinemachineController.enabled = false;
    }


    public void HideDialogue()
    {
        input.SwitchCurrentActionMap("Player");
        panel.SetActive(false);
        cinemachineController.enabled = true;
    }


    public void SelectChoice(int index)
    {
        onChoiceSelected?.Invoke(currentLine.choices[index].id);

        if (currentLine.choices[index].nextLine != null)
        {
            if (!currentLine.choices[index].nextLine.player)
            {
                panel.SetActive(false);
                npcDialogueScreen.ShowDialogue(currentLine.choices[index].nextLine, currentSpeaker);
            }
            else
            {
                ShowDialogue(currentLine.choices[index].nextLine, currentSpeaker);
            }
        }
        else
        {
            HideDialogue();
        }
    }


    public void Continue()
    {
        if (currentLine.nextLine != null)
        {
            if (!currentLine.nextLine.player)
            {
                panel.SetActive(false);
                npcDialogueScreen.ShowDialogue(currentLine.nextLine, currentSpeaker);
            }
            else
            {
                ShowDialogue(currentLine.nextLine, playerName);
            }
        }
        else
        {
            HideDialogue();
        }
    }
}

