using Stride.Engine;
using Stride.UI;

namespace StrideTemplate.UI;

public abstract class MenuScreen
{
    private Entity? _entity;

    public bool IsVisible => _entity?.Scene != null;

    protected abstract UIElement BuildLayout();

    public void Show(Scene rootScene)
    {
        _entity ??= UIHelper.CreateUIEntity(GetType().Name, BuildLayout());
        _entity.Scene = rootScene;
    }

    public void Hide()
    {
        if (_entity != null)
            _entity.Scene = null;
    }
}
