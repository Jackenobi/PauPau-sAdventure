using TMPro;
using UnityEngine;

public class DialogueScreen : MonoBehaviour
{
    private DialogueLine currentLine;

    public GameObject panel;
    public TMP_Text nameTMP;
    public TMP_Text dialogueTextTMP;
    public GameObject[] choiceButtons;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ShowDialogue(DialogueLine dialogue, string speakerName)
    {
        currentLine = dialogue;

        nameTMP.text = speakerName;
        dialogueTextTMP.text = dialogue.text;

        panel.SetActive(true);
    }

    public void HideDialogue()
    {
        panel.SetActive(false);
    }

    public void SelectChoice(int index)
    {
        ShowDialogue(currentLine.choices[index].nextLine, "");
    }

}
