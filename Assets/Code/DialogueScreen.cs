using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class DialogueScreen : MonoBehaviour
{
    private DialogueLine currentLine;
    private string currentSpeaker;
    private bool dialogueActive;

    [Header("Quest Manager")]
    public MonoBehaviour questManagerObject;
    private IQuestManager questManager;

    [Header("UI Panel")]
    public GameObject panel;

    [Header("Dialogue Layout")]
    public GameObject leftContainer;
    public GameObject rightContainer;

    [Header("NPC UI")]
    public TMP_Text leftNameTMP;
    public TMP_Text leftDialogueTMP;
    public Image npcPortrait;

    [Header("Player UI")]
    public TMP_Text rightNameTMP;
    public TMP_Text rightDialogueTMP;
    public Image playerPortrait;

    [Header("Choices")]
    public GameObject[] choiceButtons;

    [Header("Camera & Input")]
    public CinemachineInputAxisController cinemachineController;
    public PlayerInput input;

    [Header("Player Name")]
    public string playerName = "PaoPao";

    [Header("Portrait Pulse")]
    public float pulseScale = 1.15f;
    public float pulseDuration = 0.15f;

    private Coroutine portraitPulseRoutine;

    // Input
    private InputAction submitAction;

    // FMOD
    private EventInstance voiceInstance;

    void Start()
    {
        if (questManagerObject != null)
            questManager = questManagerObject.GetComponent<IQuestManager>();

        submitAction = input.actions.FindAction("Submit");
        submitAction.performed += OnSubmitPressed;

        panel.SetActive(false);
        dialogueActive = false;
    }

    void OnDestroy()
    {
        submitAction.performed -= OnSubmitPressed;
    }

    // =========================
    // SHOW DIALOGUE
    // =========================
    public void ShowDialogue(DialogueLine dialogue, string npcFallbackName)
    {
        dialogueActive = true;
        currentLine = dialogue;

        bool isPlayer = dialogue.player;

        currentSpeaker =
            !string.IsNullOrWhiteSpace(dialogue.speakerName)
                ? dialogue.speakerName
                : (isPlayer ? playerName : npcFallbackName);

        // Reset UI
        leftContainer.SetActive(false);
        rightContainer.SetActive(false);
        npcPortrait.gameObject.SetActive(false);
        playerPortrait.gameObject.SetActive(false);

        // Player
        if (isPlayer)
        {
            rightContainer.SetActive(true);
            rightNameTMP.text = currentSpeaker;
            rightDialogueTMP.text = dialogue.text;

            if (dialogue.playerPortrait != null)
            {
                playerPortrait.sprite = dialogue.playerPortrait;
                playerPortrait.gameObject.SetActive(true);
                PulsePortrait(playerPortrait);
            }
        }
        // NPC
        else
        {
            leftContainer.SetActive(true);
            leftNameTMP.text = currentSpeaker;
            leftDialogueTMP.text = dialogue.text;

            if (dialogue.npcPortrait != null)
            {
                npcPortrait.sprite = dialogue.npcPortrait;
                npcPortrait.gameObject.SetActive(true);
                PulsePortrait(npcPortrait);
            }
        }

        // Voice
        PlayVoice(dialogue);

        // Choices
        bool hasChoices = dialogue.choices != null && dialogue.choices.Length > 0;

        foreach (var btn in choiceButtons)
            btn.SetActive(false);

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
            EventSystem.current.SetSelectedGameObject(null);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        input.SwitchCurrentActionMap("UI");
        cinemachineController.enabled = false;
        panel.SetActive(true);
    }

    // =========================
    // SUBMIT INPUT
    // =========================
    private void OnSubmitPressed(InputAction.CallbackContext ctx)
    {
        if (!dialogueActive)
            return;

        // Wenn Choices sichtbar sind → UI übernimmt
        foreach (var btn in choiceButtons)
        {
            if (btn.activeSelf)
                return;
        }

        Continue();
    }

    // =========================
    // CONTINUE
    // =========================
    public void Continue()
    {
        if (currentLine.nextLine != null)
            ShowDialogue(currentLine.nextLine, currentSpeaker);
        else
            HideDialogue();
    }

    // =========================
    // CHOICES
    // =========================
    public void SelectChoice(int index)
    {
        if (questManager != null && currentLine.choices.Length > 0)
        {
            questManager.OnAnswerSelected(currentLine.choices[index].isCorrect);
        }

        if (currentLine.choices[index].nextLine != null)
            ShowDialogue(currentLine.choices[index].nextLine, currentSpeaker);
        else
            HideDialogue();
    }

    // =========================
    // HIDE
    // =========================
    public void HideDialogue()
    {
        dialogueActive = false;

        voiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        voiceInstance.release();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        input.SwitchCurrentActionMap("Player");
        panel.SetActive(false);
        cinemachineController.enabled = true;
    }

    // =========================
    // VOICE
    // =========================
    void PlayVoice(DialogueLine line)
    {
        if (!line.voiceEvent.IsNull)
        {
            voiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            voiceInstance.release();

            voiceInstance = RuntimeManager.CreateInstance(line.voiceEvent);
            voiceInstance.start();
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
        Vector3 original = target.localScale;
        Vector3 targetScale = original * pulseScale;

        float t = 0f;
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(original, targetScale, t / pulseDuration);
            yield return null;
        }

        t = 0f;
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(targetScale, original, t / pulseDuration);
            yield return null;
        }

        target.localScale = original;
    }
}
