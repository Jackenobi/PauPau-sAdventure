using UnityEngine;
using TMPro;

public class LinearQuestManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text questText;

    [Header("Quest 1: Nachbarn")]
    public NPCs[] nachbarn;

    [Header("Quest 2: Bauarbeiter")]
    public NPCs bauarbeiter;
    public Item tapeItem;
    public Inventory inventory;

    private int talkedToCount = 0;
    private bool nachbarnQuestDone = false;
    private bool bauarbeiterQuestDone = false;

    void Start()
    {
        // Nachbarn registrieren
        foreach (var npc in nachbarn)
        {
            npc.onInteracted += () => OnNeighborTalked(npc);
        }

        // Bauarbeiter registrieren
        bauarbeiter.onInteracted += OnBauarbeiterTalked;

        // Starttext
        UpdateNeighborQuestText();

        if (tapeItem != null)
            tapeItem.gameObject.SetActive(false);
    }

    //NACHBARN

    void OnNeighborTalked(NPCs npc)
    {
        if (nachbarnQuestDone)
            return;

        if (!npc.hasSpoken)
        {
            npc.hasSpoken = true;
            talkedToCount++;
            UpdateNeighborQuestText();

            if (talkedToCount >= nachbarn.Length)
            {
                nachbarnQuestDone = true;
                questText.text = "All neighbors talked to!";
                tapeItem.gameObject.SetActive(true);
            }
        }
    }

    void UpdateNeighborQuestText()
    {
        questText.text = $"Talk to your neighbors ({talkedToCount}/{nachbarn.Length})";
    }

    //BAUARBEITER

    void OnBauarbeiterTalked()
    {
        if (!nachbarnQuestDone)
            return;

        if (bauarbeiterQuestDone)
            return;

        if (!inventory.HasItem(ItemType.Tape))
        {
            questText.text = "Bring tape to the construction worker";
            return;
        }

        // QUEST ABSCHLIESSEN
        bauarbeiterQuestDone = true;
        questText.text = "Brücke repariert!";
        tapeItem.gameObject.SetActive(false);
    }
}
