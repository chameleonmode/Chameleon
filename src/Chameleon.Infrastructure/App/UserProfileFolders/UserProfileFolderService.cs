using AutoMapper;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Repository;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.UserProfileFolders
{
    public class UserProfileFolderService : IUserProfileFolderService
    {
        private readonly IUserProfileFolderRepository _repository;
        private readonly IAuthSession _authSession;

        public UserProfileFolderService(
            IUserProfileFolderRepository repository,
            IAuthSession authSession
            )
        {
            _repository = repository;
            _authSession = authSession;

            InitFolders();
        }

        private void InitFolders()
        {
            _folders = new Lazy<Domain.Entities.UserProfileFolders>(() => GetFolders(true), true);
        }

        private Lazy<Domain.Entities.UserProfileFolders> _folders;
        private Domain.Entities.UserProfileFolders Folders => _folders.Value;
        private Domain.Entities.UserProfileFolders GetFolders(bool ignoreCache = false)
        {
            var entities = _repository.GetAll(ignoreCache);

#pragma warning disable IDE0028 // Simplify collection initialization
            return new Domain.Entities.UserProfileFolders { entities };
#pragma warning restore IDE0028 // Simplify collection initialization
        }

        public IUserProfileFolders GetAll()
        {
            return Folders;
        }
        public async Task<IUserProfileFolders> GetAllAsync()
        {
            if (!_folders.IsValueCreated)
            {
                return await Task.Run(() => GetAll());
            }
            return GetAll();
        }

        public void Sync()
        {
            InitFolders();
        }

        public string GetTitle(int? folderId)
        {
            if (!folderId.HasValue)
            {
                return "Profiles";
            }

            var folder = Folders.FirstOrDefault(item => item.Id == folderId);
            return folder == null ? throw new KeyNotFoundException(folderId.ToString()) : folder.Title;
        }

        public IUserProfileFolder Create(string title = null)
        {
            var folders = Folders;
            if (string.IsNullOrEmpty(title))
            {
                title = $"New Folder {folders.Count + 1}";
            }

            var entity = new UserProfileFolder
            {
                Title = title
            };

            _repository.Insert(entity);
            folders.Add(entity);
            return entity;
        }

        public IUserProfileFolder Get(int Id)
        {
            var entity = _repository.Get(Id);
            return entity;
        }

        public void Save(IUserProfileFolder folder)
        {
            _repository.InsertOrUpdate(folder);
        }

        public void Delete(IUserProfileFolder folder)
        {
            _repository.Delete(folder);
            Folders.Remove(folder);
        }

        public bool IsSharedFolder(IUserProfileFolder folder) => 
            folder?.CreatorUserId != _authSession?.UserId && folder?.CreatorUserId != null;
    }
}
