using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

[RequireComponent(typeof(UIDocument))]
public class TouchIndicatorUI : MonoBehaviour
{

    [Header("Editor")]
    [SerializeField] private bool useTouchSimulation = true; // desmarque ao testar no Device Simulator

    private bool _simulationEnabled;
    [Header("Visual")]
    [SerializeField] private float size = 120f;

    [Header("Animação por quadros (opcional)")]
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 12f;
    [SerializeField] private bool loop = true;

    [Header("Saída")]
    [SerializeField] private int removeDelayMs = 400;

    


    private class Indicator
    {
        public VisualElement element;
        public IVisualElementScheduledItem animation;
    }

    private VisualElement root;
    private readonly Dictionary<int, Indicator> indicators = new();
    private readonly HashSet<int> seen = new();
    private readonly List<int> toRemove = new();

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        EnhancedTouchSupport.Enable();
#if UNITY_EDITOR
        if (useTouchSimulation)
        {
            TouchSimulation.Enable();
            _simulationEnabled = true;
        }
#endif
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (_simulationEnabled)
        {
            TouchSimulation.Disable();
            _simulationEnabled = false;
        }
#endif
        EnhancedTouchSupport.Disable();

        foreach (var ind in indicators.Values)
        {
            ind.animation?.Pause();
            ind.element.RemoveFromHierarchy();
        }
        indicators.Clear();
    }

    void Update()
    {
        if (root == null || root.panel == null)
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            if (root == null || root.panel == null) return;
        }

        seen.Clear();

        foreach (var touch in Touch.activeTouches)
        {
            int id = touch.touchId;
            seen.Add(id);

            var screen = touch.screenPosition;
            if (!float.IsFinite(screen.x) || !float.IsFinite(screen.y)) continue;
            var pos = RuntimePanelUtils.ScreenToPanel(root.panel,
                new Vector2(screen.x, Screen.height - screen.y));

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Spawn(id, pos);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (!indicators.ContainsKey(id)) Spawn(id, pos);
                    Move(indicators[id].element, pos);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Release(id);
                    break;
            }
        }

       // Limpa indicadores de toques que sumiram sem a fase "Ended"


        toRemove.Clear();
        foreach (var id in indicators.Keys)
            if (!seen.Contains(id)) toRemove.Add(id);
        foreach (var id in toRemove) Release(id);
    }

    private void Spawn(int id, Vector2 pos)
    {
        if (indicators.ContainsKey(id)) return;

        var el = new VisualElement();
        el.AddToClassList("touch-indicator");
        el.pickingMode = PickingMode.Ignore;          
        el.style.position = Position.Absolute;
        el.style.width = size;
        el.style.height = size;

        var ind = new Indicator { element = el };

        if (frames != null && frames.Length > 0)
        {
            int frame = 0;
            el.style.backgroundImage = new StyleBackground(frames[0]);

            ind.animation = el.schedule.Execute(() =>
            {
                frame++;
                if (frame >= frames.Length)
                {
                    if (!loop) { ind.animation.Pause(); return; }
                    frame = 0;
                }
                el.style.backgroundImage = new StyleBackground(frames[frame]);
            }).Every((long)(1000f / Mathf.Max(fps, 1f)));
        }

        root.Add(el);
        el.BringToFront(); // fica por cima do menu
        indicators[id] = ind;
        Move(el, pos);
    }

    private void Release(int id)
    {
        if (!indicators.TryGetValue(id, out var ind)) return;
        indicators.Remove(id);

        var el = ind.element;
        el.AddToClassList("touch-indicator--hidden"); // dispara a transição do USS
        el.schedule.Execute(() =>
        {
            ind.animation?.Pause();
            el.RemoveFromHierarchy();
        }).StartingIn(removeDelayMs);
    }

    private void Move(VisualElement el, Vector2 pos)
    {
        el.style.left = pos.x - size / 2f;
        el.style.top = pos.y - size / 2f;
    }
}