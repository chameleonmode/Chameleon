using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Bookmarks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.Bookmarks
{
    public class BookmarkFileService
        : IBookmarkFileService
    {
        private readonly IBookmarkFileRepository _repository;

        public BookmarkFileService(
            IBookmarkFileRepository repository
            )
        {
            _repository = repository;

            var entities = _repository.GetAll();
            _settings = new BookmarkFiles()
            {
                entities
            };
        }

        private IBookmarkFiles _settings;
        private IBookmarkFiles Settings
        {
            get => _settings;
        }

        public IBookmarkFiles GetAll()
        {
            var entities = _repository.GetAll();
            var bookmarkFiles = new BookmarkFiles()
            {
                entities
            };
            return bookmarkFiles;
        }

        public IBookmarkFile Update(IBookmarkFile bookmarkFile)
        {
            _repository.Update(bookmarkFile);
            return bookmarkFile;
        }

        public void Delete(IBookmarkFile bookmarkFile)
        {
            if (bookmarkFile == null)
            {
                throw new ArgumentNullException();
            }

            _repository.Delete(bookmarkFile.Id);
            Settings.Remove(bookmarkFile);
        }

        public IBookmarkFile Save(IBookmarkFile bookmarkFile)
        {
            if (bookmarkFile == null)
            {
                throw new ArgumentNullException();
            }

            _repository.Insert(bookmarkFile);
            return bookmarkFile;
        }
    }
}
