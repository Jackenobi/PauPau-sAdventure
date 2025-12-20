using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Quest: Seemann")]
    public NPCs seemann;
    public DialogueLine seemannBefore;
    public DialogueLine seemannDuring;
    public DialogueLine seemannNoItem;
    public DialogueLine seemannDone;
    public string tempelEingangSceneName = "Tempeleingang";

    [Header("Quest: Möwe")]
    public NPCs moewe;
    public Item[] eier; // ei1 bis ei8
    public GameObject[] rewardObjects; // Die zwei GameObjects die aktiviert werden
    public DialogueLine moeweBefore;
    public DialogueLine moeweDuring;
    public DialogueLine moeweNotAllEggs;
    public DialogueLine moeweEggsComplete;

    [Header("Quest: Schaf/Marco")]
    public NPCs schaf;
    public DialogueLine schafBefore;
    public DialogueLine[] schafQuestions; // 3 Fragen
    public DialogueLine schafWrongAnswer;
    public DialogueLine schafComplete;


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
    private bool schafQuestActive = false;
    private bool schafQuestStarted = false;
    private bool schafQuestDone = false;
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

            // Anfangs gesperrter Dialog
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
        }

        if (seemann != null)
        {
            seemann.onInteracted += OnSeemannTalked;

            // Anfangs gesperrter Dialog
            if (seemannBefore != null)
                seemann.dialogue = seemannBefore;
        }

        if (moewe != null)
        {
            moewe.onInteracted += OnMoeweTalked;

            // Möwe Quest ist immer aktiv (kann jederzeit gestartet werden)
            moeweQuestActive = true;

            // Anfangs-Dialog
            if (moeweBefore != null)
                moewe.dialogue = moeweBefore;
        }

        if (schaf != null)
        {
            schaf.onInteracted += OnSchafTalked;

            // Anfangs-Dialog (wird aktiviert wenn Möwen-Quest fertig ist)
            if (schafBefore != null)
                schaf.dialogue = schafBefore;
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

        // Alle Eier am Anfang verstecken
        if (eier != null)
        {
            foreach (var ei in eier)
            {
                if (ei != null)
                    ei.gameObject.SetActive(false);
            }
        }

        // Reward Objects am Anfang verstecken
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

        // Prüfe ob ALLE Quests fertig sind → aktiviere Seemann-Quest
        if (bauarbeiterQuestDone && nachbarnAfterUnlocked && !seemannQuestActive)
        {
            seemannQuestActive = true;

            // Seemann Dialog auf "during" umstellen
            if (seemannDuring != null)
                seemann.dialogue = seemannDuring;
        }

        // Prüfe ob Eier eingesammelt werden
        if (moeweQuestStarted && !moeweQuestDone)
        {
            // Zähle wie viele Eggs im Inventar sind
            int eggsInInventory = inventory.CountItems(ItemType.Egg);

            // Update Zähler wenn sich was geändert hat
            if (eggsInInventory != collectedEggs)
            {
                collectedEggs = eggsInInventory;
                questTMP.text = $"Collect all eggs ({collectedEggs}/8)";

                // Alle Eier gesammelt?
                if (collectedEggs >= 8)
                {
                    moeweQuestDone = true;
                    questTMP.text = "All eggs collected! Return to the seagull";

                    // Dialog auf "complete" umstellen
                    if (moeweEggsComplete != null)
                        moewe.dialogue = moeweEggsComplete;
                }
            }
        }

        // Aktiviere Schaf-Quest wenn Möwen-Quest fertig ist
        if (moeweQuestDone && !schafQuestActive)
        {
            schafQuestActive = true;
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

                // Start-Dialog läuft bereits, NACH diesem Dialog auf "during" wechseln
                StartCoroutine(SwitchToDuringAfterDialogue(npc, index));
            }
            else
            {
                // Wurde schon angesprochen → zeige "during" Dialog
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

    IEnumerator SwitchToDuringAfterDialogue(NPCs npc, int index)
    {
        // Warte kurz, damit der Start-Dialog erst abgespielt wird
        yield return new WaitForSeconds(0.1f);

        // Wechsel zu "during" Dialog für nächstes Mal
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

        // Nachbarn bleiben auf "during" Dialog, wechseln NICHT zu "after"!
        // Der Wechsel passiert erst, wenn Map Item aufgesammelt wird (siehe Update)

        // Bauarbeiter-Quest aktiviert
        bauarbeiterQuestActive = true;

        // Bauarbeiter neuer Dialog
        if (bauarbeiterDuring != null)
            bauarbeiter.dialogue = bauarbeiterDuring;
    }


    // BAUARBEITER-QUEST
    private void OnBauarbeiterTalked()
    {
        // Wenn noch gesperrt → nur Before-Dialog
        if (!bauarbeiterQuestActive)
        {
            if (bauarbeiterBefore != null)
                bauarbeiter.dialogue = bauarbeiterBefore;
            return;
        }

        // Wenn Quest aktiv, aber noch nicht gestartet → starte sie
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

        // Wenn Tape da ist und Quest noch nicht abgeschlossen → DONE Dialog zeigen
        if (bauarbeiterQuestStarted && !bauarbeiterQuestDone && inventory.HasItem(ItemType.Tape))
        {
            // Zeige DONE Dialog beim Abgeben
            if (bauarbeiterDone != null)
                bauarbeiter.dialogue = bauarbeiterDone;

            // QUEST ABSCHLIESSEN mit Blackscreen-Animation
            StartCoroutine(CompleteBauarbeiterQuest());
            return;
        }

        // Wenn Quest schon abgeschlossen → After-Dialog (immer wieder)
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

    // SEEMANN-QUEST
    private void OnSeemannTalked()
    {
        // Wenn noch gesperrt → nur Before-Dialog
        if (!seemannQuestActive)
        {
            if (seemannBefore != null)
                seemann.dialogue = seemannBefore;
            return;
        }

        // Wenn Quest aktiv, aber noch nicht gestartet → starte sie
        if (!seemannQuestStarted)
        {
            seemannQuestStarted = true;
            StartCoroutine(QuestSeemann());
            return;
        }

        // Wenn Quest bereits gestartet, aber Map noch nicht gefunden
        if (seemannQuestStarted && !seemannQuestDone && !inventory.HasItem(ItemType.Map))
        {
            if (seemannNoItem != null)
                seemann.dialogue = seemannNoItem;
            return;
        }

        // Wenn Map da ist und Quest noch nicht abgeschlossen → DONE Dialog & Scene laden
        if (seemannQuestStarted && !seemannQuestDone && inventory.HasItem(ItemType.Map))
        {
            // Zeige DONE Dialog
            if (seemannDone != null)
                seemann.dialogue = seemannDone;

            // Quest abschließen und Scene laden
            StartCoroutine(CompleteSeemannQuest());
            return;
        }
    }

    IEnumerator QuestSeemann()
    {
        // Questlog aktualisieren
        questTMP.text = "Find the Map";

        // Map Item aktivieren (falls noch nicht geschehen)
        if (mapItem != null && !mapItem.gameObject.activeSelf)
            mapItem.gameObject.SetActive(true);

        seemann.dialogue = seemannDuring;

        // Auf Map warten
        yield return WaitForItem(ItemType.Map, 1, null);

        // Questlog aktualisieren
        questTMP.text = "Bring the map to the sailor";

        if (seemannDone != null)
            seemann.dialogue = seemannDone;
    }

    IEnumerator CompleteSeemannQuest()
    {
        seemannQuestDone = true;

        // Warte kurz damit Dialog fertig abgespielt wird
        yield return new WaitForSeconds(1f);

        questTMP.text = "Setting sail...";

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

        // 3 Sekunden warten
        yield return new WaitForSeconds(3f);

        // Scene laden
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

    // MÖWEN-QUEST
    private void OnMoeweTalked()
    {
        // Wenn Quest noch nicht gestartet → starte sie
        if (!moeweQuestStarted)
        {
            moeweQuestStarted = true;
            StartCoroutine(QuestMoewe());
            return;
        }

        // Wenn Quest läuft aber noch nicht alle Eier gesammelt
        if (moeweQuestStarted && !moeweQuestDone)
        {
            if (moeweNotAllEggs != null)
                moewe.dialogue = moeweNotAllEggs;
            return;
        }

        // Wenn alle Eier gesammelt → Belohnung geben
        if (moeweQuestDone && collectedEggs >= 8)
        {
            if (moeweEggsComplete != null)
                moewe.dialogue = moeweEggsComplete;

            // Reward Objects aktivieren
            StartCoroutine(CompleteMoeweQuest());
        }
    }

    IEnumerator QuestMoewe()
    {
        // Questlog aktualisieren
        questTMP.text = "Collect all eggs (0/8)";

        // Alle Eier aktivieren
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

        yield return null;
    }

    IEnumerator CompleteMoeweQuest()
    {
        // Nur einmal ausführen
        if (rewardObjects == null || rewardObjects.Length == 0)
            yield break;

        // Prüfe ob schon aktiviert
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

        // Warte kurz
        yield return new WaitForSeconds(0.5f);

        // Aktiviere Reward Objects
        foreach (var obj in rewardObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        questTMP.text = "Quest complete!";
    }

    // SCHAF/MARCO-QUEST
    private void OnSchafTalked()
    {
        // Wenn noch gesperrt
        if (!schafQuestActive)
        {
            if (schafBefore != null)
                schaf.dialogue = schafBefore;
            return;
        }

        // Wenn Quest noch nicht gestartet → starte sie
        if (!schafQuestStarted)
        {
            schafQuestStarted = true;
            StartCoroutine(QuestSchaf());
            return;
        }

        // Wenn Quest läuft → zeige aktuelle Frage
        if (schafQuestStarted && !schafQuestDone)
        {
            if (correctAnswers < schafQuestions.Length)
                schaf.dialogue = schafQuestions[correctAnswers];
            return;
        }

        // Wenn Quest fertig
        if (schafQuestDone)
        {
            if (schafComplete != null)
                schaf.dialogue = schafComplete;
        }
    }

    IEnumerator QuestSchaf()
    {
        // Questlog aktualisieren
        questTMP.text = "Answer Marco's three questions";

        correctAnswers = 0;

        // Erste Frage stellen
        if (schafQuestions != null && schafQuestions.Length > 0)
            schaf.dialogue = schafQuestions[0];

        yield return null;
    }

    // Diese Methode wird vom DialogueScreen aufgerufen wenn eine Antwort gewählt wird
    public void OnAnswerSelected(bool isCorrect)
    {
        if (!schafQuestStarted || schafQuestDone)
            return;

        if (isCorrect)
        {
            correctAnswers++;
            questTMP.text = $"Answer Marco's three questions ({correctAnswers}/3)";

            // Alle Fragen richtig beantwortet?
            if (correctAnswers >= 3)
            {
                schafQuestDone = true;
                questTMP.text = "Quest complete!";

                if (schafComplete != null)
                    schaf.dialogue = schafComplete;
            }
            else
            {
                // Nächste Frage
                if (correctAnswers < schafQuestions.Length)
                    schaf.dialogue = schafQuestions[correctAnswers];
            }
        }
        else
        {
            // Falsche Antwort → Reset
            correctAnswers = 0;
            questTMP.text = "Wrong answer! Starting over...";

            if (schafWrongAnswer != null)
                schaf.dialogue = schafWrongAnswer;

            // Nach kurzer Zeit wieder zur ersten Frage
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
}