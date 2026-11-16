using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// Tutorial: pergunta (Sim/Não) e, se aceito, as páginas em sequência.
/// Ao terminar (ou ao recusar) chama onFinished.
/// </summary>
public class TutorialPanel
{
    private readonly VisualElement _question;
    private readonly VisualElement _pagesRoot;
    private readonly List<VisualElement> _pages = new List<VisualElement>();
    private readonly Button _yes;
    private readonly Button _no;
    private readonly Button _next;
    private readonly Action _onFinished;

    private int _currentPage;

    public TutorialPanel(VisualElement root, UISoundManager sound, Action onFinished)
    {
        _onFinished = onFinished;

        _question = root.Q<VisualElement>(UINames.TutorialQuestion);
        _pagesRoot = root.Q<VisualElement>(UINames.TutorialPages);
        _yes = root.Q<Button>(UINames.TutorialYes);
        _no = root.Q<Button>(UINames.TutorialNo);
        _next = root.Q<Button>(UINames.NextPage);

        for (int i = 1; i <= UINames.TutorialPageCount; i++)
        {
            var page = root.Q<VisualElement>(UINames.TutorialPagePrefix + i);
            if (page != null) _pages.Add(page);
        }

        _yes?.RegisterCallback<ClickEvent>(OnYes);
        _no?.RegisterCallback<ClickEvent>(OnNo);
        _next?.RegisterCallback<ClickEvent>(OnNext);

        UIUtils.BindSound(sound, _yes, "Confirm");
        UIUtils.BindSound(sound, _no, "Confirm");
        UIUtils.BindSound(sound, _next, "Confirm");
    }

    // ---------- Pergunta ----------
    public void ShowQuestion()
    {
        if (_question == null) return;

        _question.style.display = DisplayStyle.Flex;
        _question.RemoveFromClassList(UINames.TutorialOff);
        _question.schedule.Execute(() => _question.AddToClassList(UINames.TutorialQuestionOn));
    }

    private void CloseQuestion()
    {
        if (_question == null) return;

        _question.style.display = DisplayStyle.None;
        _question.RemoveFromClassList(UINames.TutorialOn);
        _question.RemoveFromClassList(UINames.TutorialQuestionOn);
        _question.AddToClassList(UINames.TutorialOff);
    }

    private void OnYes(ClickEvent evt)
    {
        CloseQuestion();
        StartPages();
    }

    private void OnNo(ClickEvent evt)
    {
        CloseQuestion();
        _onFinished?.Invoke();
    }

    // ---------- Páginas ----------
    private void StartPages()
    {
        if (_pagesRoot == null) return;

        _pagesRoot.style.display = DisplayStyle.Flex;
        _currentPage = 0;
        ShowPage(_currentPage);

        _pagesRoot.RemoveFromClassList(UINames.TutorialOff);
        _pagesRoot.schedule.Execute(() => _pagesRoot.AddToClassList(UINames.TutorialOn));
    }

    private void OnNext(ClickEvent evt)
    {
        evt.StopPropagation();

        _currentPage++;

        if (_currentPage >= _pages.Count) EndTutorial();
        else ShowPage(_currentPage);
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < _pages.Count; i++)
            _pages[i].style.display = i == index ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void EndTutorial()
    {
        if (_pagesRoot != null)
        {
            _pagesRoot.style.display = DisplayStyle.None;
            _pagesRoot.RemoveFromClassList(UINames.TutorialOn);
            _pagesRoot.AddToClassList(UINames.TutorialOff);
        }

        CloseQuestion();
        _onFinished?.Invoke();
    }

    public void Dispose()
    {
        _yes?.UnregisterCallback<ClickEvent>(OnYes);
        _no?.UnregisterCallback<ClickEvent>(OnNo);
        _next?.UnregisterCallback<ClickEvent>(OnNext);
    }
}
