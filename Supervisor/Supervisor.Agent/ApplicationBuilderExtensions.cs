using System;
using Michonne.Implementation;
using Microsoft.AspNet.Builder;
using RAFTiNG;

namespace Supervisor.Agent
{
    public static class ApplicationBuilderExtensions
    {
	    public static IApplicationBuilder UseSupervisorAgent(this IApplicationBuilder app)
	    {
			var nodeId = Guid.NewGuid().ToString();

			var middleware = new Middleware();
		    var settings = new NodeSettings();

			var testedNode = new Node<string>(TestHelpers.GetPool().BuildSequencer(), 
				settings,
				middleware, 
				new StateMachine());

			return app;
	    }
	}
}