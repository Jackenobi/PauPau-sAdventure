using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonPuzzleManager : MonoBehaviour
{
    public List<PuzzleButton> doorButtons;
    public List<PuzzleButton> floorButtons;
    public float lightDelay = 1.2f; // Langsamer (vorher 0.6f)
    public string nextSceneName;

    [Header("Error Feedback")]
    public Material errorMaterial; // Rotes Material für Fehler
    public AudioClip errorSound; // Fehler-Sound

    private List<int> sequence = new List<int>();
    private int inputIndex = 0;
    private int round = 0;
    private int[] rounds = { 3, 4, 5 };
    private bool playerCanInput = false;
    private FinalQuestSimon quest;

    public void StartPuzzle(FinalQuestSimon q)
    {
        quest = q;
        round = 0;
        StartCoroutine(StartRound());
    }

    IEnumerator StartRound()
    {
        playerCanInput = false;
        inputIndex = 0;
        sequence.Clear();

        int length = rounds[round];
        for (int i = 0; i < length; i++)
            sequence.Add(Random.Range(0, doorButtons.Count));

        yield return new WaitForSeconds(0.5f);

        foreach (int index in sequence)
        {
            doorButtons[index].LightUp();
            yield return new WaitForSeconds(lightDelay);
        }

        playerCanInput = true;
    }

    public void PlayerPressedButton(PuzzleButton button)
    {
        if (!playerCanInput)
            return;

        if (button.buttonIndex != sequence[inputIndex])
        {
            // ❌ FEHLER!
            StartCoroutine(ShowError());
            return;
        }

        inputIndex++;

        if (inputIndex >= sequence.Count)
        {
            round++;

            if (round >= rounds.Length)
            {
                quest.OnPuzzleCompleted();
            }
            else
            {
                quest.OnRoundCompleted(round);
                StartCoroutine(StartRound());
            }
        }
    }

    IEnumerator ShowError()
    {
        playerCanInput = false;

        // Alle Tür-Buttons rot aufleuchten
        foreach (var btn in doorButtons)
        {
            btn.ShowError(errorMaterial, errorSound);
        }

        yield return new WaitForSeconds(3f);

        // Von vorne starten (gleiche Runde!)
        quest.OnRoundReset(round);
        StartCoroutine(StartRound());
    }
}