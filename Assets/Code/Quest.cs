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


    private bool nachbarnQuestActive = false;
    private bool nachbarnQuestDone = false;
    private bool nachbarnAfterUnlocked = false;
    private bool bauarbeiterQuestActive = false;
    private bool bauarbeiterQuestStarted = false;
    private bool bauarbeiterQuestDone = false;
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

        // Blackscreen unsichtbar machen
        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.gameObject.SetActive(false);
        }

        // Map Item am Anfang verstecken (falls vorhanden)
        if (mapItem != null)
            mapItem.gameObject.SetActive(false);
    }

    void Update()
    {
        // Prüfe ob Map Item aufgesammelt wurde
        if (nachbarnQuestDone && !nachbarnAfterUnlocked && inventory.HasItem(ItemType.Map))
        {
            nachbarnAfterUnlocked = true;

            // Nachbarn auf After-Dialog umstellen
            for (int i = 0; i < nachbarn.Length; i++)
            {
                if (i < nachbarnAfter.Length && nachbarnAfter[i] != null)
                    nachbarn[i].dialogue = nachbarnAfter[i];
            }
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
            // Nachbarn bleiben auf "during" Dialog bis Map Item gefunden wurde
            if (nachbarnAfterUnlocked && nachbarnAfter != null && index < nachbarnAfter.Length && nachbarnAfter[index] != null)
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


        // Bauarbeiter-Quest aktiviert
        bauarbeiterQuestActive = true;

        // Bauarbeiter neuer Dialog
        if (bauarbeiterDuring != null)
            bauarbeiter.dialogue = bauarbeiterDuring;
    }


    // BAUARBEITER-QUEST
    private void OnBauarbeiterTalked()
    {
        // Wenn noch gesperrt - nur Before-Dialog
        if (!bauarbeiterQuestActive)
        {
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
            return;
        }

        // Wenn Quest aktiv, aber noch nicht gestartet - starte sie
        if (!bauarbeiterQuestStarted)
        {
            bauarbeiterQuestStarted = true;
            StartCoroutine(QuestBauarbeiter());
            return;
        }

        // Wenn Quest bereits gestartet, aber Tape noch nicht abgegeben
        if (bauarbeiterQuestStarted && !bauarbeiterQuestDone && !inventory.HasItem(ItemType.Tape))
        {
            if (bauarbeiterNoItem != null)
                bauarbeiter.dialogue = bauarbeiterNoItem;
            return;
        }

        // Wenn Tape da ist und Quest noch nicht abgeschlossen  DONE Dialog zeigen
        if (bauarbeiterQuestStarted && !bauarbeiterQuestDone && inventory.HasItem(ItemType.Tape))
        {
            // Zeige DONE Dialog beim Abgeben
            if (bauarbeiterDone != null)
                bauarbeiter.dialogue = bauarbeiterDone;

            // QUEST ABSCHLIESSEN mit Blackscreen-Animation
            StartCoroutine(CompleteBauarbeiterQuest());
            return;
        }

        // Wenn Quest schon abgeschlossen- After-Dialog (immer wieder)
        if (bauarbeiterQuestDone)
        {
            if (bauarbeiterAfter != null)
                bauarbeiter.dialogue = bauarbeiterAfter;
        }
    }


    IEnumerator QuestBauarbeiter()
    {
        // Questlog aktualisieren (kein neues Display erstellen, da questTMP bereits existiert)
        questTMP.text = "Find some Tape";

        if (tapeItem != null)
            tapeItem.gameObject.SetActive(true);

        bauarbeiter.dialogue = bauarbeiterDuring;

        // Auf Tape warten
        yield return WaitForItem(ItemType.Tape, 1, null);

        // Questlog aktualisieren
        questTMP.text = "Bring the tape to the construction worker";

        if (bauarbeiterDone != null)
            bauarbeiter.dialogue = bauarbeiterDone;
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

    IEnumerator CompleteBauarbeiterQuest()
    {
        bauarbeiterQuestDone = true;

        // Blackscreen einblenden
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

        // Während Blackscreen: Brücken austauschen
        yield return new WaitForSeconds(2f); // Hier später Sound abspielen

        if (brueckeKaputt != null)
            brueckeKaputt.SetActive(false);
        if (brueckeHeil != null)
            brueckeHeil.SetActive(true);

        // Blackscreen ausblenden
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

        // Quest abschließen
        questTMP.text = "Bridge fixed!";

        if (bauarbeiterAfter != null)
            bauarbeiter.dialogue = bauarbeiterAfter;

        if (tapeItem != null)
            tapeItem.gameObject.SetActive(false);
    }
}