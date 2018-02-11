using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMon.Models.Ccu;
using CMon.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CMon.Hubs
{
	[Authorize]
	public class DashboardHub : HubWithPresence
	{
		private readonly IMediator _mediator;

		public DashboardHub(IMediator mediator, IUserTracker<HubWithPresence> userTracker) : base(userTracker)
		{
			_mediator = mediator;
		}

		public override async Task OnConnectedAsync()
		{
			var contract = await _mediator.Send(new GetContract
			{
				UserName = Context.User.Identity.Name,
			});

			if (contract?.Devices != null)
			{
				foreach (var device in contract.Devices)
				{
					await Groups.AddAsync(Context.ConnectionId, "Device." + device.Id);
				}
			}

			await base.OnConnectedAsync();
		}

		public Task Log(long deviceId, string message)
		{
			return Clients.Group("Device." + deviceId).InvokeAsync("Log", deviceId, message);
		}

		public Task OnStatusUpdated(long deviceId, string status)
		{
			// return Clients.Client(Context.ConnectionId).InvokeAsync("StatusUpdated", status);

			return Clients.Group("Device." + deviceId).InvokeAsync("StatusUpdated", deviceId, status);
		}

		public Task OnStateAndEvents(long deviceId, StateAndEventsResult status)
		{
			return Clients.Group("Device." + deviceId).InvokeAsync("StateAndEvents", deviceId, status);
		}

		public Task OnInputTemperature(long deviceId, short inputNo, decimal temp)
		{
			return Clients.Group("Device." + deviceId).InvokeAsync("InputTemperature", deviceId, inputNo, temp);
		}
	}

	public class DashboardNotifier
	{
		private readonly HubLifetimeManager<DashboardHub> _hublifetimeManager;

		public DashboardNotifier(HubLifetimeManager<DashboardHub> hublifetimeManager)
		{
			_hublifetimeManager = hublifetimeManager;
		}

		public Task Notify(Func<DashboardHub, Task> action)
		{
			return ((DefaultPresenceHublifetimeManager<DashboardHub>)_hublifetimeManager).Notify(action);
		}
	}

	[Authorize]
	public class Chat : HubWithPresence
	{
		public Chat(IUserTracker<Chat> userTracker) : base(userTracker)
		{
		}

		public override async Task OnConnectedAsync()
		{
			await Clients.Client(Context.ConnectionId).InvokeAsync("SetUsersOnline", await GetUsersOnline());

			await base.OnConnectedAsync();
		}

		public override Task OnUsersJoined(UserDetails[] users)
		{
			return Clients.Client(Context.ConnectionId).InvokeAsync("UsersJoined", new[] { users });
		}

		public override Task OnUsersLeft(UserDetails[] users)
		{
			return Clients.Client(Context.ConnectionId).InvokeAsync("UsersLeft", new[] { users });
		}

		public async Task Send(string message)
		{
			await Clients.All.InvokeAsync("Send", Context.User.Identity.Name, message);
		}
	}

	public class HubWithPresence : Hub
	{
		private IUserTracker<HubWithPresence> _userTracker;

		public HubWithPresence(IUserTracker<HubWithPresence> userTracker)
		{
			_userTracker = userTracker;
		}

		public Task<IEnumerable<UserDetails>> GetUsersOnline()
		{
			return _userTracker.UsersOnline();
		}

		public virtual Task OnUsersJoined(UserDetails[] user)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnUsersLeft(UserDetails[] user)
		{
			return Task.CompletedTask;
		}
	}

	public class UserDetails
	{
		public UserDetails(string connectionId, string name)
		{
			ConnectionId = connectionId;
			Name = name;
		}

		public string ConnectionId { get; }
		public string Name { get; }
	}

	public interface IUserTracker<out THub>
	{
		Task<IEnumerable<UserDetails>> UsersOnline();
		Task AddUser(HubConnectionContext connection, UserDetails userDetails);
		Task RemoveUser(HubConnectionContext connection);

		event Action<UserDetails[]> UsersJoined;
		event Action<UserDetails[]> UsersLeft;
	}

	public class InMemoryUserTracker<THub> : IUserTracker<THub>
	{
		private readonly ConcurrentDictionary<HubConnectionContext, UserDetails> _usersOnline
			= new ConcurrentDictionary<HubConnectionContext, UserDetails>();

		public event Action<UserDetails[]> UsersJoined;
		public event Action<UserDetails[]> UsersLeft;

		public Task<IEnumerable<UserDetails>> UsersOnline()
			=> Task.FromResult(_usersOnline.Values.AsEnumerable());

		public Task AddUser(HubConnectionContext connection, UserDetails userDetails)
		{
			_usersOnline.TryAdd(connection, userDetails);
			UsersJoined(new[] { userDetails });

			return Task.CompletedTask;
		}

		public Task RemoveUser(HubConnectionContext connection)
		{
			if (_usersOnline.TryRemove(connection, out var userDetails))
			{
				UsersLeft(new[] { userDetails });
			}

			return Task.CompletedTask;
		}
	}

	public class DefaultPresenceHublifetimeManager<THub> : PresenceHubLifetimeManager<THub, DefaultHubLifetimeManager<THub>>
	where THub : HubWithPresence
	{
		public DefaultPresenceHublifetimeManager(IUserTracker<THub> userTracker, IServiceScopeFactory serviceScopeFactory,
			ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
			: base(userTracker, serviceScopeFactory, loggerFactory, serviceProvider)
		{
		}
	}

	/*public class RedisPresenceHublifetimeManager<THub> : PresenceHubLifetimeManager<THub, RedisHubLifetimeManager<THub>>
	where THub : HubWithPresence
	{
		public RedisPresenceHublifetimeManager(IUserTracker<THub> userTracker, IServiceScopeFactory serviceScopeFactory,
			ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
			: base(userTracker, serviceScopeFactory, loggerFactory, serviceProvider)
		{
		}
	}*/

	public class PresenceHubLifetimeManager<THub, THubLifetimeManager> : HubLifetimeManager<THub>, IDisposable
		where THubLifetimeManager : HubLifetimeManager<THub>
		where THub : HubWithPresence
	{
		private readonly HubConnectionList _connections = new HubConnectionList();
		private readonly IUserTracker<THub> _userTracker;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly ILogger _logger;
		private readonly IServiceProvider _serviceProvider;
		private readonly HubLifetimeManager<THub> _wrappedHubLifetimeManager;
		private IHubContext<THub> _hubContext;

		public PresenceHubLifetimeManager(IUserTracker<THub> userTracker, IServiceScopeFactory serviceScopeFactory,
			ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
		{
			_userTracker = userTracker;
			_userTracker.UsersJoined += OnUsersJoined;
			_userTracker.UsersLeft += OnUsersLeft;

			_serviceScopeFactory = serviceScopeFactory;
			_serviceProvider = serviceProvider;
			_logger = loggerFactory.CreateLogger<PresenceHubLifetimeManager<THub, THubLifetimeManager>>();
			_wrappedHubLifetimeManager = serviceProvider.GetRequiredService<THubLifetimeManager>();
		}

		public override async Task OnConnectedAsync(HubConnectionContext connection)
		{
			await _wrappedHubLifetimeManager.OnConnectedAsync(connection);
			_connections.Add(connection);
			await _userTracker.AddUser(connection, new UserDetails(connection.ConnectionId, connection.User.Identity.Name));
		}

		public override async Task OnDisconnectedAsync(HubConnectionContext connection)
		{
			await _wrappedHubLifetimeManager.OnDisconnectedAsync(connection);
			_connections.Remove(connection);
			await _userTracker.RemoveUser(connection);
		}

		private async void OnUsersJoined(UserDetails[] users)
		{
			await Notify(hub =>
			{
				if (users.Length == 1)
				{
					if (users[0].ConnectionId != hub.Context.ConnectionId)
					{
						return hub.OnUsersJoined(users);
					}
				}
				else
				{
					return hub.OnUsersJoined(
						users.Where(u => u.ConnectionId != hub.Context.Connection.ConnectionId).ToArray());
				}
				return Task.CompletedTask;
			});
		}

		private async void OnUsersLeft(UserDetails[] users)
		{
			await Notify(hub => hub.OnUsersLeft(users));
		}

		public async Task Notify(Func<THub, Task> invocation)
		{
			foreach (var connection in _connections)
			{
				using (var scope = _serviceScopeFactory.CreateScope())
				{
					var hubActivator = scope.ServiceProvider.GetRequiredService<IHubActivator<THub>>();
					var hub = hubActivator.Create();

					if (_hubContext == null)
					{
						// Cannot be injected due to circular dependency
						_hubContext = _serviceProvider.GetRequiredService<IHubContext<THub>>();
					}

					hub.Clients = _hubContext.Clients;
					hub.Context = new HubCallerContext(connection);
					hub.Groups = _hubContext.Groups;

					try
					{
						await invocation(hub);
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Presence notification failed.");
					}
					finally
					{
						hubActivator.Release(hub);
					}
				}
			}
		}

		public void Dispose()
		{
			_userTracker.UsersJoined -= OnUsersJoined;
			_userTracker.UsersLeft -= OnUsersLeft;
		}

		public override Task InvokeAllAsync(string methodName, object[] args)
		{
			return _wrappedHubLifetimeManager.InvokeAllAsync(methodName, args);
		}

		public override Task InvokeAllExceptAsync(string methodName, object[] args, IReadOnlyList<string> excludedIds)
		{
			return _wrappedHubLifetimeManager.InvokeAllExceptAsync(methodName, args, excludedIds);
		}

		public override Task InvokeConnectionAsync(string connectionId, string methodName, object[] args)
		{
			return _wrappedHubLifetimeManager.InvokeConnectionAsync(connectionId, methodName, args);
		}

		public override Task InvokeGroupAsync(string groupName, string methodName, object[] args)
		{
			return _wrappedHubLifetimeManager.InvokeGroupAsync(groupName, methodName, args);
		}

		public override Task InvokeUserAsync(string userId, string methodName, object[] args)
		{
			return _wrappedHubLifetimeManager.InvokeUserAsync(userId, methodName, args);
		}

		public override Task AddGroupAsync(string connectionId, string groupName)
		{
			return _wrappedHubLifetimeManager.AddGroupAsync(connectionId, groupName);
		}

		public override Task RemoveGroupAsync(string connectionId, string groupName)
		{
			return _wrappedHubLifetimeManager.RemoveGroupAsync(connectionId, groupName);
		}
	}

}
