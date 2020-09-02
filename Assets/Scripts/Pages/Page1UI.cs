using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Page1UI : MonoBehaviour, IPageEvents
{
    [Header("References")]
    [SerializeField] GameEvents events = null;

    [Header("Prefabs")]
    public SectionData sectionPrefab;

    [Header("UI Section")]
    [SerializeField] RectTransform sectionContentArea;

    List<SectionData> sectionButtons = new List<SectionData>();

    public GameObject getGameObject => this.gameObject;

    void Awake()
    {
        events.sectionUI += SectionDisplay;
    }

    void OnApplicationQuit()
    {
        events.sectionUI -= SectionDisplay;
    }

    void EraseQuestionBlock()
    {
        foreach (var item in sectionButtons)
        {
            Destroy(item.gameObject);
        }

        sectionButtons.Clear();
    }

    void SectionDisplay(Section[] sections)
    {
        if(sectionButtons.Count == 0)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                SectionData sectionButton = Instantiate(sectionPrefab, sectionContentArea);
                sectionButtons.Add(sectionButton);
                sectionButton.SetSectionData(sections[i], i);
            }
        }
    }

    public void EnterPage(Action callback)
    {
        callback?.Invoke();
    }

    public void ExitPage(Action callback)
    {
        callback?.Invoke();
    }
}
