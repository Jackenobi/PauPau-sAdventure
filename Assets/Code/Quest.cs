using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Quest : MonoBehaviour
{

    public DialogueScreen dialogueScreen;
    public Transform questScreen;
    public GameObject questDisplayPrefab;
    public Inventory inventory;

    [Header("Quest Nachbarn")] //Überschrift im Inspector
    public NPCs npc1;
    public NPCs npc2;
    public NPCs npc3;
    public DialogueLine npc1After;

    [Header("Quest Bauarbeiter")]
    public NPCs bauarbeiter;
    public Item tape;
    public DialogueLine bauarbeiterAfter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //beide Quests gleichzeitig starten
        StartCoroutine(QuestNachbarn());
        StartCoroutine(QuestBauarbeiter());
    }

    IEnumerator QuestNachbarn()
    {
        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        var questTMP = questDisplay.GetComponentInChildren<TMP_Text>();
        questTMP.text = "Talk to your neighbors";


        yield return WaitForChoice("dashierincodeschreiben");

        npc1.dialogue = npc1After;

    }

    IEnumerator QuestBauarbeiter()
    {
        yield return WaitForNPC(bauarbeiter);
        yield return new WaitUntil(() => dialogueScreen.panel.activeSelf == false);

        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        var questTMP = questDisplay.GetComponentInChildren<TMP_Text>();

        questTMP.text = "Find 1 Tape";

        tape.gameObject.SetActive(true);

        yield return WaitForItem(ItemType.Tape, 1);

        bauarbeiter.dialogue = bauarbeiterAfter;
    }


    IEnumerator WaitForNPC (NPCs npc)
    {
        bool talkedToNPC = false;
       
       //Variabel, die eine Funktion speichert 
        System.Action action = () =>
        {
            talkedToNPC = true;
        };

        npc.onInteracted += action; //fügt die Funktion der Liste hinzu
        //coroutine wartet bis die Bedingung true ist
        yield return new WaitUntil(() => talkedToNPC);

        npc.onInteracted -= action; //entfernt die Funktion aus der Liste
    }

    IEnumerator WaitForChoice(string id)
    {
        bool choiceSelected = false;

        //Variabel, die eine Funktion speichert 
        System.Action<string> action = (inputId) =>
        {
            if(inputId == id)
            choiceSelected = true;
        };

        dialogueScreen.onChoiceSelected += action; //fügt die Funktion der Liste hinzu
        //coroutine wartet bis die Bedingung true ist
        yield return new WaitUntil(() => choiceSelected);

        dialogueScreen.onChoiceSelected -= action; //entfernt die Funktion aus der Liste
    }

    IEnumerator WaitForItem(ItemType item, int amount)
    {
        bool gotItem = false;

        //Variabel, die eine Funktion speichert 
        System.Action<ItemType> action = (type) =>
        {
            if(inventory.CountItems(type) >= amount)
            gotItem = true;
        };

        inventory.onItemCollected += action; //fügt die Funktion der Liste hinzu
        //coroutine wartet bis die Bedingung true ist
        yield return new WaitUntil(() => gotItem);

        inventory.onItemCollected -= action; //entfernt die Funktion aus der Liste
    }

}
