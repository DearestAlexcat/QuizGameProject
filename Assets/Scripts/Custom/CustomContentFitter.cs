using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomContentFitter : MonoBehaviour
{
    private Vector2 _defaultSize;

    private void OnValidate()
    {
        var rectTransform = (RectTransform)transform;
        _defaultSize = rectTransform.sizeDelta;
    }

    public void RefreshSize()
    {
        var rectTransform = (RectTransform)transform;
        RefreshTransformSize(rectTransform);
    }

    private void RefreshTransformSize(RectTransform transform)
    {
        var bounds = new RectBounds();
        var contentFitter = transform.GetComponent<CustomContentFitter>();
        bool isContentFitter = contentFitter != null;

        var childBounds = new List<RectBounds>();
        foreach (RectTransform child in transform)
        {
            if (!child.gameObject.activeSelf)
            {
                continue;
            }

            RefreshTransformSize(child);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform);

            if (!isContentFitter)
            {
                continue;
            }
            childBounds.Add(new RectBounds(-child.anchoredPosition, child.sizeDelta));
            bounds.Encapsulate(-child.anchoredPosition, child.sizeDelta);
        }

        if (!isContentFitter)
        {
            return;
        }

        bounds.Encapsulate(Vector2.zero, contentFitter._defaultSize);
        transform.sizeDelta = bounds.max - bounds.min;
    }

    private class RectBounds
    {
        public Vector2 min;
        public Vector2 max;

        public RectBounds()
        {
            min = Vector2.positiveInfinity;
            max = Vector2.negativeInfinity;
        }

        public RectBounds(Vector2 position, Vector2 size)
        {
            min = position;
            max = new Vector2(position.x + size.x, position.y + size.y);
        }

        public void Encapsulate(Vector2 position, Vector2 size)
        {
            var min = position;
            var max = new Vector2(-position.x + size.x, position.y + size.y);

            if (min.x < this.min.x)
            {
                this.min.x = min.x;
            }
            if (max.x > this.max.x)
            {
                this.max.x = max.x;
            }
            if (min.y < this.min.y)
            {
                this.min.y = min.y;
            }
            if (max.y > this.max.y)
            {
                this.max.y = max.y;
            }
        }
    }
}