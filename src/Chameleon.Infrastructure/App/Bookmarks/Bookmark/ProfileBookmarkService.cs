using AutoMapper;
using Chameleon.Common.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Bookmarks;
using Chameleon.Interfaces.Repository;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class ProfileBookmarkService : IProfileBookmarkService
    {
        private readonly IMapper _mapper;
        private readonly IProfileBookmarkRepository _bookmarkRepository;
        private readonly Dictionary<int, ProfileBookmarks> _entities
            = new Dictionary<int, ProfileBookmarks>();

        public ProfileBookmarkService(
          IMapper mapper,
          IProfileBookmarkRepository bookmarkRepository)
        {
            _mapper = mapper;
            _bookmarkRepository = bookmarkRepository;
        }

        public void Delete(IProfileBookmark bookmark)
        {
            EnsureEntitiesContainsProfileId(bookmark.ProfileId);
           
            _bookmarkRepository.Delete(bookmark.Id);
            this.InvokeOnUiThread(() => 
            {
                if (bookmark.BookmarkType == BookmarkType.GlobalFolder)
                {
                    DeleteGlobalBookmark(bookmark);
                }
                else
                {
                    DeleteProfileBookmark(bookmark);
                }
            });
        }

        private void DeleteGlobalBookmark(IProfileBookmark bookmark)
        {
            var bookmarkId = bookmark.Id;
            foreach (var pair in _entities)
            {
                pair.Value.Remove(x => x.Id == bookmarkId);
            }
        }

        private void DeleteProfileBookmark(IProfileBookmark bookmark)
        {
            _entities[bookmark.ProfileId]
                .Remove(x => x.Id == bookmark.Id);
        }

        public IProfileBookmarks GetAll(int profileId, bool ignoreCache = false)
        {
            if (ignoreCache)
            {
                _entities.Clear();
            }

            if (_entities.TryGetValue(profileId, out var bookmarks))
            {
                return bookmarks;
            }

            var request = new UserProfileGetAllRequestDto(profileId);
            var items = _bookmarkRepository.GetAll(ignoreCache, request);
            bookmarks = new ProfileBookmarks(profileId, items);
            _entities[profileId] = bookmarks;
            return bookmarks;   
        }

        public IProfileBookmark Create(IProfileBookmark bookmark)
        {
            EnsureEntitiesContainsProfileId(bookmark.ProfileId);
            var bookmarkMap = _mapper.Map<ProfileBookmark>(bookmark);
            _bookmarkRepository.Insert(bookmarkMap);
            if (bookmarkMap.BookmarkType == BookmarkType.GlobalFolder)
            {
                AddGlobalBookmark(bookmarkMap);
            }
            else
            {
                AddProfileBookmark(bookmarkMap);
            }
            return bookmarkMap;
        }

        private void AddGlobalBookmark(IProfileBookmark bookmark)
        {
            foreach (var pair in _entities)
            {
                pair.Value.Add(bookmark);
            }
        }

        private void AddProfileBookmark(IProfileBookmark bookmark)
        {
            _entities[bookmark.ProfileId].Add(bookmark);
        }

        public void Update(IProfileBookmark bookmark)
        {
            var bookmarkMap = _mapper.Map<ProfileBookmark>(bookmark);
            this.InvokeOnUiThread(() => _bookmarkRepository.Update(bookmarkMap));
        }

        private void EnsureEntitiesContainsProfileId(int profileId)
        {
            if (!_entities.ContainsKey(profileId))
            {
                _entities.Add(profileId, new ProfileBookmarks(profileId, GetAll(profileId)));
            }
        }
    }
}
