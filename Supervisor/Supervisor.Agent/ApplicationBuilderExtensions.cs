using System;
using Michonne.Implementation;
using Microsoft.AspNet.Builder;
using RAFTiNG;
using Supervisor.Common;

namespace Supervisor.Agent
{
    public static class ApplicationBuilderExtensions
    {
	    public static IApplicationBuilder UseSupervisorAgent(this IApplicationBuilder app, string name = "default")
	    {
		    var settings = new NodeSettings()
		    {
			    NodeId = string.Format("{0}.{1}", Guid.NewGuid(), name),
				TimeoutInMs = 10,
				Nodes = new string[] {}
		    };

			var testedNode = new Node<Service>(TestHelpers.GetPool().BuildSequencer(), 
				settings,
				new Middleware(), 
				new StateMachine());

			testedNode.Initialize();
			testedNode.AddEntry(new Service());
		    testedNode.SendCommand(new Service());
			return app;
	    }
	}
}