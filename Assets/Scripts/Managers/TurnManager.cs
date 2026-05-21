using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class TurnManager : MonoBehaviour
{
    [Header("Active Players")]
    [SerializeField] private PlayerActions PlAc;
    [SerializeField] private EnemyBehaviour[] _Enemies;
    [SerializeField] private List<EnemyBehaviour> _Players = new List<EnemyBehaviour>();

    [Header("Turn Information")]
    [SerializeField] public int currentTurn = 0;
    IEnumerator startTurnRoutine;
    public static Action PlayerStartTurn;

    OnScreenAnnouncement news;

    [SerializeField] CanvasGroup endScreen;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxVictory;
    [SerializeField] private AudioClip sfxFail;

    void OnEnable()
    {
        EnemyBehaviour.OnEndTurn += fInvokeNextTurn;
        PlayerActions.OnEndTurn += fInvokeNextTurn;
        EnemyBehaviour.OnEnemyWin += EndGame;
        PlayerActions.OnPlayerWin += EndGame;
    }

    void OnDisable()
    {
        EnemyBehaviour.OnEndTurn -= fInvokeNextTurn;
        PlayerActions.OnEndTurn -= fInvokeNextTurn;
        EnemyBehaviour.OnEnemyWin -= EndGame;
        PlayerActions.OnPlayerWin -= EndGame;
    }

    void Awake()
    {
        news = FindFirstObjectByType<OnScreenAnnouncement>();
    }

    void Start()
    {
        _Players.Add(null);
        for (int i = 0; i < _Enemies.Length; i++)
            if (_Enemies[i].gameObject.activeSelf) _Players.Add(_Enemies[i]);
        PlayerStartTurn?.Invoke();
        news.SlideText("Tu Turno", new Color(0.7294118f, 0.3333333f, 0.8679245f));
    }

    void fInvokeNextTurn() { Invoke("fNextTurn", 1); }

    public void fNextTurn()
    {
        startTurnRoutine = LoadNextTurn();
        StartCoroutine(startTurnRoutine);
    }

    IEnumerator LoadNextTurn()
    {
        CheckAllCardCounts();
        yield return new WaitForEndOfFrame();
        currentTurn++;
        if (currentTurn >= _Players.Count) currentTurn = 0;
        if (_Players[currentTurn] != null)
        {
            yield return new WaitForSeconds(1);
            news.SlideText("Turno de " + _Players[currentTurn].name, new Color(1, 0.491087f, 0f));
            _Players[currentTurn].profileBox.DOFade(1, 0.15f);
            yield return new WaitForSeconds(2);
            _Players[currentTurn].StartTurn();
        }
        else
        {
            Debug.Log("Player's turn");
            if (!PlAc.skipTurn)
            {
                PlayerStartTurn?.Invoke();
                news.SlideText("Tu Turno", Color.purple);
            }
            else
            {
                news.SplashText("Te han bloqueado el turno!", new Color(0.1202933f, 1, 0));
                PlAc.skipTurn = false;
                currentTurn++;
                yield return new WaitForSeconds(1);
                news.SlideText("Turno de " + _Players[currentTurn].name, new Color(1, 0.491087f, 0f));
                _Players[currentTurn].profileBox.DOFade(1, 0.15f);
                yield return new WaitForSeconds(2);
                _Players[currentTurn].StartTurn();
            }
        }
        StopCoroutine(startTurnRoutine);
    }

    void EndGame(bool victor)
    {
        Debug.Log("GameEnded");
        if (victor)
        {
            news.SplashText("¡HAS GANADO!", Color.gold);
            audioSource.clip = sfxVictory;
            audioSource.Play();
            Debug.LogWarning(" Victory");
        }
        else
        {
            news.SplashText("¡HAS PERDIDO!", Color.red);
            audioSource.clip = sfxFail;
            audioSource.Play();
            Debug.LogWarning("FAilure");
        }

        StartCoroutine("ExitTheGame");
    }

    public void CheckAllCardCounts()
    {
        Debug.Log("Checking Cards");
        for (int i = 1; i < _Players.Count; i++)
        {

            while (_Players[i]._EnSy.hand.Count > 4)
            {
                GameObject toRemove = _Players[i]._EnSy.hand[_Players[i]._EnSy.hand.Count - 1];
                _Players[i]._EnSy.hand.Remove(toRemove);
                toRemove.transform.SetParent(FindFirstObjectByType<CardSystem>().deckPosition);
                toRemove.SetActive(false);
                Debug.Log("Removing card from " + _Players[i].gameObject.name); 
            }

            while (_Players[i]._EnSy.hand.Count < 4)
            { 
                _Players[i]._EnAc.DrawCard(); 
                Debug.Log("Adding card to " + _Players[i].gameObject.name); 
            }
        }

        while (PlAc.CdSy.hand.Count > 4)
        {
            GameObject toRemove = PlAc.CdSy.hand[PlAc.CdSy.hand.Count - 1];
            PlAc.CdSy.hand.Remove(toRemove);
            toRemove.transform.SetParent(FindFirstObjectByType<CardSystem>().deckPosition);
            toRemove.SetActive(false);
            Debug.Log("Removing card from Player's deck");
        }

        while (PlAc.CdSy.hand.Count < 4)
        {
            PlAc.CdSy.DrawCard();
            Debug.Log("Adding card to Player's deck");
        }
    }

    public void ExitGame()
    {
        StopAllCoroutines();
        StartCoroutine("ExitTheGame");
    }

    IEnumerator ExitTheGame()
    {
        yield return new WaitForSeconds(1);

        endScreen.DOFade(1, 3);

        yield return new WaitForSeconds(4);

        SceneManager.LoadScene(0);
    }
}
