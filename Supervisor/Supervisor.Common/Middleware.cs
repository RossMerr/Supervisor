using System;
using System.Collections.Generic;
using log4net;
using Michonne.Implementation;
using Michonne.Interfaces;
using RAFTiNG.Services;

namespace Supervisor.Common
{
	public class Middleware : IMiddleware
	{
		private readonly Dictionary<string, Action<object>> _endpoints = new Dictionary<string, Action<object>>();
		private readonly Dictionary<string, ISequencer> _sequencers = new Dictionary<string, ISequencer>();
		private readonly ILog _logger = LogManager.GetLogger(typeof(Middleware));
		private readonly ISequencer _sequencer;

		public IUnitOfExecution RootUnitOfExecution { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Middleware"/> class.
		/// </summary>
		/// <param name="asyncMode">if set to <c>true</c> middleware is async mode.</param>
		public Middleware(bool asyncMode = true)
		{
			if (asyncMode)
			{
				RootUnitOfExecution = TestHelpers.GetPool();
			}
			else
			{
				RootUnitOfExecution = TestHelpers.GetSynchronousUnitOfExecution();
			}
			_sequencer = RootUnitOfExecution.BuildSequencer();
		}

		/// <summary>
		/// Sends a message to a specific address.
		/// </summary>
		/// <param name="addressDest">The address to send the message to.</param>
		/// <param name="message">The message to be sent.</param>
		/// <returns>false if the message was not sent.</returns>
		/// <remarks>This is a best effort delivery contract. There is no guaranteed delivery.</remarks>
		public bool SendMessage(string addressDest, object message)
		{
			if (!_endpoints.ContainsKey(addressDest))
			{
				return false;
			}
			try
			{
				_sequencers[addressDest].Dispatch(
					() => _endpoints[addressDest].Invoke(message));
			}
			catch (Exception e)
			{
				// exceptions must not cross middleware boundaries
				_logger.Error("Exception raised when processing message.", e);
			}
			return true;
		}

		/// <summary>
		/// Registers the end point to process received messages.
		/// </summary>
		/// <param name="address">The address to register to.</param>
		/// <param name="messageReceived">The message processing method.</param>
		/// <exception cref="System.InvalidOperationException">If an endpoint is registered more than once.</exception>
		public void RegisterEndPoint(string address, Action<object> messageReceived)
		{
			_sequencer.Dispatch(
				() =>
				{
					if (string.IsNullOrEmpty(address))
					{
						throw new ArgumentNullException(address, "addressDest must contain a value.");
					}

					if (messageReceived == null)
					{
						throw new ArgumentNullException("messageReceived");
					}

					if (_endpoints.ContainsKey(address))
					{
						// double registration is development error.
						throw new InvalidOperationException("Invalid registration attempt: endpoints can only be registered once.");
					}

					_endpoints[address] = messageReceived;
					_sequencers[address] = RootUnitOfExecution.BuildSequencer();
				});
		}
	}
}