using System;
using Chameleon.Common.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.OutReach;

namespace Chameleon.Infrastructure.OutReach
{
    public class OutReachTemplateService
        : IOutReachTemplateService
    {
        private readonly IOutReachTemplateRepository _repository;

        public OutReachTemplateService(
            IOutReachTemplateRepository repository
            )
        {
            _repository = repository;
        }

        private IOutReachTemplates _outReachTemplates;
        private IOutReachTemplates OutReachTemplates
        {
            get
            {
                if (_outReachTemplates == null)
                {
                    var entities = _repository.GetAll();
                    _outReachTemplates = new OutReachTemplates
                    {
                        entities
                    };
                }
                return _outReachTemplates;
            }
        }              

        public IOutReachTemplates GetAll()
        {
            return OutReachTemplates;
        }

        public IOutReachTemplate Create(IOutReachTemplate entity)
        {
            _repository.Insert(entity);
            this.InvokeOnUiThread(() => OutReachTemplates.Add(entity));
            return entity;
        }

        public IOutReachTemplate Get(int Id)
        {
            var entity = _repository.Get(Id);
            return entity;
        }

        public void Save(IOutReachTemplate outReachTemplate)
        {
            if (outReachTemplate == null)
            {
                throw new ArgumentNullException();
            }
            _repository.Update(outReachTemplate);
        }

        public void Delete(IOutReachTemplate outReachTemplate)
        {
            if (outReachTemplate == null)
            {
                throw new ArgumentNullException();
            }

            _repository.Delete(outReachTemplate.Id);
            this.InvokeOnUiThread(() => OutReachTemplates.Remove(outReachTemplate));
        }
    }
}
