#nullable enable
using Godot;
using Godot.Collections;

namespace YAT.Scenes;

public partial class ContextMenu : PopupMenu
{
    private Viewport? _viewport;
    private Rect2 _viewportRect;
    [Export] public ushort WallMargin { get; set; } = 16;
    [Export] public bool ShrinkToFit { get; set; } = true;

    public override void _Ready()
    {
        _viewport = GetTree().Root.GetViewport();
        _viewportRect = _viewport.GetVisibleRect();

        Hide();

        if (ShrinkToFit)
        {
            MenuChanged += Shrink;
        }
    }

    private void Shrink()
    {
        Vector2 itemsSize = GetItemsSize();

        Size = new Vector2I { X = 1, Y = 1 };
        Size = (Vector2I)itemsSize;
    }

    private Vector2 GetItemsSize()
    {
        Array<Node> items = GetChildren();
        Vector2 size = new Vector2();

        foreach (Node item in items)
        {
            if (item is Control control)
            {
                size += control.Size;
            }

            if (item is PopupMenu popupMenu)
            {
                size += popupMenu.Size;
            }
        }

        return size;
    }

    public void ShowNextToMouse()
    {
        if (_viewport is null)
        {
            return;
        }

        Vector2 mousePos = _viewport.GetMousePosition();
        (float limitX, float limitY) = CalculateLimits(_viewportRect);

        Show();

        Position = new Vector2I
        {
            X = (int)Mathf.Clamp(mousePos.X, WallMargin, limitX),
            Y = (int)Mathf.Clamp(mousePos.Y, WallMargin, limitY)
        };
    }

    public (float, float) CalculateLimits(Rect2 rect)
    {
        float limitX = rect.Size.X - Size.X - WallMargin;
        float limitY = rect.Size.Y - Size.Y - WallMargin;

        return ((int)limitX, (int)limitY);
    }
}
