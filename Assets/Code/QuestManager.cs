using System.Collections;
using UnityEngine;
using TMPro;
//WIP VLLT RAUS
// LinearQuestManager: zwei lineare Quests (Nachbarn -> Bauarbeiter), UI + Sicherheitschecks
public class LinearQuestManager : MonoBehaviour
{
    [Header("UI")]
    public DialogueScreen dialogueScreen;    // dein vorhandenes DialogueScreen
    public Transform questScreen;            // Parent für Quest UI entries
    public GameObject questDisplayPrefab;    // Prefab mit TMP Text inside
    public Inventory inventory;

    [Header("Quest 1: Nachbarn")]
    public NPCs[] nachbarn;                  // die Nachbars-NPCs (in Reihenfolge)
    public DialogueLine[] nachbarnDuring;    // per-NPC während-Dialoge
    public DialogueLine[] nachbarnAfter;     // per-NPC after-Dialoge

    [Header("Quest 2: Bauarbeiter")]
    public NPCs bauarbeiter;
    public Item tapeItem;                    // GameObject Item (wird aktiviert)
    public DialogueLine bauarbeiterBefore;  // shown while locked
    public DialogueLine bauarbeiterDuring;
    public DialogueLine bauarbeiterNoItem;
    public DialogueLine bauarbeiterDone;
    public DialogueLine bauarbeiterAfter;

    // intern
    private GameObject currentQuestDisplay;  // UI Instance for active quest
    private TMP_Text currentQuestText;
    private int currentQuestIndex = 0;       // 0 = Nachbarn, 1 = Bauarbeiter
    private bool nachbarnQuestActive = false;
    private bool nachbarnQuestDone = false;
    private int talkedToCount = 0;
    private bool bauarbeiterQuestStarted = false;

    void Start()
    {
        // Register listeners for Nachbarn
        foreach (var npc in nachbarn)
        {
            if (npc != null)
                npc.onInteracted += () => HandleNeighborInteracted(npc);
        }

        // Register Bauarbeiter
        if (bauarbeiter != null)
        {
            bauarbeiter.onInteracted += HandleBauarbeiterInteracted;

            // Initially set Bauarbeiter to "before" dialog (locked)
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
        }

        // Ensure tape item is off (hidden) until Bauarbeiter quest
        if (tapeItem != null)
            tapeItem.gameObject.SetActive(false);

        // Start first quest automatically (if you want that)
        StartNextQuest();
    }

    // ---------- Quest flow management ----------
    private void StartNextQuest()
    {
        if (currentQuestIndex == 0)
        {
            StartCoroutine(StartNachbarnQuest());
        }
        else if (currentQuestIndex == 1)
        {
            // Bauarbeiter quest will start only after Nachbarn done
            // We'll create the quest UI only when it's actually allowed
            // (it will be triggered by completion of Nachbarn quest)
        }
    }

    // ---------- Nachbarn Quest ----------
    IEnumerator StartNachbarnQuest()
    {
        // Protect against double-start
        if (nachbarnQuestActive || nachbarnQuestDone)
            yield break;

        nachbarnQuestActive = true;
        talkedToCount = 0;

        // Create UI entry
        currentQuestDisplay = Instantiate(questDisplayPrefab, questScreen);
        currentQuestText = currentQuestDisplay.GetComponentInChildren<TMP_Text>();
        if (currentQuestText != null)
            currentQuestText.text = $"Talk to your neighbors ({talkedToCount}/{nachbarn.Length})";

        yield return null;
    }

    private void HandleNeighborInteracted(NPCs npc)
    {
        if (!nachbarnQuestActive || nachbarnQuestDone)
            return;

        int index = System.Array.IndexOf(nachbarn, npc);
        if (index < 0) return;

        // First time talking to this NPC?
        if (!npc.hasSpoken)
        {
            npc.hasSpoken = true;
            talkedToCount++;

            if (currentQuestText != null)
                currentQuestText.text = $"Talk to your neighbors ({talkedToCount}/{nachbarn.Length})";

            // When all done:
            if (talkedToCount >= nachbarn.Length)
            {
                nachbarnQuestDone = true;
                CompleteNachbarnQuest();
            }
        }
        else
        {
            // Already spoke once: switch to during-dialog if present
            if (nachbarnDuring != null && index < nachbarnDuring.Length && nachbarnDuring[index] != null)
                npc.dialogue = nachbarnDuring[index];
        }
    }

