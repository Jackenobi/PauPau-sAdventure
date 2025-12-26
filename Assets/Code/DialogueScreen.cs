using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueScreen : MonoBehaviour
{
    private DialogueLine currentLine;
    private string currentSpeaker;

    public event System.Action<string> onChoiceSelected;

    [Header("UI Panel")]
    public GameObject panel;

    [Header("Dialogue Layout")]
    public GameObject leftContainer;   // NPC
    public GameObject rightContainer;  // Player

    [Header("NPC (Left) UI")]
    public TMP_Text leftNameTMP;
    public TMP_Text leftDialogueTMP;
    public Image npcPortrait;

    [Header("Player (Right) UI")]
    public TMP_Text rightNameTMP;
    public TMP_Text rightDialogueTMP;
    public Image playerPortrait;

    [Header("Choices")]
    public GameObject[] choiceButtons;
    public GameObject continueButton;

    [Header("Camera & Input")]
    public CinemachineInputAxisController cinemachineController;
    public PlayerInput input;

    [Header("Quest Manager")]
    public MonoBehaviour questManager;
    private IQuestManager quest;


    [Header("Player Name")]
    public string playerName = "PaoPao";

    [Header("Portrait Pulse")]
    public float pulseScale = 1.15f;
    public float pulseDuration = 0.15f;

    private Coroutine portraitPulseRoutine;

    void Awake()
    {
        quest = questManager as IQuestManager;

        if (quest == null)
        {
            Debug.LogError("DialogueScreen: QuestManager implements kein IQuestManager!");
        }
    }


    public void ShowDialogue(DialogueLine dialogue, string npcFallbackName)
    {
        currentLine = dialogue;
        bool isPlayer = dialogue.player;

        // =========================
        // SPRECHERNAME AUFLÖSEN
        // =========================
        string resolvedSpeakerName =
            !string.IsNullOrWhiteSpace(dialogue.speakerName)
                ? dialogue.speakerName
                : (isPlayer ? playerName : npcFallbackName);

        currentSpeaker = resolvedSpeakerName;

        // =========================
        // RESET UI
        // =========================
        leftContainer.SetActive(false);
        rightContainer.SetActive(false);
        npcPortrait.gameObject.SetActive(false);
        playerPortrait.gameObject.SetActive(false);

        // =========================
        // PLAYER → RECHTS
        // =========================
        if (isPlayer)
        {
            rightContainer.SetActive(true);
            rightNameTMP.text = resolvedSpeakerName;
            rightDialogueTMP.text = dialogue.text;

            if (dialogue.playerPortrait != null)
            {
                playerPortrait.sprite = dialogue.playerPortrait;
                playerPortrait.gameObject.SetActive(true);
                PulsePortrait(playerPortrait);
            }
        }
        // =========================
        // NPC → LINKS
        // =========================
        else
        {
            leftContainer.SetActive(true);
            leftNameTMP.text = resolvedSpeakerName;
            leftDialogueTMP.text = dialogue.text;

            if (dialogue.npcPortrait != null)
            {
                npcPortrait.sprite = dialogue.npcPortrait;
                npcPortrait.gameObject.SetActive(true);
                PulsePortrait(npcPortrait);
            }
        }

        // =========================
        // CHOICES / CONTINUE
        // =========================
        bool hasChoices = dialogue.choices != null && dialogue.choices.Length > 0;

        // Alles aus
        for (int i = 0; i < choiceButtons.Length; i++)
            choiceButtons[i].SetActive(false);

        continueButton.SetActive(false);

        if (hasChoices)
        {
            for (int i = 0; i < dialogue.choices.Length && i < choiceButtons.Length; i++)
            {
                choiceButtons[i].SetActive(true);
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text =
                    dialogue.choices[i].text;
            }

            EventSystem.current.SetSelectedGameObject(choiceButtons[0]);
        }
        else
        {
            continueButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(continueButton);
        }

       
        // INPUT & CAMERA 
        input.SwitchCurrentActionMap("UI");
        cinemachineController.enabled = false;
        panel.SetActive(true);
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

        if (questManager != null)
        {
            quest.OnAnswerSelected(currentLine.choices[index].isCorrect);
        }

        if (currentLine.choices[index].nextLine != null)
        {
            ShowDialogue(currentLine.choices[index].nextLine, currentSpeaker);
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
            ShowDialogue(currentLine.nextLine, currentSpeaker);
        }
        else
        {
            HideDialogue();
        }
    }

    // =========================
    // PORTRAIT PULSE
    // =========================
    void PulsePortrait(Image portrait)
    {
        if (portraitPulseRoutine != null)
            StopCoroutine(portraitPulseRoutine);

        portraitPulseRoutine = StartCoroutine(PulseRoutine(portrait.transform));
    }

    IEnumerator PulseRoutine(Transform target)
    {
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * pulseScale;

        float t = 0f;
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(originalScale, targetScale, t / pulseDuration);
            yield return null;
        }

        t = 0f;
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(targetScale, originalScale, t / pulseDuration);
            yield return null;
        }

        target.localScale = originalScale;
    }
}
