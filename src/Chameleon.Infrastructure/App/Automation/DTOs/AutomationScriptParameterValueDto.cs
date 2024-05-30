using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.App.Automation.DTOs;
public class AutomationScriptParameterValueDto
    : IEntityDto<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public int ParameterId { get; set; }
}
