using Microsoft.Xna.Framework;

namespace MyEngine.Utils.Tween;

public interface ITweener
{
    bool IsCompleted { get; }
    public void Update(GameTime gameTime);
}