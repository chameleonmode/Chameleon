using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.Views
{
    public interface IViewControl : IUserControl
    {
        /// <summary>
        /// Event when view is loaded
        /// </summary>
        event EventHandler Loaded;

        /// <summary>
        /// Event when view is loaded
        /// </summary>
        event EventHandler Unloaded;

        /// <summary>
        /// Event when view content is rendered
        /// </summary>
        event EventHandler ContentRendered;

        /// <summary>
        /// Can control visibility of an view control
        /// </summary>
        bool Visibility { get; set; }

        object GetContent();
    }
}
