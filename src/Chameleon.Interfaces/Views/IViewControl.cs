using Chameleon.Interfaces.Ioc;
using Microsoft.Maui;
using System;

namespace Chameleon.Interfaces.Views
{
    public interface IViewControl : IUserControl
    {
        /// <summary>
        /// Event when view is loaded
        /// </summary>
        event Action Loaded;

        /// <summary>
        /// Event when view is loaded
        /// </summary>
        event Action Unloaded;

        /// <summary>
        /// Event when view content is rendered
        /// </summary>
        event EventHandler ContentRendered;

        /// <summary>
        /// Can control visibility of an view control
        /// </summary>
        Visibility Visibility { get; set; }

        object GetContent();
    }
}
