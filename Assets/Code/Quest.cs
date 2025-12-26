using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Quest : MonoBehaviour, IQuestManager

{
    [Header("UI")]
    public DialogueScreen dialogueScreen;
    public Transform questScreen;
    public GameObject questDisplayPrefab;
    public Inventory inventory;
    public CanvasGroup blackScreen;

    [Header("Quest: Nachbarn")]
    public NPCs[] nachbarn;
    public Item mapItem;
    public DialogueLine[] nachbarnDuring;
    public DialogueLine[] nachbarnAfter;

    [Header("Quest: Bauarbeiter")]
    public NPCs bauarbeiter;
    public Item tapeItem;
    public GameObject brueckeKaputt;
    public GameObject brueckeHeil;
    public DialogueLine bauarbeiterBefore;
    public DialogueLine bauarbeiterDuring;
    public DialogueLine bauarbeiterNoItem;
    public DialogueLine bauarbeiterDone;
    public DialogueLine bauarbeiterAfter;

    [Header("Quest: Seemann")]
    public NPCs seemann;
    public DialogueLine seemannBefore;
    public DialogueLine seemannDuring;
    public DialogueLine seemannNoItem;
    public DialogueLine seemannDone;
    public string tempelEingangSceneName = "Tempeleingang";

    [Header("Quest: Möwe")]
    public NPCs moewe;
    public Item[] eier;
    public GameObject[] rewardObjects;
    public DialogueLine moeweBefore;
    public DialogueLine moeweDuring;
    public DialogueLine moeweNotAllEggs;
    public DialogueLine moeweEggsComplete;
    public DialogueLine moeweAfter;

    [Header("Quest: Schaf/Marco")]
    public NPCs schaf;
    public DialogueLine schafBefore;
    public DialogueLine[] schafQuestions;
    public DialogueLine schafWrongAnswer;
    public DialogueLine schafComplete;

    [Header("Quest: Opossum Versteckspiel")]
    public NPCs opossum;
    public Transform opossumVersteckPosition;
    public Transform playerSpawnPosition;
    public DialogueLine opossumBefore;
    public DialogueLine opossumStart;
    public DialogueLine opossumFound;
    public DialogueLine zumWald;
    public GameObject[] npcsToDisableAfterOpossum;

    [Header("Quest: Wald")]
    public GameObject waldEingang;
    public string waldSceneName = "Wald";

    private bool nachbarnQuestActive = false;
    private bool nachbarnQuestDone = false;
    private bool nachbarnAfterUnlocked = false;
    private bool bauarbeiterQuestActive = false;
    private bool bauarbeiterQuestStarted = false;
    private bool bauarbeiterQuestDone = false;
    private bool seemannQuestActive = false;
    private bool seemannQuestStarted = false;
    private bool seemannQuestDone = false;
    private bool moeweQuestActive = false;
    private bool moeweQuestStarted = false;
    private bool moeweQuestDone = false;
    private bool moeweRewardGiven = false;
    private bool schafQuestActive = false;
    private bool schafQuestStarted = false;
    private bool schafQuestDone = false;
    private bool opossumQuestActive = false;
    private bool opossumQuestStarted = false;
    private bool opossumQuestDone = false;
    private bool opossumIsHiding = false;
    private bool waldQuestActive = false;
    private TMP_Text questTMP;
    private int talkedToCount = 0;
    private int collectedEggs = 0;
    private int correctAnswers = 0;

    void Start()
    {
        foreach (var npc in nachbarn)
        {
            npc.onInteracted += () => OnNeighborTalked(npc);
        }

        if (bauarbeiter != null)
        {
            bauarbeiter.onInteracted += OnBauarbeiterTalked;
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
        }

        if (seemann != null)
        {
            seemann.onInteracted += OnSeemannTalked;
            if (seemannBefore != null)
                seemann.dialogue = seemannBefore;
        }

        if (moewe != null)
        {
            moewe.onInteracted += OnMoeweTalked;
            moeweQuestActive = true;
            if (moeweBefore != null)
                moewe.dialogue = moeweBefore;
        }

        if (schaf != null)
        {
            schaf.onInteracted += OnSchafTalked;
            if (schafBefore != null)
                schaf.dialogue = schafBefore;
        }

        if (opossum != null)
        {
            opossum.onInteracted += OnOpossumTalked;
            if (opossumBefore != null)
                opossum.dialogue = opossumBefore;
        }

        if (waldEingang != null)
        {
            var interactable = waldEingang.GetComponent<Interactable>();
            if (interactable != null)
                interactable.onInteracted += OnWaldEingangInteracted;

            waldEingang.SetActive(false);
        }

        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }

        if (mapItem != null)
            mapItem.gameObject.SetActive(false);

        if (eier != null)
        {
            foreach (var ei in eier)
            {
                if (ei != null)
                    ei.gameObject.SetActive(false);
            }
        }

        if (rewardObjects != null)
        {
            foreach (var obj in rewardObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (nachbarnQuestDone && !nachbarnAfterUnlocked && inventory.HasItem(ItemType.Map))
        {
            nachbarnAfterUnlocked = true;
            for (int i = 0; i < nachbarn.Length; i++)
            {
                if (i < nachbarnAfter.Length && nachbarnAfter[i] != null)
                    nachbarn[i].dialogue = nachbarnAfter[i];
            }
        }

        if (bauarbeiterQuestDone && nachbarnAfterUnlocked && !seemannQuestActive)
        {
            seemannQuestActive = true;
            if (seemannDuring != null)
                seemann.dialogue = seemannDuring;
        }

        if (moeweQuestStarted && !moeweQuestDone)
        {
            int eggsInInventory = inventory.CountItems(ItemType.Egg);
            if (eggsInInventory != collectedEggs)
            {
                collectedEggs = eggsInInventory;
                questTMP.text = $"Collect all eggs ({collectedEggs}/8)";

                if (collectedEggs >= 8)
                {
                    moeweQuestDone = true;
                    questTMP.text = "All eggs collected! Return to the seagull";
                    if (moeweEggsComplete != null)
                        moewe.dialogue = moeweEggsComplete;
                }
            }
        }

        if (moeweQuestDone && !schafQuestActive)
        {
            schafQuestActive = true;
        }

        if (schafQuestDone && !opossumQuestActive)
        {
            opossumQuestActive = true;
            if (opossumStart != null)
                opossum.dialogue = opossumStart;
        }

        if (opossumQuestDone && !waldQuestActive)
        {
            waldQuestActive = true;
            if (zumWald != null)
            {
                if (schaf != null)
                    schaf.dialogue = zumWald;
                if (opossum != null)
                    opossum.dialogue = zumWald;
            }
            if (questTMP != null)
                questTMP.text = "Explore the hidden forest";
        }
    }

    private void OnNeighborTalked(NPCs npc)
    {
        int index = System.Array.IndexOf(nachbarn, npc);
        if (index < 0) return;

        if (!nachbarnQuestActive && !nachbarnQuestDone)
        {
            nachbarnQuestActive = true;
            StartCoroutine(QuestNachbarn());
        }

        if (nachbarnQuestActive && !nachbarnQuestDone)
        {
            if (!npc.hasSpoken)
            {
                npc.hasSpoken = true;
                talkedToCount++;
                questTMP.text = $"Talked to {talkedToCount}/{nachbarn.Length} neighbors";

                if (talkedToCount >= nachbarn.Length)
                {
                    nachbarnQuestDone = true;
                    questTMP.text = "All neighbors talked to!";
                    StartCoroutine(CompleteNeighborQuest());
                }

                StartCoroutine(SwitchToDuringAfterDialogue(npc, index));
            }
            else
            {
                if (nachbarnDuring != null && index < nachbarnDuring.Length && nachbarnDuring[index] != null)
                    npc.dialogue = nachbarnDuring[index];
            }
        }
        else if (nachbarnQuestDone)
        {
            if (nachbarnAfterUnlocked && nachbarnAfter != null && index < nachbarnAfter.Length && nachbarnAfter[index] != null)
                npc.dialogue = nachbarnAfter[index];
        }
    }

    IEnumerator SwitchToDuringAfterDialogue(NPCs npc, int index)
    {
        yield return new WaitForSeconds(0.1f);
        if (nachbarnDuring != null && index < nachbarnDuring.Length && nachbarnDuring[index] != null)
            npc.dialogue = nachbarnDuring[index];
    }

    IEnumerator QuestNachbarn()
    {
        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Talk to your neighbors";
        yield return null;
    }

    IEnumerator CompleteNeighborQuest()
    {
        yield return new WaitForSeconds(1f);
        bauarbeiterQuestActive = true;
        if (bauarbeiterDuring != null)
            bauarbeiter.dialogue = bauarbeiterDuring;
    }

    private void OnBauarbeiterTalked()
    {
        if (!bauarbeiterQuestActive)
        {
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
            return;
        }

        if (!bauarbeiterQuestStarted)
        {
            bauarbeiterQuestStarted = true;
            StartCoroutine(QuestBauarbeiter());
            return;
        }

        if (bauarbeiterQuestStarted && !bauarbeiterQuestDone && !inventory.HasItem(ItemType.Tape))
        {
            if (bauarbeiterNoItem != null)
                bauarbeiter.dialogue = bauarbeiterNoItem;
            return;
        }

        if (bauarbeiterQuestStarted && !bauarbeiterQuestDone && inventory.HasItem(ItemType.Tape))
        {
            if (bauarbeiterDone != null)
                bauarbeiter.dialogue = bauarbeiterDone;
            StartCoroutine(CompleteBauarbeiterQuest());
            return;
        }

        if (bauarbeiterQuestDone)
        {
            if (bauarbeiterAfter != null)
                bauarbeiter.dialogue = bauarbeiterAfter;
        }
    }

    IEnumerator QuestBauarbeiter()
    {
        questTMP.text = "Find some Tape";
        if (tapeItem != null)
            tapeItem.gameObject.SetActive(true);
        bauarbeiter.dialogue = bauarbeiterDuring;
        yield return WaitForItem(ItemType.Tape, 1, null);
        questTMP.text = "Bring the tape to the construction worker";
        if (bauarbeiterDone != null)
            bauarbeiter.dialogue = bauarbeiterDone;
    }

    IEnumerator CompleteBauarbeiterQuest()
    {
        bauarbeiterQuestDone = true;

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float fadeTime = 1f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 1f;
        }

        yield return new WaitForSeconds(2f);

        if (brueckeKaputt != null)
            brueckeKaputt.SetActive(false);
        if (brueckeHeil != null)
            brueckeHeil.SetActive(true);

        if (blackScreen != null)
        {
            float fadeTime = 1f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }

        questTMP.text = "Bridge fixed!";
        if (bauarbeiterAfter != null)
            bauarbeiter.dialogue = bauarbeiterAfter;
        if (tapeItem != null)
            tapeItem.gameObject.SetActive(false);
    }

    private void OnSeemannTalked()
    {
        if (!seemannQuestActive)
        {
            if (seemannBefore != null)
                seemann.dialogue = seemannBefore;
            return;
        }

        if (!seemannQuestStarted)
        {
            seemannQuestStarted = true;
            StartCoroutine(QuestSeemann());
            return;
        }

        if (seemannQuestStarted && !seemannQuestDone && !inventory.HasItem(ItemType.Map))
        {
            if (seemannNoItem != null)
                seemann.dialogue = seemannNoItem;
            return;
        }

        if (seemannQuestStarted && !seemannQuestDone && inventory.HasItem(ItemType.Map))
        {
            if (seemannDone != null)
                seemann.dialogue = seemannDone;
            StartCoroutine(CompleteSeemannQuest());
            return;
        }
    }

    IEnumerator QuestSeemann()
    {
        questTMP.text = "Find the Map";
        if (mapItem != null && !mapItem.gameObject.activeSelf)
            mapItem.gameObject.SetActive(true);
        seemann.dialogue = seemannDuring;
        yield return WaitForItem(ItemType.Map, 1, null);
        questTMP.text = "Bring the map to the sailor";
        if (seemannDone != null)
            seemann.dialogue = seemannDone;
    }

    IEnumerator CompleteSeemannQuest()
    {
        seemannQuestDone = true;
        yield return new WaitForSeconds(1f);
        questTMP.text = "Setting sail...";

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float fadeTime = 1f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 1f;
        }

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(tempelEingangSceneName);
    }

    IEnumerator WaitForNPC(NPCs npc)
    {
        bool talked = false;
        System.Action onTalked = () => talked = true;
        npc.onInteracted += onTalked;
        yield return new WaitUntil(() => talked);
        npc.onInteracted -= onTalked;
    }

    IEnumerator WaitForItem(ItemType type, int amount, System.Action onCollected)
    {
        int startCount = inventory.CountItems(type);
        yield return new WaitUntil(() => inventory.CountItems(type) >= startCount + amount);
        onCollected?.Invoke();
    }

    private void OnMoeweTalked()
    {
        if (!moeweQuestStarted)
        {
            moeweQuestStarted = true;
            StartCoroutine(QuestMoewe());
            return;
        }

        if (moeweQuestStarted && !moeweQuestDone)
        {
            if (moeweNotAllEggs != null)
                moewe.dialogue = moeweNotAllEggs;
            return;
        }

        if (moeweQuestDone && !moeweRewardGiven && collectedEggs >= 8)
        {
            if (moeweEggsComplete != null)
                moewe.dialogue = moeweEggsComplete;
            StartCoroutine(CompleteMoeweQuest());
            return;
        }

        if (moeweRewardGiven)
        {
            if (moeweAfter != null)
                moewe.dialogue = moeweAfter;
        }
    }

    IEnumerator QuestMoewe()
    {
        yield return new WaitForSeconds(0.1f);
        questTMP.text = "Collect all eggs (0/8)";
        if (eier != null)
        {
            foreach (var ei in eier)
            {
                if (ei != null)
                    ei.gameObject.SetActive(true);
            }
        }
        if (moeweDuring != null)
            moewe.dialogue = moeweDuring;
    }

    IEnumerator CompleteMoeweQuest()
    {
        if (rewardObjects == null || rewardObjects.Length == 0)
            yield break;

        bool alreadyActivated = true;
        foreach (var obj in rewardObjects)
        {
            if (obj != null && !obj.activeSelf)
            {
                alreadyActivated = false;
                break;
            }
        }

        if (alreadyActivated)
            yield break;

        yield return new WaitForSeconds(0.5f);
        foreach (var obj in rewardObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        questTMP.text = "Quest complete!";
        moeweRewardGiven = true;
        if (moeweAfter != null)
            moewe.dialogue = moeweAfter;
    }

    private void OnSchafTalked()
    {
        if (!schafQuestActive)
        {
            if (schafBefore != null)
                schaf.dialogue = schafBefore;
            return;
        }

        if (!schafQuestStarted)
        {
            schafQuestStarted = true;
            StartCoroutine(QuestSchaf());
            return;
        }

        if (schafQuestStarted && !schafQuestDone)
        {
            if (correctAnswers < schafQuestions.Length)
                schaf.dialogue = schafQuestions[correctAnswers];
            return;
        }

        if (schafQuestDone && !opossumQuestDone)
        {
            if (schafComplete != null)
                schaf.dialogue = schafComplete;
            return;
        }

        if (opossumQuestDone && waldQuestActive)
        {
            if (zumWald != null)
                schaf.dialogue = zumWald;
        }
    }

    IEnumerator QuestSchaf()
    {
        yield return new WaitForSeconds(0.1f);
        questTMP.text = "Answer Marco's three questions";
        correctAnswers = 0;
        if (schafQuestions != null && schafQuestions.Length > 0)
            schaf.dialogue = schafQuestions[0];
    }

    public void OnAnswerSelected(bool isCorrect)
    {
        if (!schafQuestStarted || schafQuestDone)
            return;

        if (isCorrect)
        {
            correctAnswers++;
            questTMP.text = $"Answer Marco's three questions ({correctAnswers}/3)";

            if (correctAnswers >= 3)
            {
                schafQuestDone = true;
                questTMP.text = "Quest complete!";
                if (schafComplete != null)
                    schaf.dialogue = schafComplete;
            }
            else
            {
                if (correctAnswers < schafQuestions.Length)
                    schaf.dialogue = schafQuestions[correctAnswers];
            }
        }
        else
        {
            correctAnswers = 0;
            questTMP.text = "Wrong answer! Starting over...";
            if (schafWrongAnswer != null)
                schaf.dialogue = schafWrongAnswer;
            StartCoroutine(ResetSchafQuest());
        }
    }

    IEnumerator ResetSchafQuest()
    {
        yield return new WaitForSeconds(2f);
        questTMP.text = "Answer Marco's three questions";
        if (schafQuestions != null && schafQuestions.Length > 0)
            schaf.dialogue = schafQuestions[0];
    }

    private void OnOpossumTalked()
    {
        if (!opossumQuestActive)
        {
            if (opossumBefore != null)
                opossum.dialogue = opossumBefore;
            return;
        }

        if (!opossumQuestStarted)
        {
            opossumQuestStarted = true;
            StartCoroutine(QuestOpossum());
            return;
        }

        if (opossumIsHiding && opossumQuestStarted && !opossumQuestDone)
        {
            opossumQuestDone = true;
            StartCoroutine(CompleteOpossumQuest());
            return;
        }

        if (opossumQuestDone && waldQuestActive)
        {
            if (zumWald != null)
                opossum.dialogue = zumWald;
        }
    }

    IEnumerator QuestOpossum()
    {
        yield return new WaitForSeconds(0.1f);
        questTMP.text = "Find the hiding opossum";
        yield return new WaitForSeconds(3f);

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float fadeTime = 1f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 1f;
        }

        yield return new WaitForSeconds(3f);

        if (opossum != null && opossumVersteckPosition != null)
        {
            opossum.transform.position = opossumVersteckPosition.position;
            opossumIsHiding = true;
            if (opossumFound != null)
                opossum.dialogue = opossumFound;
        }

        if (blackScreen != null)
        {
            float fadeTime = 1f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }
    }

    IEnumerator CompleteOpossumQuest()
    {
        questTMP.text = "You found the opossum!";
        yield return new WaitForSeconds(2f);

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float fadeTime = 1f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 1f;
        }

        yield return new WaitForSeconds(2f);

        if (playerSpawnPosition != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = playerSpawnPosition.position;
        }

        if (opossum != null && playerSpawnPosition != null)
        {
            opossum.transform.position = playerSpawnPosition.position;
            opossumIsHiding = false;
        }

        if (blackScreen != null)
        {
            float fadeTime = 1f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }

        questTMP.text = "Quest complete!";
        waldQuestActive = true;

        if (questTMP != null)
            questTMP.text = "Explore the forest";

        if (zumWald != null)
        {
            if (schaf != null)
                schaf.dialogue = zumWald;
            if (opossum != null)
                opossum.dialogue = zumWald;
        }

        if (npcsToDisableAfterOpossum != null)
        {
            foreach (var npc in npcsToDisableAfterOpossum)
            {
                if (npc != null)
                    npc.SetActive(false);
            }
        }

        if (waldEingang != null)
            waldEingang.SetActive(true);
    }

    private void OnWaldEingangInteracted()
    {
        if (!waldQuestActive)
            return;

        StartCoroutine(EnterWald());
    }

    IEnumerator EnterWald()
    {
        questTMP.text = "Entering the forest...";

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float fadeTime = 1f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
                yield return null;
            }
            blackScreen.alpha = 1f;
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(waldSceneName);
    }
}