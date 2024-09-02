using Chameleon.Application.Events.SyncChanges;
using Chameleon.Interfaces.App.GuidedTour;

namespace Chameleon.Application.Events
{
    public class ApplicationEventHandlers : IApplicationEventHandlers
    {
        public ApplicationEventHandlers(
            // injected just to create all event handlers to start them up
            IEntityEventHandler entityEventHandler,
            //IOutReachEventHandler outReachEventHandler,
            //IBookmarkEventHandler bookmarkEventHandler,
            IUserProfileEventHandler userProfileEventHandler,
            //IUserSettingsEventHandler userSettingsEvantHandler,
            IOpenBrowserEventHandler userProfileRssEventHandler,
            IUserProfileFolderEventHandler userProfileFolderEventHandler,
            //IProspectorEventHandler prospectorEventHandler,
            //ICookiesExcludedDomainEventHandler cookiesExcludedDomainEventHandler,
            //ITourHandler tourHandler,
            ISyncChangesEventHandler syncChangesEventHandler
            )
        {
        }
    }
}
