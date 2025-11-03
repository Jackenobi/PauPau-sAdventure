using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class Quest : MonoBehaviour
{

    public DialogueScreen dialogueScreen;
    public Transform questScreen;
    public GameObject questDisplayPrefab;
    public Inventory inventory;

    [Header("Quest Nachbarn")] //Überschrift im Inspector
    public NPCs npc1;
    public Item[] nachbarn;
    public DialogueLine npc1After; //neuer Dialog nach Questabschluss

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
        questTMP.text = "Find some Tape";

        tape.gameObject.SetActive(true);

        yield return WaitForItem(ItemType.Tape, 1, null); 

        bauarbeiter.dialogue = bauarbeiterAfter;

        questTMP.text = "Bring the tape to the constructionworker";

        yield return WaitForNPC(bauarbeiter);
        Destroy(questDisplay);
    }

    /*------------AUF 2 NPCs PARALLEL WARTEN-------------
		bool talkedToNPC1 = false;
		//variabel, die eine Funktion speichert
		System.Action action = () => {
			talkedToNPC1 = true;
		};
		npc1.onInteracted += action;

		bool talkedToNPC2 = false;
		//variabel, die eine Funktion speichert
		System.Action action2 = () => {
			talkedToNPC2 = true;
		};
		npc2.onInteracted += action;

		yield return new WaitUntil(() => talkedToNPC1 && talkedToNPC2);

		npc1.onInteracted -= action;
		npc2.onInteracted -= action;
		*/

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

    //wartet auf ein SPEZIFISCHES item
    //IEnumerator WaitForItem(Item item) {
    //	bool gotItem = false;

    //	//variabel, die eine Funktion speichert
    //	System.Action action = () => {
    //		gotItem = true;
    //	};

    //	item.onInteracted += action;
    //	//die coroutine wartet
    //	yield return new WaitUntil(() => gotItem);

    //	item.onInteracted -= action;
    //}

    IEnumerator WaitForItem(ItemType item, int amount, TMP_Text tmp)
    {
        bool gotItem = false;

        //variabel, die eine Funktion speichert
        System.Action<ItemType> action = (type) => {
            int count = inventory.CountItems(type);
            if (tmp != null)
            {
                tmp.text = count + "/" + amount;
            }
            if (count >= amount)
                gotItem = true;
        };

        inventory.onItemCollected += action;
        //die coroutine wartet
        yield return new WaitUntil(() => gotItem);

        inventory.onItemCollected -= action;
    }

}
