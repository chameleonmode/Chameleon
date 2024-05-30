using Chameleon.Infrastructure.Dto;
using System.Collections.Generic;

namespace Chameleon.Infrastructure.App.Automation.DTOs;
public class AutomationScriptDescriptionDto
    : IEntityDto<int>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IList<AutomationScriptParameterValueDto> Parameters { get; set; }
}
