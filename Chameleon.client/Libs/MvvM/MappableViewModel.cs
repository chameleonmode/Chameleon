using System.Reflection;
using Chameleon.lib.Common.Models.Dto;
using Chameleon.lib.CommunityToolkit.MvvM;

namespace Chameleon.client.Libs.MvvM;

public abstract class MappableViewModelBase<T>(T dto) : DtoViewModelBase<T>(dto) where T : Dto{
  public virtual T ToDto()  {
    var viewModelType = this.GetType();
    var dtoType = typeof(T);

    var viewModelProperties = viewModelType
    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Where(p => p.CanRead);

    var dtoProperties = dtoType
    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Where(p => p.CanWrite)
    .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

    foreach (var vmProp in viewModelProperties) {
      if (dtoProperties.TryGetValue(vmProp.Name, out var dtoProp) &&
        dtoProp.PropertyType.IsAssignableFrom(vmProp.PropertyType)) {
        var value = vmProp.GetValue(this);
        dtoProp.SetValue(Dto, value);
      }
    }

    return Dto;
  }
}
