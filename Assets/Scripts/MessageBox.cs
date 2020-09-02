using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.Scripting;

public class MessageBox : MonoBehaviour
{
    public enum MessageBoxButtons { Ok, YesNo }

    public float alphaComponent;
    public float animSpeed;

    CanvasGroup canvasGroup;
    Image image;
    [SerializeField] TextMeshProUGUI messageString;
    [SerializeField] Button[] messegeBoxButtons;
    
    Text button1Text, button2Text;
    Action callback = null;

    public RectTransform Window;

    void OnEnable()
    {
        messegeBoxButtons[0].onClick.AddListener(Button1);
        messegeBoxButtons[1].onClick.AddListener(Button2);
    }

    void OnDisable()
    {
        messegeBoxButtons[0].onClick.RemoveListener(Button1);
        messegeBoxButtons[1].onClick.RemoveListener(Button2);
    }

    void Start()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;

        Window.gameObject.SetActive(false);

        button1Text = messegeBoxButtons[0].GetComponentInChildren<Text>();
        button2Text = messegeBoxButtons[1].GetComponentInChildren<Text>();
    }

    void Button1()
    {
        MessageBoxClose();
        callback?.Invoke();
    }

    void Button2()
    {
        MessageBoxClose();
    }

    void SetInfo(string message, MessageBoxButtons type)
    {
        messageString.text = message;

        for (int i = 0; i < messegeBoxButtons.Length; i++)
        {
            messegeBoxButtons[i].gameObject.SetActive(false);
        }

        switch (type)
        {
            case MessageBoxButtons.Ok:
                messegeBoxButtons[0].gameObject.SetActive(true);
                button1Text.text = "Ok";
                break;
            case MessageBoxButtons.YesNo:
                messegeBoxButtons[0].gameObject.SetActive(true);
                messegeBoxButtons[1].gameObject.SetActive(true);
                button1Text.text = "Yes";
                button2Text.text = "No";
                break;
            default:
                Debug.LogWarning("MessageBox.SetInfo. Unknown button type");
                break;
        }
    }

    public void Show(string message, MessageBoxButtons type, Action callback = null)
    {
        SetInfo(message, type);
        MessageBoxOpen();
        this.callback = callback;
    }

    void MessageBoxClose()
    {
        image.DOColor(new Color(image.color.r, image.color.g, image.color.b, 0.0f), animSpeed);

        DOTween.To(() => Window.localScale, x => Window.localScale = x, new Vector3(0, 0, 1), animSpeed).OnComplete(() =>
        {
            Window.gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = false;
        });
    }

    void MessageBoxOpen()
    {
        Window.gameObject.SetActive(true);

        image.DOColor(new Color(image.color.r, image.color.g, image.color.b, alphaComponent), animSpeed);

        DOTween.To(() => Window.localScale, x => Window.localScale = x, new Vector3(1, 1, 1), animSpeed).OnComplete(()=>
        {
            canvasGroup.blocksRaycasts = true;
        });
    }
}
