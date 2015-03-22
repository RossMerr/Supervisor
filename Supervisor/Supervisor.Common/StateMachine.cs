using System;
using RAFTiNG.Services;

namespace Supervisor.Common
{
	public class StateMachine : IStateMachine<Service>
	{
		/// <summary>
		/// Commits the specified command.
		/// </summary>
		/// <param name="command">The command.</param>
		public void Commit(Service command)
		{

		}
	}
}