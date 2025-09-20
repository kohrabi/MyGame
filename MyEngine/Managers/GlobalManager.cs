using Microsoft.Xna.Framework;

namespace MyEngine.Managers;

#nullable enable
public abstract class GlobalManager
{
    private bool _enabled = false;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value)
                return;
            _enabled = value;
            if (_enabled)
                OnEnable();
            else
                OnDisable();
        }
    }

    public abstract void OnEnable();
    public abstract void OnDisable();
    public abstract void Update(GameTime gameTime);
    
    
}