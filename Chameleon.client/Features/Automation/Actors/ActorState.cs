using Chameleon.AIR.Actors.Models;

namespace Chameleon.client.Features.Automation.Actors;

public record ActorState(Opts Options,List<string> SelectedScriptFiles);