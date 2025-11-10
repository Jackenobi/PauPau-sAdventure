using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueScreen : MonoBehaviour
{
    private DialogueLine currentLine;
    private string currentSpeaker;

    // Event für Choice-Auswahl
    public event System.Action<string> onChoiceSelected;

    [Header("UI References")]
    public GameObject panel;
    public TMP_Text nameTMP;
    public TMP_Text dialogueTextTMP;
    public GameObject[] choiceButtons;
    public GameObject continueButton;

    [Header("Portraits")]
    public Image npcPortrait;          // Linkes Portrait
    public Image characterPortrait;    // Rechtes Portrait

    [Header("Dialogue Screens")]
    public playerdialogueScreen playerdialoguescreen;

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
        currentSpeaker = speakerName;

        // Namen und Text anzeigen
        nameTMP.text = speakerName;
        dialogueTextTMP.text = dialogue.text;

        //  Portraits aktualisieren
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

        //Choices vorbereiten
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

        //Continue-Button aktivieren, wenn keine Choices da sind
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

        //Action Map umschalten
        input.SwitchCurrentActionMap("UI");
        panel.SetActive(true);
    }

    public void HideDialogue()
    {
        input.SwitchCurrentActionMap("Player");
        panel.SetActive(false);
    }

    public void SelectChoice(int index)
    {
        onChoiceSelected?.Invoke(currentLine.choices[index].id);

        if (currentLine.choices[index].nextLine != null)
        {
            if (currentLine.choices[index].nextLine.player)
            {
                panel.SetActive(false);
                playerdialoguescreen.ShowDialogue(currentLine.choices[index].nextLine, currentSpeaker);
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
            if (currentLine.nextLine.player)
            {
                panel.SetActive(false);
                playerdialoguescreen.ShowDialogue(currentLine.nextLine, currentSpeaker);
            }
            else
            {
                ShowDialogue(currentLine.nextLine, "");
            }
        }
        else
        {
            HideDialogue();
        }
    }
}