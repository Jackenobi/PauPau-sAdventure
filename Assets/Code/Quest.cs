using System.Collections;
using TMPro;
using UnityEngine;

public class Quest : MonoBehaviour
{

    public DialogueScreen dialogueScreen;
    public Transform questScreen;
    public GameObject questDisplayPrefab;

    [Header("Quest 1")] //Überschrift im Inspector
    public NPCs npc1;
    public Item bottle;
    public DialogueLine dialogue2;

    [Header("Quest 2")]
    public NPCs npc2;
    public Item item2;
    public DialogueLine dialogue2npc2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //beide Quests gleichzeitig starten
        StartCoroutine(Quest1());
        StartCoroutine(Quest2());
    }

    IEnumerator Quest1()
    {
        yield return WaitForChoice("dashierincodeschreiben");

        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        var questTMP = questDisplay.GetComponent<TMP_Text>();

        questTMP.text = "finde ne die Flasche";

        bottle.gameObject.SetActive(true);

        yield return WaitForItem(bottle);

        Destroy(bottle.gameObject);
        npc1.dialogue = dialogue2;

        questTMP.text = "bringe die flasche zum NPC";

        yield return WaitForNPC(npc1);
        Destroy(questDisplay);
    }

    IEnumerator Quest2()
    {
        yield return WaitForNPC(npc2);
        yield return new WaitUntil(() => dialogueScreen.panel.activeSelf == false);

        GameObject questDisplay = Instantiate(questDisplayPrefab, questScreen);
        var questTMP = questDisplay.GetComponent<TMP_Text>();

        questTMP.text = "hilfee";

        item2.gameObject.SetActive(true);

        yield return WaitForItem(item2);

        Destroy(item2.gameObject);
        npc2.dialogue = dialogue2npc2;
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

    IEnumerator WaitForItem(Item item)
    {
        bool gotItem = false;

        //Variabel, die eine Funktion speichert 
        System.Action action = () =>
        {
            gotItem = true;
        };

        item.onInteracted += action; //fügt die Funktion der Liste hinzu
        //coroutine wartet bis die Bedingung true ist
        yield return new WaitUntil(() => gotItem);

        item.onInteracted -= action; //entfernt die Funktion aus der Liste
    }

}
