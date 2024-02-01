using AutoMapper;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper.Configuration;

namespace Chameleon.Infrastructure.AutoMapper
{
    public class AutoMapperProxy : IMapper
    {
        private MapperConfiguration _configuration = new MapperConfiguration(_ => { });
        
        private IMapper _mapper;
        private IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    _mapper = new Mapper(_configuration);
                }
                return _mapper;
            }
        }

        public void SetConfiguration(MapperConfigurationExpression configurationExpression)
        {
            var configuration = new MapperConfiguration(configurationExpression);
            configuration.AssertConfigurationIsValid();
            _configuration = configuration;
            _mapper = null;
        }

        public IConfigurationProvider ConfigurationProvider 
            => Mapper.ConfigurationProvider;
        //public Func<Type, object> ServiceCtor 
        //    => Mapper.ServiceCtor;

        public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions<object, TDestination>> opts)
        {
            return Mapper.Map(source, opts);
        }

        public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return Mapper.Map(source, opts);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return Mapper.Map(source, destination, opts);
        }

        public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions<object, object>> opts)
        {
            return Mapper.Map(sourceType, destinationType, opts);
        }

        public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions<object, object>> opts)
        {
            return Mapper.Map(source, destination, sourceType, destinationType, opts);
        }

        public TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public object Map(object source, Type sourceType, Type destinationType)
        {
            return Mapper.Map(source, sourceType, destinationType);
        }

        public object Map(object source, object destination, Type sourceType, Type destinationType)
        {
            return Mapper.Map(source, destination, sourceType, destinationType);
        }

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, object parameters = null, params Expression<Func<TDestination, object>>[] membersToExpand)
        {
            return Mapper.ProjectTo(source, parameters, membersToExpand);
        }

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, IDictionary<string, object> parameters, params string[] membersToExpand)
        {
            return Mapper.ProjectTo<TDestination>(source, parameters, membersToExpand);
        }

        public IQueryable ProjectTo(IQueryable source, Type destinationType, IDictionary<string, object> parameters = null, params string[] membersToExpand)
        {
            return Mapper.ProjectTo(source, destinationType, parameters, membersToExpand);
        }
    }
}
