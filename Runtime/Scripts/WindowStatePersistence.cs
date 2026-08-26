using UnityEngine;
using Kirurobo;

public class WindowStatePersistence : MonoBehaviour
{
    public static WindowStatePersistence Instance { get; private set; }
    public Vector2 WindowPosition { get; private set; }
    public Vector2 WindowSize { get; private set; }
    public bool IsZoomed { get; private set; }

    public Rect MonitorRect { get; private set; }

    private bool hasSavedState;
    public bool HasSavedState => hasSavedState;
    public Vector2 NormalizedPosition { get; private set; }
    public Vector2 NormalizedSize { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveState(UniWindowController controller)
    {
        if (controller == null)
            return;

        WindowPosition = controller.windowPosition;
        WindowSize = controller.windowSize;
        IsZoomed = controller.isZoomed;

        int monitorIndex = FindMonitorForWindow(WindowPosition, WindowSize);

        MonitorRect = UniWindowController.GetMonitorRect(monitorIndex);

        if (MonitorRect.width > 0 && MonitorRect.height > 0)
        {
             if (IsZoomed)
            {
                // When maximized, don't normalize the window rectangle.
                // The OS will restore the maximized size based on the target monitor.
                NormalizedPosition = Vector2.zero;
                NormalizedSize = Vector2.one;
            }
            else
            {
                NormalizedPosition = new Vector2(
                    (WindowPosition.x - MonitorRect.x) / MonitorRect.width,
                    (WindowPosition.y - MonitorRect.y) / MonitorRect.height
                );

                NormalizedSize = new Vector2(
                    WindowSize.x / MonitorRect.width,
                    WindowSize.y / MonitorRect.height
                );
            }
        }

        hasSavedState = true;
    }

    private int FindMonitorForWindow(Vector2 windowPosition, Vector2 windowSize)
{
    int monitorCount = UniWindowController.GetMonitorCount();

    if (monitorCount <= 0)
        return 0;

    Vector2 windowCenter = windowPosition + windowSize * 0.5f;

    for (int i = 0; i < monitorCount; i++)
    {
        Rect monitor = UniWindowController.GetMonitorRect(i);

        if (monitor.Contains(windowCenter))
        {
            return i;
        }
    }

    return 0;
}

   public bool TryGetState(
    out Vector2 position,
    out Vector2 size,
    out Rect monitorRect,
    out Vector2 normalizedPosition,
    out Vector2 normalizedSize,
    out bool isZoomed)
{
    position = WindowPosition;
    size = WindowSize;
    monitorRect = MonitorRect;
    normalizedPosition = NormalizedPosition;
    normalizedSize = NormalizedSize;
    isZoomed = IsZoomed;

    return hasSavedState;
}
}