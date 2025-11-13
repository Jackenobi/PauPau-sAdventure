using System.Collections;
using UnityEngine;
using TMPro;

public class Quest : MonoBehaviour
{
    [Header("UI")]
    public DialogueScreen dialogueScreen;
    public Transform questScreen;
    public GameObject questDisplayPrefab;
    public Inventory inventory;

    [Header("Quest: Nachbarn")]
    public NPCs[] nachbarn;
    public DialogueLine[] nachbarnDuring;
    public DialogueLine[] nachbarnAfter;

    [Header("Quest: Bauarbeiter")]
    public NPCs bauarbeiter;
    public Item tapeItem;
    public DialogueLine bauarbeiterBefore;
    public DialogueLine bauarbeiterDuring;
    public DialogueLine bauarbeiterNoItem;   
    public DialogueLine bauarbeiterDone;
    public DialogueLine bauarbeiterAfter;


    private bool nachbarnQuestActive = false;
    private bool nachbarnQuestDone = false;
    private bool bauarbeiterQuestActive = false;
    private bool bauarbeiterQuestStarted = false; 
    private TMP_Text questTMP;
    private int talkedToCount = 0;

    void Start()
    {
        foreach (var npc in nachbarn)
        {
            npc.onInteracted += () => OnNeighborTalked(npc);
        }

        if (bauarbeiter != null)
        {
            bauarbeiter.onInteracted += OnBauarbeiterTalked;

            // Anfangs gesperrter Dialog
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
        }
    }

    // NACHBARN-QUEST
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
            }
            else
            {
                if (nachbarnDuring != null && index < nachbarnDuring.Length && nachbarnDuring[index] != null)
                    npc.dialogue = nachbarnDuring[index];
            }
        }
        else if (nachbarnQuestDone)
        {
            if (nachbarnAfter != null && index < nachbarnAfter.Length && nachbarnAfter[index] != null)
                npc.dialogue = nachbarnAfter[index];
        }
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

        for (int i = 0; i < nachbarn.Length; i++)
        {
            if (i < nachbarnAfter.Length && nachbarnAfter[i] != null)
                nachbarn[i].dialogue = nachbarnAfter[i];
        }

        //Bauarbeiter-Quest aktiviert
        bauarbeiterQuestActive = true;

        //Bauarbeiter neuer Dialog
        if (bauarbeiterDuring != null)
            bauarbeiter.dialogue = bauarbeiterDuring;
    }


    //BAUARBEITER-QUEST
    private void OnBauarbeiterTalked()
    {
        // Wenn noch gesperrt  nur Before-Dialog
        if (!bauarbeiterQuestActive)
        {
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
            return;
        }

        // Wenn Quest aktiv, aber noch nicht gestartet  starte sie
        if (!bauarbeiterQuestStarted)
        {
            bauarbeiterQuestStarted = true;
            StartCoroutine(QuestBauarbeiter());
            return;
        }

        // Wenn Quest bereits gestartet, aber Tape noch nicht abgegeben
        if (bauarbeiterQuestStarted && !inventory.HasItem(ItemType.Tape))
        {
            if (bauarbeiterNoItem != null)
                bauarbeiter.dialogue = bauarbeiterNoItem;
            return;
        }

        // Wenn Tape schon da  "Done"-Dialog
        if (bauarbeiterQuestStarted && inventory.HasItem(ItemType.Tape))
        {
            if (bauarbeiterDone != null)
                bauarbeiter.dialogue = bauarbeiterDone;
        }
    }


    IEnumerator QuestBauarbeiter()
    {
        // Questlog erzeugen
        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        var questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Find some Tape";

        if (tapeItem != null)
            tapeItem.gameObject.SetActive(true);

        bauarbeiter.dialogue = bauarbeiterDuring;

        // Auf Tape warten
        yield return WaitForItem(ItemType.Tape, 1, null);

        bauarbeiter.dialogue = bauarbeiterDone;
        questTMP.text = "Bring the tape to the construction worker";

        yield return WaitForNPC(bauarbeiter);

        Destroy(questDisplay);
        bauarbeiter.dialogue = bauarbeiterAfter;
    }

    // HILFSFUNKTIONEN

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
}
