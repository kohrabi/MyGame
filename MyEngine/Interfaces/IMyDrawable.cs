
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace MyEngine.Interfaces;

// From Monogame Framework
// Is this retarded?
/// <summary>Interface for drawable entities.</summary>
public interface IMyDrawable
{
    /// <summary>
    /// The draw order of this <see cref="IMyDrawable" /> relative
    /// to other <see cref="IMyDrawable" /> instances.
    /// </summary>
    int DrawOrder { get; }

    /// <summary>
    /// Indicates if <see cref="Draw" /> will be called.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    /// Raised when <see cref="DrawOrder" /> changed.
    /// </summary>
    event EventHandler<EventArgs> DrawOrderChanged;

    /// <summary>
    /// Raised when <see cref="Visible" /> changed.
    /// </summary>
    event EventHandler<EventArgs> VisibleChanged;

    /// <summary>
    /// Called when this <see cref="IMyDrawable" /> should draw itself.
    /// </summary>
    /// <param name="spriteBatch">Use this sprite batch draw />.</param>
    /// <param name="gameTime">The elapsed time since the last call to <see cref="Draw" />.</param>
    void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}