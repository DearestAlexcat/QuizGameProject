using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

interface IPageEvents
{
    void EnterPage(Action callback);
    void ExitPage(Action callback);
    GameObject getGameObject { get; }
}

[Serializable]
public class UIManagerParameters
{
    [Header("Button back param")]
    [SerializeField] string[] goToText =
    {
        "",
        "< Go to qualification Page",
        "< Go to skill statistics Page"
    };
    public string[] GoToText => goToText;

    [Header("Animation speed")]
    public float menuSpeed;
    public float loadingSpeed;
}

[Serializable]
public class UIElements
{
    [Header("Buttons Submit")]
    public Text buttonBack;

    [Header("Canvas Groups")]
    public CanvasGroup pageCanvasGroup;
    public CanvasGroup backgroundCanvasGroup;

    [Header("RectTransforms")]
    public RectTransform menuItem;
    public RectTransform backgroundRect;

    [Header("RectTransforms")]
    public Text path;

    [Header("Loading anim")]
    public CanvasGroup loadingAnim;
    public TextMeshProUGUI left;
    public TextMeshProUGUI right;
}

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameEvents events = null;
    [SerializeField] MessageBox messageBox = null;
    [SerializeField] GameObject pagesHolder = null;
    [Space]
    [SerializeField] UIManagerParameters uIManagerParameters = new UIManagerParameters();
    [SerializeField] UIElements uIElements = new UIElements();

    IPageEvents[] pages;
    
    int currentPage;
    int previousPage;
    bool switchMenu = false; // для работы меню
    string currentPath;

    //------------------------------------------------------------------------------------------------------------------------

    void SetTextBackButton(int index)
    {
        if (string.IsNullOrEmpty(uIManagerParameters.GoToText[index]))
        {
            uIElements.buttonBack.gameObject.SetActive(false);
        }
        else
        {
            uIElements.buttonBack.gameObject.SetActive(true);
            uIElements.buttonBack.text = uIManagerParameters.GoToText[index];
        }
    }

    #region Pagination
        public void GotoNextPage(string pageName)
        {
            pages[currentPage].EnterPage(() =>
            {
                StartLoadingAnimation(NextPage);
                NextPath(pageName);
            });
        }

        void GotoPreviousPage()
        {                
            pages[currentPage].ExitPage(() => 
            {
                StartLoadingAnimation(PreviousPage);
                PrevPath();
            });
        }

        void StartLoadingAnimation(Action action)
        {
            uIElements.backgroundCanvasGroup.blocksRaycasts = false; // Нельзя взаимодействовать с интерфейсом при подгрузке

            uIElements.loadingAnim.blocksRaycasts = true;   
            uIElements.loadingAnim.DOFade(1.0f, uIManagerParameters.loadingSpeed); // Отобразить подгрузку

            int loops = UnityEngine.Random.Range(1, 5);

            Color leftEnd = uIElements.left.color; 
            Color rightEnd = uIElements.right.color;
            leftEnd.a = 0.5f;
            rightEnd.a = 1.0f;

            Sequence sequence = DOTween.Sequence();
                sequence.OnComplete(
                    () => {
                        // Сброс параметров
                        uIElements.loadingAnim.blocksRaycasts = false;
                        uIElements.left.color = new Color(uIElements.left.color.r, uIElements.left.color.g, uIElements.left.color.b, 1.0f); 
                        uIElements.right.color = new Color(uIElements.left.color.r, uIElements.left.color.g, uIElements.left.color.b, 0.5f);
                        uIElements.left.fontSize = 180;
                        uIElements.right.fontSize = 120;
                        uIElements.loadingAnim.alpha = 0.0f;

                        // Переход на страницу 
                        action?.Invoke();
                    });

            // Anim color 
            sequence.Insert(0, DOTween.To(() => uIElements.left.color.a, x => {
                                leftEnd.a = x;
                                uIElements.left.color = leftEnd;
                            }, 0.5f, uIManagerParameters.loadingSpeed).SetEase(Ease.InOutQuad).SetLoops(loops, LoopType.Yoyo));

            sequence.Insert(0, DOTween.To(() => uIElements.right.color.a, x => {
                                rightEnd.a = x;
                                uIElements.right.color = rightEnd;
                            }, 1.0f, uIManagerParameters.loadingSpeed).SetEase(Ease.InOutQuad).SetLoops(loops, LoopType.Yoyo));

            // Anim font size
            sequence.Insert(0, DOTween.To(() => uIElements.left.fontSize,  x => uIElements.left.fontSize = x,  120, uIManagerParameters.loadingSpeed).SetEase(Ease.InOutQuad).SetLoops(loops, LoopType.Yoyo));
            sequence.Insert(0, DOTween.To(() => uIElements.right.fontSize, x => uIElements.right.fontSize = x, 180, uIManagerParameters.loadingSpeed).SetEase(Ease.InOutQuad).SetLoops(loops, LoopType.Yoyo));
        }

        void NextPath(string pageName)
        {
            if (pageName != null)
            {
                currentPath = uIElements.path.text;
                if (string.IsNullOrEmpty(currentPath))
                {
                    currentPath += pageName;
                }
                else
                {
                    currentPath += " / " + pageName;
                }
            }
        }

        void PrevPath()
        {
            int startIdx = uIElements.path.text.LastIndexOf(" / ");

            currentPath = uIElements.path.text;

            if (startIdx > -1)
            {
                currentPath = currentPath.Remove(startIdx);
            }
            else
            {
                currentPath = default(string);
            }
        }

        void SetPatch()
        {
            if (string.IsNullOrEmpty(currentPath)) // Если нечего отображать
            {
                uIElements.path.gameObject.SetActive(false);
            }
            else
            {
                uIElements.path.gameObject.SetActive(true);
            }

            uIElements.path.text = currentPath;
        }

        void NextPage()
        {
            if (currentPage == pages.Length)
            {
                return;
            }

            previousPage = currentPage;
            currentPage++;

            SetTextBackButton(currentPage); 

            pages[previousPage].getGameObject.SetActive(false);
            pages[currentPage].getGameObject.SetActive(true);

            SetPatch();

            uIElements.backgroundCanvasGroup.blocksRaycasts = true;
        }

        void PreviousPage()
        {
            if (previousPage == -1)
            {
                return;
            }

            SetTextBackButton(previousPage);

            pages[currentPage].getGameObject.SetActive(false);
            pages[previousPage].getGameObject.SetActive(true);

            SetPatch();

            uIElements.backgroundCanvasGroup.blocksRaycasts = true;

            currentPage = previousPage;
            previousPage--;
        }
    #endregion

    public void EmptyMethod() // Событие OnClick
    {
        messageBox.Show("This feature is under development", MessageBox.MessageBoxButtons.Ok);
    }

    public void MenuItemStartAnim()
    {
        float param = switchMenu ? Mathf.Ceil(uIElements.backgroundRect.rect.height) : 0.0f;
        uIElements.pageCanvasGroup.blocksRaycasts = switchMenu;
        uIElements.menuItem.DOAnchorPosY(param, uIManagerParameters.menuSpeed).OnComplete(() => { switchMenu = !switchMenu; });
    }

    void Start()
    {
        pages = pagesHolder.GetComponentsInChildren<IPageEvents>(true);

        uIElements.menuItem.gameObject.SetActive(true);

        SetTextBackButton(0);

        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);
        uIElements.menuItem.anchoredPosition = new Vector2(uIElements.menuItem.anchoredPosition.x, Mathf.Ceil(uIElements.backgroundRect.rect.height));
    }

    void OnEnable()
    {
        events.displayNextPage += GotoNextPage;
        events.displayPreviousPage += GotoPreviousPage;
    }

    void OnDisable()
    {
        events.displayNextPage -= GotoNextPage;
        events.displayPreviousPage -= GotoPreviousPage;
    }
}





