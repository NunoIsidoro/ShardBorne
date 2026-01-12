using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject root;

    [Header("Refs")]
    public TMP_Text speakerText;
    public TMP_Text contentText;
    public Image portraitImage;

    [Header("Choices")]
    public Transform choicesContainer;
    public Button choiceButtonPrefab;

    [Header("Flow")]
    public Button continueButton;
    [Min(1f)] public float charsPerSecond = 45f;

    [Header("Audio Settings")]
    public AudioSource voiceSource;

    // Runtime
    Action onLineComplete;
    Coroutine typeCoroutine;
    string currentFullText = "";
    string currentSpeaker = "";

    void Awake()
    {
        if (continueButton != null) continueButton.onClick.AddListener(OnContinuePressed);
        Hide();
    }

    public void Show() { if (root) root.SetActive(true); }
    public void Hide() { if (root) root.SetActive(false); }

    public void ClearChoices()
    {
        if (!choicesContainer) return;
        for (int i = choicesContainer.childCount - 1; i >= 0; i--)
            Destroy(choicesContainer.GetChild(i).gameObject);
    }

    public void DisplayLine(string speaker, string text, Sprite portrait, AudioClip voiceClip, Action onComplete)
    {
        Show();
        ClearChoices();

        currentSpeaker = speaker ?? "";
        currentFullText = text ?? "";
        onLineComplete = onComplete;

        if (portraitImage)
        {
            portraitImage.sprite = portrait;
            portraitImage.enabled = portrait != null;
        }
        if (speakerText) speakerText.text = currentSpeaker;

        // --- 1. INÍCIO DO SOM ---
        if (voiceSource != null)
        {
            voiceSource.Stop(); // Garante que parou o anterior

            if (voiceClip != null)
            {
                voiceSource.clip = voiceClip;
                voiceSource.loop = true; // IMPORTANTE: O som repete se o texto for muito longo
                voiceSource.pitch = 1f;
                voiceSource.Play(); // Começa a tocar agora
            }
        }
        // ------------------------

        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        typeCoroutine = StartCoroutine(TypeText(currentFullText));
    }

    IEnumerator TypeText(string text)
    {
        if (contentText) contentText.text = "";
        float delay = 1f / Mathf.Max(1f, charsPerSecond);

        for (int i = 0; i < text.Length; i++)
        {
            if (contentText) contentText.text += text[i];
            yield return new WaitForSeconds(delay);
        }

        // --- 2. FIM DO SOM (Quando a escrita acaba naturalmente) ---
        StopVoice();
        // -----------------------------------------------------------

        typeCoroutine = null;
    }

    public void SkipTypewriter()
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            typeCoroutine = null;
            if (contentText) contentText.text = currentFullText;

            // --- 3. FIM DO SOM (Quando o jogador salta o texto) ---
            StopVoice();
            // ------------------------------------------------------
        }
    }

    // Função auxiliar para parar o som
    void StopVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }
    }

    void OnContinuePressed()
    {
        if (typeCoroutine != null) SkipTypewriter();
        else onLineComplete?.Invoke();
    }

    public void DisplayChoices(List<(string label, Action onClick)> choices)
    {
        ClearChoices();
        if (choicesContainer == null || choiceButtonPrefab == null || choices == null) return;

        foreach (var c in choices)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);
            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt) txt.text = c.label;
            btn.onClick.AddListener(() => c.onClick?.Invoke());
        }
    }
}