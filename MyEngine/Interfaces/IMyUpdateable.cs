
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace MyEngine.Interfaces;

/// <summary>Interface for updateable entities.</summary>
public interface IMyUpdateable
{
    int UpdateOrder { get; }
    
    /// <summary>
    /// Called when this <see cref="IMyUpdateable" /> should update itself.
    /// </summary>
    /// <param name="gameTime">The elapsed time since the last call to <see cref="Update" />.</param>
    void Update(GameTime gameTime);
    
    /// <summary>
    /// Raised when <see cref="UpdateOrder" /> changed.
    /// </summary>
    event EventHandler<EventArgs> UpdateOrderChanged;
}