    private void CompleteNachbarnQuest()
    {
        // update all NPCs to "after" dialogs if present
        for (int i = 0; i < nachbarn.Length; i++)
        {
            if (i < nachbarnAfter.Length && nachbarnAfter[i] != null)
                nachbarn[i].dialogue = nachbarnAfter[i];
        }

        // remove the current quest UI
        if (currentQuestDisplay != null)
        {
            Destroy(currentQuestDisplay);
            currentQuestDisplay = null;
            currentQuestText = null;
        }

        // Unlock Bauarbeiter quest
        bauarbeiterQuestStarted = false; // still not started, but allowed
        currentQuestIndex = 1;
        // Set bauarbeiter dialog to During (now unlocked prompt)
        if (bauarbeiterDuring != null)
            bauarbeiter.dialogue = bauarbeiterDuring;

        // Optionally show the next quest immediately in the log or only upon interaction
        // Here we show it when the user interacts with Bauarbeiter, so do nothing now.
    }

    // ---------- Bauarbeiter Quest ----------
    private void HandleBauarbeiterInteracted()
    {
        // If Bauarbeiter still locked (nachbarn not done), show 'before' dialog and exit
        if (!nachbarnQuestDone)
        {
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
            return;
        }

        // If bauarbeiter quest not yet started, start it and create UI
        if (!bauarbeiterQuestStarted)
        {
            bauarbeiterQuestStarted = true;
            StartCoroutine(StartBauarbeiterQuest());
            return;
        }

        // If quest started but tape not yet in inventory, show NoItem dialog
        if (bauarbeiterQuestStarted && !inventory.HasItem(ItemType.Tape))
        {
            if (bauarbeiterNoItem != null)
                bauarbeiter.dialogue = bauarbeiterNoItem;
            return;
        }

        // If player has tape, show done dialog (or trigger finalization)
        if (bauarbeiterQuestStarted && inventory.HasItem(ItemType.Tape))
        {
            if (bauarbeiterDone != null)
                bauarbeiter.dialogue = bauarbeiterDone;

            // Optionally complete quest immediately when spoken with and has item:
            StartCoroutine(CompleteBauarbeiterQuest());
        }
    }

    IEnumerator StartBauarbeiterQuest()
    {
        // Create UI entry (one instance only)
        if (currentQuestDisplay == null)
        {
            currentQuestDisplay = Instantiate(questDisplayPrefab, questScreen);
            currentQuestText = currentQuestDisplay.GetComponentInChildren<TMP_Text>();
        }

        if (currentQuestText != null)
            currentQuestText.text = "Find some Tape";

        // Enable the tape item in the world
        if (tapeItem != null)
            tapeItem.gameObject.SetActive(true);

        // Set dialog
        bauarbeiter.dialogue = bauarbeiterDuring;

        // Wait until player collects tape (handled with WaitForItem helper)
        yield return StartCoroutine(WaitForItem(ItemType.Tape, 1, null));

        // If item collected, inform player via UI
        if (currentQuestText != null)
            currentQuestText.text = "Bring the tape to the construction worker";

        // Now wait for player to return and talk to bauarbeiter
        yield return StartCoroutine(WaitForNPC(bauarbeiter));

        // Then complete the quest
        yield return StartCoroutine(CompleteBauarbeiterQuest());
    }

    IEnumerator CompleteBauarbeiterQuest()
    {
        // prevent double-completion
        if (currentQuestText != null)
            currentQuestText.text = "Quest complete!";

        // give final dialog
        if (bauarbeiterAfter != null)
            bauarbeiter.dialogue = bauarbeiterAfter;

        // small delay so player can read
        yield return new WaitForSeconds(1f);

        // remove UI
        if (currentQuestDisplay != null)
        {
            Destroy(currentQuestDisplay);
            currentQuestDisplay = null;
            currentQuestText = null;
        }

        // Sequence done: could advance currentQuestIndex further if more quests are present
        currentQuestIndex = 2; // past last
    }

    // ---------- Helpers ----------
    IEnumerator WaitForNPC(NPCs npc)
    {
        bool talked = false;
        System.Action onTalk = () => talked = true;
        npc.onInteracted += onTalk;
        yield return new WaitUntil(() => talked);
        npc.onInteracted -= onTalk;
    }

    IEnumerator WaitForItem(ItemType type, int amount, System.Action onCollected)
    {
        int startCount = inventory.CountItems(type);
        yield return new WaitUntil(() => inventory.CountItems(type) >= startCount + amount);
        onCollected?.Invoke();
    }
}

