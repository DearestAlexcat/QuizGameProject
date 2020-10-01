using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ViewPage1 : MonoBehaviour, IPageEvents
{
    [Header("Prefabs")]
    public SectionData sectionPrefab;

    [Header("Links")]
    [SerializeField]
    ProjectModel _model;
    [SerializeField]
    Pagination _pagination;
    [SerializeField]
    Canvas _parentCanvas;
    [SerializeField]
    RectTransform _startObjectPos;

    [Header("Links for Scaling")]
    [SerializeField]
    RectTransform _navigation;
    [SerializeField]
    RectTransform _pageHolder;
    [SerializeField]
    RectTransform _head;
    [SerializeField]
    RectTransform _rootCanvas;
    [SerializeField]
    RectTransform _generalHolder;

    List<SectionData> sectionButtons = new List<SectionData>();
    GameController _controller = new GameController();

    void OnSectionChoisedEventHandler(object sender, SectionChoisedEventArgs e)
    {
        _pagination.GotoNextPage(e.Name, () =>
        {
            bool result =_controller.Execute(ModelOperation.ChoiseSection, _model, e);
            if(result)
            {
                _pagination.SequenceAnim.Kill();
            }
        });
    }

    public void EnterPage()
    {
        _pageHolder.sizeDelta = new Vector2(_pageHolder.rect.width, _rootCanvas.rect.height - _head.rect.height);
        _navigation.gameObject.SetActive(false);
        _parentCanvas.enabled = true;
    }

    public void ExitPage()
    {
        _pageHolder.sizeDelta = new Vector2(_pageHolder.rect.width, 
                  _rootCanvas.rect.height - _head.rect.height - _navigation.rect.height);
        _navigation.gameObject.SetActive(true);
        _parentCanvas.enabled = false;
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
        RectTransform rectButton = null;
        SectionData sectionButton = null;

        float y_pos = _startObjectPos.rect.height * -1;
        float dy = 0;

        if (sectionButtons.Count == 0)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                sectionButton = Instantiate(sectionPrefab, transform);
                sectionButtons.Add(sectionButton);
                sectionButton.SetSectionData(sections[i], i);
                sectionButton.sectionChoised += OnSectionChoisedEventHandler;

                rectButton = sectionButton.GetComponent<RectTransform>();
                dy = y_pos - i * rectButton.rect.height;
                rectButton.anchoredPosition = new Vector3(0, dy, 1);
            }
        }
    }

    private void Start()
    {
        _generalHolder.sizeDelta = new Vector2(_pageHolder.rect.width,
            _rootCanvas.rect.height - _head.rect.height);
        EnterPage();
        _pagination.RegisterPage(0, this);
        SectionDisplay(_model.Sections); // Data Bind
    }
}
