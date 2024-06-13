using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.App.Automation.DTOs;
public class AutomationScriptParameterDto
    : IEntityDto<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ScriptId { get; set; }
    public AutomationScriptParameterValueDto Value { get; set; }
}
