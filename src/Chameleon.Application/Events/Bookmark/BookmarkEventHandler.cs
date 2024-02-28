using Chameleon.Interfaces.Bookmarks;
using Chameleon.Prism.Events;

namespace Chameleon.Application.Events
{
    public class BookmarkEventHandler
        : IBookmarkEventHandler
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IBookmarkFileService _bookmarkFileService;
        private readonly IProfileBookmarkService _profileBookmarkService;

        public BookmarkEventHandler(
            IEventAggregator eventAggregator,
            IBookmarkFileService bookmarkFileService,
            IProfileBookmarkService profileBookmarkService)
        {
            _eventAggregator = eventAggregator;
            _bookmarkFileService = bookmarkFileService;
            _profileBookmarkService = profileBookmarkService;

            _eventAggregator
               .GetEvent<DeleteBookmarkEvent>()
               .Subscribe(args => DeleteBookmark(args.ProfileBookmark));

            _eventAggregator
               .GetEvent<DeleteBookmarkInFolderEvent>()
               .Subscribe(args => DeleteBookmarkFileInFolder(args.BookmarkFile, args.ProfileBookmark));

            _eventAggregator
               .GetEvent<UpdateBookmarkEvent>()
               .Subscribe(args => UpdateBookmark(args.ProfileBookmark));

            _eventAggregator
               .GetEvent<CreateBookmarkEvent>()
               .Subscribe(args => CreateBookmark(args.ProfileBookmark));
        }

        private void CreateBookmark(IProfileBookmark profileBookmark)
        {
            _profileBookmarkService.Create(profileBookmark);
            ChangeBookmark(profileBookmark);
        }

        private void UpdateBookmark(IProfileBookmark profileBookmark)
        {
            _profileBookmarkService.Update(profileBookmark);
            ChangeBookmark(profileBookmark);            
        }

        private void DeleteBookmarkFileInFolder(IBookmarkFile bookmarkFile, IProfileBookmark profileBookmark)
        {
            profileBookmark.BookmarkFiles.Remove(bookmarkFile);
            _profileBookmarkService.Update(profileBookmark);
            _bookmarkFileService.Delete(bookmarkFile);
        }

        private void DeleteBookmark(IProfileBookmark profileBookmark)
        {
            _profileBookmarkService.Delete(profileBookmark);
            ChangeBookmark(profileBookmark);
        }

        private void ChangeBookmark(IProfileBookmark profileBookmark)
        {
            _eventAggregator
               .GetEvent<ChangeBookmarkEvent>()
               .Publish(new BookmarkEventArgs(profileBookmark));
        }
    }
}
