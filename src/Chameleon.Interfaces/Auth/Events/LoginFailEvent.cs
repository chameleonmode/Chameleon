using System;

using Chameleon.lib.Common.Interfaces.Sys;


namespace Chameleon.Interfaces.Auth {
	public class LoginFailEvent : PubSubEvent<EventArgs> { }
}