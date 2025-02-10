using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedUIManager : MonoBehaviour
{
    public GameManager GameManager;

    private float zoomAmount = 1.2f;
    private Vector3 originalScale = new Vector3(1, 1, 1);

    public Image GetReadyImage;
    public Image AttackerImage;
    public Image DefenderImage;
    public Image GoalImage;
    public Image TimeoutImage;
    public Image DrawImage;
    public Image DefenderWinImage;
    public List<Image> RoundList = new List<Image>();
    public List<Image> CountdownList = new List<Image>();

    public void StartAnimationSequance(int currentRound)
    {
        Debug.Log("StartAnimationSequance "+currentRound);
        StartCoroutine(AnimateImage(currentRound-1));
    }

    public void TimeoutAnimation()
    {
        StartCoroutine(AnimateTimeoutImage());
    }

    public void GoalAnimation()
    {
        StartCoroutine(AnimateGoalImage());
    }

    public void DefenderWinAnimation()
    {
        StartCoroutine(AnimateDefenderWinImage());
    }
    public void DrawAnimation()
    {
        StartCoroutine(AnimateDrawImage());
    }

    IEnumerator AnimateImage(int roundIndex)
    {
        GetReadyImage.rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(GetReadyImage, 0f, 1f, 0.5f));
        yield return StartCoroutine(ZoomImage(GetReadyImage, originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(GetReadyImage, 1f, 0f, 0.5f));
        yield return new WaitForSeconds(0.5f);

        if (GameManager.playerIsAttacking)
        {
            AttackerImage.rectTransform.localScale = originalScale;
            yield return StartCoroutine(FadeImage(AttackerImage, 0f, 1f, 0.5f));
            yield return StartCoroutine(ZoomImage(AttackerImage, originalScale, originalScale * zoomAmount, 1f));
            yield return StartCoroutine(FadeImage(AttackerImage, 1f, 0f, 0.5f));
        }
        else
        {
            DefenderImage.rectTransform.localScale = originalScale;
            yield return StartCoroutine(FadeImage(DefenderImage, 0f, 1f, 0.5f));
            yield return StartCoroutine(ZoomImage(DefenderImage, originalScale, originalScale * zoomAmount, 1f));
            yield return StartCoroutine(FadeImage(DefenderImage, 1f, 0f, 0.5f));
        }
        yield return new WaitForSeconds(0.5f);

        RoundList[roundIndex].rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(RoundList[roundIndex], 0f, 1f, 0.5f));
        yield return StartCoroutine(ZoomImage(RoundList[roundIndex], originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(RoundList[roundIndex], 1f, 0f, 0.5f));
        yield return new WaitForSeconds(0.5f);

        CountdownList[2].rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(CountdownList[2], 0f, 1f, 0.2f));
        yield return StartCoroutine(ZoomImage(CountdownList[2], originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(CountdownList[2], 1f, 0f, 0.2f));

        CountdownList[1].rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(CountdownList[1], 0f, 1f, 0.2f));
        yield return StartCoroutine(ZoomImage(CountdownList[1], originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(CountdownList[1], 1f, 0f, 0.2f));

        CountdownList[0].rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(CountdownList[0], 0f, 1f, 0.2f));
        yield return StartCoroutine(ZoomImage(CountdownList[0], originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(CountdownList[0], 1f, 0f, 0.2f));

        GameManager.StartTheGame();
    }

    IEnumerator AnimateTimeoutImage()
    {
        TimeoutImage.rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(TimeoutImage, 0f, 1f, 0.1f));
        yield return StartCoroutine(ZoomImage(TimeoutImage, originalScale, originalScale * 1.5f, 1f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeImage(TimeoutImage, 1f, 0f, 0.1f));
    }

    IEnumerator AnimateGoalImage()
    {
        GoalImage.rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(GoalImage, 0f, 1f, 0.1f));
        yield return StartCoroutine(ZoomImage(GoalImage, originalScale, originalScale * 1.5f, 1f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeImage(GoalImage, 1f, 0f, 0.1f));
    }

    IEnumerator AnimateDefenderWinImage()
    {
        DefenderWinImage.rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(DefenderWinImage, 0f, 1f, 0.1f));
        yield return StartCoroutine(ZoomImage(DefenderWinImage, originalScale, originalScale * 1.5f, 1f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeImage(DefenderWinImage, 1f, 0f, 0.1f));
    }

    IEnumerator AnimateDrawImage()
    {
        DrawImage.rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(DrawImage, 0f, 1f, 0.5f));
        yield return StartCoroutine(ZoomImage(DrawImage, originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(DrawImage, 1f, 0f, 0.5f));
        yield return new WaitForSeconds(0.5f);

        CountdownList[2].rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(CountdownList[2], 0f, 1f, 0.2f));
        yield return StartCoroutine(ZoomImage(CountdownList[2], originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(CountdownList[2], 1f, 0f, 0.2f));

        CountdownList[1].rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(CountdownList[1], 0f, 1f, 0.2f));
        yield return StartCoroutine(ZoomImage(CountdownList[1], originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(CountdownList[1], 1f, 0f, 0.2f));

        CountdownList[0].rectTransform.localScale = originalScale;
        yield return StartCoroutine(FadeImage(CountdownList[0], 0f, 1f, 0.2f));
        yield return StartCoroutine(ZoomImage(CountdownList[0], originalScale, originalScale * zoomAmount, 1f));
        yield return StartCoroutine(FadeImage(CountdownList[0], 1f, 0f, 0.2f));

        // Start pinalty
    }

    IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = image.gameObject.AddComponent<CanvasGroup>();
        }

        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    IEnumerator ZoomImage(Image image, Vector3 startScale, Vector3 endScale, float duration)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float lerpValue = timeElapsed / duration;
            image.rectTransform.localScale = Vector3.Lerp(startScale, endScale, lerpValue);
            yield return null;
        }

        image.rectTransform.localScale = endScale;
    }
}
