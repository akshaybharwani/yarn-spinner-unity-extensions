using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StoryImageBox : MonoBehaviour
{
    [SerializeField]
    private Image _backgroundImage = null;

    private LayoutElement _layoutElement;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _layoutElement = GetComponent<LayoutElement>();
    }

    public void InstantiateStoryImageBox(Sprite backgroundImage, float animationDuration, int height)
    {
        _layoutElement.DOMinSize(new Vector2(0, height), animationDuration + 1);

        StartCoroutine(ShowImageAfterSettingHeight(backgroundImage, animationDuration));
    }

    private IEnumerator ShowImageAfterSettingHeight(Sprite backgroundImage, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait + 1);

        _backgroundImage.sprite = backgroundImage;
        _canvasGroup.DOFade(1, timeToWait);
    }
}
