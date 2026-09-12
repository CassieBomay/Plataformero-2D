using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Level_UI : MonoBehaviour
{
    [SerializeField] GameObject victoryPopup;
    [SerializeField] GameObject deathPopup;
    [SerializeField] CanvasGroup fade;

    [SerializeField] float fadeDuration = 1f;

    private Event_Manager eventManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventManager = FindAnyObjectByType<Event_Manager>();
        eventManager.OnVictory += ShowVictory;
        eventManager.OnPlayerDeath += ShowDeath;

        victoryPopup.SetActive(false);

        deathPopup.SetActive(false);

        fade.alpha = 0f;


    }

    private void ShowVictory()
    {
        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        yield return StartCoroutine(FadeToBlack());

        victoryPopup.SetActive(true);
    }

    private void ShowDeath()
    {
        StartCoroutine(DeathSequence());
    }
    private IEnumerator DeathSequence()
    {
        yield return StartCoroutine(FadeToBlack());

        deathPopup.SetActive(true);
    }

    private IEnumerator FadeToBlack()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            fade.alpha =Mathf.Clamp01(timer / fadeDuration);

            yield return null;
        }

        fade.alpha = 1f;
    }

    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.OnVictory -= ShowVictory;

            eventManager.OnPlayerDeath -= ShowDeath;
        }
    }

}
