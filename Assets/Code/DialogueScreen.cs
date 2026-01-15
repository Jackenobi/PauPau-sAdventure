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

    [Header("Quest Manager")]
    public MonoBehaviour questManagerObject;

    private IQuestManager questManager;

    void Start()
    {
        // Hol dir das Interface vom MonoBehaviour
        if (questManagerObject != null)
            questManager = questManagerObject.GetComponent<IQuestManager>();

        panel.SetActive(false);
    }

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
    public GameObject continueButton;

    [Header("Camera & Input")]
    public CinemachineInputAxisController cinemachineController;
    public PlayerInput input;

    [Header("Player Name")]
    public string playerName = "PaoPao";

    [Header("Portrait Pulse")]
    public float pulseScale = 1.15f;
    public float pulseDuration = 0.15f;

    private Coroutine portraitPulseRoutine;


    // FMOD

    private EventInstance voiceInstance;


    // SHOW DIALOGUE

    public void ShowDialogue(DialogueLine dialogue, string npcFallbackName)
    {
        currentLine = dialogue;
        bool isPlayer = dialogue.player;

        // Speaker Name

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


        // Play Voice

        PlayVoice(dialogue);


        // Choices / Continue

        bool hasChoices = dialogue.choices != null && dialogue.choices.Length > 0;

        foreach (var btn in choiceButtons)
            btn.SetActive(false);

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

        input.SwitchCurrentActionMap("UI");
        cinemachineController.enabled = false;
        panel.SetActive(true);
    }

    // FMOD VOICE
   
    void PlayVoice(DialogueLine line)
    {
        if (!line.voiceEvent.IsNull)
        {
            voiceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            voiceInstance.release();

            voiceInstance = RuntimeManager.CreateInstance(line.voiceEvent);
            voiceInstance.start();
        }
    }

    public void HideDialogue()
    {
        voiceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        voiceInstance.release();

        input.SwitchCurrentActionMap("Player");
        panel.SetActive(false);
        cinemachineController.enabled = true;
    }

    public void SelectChoice(int index)
    {
        // Quest Manager über richtige/falsche Antwort informieren
        if (questManager != null && currentLine.choices.Length > 0)
        {
            questManager.OnAnswerSelected(currentLine.choices[index].isCorrect);
        }

        if (currentLine.choices[index].nextLine != null)
            ShowDialogue(currentLine.choices[index].nextLine, currentSpeaker);
        else
            HideDialogue();
    }

    public void Continue()
    {
        if (currentLine.nextLine != null)
            ShowDialogue(currentLine.nextLine, currentSpeaker);
        else
            HideDialogue();
    }

    
    // Portrait Pulse
   
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
