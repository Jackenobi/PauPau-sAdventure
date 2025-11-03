using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DialogueScreen : MonoBehaviour
{
    private DialogueLine currentLine;
    private string currentSpeaker;

    //string ist die ID der Choice
    public event System.Action<string> onChoiceSelected;

    public GameObject panel;
    public TMP_Text nameTMP;
    public TMP_Text dialogueTextTMP;
    public GameObject[] choiceButtons;
    public GameObject continueButton;

    public PlayerInput input;
    

    public Image npcPortrait;
    public Image characterPortrait;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ShowDialogue(DialogueLine dialogue, string speakerName)
    {
        currentLine = dialogue;
        currentSpeaker = speakerName;

        nameTMP.text = speakerName;
        dialogueTextTMP.text = dialogue.text;

        //for Loop for choices 
        for(int i = 0; i < choiceButtons.Length; i++)
        {
            if(i < dialogue.choices.Length)
            {
                choiceButtons[i].SetActive(true);
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text = dialogue.choices[i].text;
            }
            else
            {
                choiceButtons[i].SetActive(false);
            }
        }

        if(dialogue.choices.Length == 0)
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
    }

    public void HideDialogue()
    {
        input.SwitchCurrentActionMap("Player");
        
        panel.SetActive(false);
    }

    public void SelectChoice(int index)
    {
        onChoiceSelected?.Invoke(currentLine.choices[index].id);
        ShowDialogue(currentLine.choices[index].nextLine, currentSpeaker);
    }

    public void Continue()
    {
        if(currentLine.nextLine != null)
        {
            ShowDialogue(currentLine.nextLine, "");
        }
        else
        {
            HideDialogue();
        }
    }


}
