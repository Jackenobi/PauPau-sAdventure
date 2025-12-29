using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimonPuzzleManager : MonoBehaviour
{
    public List<PuzzleButton> doorButtons;
    public List<PuzzleButton> floorButtons;

    public float lightDelay = 0.6f;
    public string nextSceneName;

    private List<int> sequence = new List<int>();
    private int inputIndex = 0;
    private int round = 0;

    private int[] rounds = { 3, 5, 8 };
    private bool playerCanInput = false;

    public void StartPuzzle()
    {
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
            // ❌ Fehler → Neustart
            StartPuzzle();
            return;
        }

        inputIndex++;

        if (inputIndex >= sequence.Count)
        {
            round++;

            if (round >= rounds.Length)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                StartCoroutine(StartRound());
            }
        }
    }
}
