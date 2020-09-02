using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScrollController : MonoBehaviour
{
    [SerializeField]  Scrollbar scrollbar = null;

    [SerializeField] Vector2 minScale = new Vector2(1.0f, 1.0f);
    [SerializeField] Vector2 maxScale = new Vector2(1.0f, 1.0f);

    List<ScrollElement> scrollableItems = new List<ScrollElement>();

    [SerializeField] float speed = 1.0f;
    [SerializeField] float step;

    [Range(0.1f, 0.5f)]
    [SerializeField]
    float percentageOfStep = 0.18f;

    float currentCursorPos, prevCursorPos;
    float otherStep;
    float target;
    float targetScrollbarValue;

    int left, right, current;
    bool resetScroll;

    void ResetScroll()
    {
        if (scrollableItems.Count > 1)
        {
            step = 1.0f / (scrollableItems.Count - 1);
        }
        
        otherStep = step * percentageOfStep;

        resetScroll = true;
        target = 0;

        right = 1;
        current = 0;
        left = -1;

        prevCursorPos = 0;
        currentCursorPos = 0;
    }

    public void Attach(ScrollElement scrollElement)
    {            
        if(scrollElement != null)
        {
            if(scrollableItems.Count != 0) // Первый элемент не должен блокироваться
            {
                scrollElement.cg.blocksRaycasts = false;  
            }

            scrollableItems.Add(scrollElement);
            ResetScroll();
        }
        else
        {
            Debug.LogError("ScrollController.Attach. ScrollElement is null");
        }
    }

    public void Detach(ScrollElement scrollElement)
    {       
        if (scrollElement != null)
        {
            scrollableItems.Remove(scrollElement);
            ResetScroll();
        }
        else
        {
            Debug.LogError("ScrollController.Detach. ScrollElement is null");
        }
    }

    void SwitchRaycasts()
    {
        if (scrollableItems == null || scrollableItems.Count < 2)
        {
            return;
        }

        if (left >= 0)
        {
            scrollableItems[left].cg.blocksRaycasts = false;
        }

        if (right < scrollableItems.Count)
        {
            scrollableItems[right].cg.blocksRaycasts = false;
        }
        
        scrollableItems[current].cg.blocksRaycasts = true;
    }

    void Update()
    {
        if (scrollableItems == null || scrollableItems.Count < 2)
        {
            scrollableItems[current].transform.localScale = Vector2.Lerp(scrollableItems[current].transform.localScale, maxScale, speed);
            return;
        }

        if (resetScroll)
        {
            resetScroll = false;
            scrollbar.value = 0.0f;
        }

        bool isPressedButton = Input.GetMouseButton(0);

        targetScrollbarValue = isPressedButton ? scrollbar.value : target;

        if (scrollbar.value > currentCursorPos - otherStep || scrollbar.value < prevCursorPos + otherStep)
        {
            if (scrollbar.value > currentCursorPos + otherStep)
            {
                target += step;

                prevCursorPos = currentCursorPos;
                currentCursorPos = target;

                left++;
                right++;
                current++;

                SwitchRaycasts();
            }
     
            if (scrollbar.value < prevCursorPos - otherStep)
            {
                target -= step;

                currentCursorPos = prevCursorPos;
                prevCursorPos = target;

                left--;
                right--;
                current--;

                SwitchRaycasts();
            }
        }

        if (left >= 0)
        {
            scrollableItems[left].transform.localScale = Vector2.Lerp(scrollableItems[left].transform.localScale, minScale, speed);
        }
        
        if (right < scrollableItems.Count)
        {
            scrollableItems[right].transform.localScale = Vector2.Lerp(scrollableItems[right].transform.localScale, minScale, speed);
        }

        scrollableItems[current].transform.localScale  = Vector2.Lerp(scrollableItems[current].transform.localScale, maxScale, speed);
        

        if (!Mathf.Approximately(scrollbar.value, targetScrollbarValue))
        {
            scrollbar.value = Mathf.MoveTowards(scrollbar.value, target, speed);
        }
        else if (!isPressedButton)
        {
            prevCursorPos = currentCursorPos = target;
        }
    }
}

