using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Faz um ScrollView horizontal funcionar como carrossel com "trava":
/// ao soltar o dedo, o item mais próximo do centro (ou o vizinho, se foi um deslize)
/// anima até ficar centralizado. Nunca pula mais de um item por deslize.
///
/// Requisitos no UI Builder: ScrollView com Mode = Horizontal.
/// </summary>
public class CarouselSnap
{
    private const string SelectedClass = "carousel-item--selected";

    private readonly ScrollView _scroll;
    private readonly float _snapMs;
    private readonly float _minSwipePx;

    private bool _dragging;
    private int _startIndex;
    private float _startOffset;
    private int _currentIndex;
    private IVisualElementScheduledItem _anim;

    public int CurrentIndex => _currentIndex;

    /// <summary>Disparado quando um item termina de travar no centro.</summary>
    public event Action<int> Snapped;

    public CarouselSnap(ScrollView scroll, float snapMs = 250f, float minSwipePx = 40f)
    {
        _scroll = scroll;
        _snapMs = Mathf.Max(1f, snapMs);
        _minSwipePx = minSwipePx;

        if (_scroll == null) return;

        // Sem elástico e sem inércia: quem decide onde parar é este script
        _scroll.touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;
        _scroll.scrollDecelerationRate = 0f;
        _scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

        _scroll.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        _scroll.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
        _scroll.RegisterCallback<PointerCancelEvent>(OnPointerCancel, TrickleDown.TrickleDown);
        _scroll.RegisterCallback<PointerCaptureOutEvent>(OnCaptureOut, TrickleDown.TrickleDown);
        _scroll.contentViewport.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    // ---------- API pública ----------

    /// <summary>Chame depois de adicionar ou remover itens do carrossel.</summary>
    public void Refresh()
    {
        if (_scroll == null) return;
        _scroll.schedule.Execute(() =>
        {
            ApplyEdgePadding();
            SnapTo(Mathf.Clamp(_currentIndex, 0, Mathf.Max(0, ItemCount() - 1)), false);
        }).StartingIn(50);
    }

    /// <summary>Centraliza o item de índice dado (útil para botões de seta).</summary>
    public void SnapTo(int index, bool animate)
    {
        int count = ItemCount();
        if (count == 0) return;

        index = Mathf.Clamp(index, 0, count - 1);
        float target = OffsetForIndex(index);

        _currentIndex = index;
        UpdateSelectedClass();

        _anim?.Pause();
        float from = _scroll.scrollOffset.x;

        if (!animate || Mathf.Approximately(from, target))
        {
            SetX(target);
            Snapped?.Invoke(index);
            return;
        }

        float start = Time.realtimeSinceStartup;
        _anim = _scroll.schedule.Execute(() =>
        {
            float t = Mathf.Clamp01((Time.realtimeSinceStartup - start) * 1000f / _snapMs);
            float eased = 1f - Mathf.Pow(1f - t, 3f);   // desacelera no final
            SetX(Mathf.Lerp(from, target, eased));

            if (t >= 1f)
            {
                _anim.Pause();
                Snapped?.Invoke(index);
            }
        }).Every(16);
    }

    // ---------- Arrasto ----------

    private void OnPointerDown(PointerDownEvent evt)
    {
        _dragging = true;
        _startOffset = _scroll.scrollOffset.x;
        _startIndex = _currentIndex;
        _anim?.Pause();   // o dedo interrompe uma trava em andamento
    }

    private void OnPointerUp(PointerUpEvent evt) => FinishDrag();
    private void OnPointerCancel(PointerCancelEvent evt) => FinishDrag();
    private void OnCaptureOut(PointerCaptureOutEvent evt) => FinishDrag();

    private void FinishDrag()
    {
        if (!_dragging) return;
        _dragging = false;

        if (ItemCount() == 0) return;

        int nearest = NearestIndex();
        float delta = _scroll.scrollOffset.x - _startOffset;

        // Deslize curto: vai para o vizinho mesmo sem passar da metade
        int target = nearest;
        if (Mathf.Abs(delta) >= _minSwipePx && nearest == _startIndex)
            target = _startIndex + (delta > 0f ? 1 : -1);

        // Nunca pula mais de um item por deslize
        target = Mathf.Clamp(target, _startIndex - 1, _startIndex + 1);

        SnapTo(target, true);
    }

    // ---------- Cálculos de posição ----------

    private int ItemCount() => _scroll.contentContainer.childCount;

    private float ViewportWidth() => _scroll.contentViewport.layout.width;

    private float OffsetForIndex(int index)
    {
        var item = _scroll.contentContainer[index];
        float itemCenter = item.layout.x + item.layout.width * 0.5f;
        float max = Mathf.Max(0f, _scroll.contentContainer.layout.width - ViewportWidth());
        return Mathf.Clamp(itemCenter - ViewportWidth() * 0.5f, 0f, max);
    }

    private int NearestIndex()
    {
        float viewCenter = _scroll.scrollOffset.x + ViewportWidth() * 0.5f;
        int best = 0;
        float bestDist = float.MaxValue;

        for (int i = 0; i < ItemCount(); i++)
        {
            var item = _scroll.contentContainer[i];
            float dist = Mathf.Abs(item.layout.x + item.layout.width * 0.5f - viewCenter);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = i;
            }
        }
        return best;
    }

    private void SetX(float x)
    {
        _scroll.scrollOffset = new Vector2(x, _scroll.scrollOffset.y);
    }

    // Margem nas pontas para o primeiro e o último item também conseguirem centralizar
    private void ApplyEdgePadding()
    {
        int count = ItemCount();
        float vw = ViewportWidth();
        if (count == 0 || float.IsNaN(vw) || vw <= 0f) return;

        float firstW = _scroll.contentContainer[0].layout.width;
        float lastW = _scroll.contentContainer[count - 1].layout.width;
        if (float.IsNaN(firstW) || float.IsNaN(lastW)) return;

        _scroll.contentContainer.style.paddingLeft = Mathf.Max(0f, (vw - firstW) * 0.5f);
        _scroll.contentContainer.style.paddingRight = Mathf.Max(0f, (vw - lastW) * 0.5f);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt) => Refresh();

    private void UpdateSelectedClass()
    {
        for (int i = 0; i < ItemCount(); i++)
            _scroll.contentContainer[i].EnableInClassList(SelectedClass, i == _currentIndex);
    }

    public void Dispose()
    {
        _anim?.Pause();
        if (_scroll == null) return;

        _scroll.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        _scroll.UnregisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
        _scroll.UnregisterCallback<PointerCancelEvent>(OnPointerCancel, TrickleDown.TrickleDown);
        _scroll.UnregisterCallback<PointerCaptureOutEvent>(OnCaptureOut, TrickleDown.TrickleDown);
        _scroll.contentViewport.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }
}