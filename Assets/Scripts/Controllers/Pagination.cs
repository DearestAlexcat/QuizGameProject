using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Pagination : MonoBehaviour
{
    [Header("Loading anim")]
    [SerializeField] CanvasGroup loadingAnim;
    [SerializeField] TextMeshProUGUI left;
    [SerializeField] TextMeshProUGUI right;
    [SerializeField] float speedAnim;
    [SerializeField] int timeout;
    [SerializeField] CanvasGroup pageHolderCG;

    int currentPage;
    int previousPage;
    string currentPath;
    public Sequence sequenceAnim;

    public Sequence SequenceAnim => sequenceAnim;

    [SerializeField]
    string[] goToText =
    {
        "",
        "< Go to qualification Page",
        "< Go to skill statistics Page"
    };

    SortedDictionary<int, IPageEvents> pages = new SortedDictionary<int, IPageEvents>();

    public Text path;
    [Header("Buttons Submit/Next")]
    public Text buttonBack;

    public void RegisterPage(int numberPage, IPageEvents pageEvents)
    {
        if (!pages.ContainsKey(numberPage))
        {
            pages.Add(numberPage, pageEvents);
        }
    }

    public void UnregisterPage()
    {
        pages.Clear();
    }

    public void GotoNextPage(string pageName, System.Action action = null)
    {
        StartLoadingAnimation(() => {
                NextPage();
                NextPath(pageName);
                action?.Invoke();
            });
    }

    public void GotoPreviousPage() // OnClick. Navigation -> Button
    {
        PreviousPage();
        PrevPath();
    }

    void StartLoadingAnimation(TweenCallback tc) // Анимацию отключает Page после загрузки. Только при переходе на следующую
    {
        pageHolderCG.blocksRaycasts = false; // Нельзя взаимодействовать с интерфейсом при подгрузке
        loadingAnim.blocksRaycasts = true;

        loadingAnim.DOFade(1.0f, speedAnim).OnComplete(tc); // Отобразить подгрузку, и только после делать переход
       
        Color leftEnd = left.color;
        Color rightEnd = right.color;
        leftEnd.a = 0.5f;
        rightEnd.a = 1.0f;
        
        sequenceAnim = DOTween.Sequence();

        sequenceAnim.OnComplete(()=>
        {
            //TODO: Действия, когда не удалось загрузить данные
            GotoPreviousPage();
        });

        sequenceAnim.OnKill(() =>
        {
            // Сброс параметров
            loadingAnim.blocksRaycasts = false;
            left.color = new Color(left.color.r, left.color.g, left.color.b, 1.0f);
            right.color = new Color(left.color.r, left.color.g, left.color.b, 0.5f);
            left.fontSize = 180;
            right.fontSize = 120;
            loadingAnim.alpha = 0.0f;

            pageHolderCG.blocksRaycasts = true;
        });

        int delay = Mathf.CeilToInt(timeout / speedAnim);

        // Anim color 
        sequenceAnim.Insert(0, DOTween.To(() => left.color.a,  x => { leftEnd.a = x; left.color = leftEnd; },    0.5f, speedAnim).SetEase(Ease.InOutQuad).SetLoops(delay, LoopType.Yoyo));
        sequenceAnim.Insert(0, DOTween.To(() => right.color.a, x => { rightEnd.a = x; right.color = rightEnd; }, 1.0f, speedAnim).SetEase(Ease.InOutQuad).SetLoops(delay, LoopType.Yoyo));
        // Anim font size
        sequenceAnim.Insert(0, DOTween.To(() => left.fontSize,  x => left.fontSize = x, 120,  speedAnim).SetEase(Ease.InOutQuad).SetLoops(delay, LoopType.Yoyo));
        sequenceAnim.Insert(0, DOTween.To(() => right.fontSize, x => right.fontSize = x, 180, speedAnim).SetEase(Ease.InOutQuad).SetLoops(delay, LoopType.Yoyo));
    }

    void NextPath(string pageName)
    {
        if (pageName != null)
        {
            currentPath = path.text;
            if (string.IsNullOrEmpty(currentPath))
            {
                currentPath += pageName;
            }
            else
            {
                currentPath += " / " + pageName;
            }

            path.text = currentPath;
        }
    }

    void PrevPath()
    {
        int startIdx = path.text.LastIndexOf(" / ");

        currentPath = path.text;
        if (startIdx > -1)
        {
            currentPath = currentPath.Remove(startIdx);
        }
        else
        {
            currentPath = default;
        }

        path.text = currentPath;
    }

    void SetTextBackButton(int index)
    {
        if (string.IsNullOrEmpty(goToText[index]))
        {
            buttonBack.gameObject.SetActive(false);
        }
        else
        {
            buttonBack.gameObject.SetActive(true);
            buttonBack.text = goToText[index];
        }
    }

    void NextPage()
    {
        if (currentPage == pages.Count)
        {
            return;
        }

        previousPage = currentPage;
        currentPage++;

        SetTextBackButton(currentPage);

        pages[previousPage].ExitPage();
        pages[currentPage].EnterPage();
    }

    void PreviousPage()
    {
        if (previousPage == -1)
        {
            return;
        }

        SetTextBackButton(previousPage);

        pages[currentPage].ExitPage();
        pages[previousPage].EnterPage();

        currentPage = previousPage;
        previousPage--;
    }

    private void Start()
    {
        path.text = default;
    }
}
