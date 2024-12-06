using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;

namespace AnimationParser.Core;

public partial class AnimationContext
{
    private readonly Dictionary<string, AnimationObject> _animatedObject = [];

    public ReadOnlyDictionary<string, AnimationObject> AnimatedObjects => new(_animatedObject);

    public void DeclareObject(string name, AnimationObject value)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        if (_animatedObject.ContainsKey(name))
        {
            throw new Exception($"Variable '{name}' is already declared");
        }

        OnObjectAdded(name, value);
    }

    public AnimationObject? GetObject(string name)
    {
        return _animatedObject.TryGetValue(name, out var value) ? value : default;
    }

    public void ShiftObject(string name, Direction direction)
    {
        if (!_animatedObject.TryGetValue(name, out var obj))
        {
            throw new Exception($"Variable '{name}' is not declared");
        }

        OnObjectShifting(name, obj, direction);
    }

    public void PlaceObject(string name, Vector2 position)
    {
        if (!_animatedObject.TryGetValue(name, out var obj))
        {
            throw new Exception($"Variable '{name}' is not declared");
        }

        Debug.Assert(obj != null, "Object should not be null");

        OnObjectPlaced(name, obj, position);
    }

    public void EraseObject(string name)
    {
        if (!_animatedObject.TryGetValue(name, out var obj))
        {
            throw new Exception($"Variable '{name}' is not declared");
        }

        OnObjectErasingOut(name, obj);
    }

    protected virtual void OnObjectPlaced(string name, AnimationObject obj, Vector2 position)
    {
        obj.Position = position;
    }

    protected virtual void OnObjectAdded(string name, AnimationObject obj)
    {
        _animatedObject[name] = obj;
    }

    protected virtual void OnObjectErasingOut(string name, AnimationObject obj)
    {
        _animatedObject.Remove(name);
    }

    protected virtual void OnObjectShifting(string name, AnimationObject obj, Direction direction)
    {
    }
}
