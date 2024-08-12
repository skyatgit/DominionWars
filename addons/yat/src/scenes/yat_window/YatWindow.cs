using Godot;
using YAT.Helpers;
using YAT.Resources;

namespace YAT.Scenes;

public partial class YatWindow : Window
{
    [Signal]
    public delegate void WindowMovedEventHandler(Vector2 position);

    public enum EWindowPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center
    }

    private const float WindowMoveRefreshRate = 0.0128f;

    private Vector2 _previousPosition;

    private Viewport _viewport;
    private float _windowMoveTimer;
    [Export] public bool AllowToGoOffScreen = true;
    protected PanelContainer Content;

    [Export] public EWindowPosition DefaultWindowPosition = EWindowPosition.Center;

    protected Yat Yat;

    [Export(PropertyHint.Range, "0, 128, 1")]
    public ushort ViewportEdgeOffset { get; set; }

    public ContextMenu ContextMenu { get; private set; }
    public Vector2I InitialSize { get; private set; }

    public bool IsWindowMoving { get; private set; }

    public override void _Ready()
    {
        Yat = GetNode<Yat>("/root/YAT");
        Yat.PreferencesManager.PreferencesUpdated += UpdateOptions;

        Content = GetNode<PanelContainer>("Content");

        _viewport = Yat.GetTree().Root.GetViewport();
        _viewport.SizeChanged += OnViewportSizeChanged;

        ContextMenu = GetNode<ContextMenu>("ContextMenu");
        InitialSize = Size;
        Title = Yat.Title;
        WindowInput += OnWindowInput;
        WindowMoved += OnWindowMoved;

        Move(DefaultWindowPosition, ViewportEdgeOffset);
        OnViewportSizeChanged();
        UpdateOptions(Yat.PreferencesManager.Preferences);
    }

    public void ResetPosition() => Move(DefaultWindowPosition, ViewportEdgeOffset);

    public override void _Process(double delta)
    {
        _windowMoveTimer += (float)delta;

        if (_windowMoveTimer >= WindowMoveRefreshRate && _previousPosition != Position)
        {
            IsWindowMoving = true;
            _windowMoveTimer = 0f;
            _previousPosition = Position;
            EmitSignal(SignalName.WindowMoved, Position);
        }
        else
        {
            IsWindowMoving = false;
        }
    }

    private void OnWindowInput(InputEvent @event)
    {
        if (!Yat.HasContextMenu)
        {
            return;
        }

        if (@event.IsActionPressed(Keybindings.ContextMenu) && ContextMenu.ItemCount > 0)
        {
            ContextMenu.ShowNextToMouse();
        }
        else
        {
            ContextMenu.Hide();
        }
    }

    private void OnWindowMoved(Vector2 position)
    {
        if (!AllowToGoOffScreen)
        {
            (float limitX, float limitY) = CalculateLimits(_viewport.GetVisibleRect());

            Position = new Vector2I(
                (int)Mathf.Clamp(Position.X, ViewportEdgeOffset, limitX),
                (int)Mathf.Clamp(Position.Y, ViewportEdgeOffset, limitY)
            );
        }
    }

    private (float, float) CalculateLimits(Rect2 rect)
    {
        float limitX = rect.Size.X - Size.X - ViewportEdgeOffset;
        float limitY = rect.Size.Y - Size.Y - ViewportEdgeOffset;

        return ((int)limitX, (int)limitY);
    }

    private void OnViewportSizeChanged()
    {
        Vector2I viewportSize = (Vector2I)_viewport.GetVisibleRect().Size;

        MaxSize = MaxSize with { X = viewportSize.X - ViewportEdgeOffset, Y = viewportSize.Y - ViewportEdgeOffset };
    }

    public void Move(EWindowPosition position, uint offset = 0)
    {
        switch (position)
        {
            case EWindowPosition.TopLeft:
                MoveTopLeft(offset);
                break;
            case EWindowPosition.TopRight:
                MoveTopRight(offset);
                break;
            case EWindowPosition.BottomRight:
                MoveBottomRight(offset);
                break;
            case EWindowPosition.BottomLeft:
                MoveBottomLeft(offset);
                break;
            case EWindowPosition.Center:
                MoveToTheCenter();
                break;
        }
    }

    protected void MoveTopLeft(uint offset) => Position = new Vector2I((int)offset, (int)offset);

    protected void MoveTopRight(uint offset)
    {
        Rect2 viewportRect = GetTree().Root.GetViewport().GetVisibleRect();
        Vector2 bottomLeft = viewportRect.Position + viewportRect.Size;
        Rect2 rect = GetVisibleRect();

        Position = new Vector2I(
            (int)(bottomLeft.X - rect.Size.X - offset),
            (int)offset
        );
    }

    protected void MoveBottomRight(uint offset)
    {
        Rect2 viewportRect = GetTree().Root.GetViewport().GetVisibleRect();
        Vector2 topRight = viewportRect.Position + viewportRect.Size;
        Rect2 rect = GetVisibleRect();

        Position = new Vector2I(
            (int)(topRight.X - rect.Size.X - offset),
            (int)(topRight.Y - rect.Size.Y - offset)
        );
    }

    protected void MoveBottomLeft(uint offset)
    {
        Rect2 viewportRect = GetTree().Root.GetViewport().GetVisibleRect();
        Vector2 bottomLeft = viewportRect.Position + viewportRect.Size;
        Rect2 rect = GetVisibleRect();

        Position = new Vector2I(
            (int)offset,
            (int)(bottomLeft.Y - rect.Size.Y - offset)
        );
    }

    protected void MoveToTheCenter() => MoveToCenter();

    protected void UpdateOptions(YatPreferences prefs)
    {
        AddThemeFontSizeOverride("title_font_size", prefs.BaseFontSize);
        AddThemeFontOverride("title_font", prefs.BaseFont);

        Theme theme = Content.Theme;
        theme.DefaultFont = prefs.BaseFont;
        theme.DefaultFontSize = prefs.BaseFontSize;
        Content.Theme = theme;
    }
}
