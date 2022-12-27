using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Multiplay;
using UnityEngine;

/// <summary>
/// An example of how to access and react to multiplay server events.
/// </summary>
public class ServerEvents : NetworkBehaviour
{
private MultiplayEventCallbacks m_MultiplayEventCallbacks;
private IServerEvents m_ServerEvents;

/// <summary>
/// This should be done early in the server's lifecycle, as you'll want to receive events as soon as possible.
/// </summary>
private async void Start()
	{
		if (IsServer)
		{
			// We must first prepare our callbacks like so:
			m_MultiplayEventCallbacks = new MultiplayEventCallbacks();
			m_MultiplayEventCallbacks.Allocate += OnAllocate;
			m_MultiplayEventCallbacks.Deallocate += OnDeallocate;
			m_MultiplayEventCallbacks.Error += OnError;
			m_MultiplayEventCallbacks.SubscriptionStateChanged += OnSubscriptionStateChanged;

			// We must then subscribe.
			m_ServerEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(m_MultiplayEventCallbacks);

		}
	}

/// <summary>
/// Handler for receiving the allocation multiplay server event.
/// </summary>
/// <param name="allocation">The allocation received from the event.</param>
private void OnAllocate(MultiplayAllocation allocation)
	{
// Here is where you handle the allocation.
// This is highly dependent on your game, however this would typically be some sort of setup process.
// Whereby, you spawn NPCs, setup the map, log to a file, or otherwise prepare for players.
// Once you the allocation has been handled, you can then call ReadyServerForPlayersAsync()!
	}

/// <summary>
/// Handler for receiving the deallocation multiplay server event.
/// </summary>
/// <param name="deallocation">The deallocation received from the event.</param>
private void OnDeallocate(MultiplayDeallocation deallocation)
	{
// Here is where you handle the deallocation.
// This is highly dependent on your game, however this would typically be some sort of teardown process.
// You might want to deactivate unnecessary NPCs, log to a file, or perform any other cleanup actions.
	}

/// <summary>
/// Handler for receiving the error multiplay server event.
/// </summary>
/// <param name="error">The error received from the event.</param>
private void OnError(MultiplayError error)
	{
// Here is where you handle the error.
// This is highly dependent on your game. You can inspect the error by accessing the error.Reason and error.Detail fields.
// You can switch on the error.Reason field, log the error, or otherwise handle it as you need to.
	}

/// <summary>
/// 
/// </summary>
/// <param name="state"></param>
private void OnSubscriptionStateChanged(MultiplayServerSubscriptionState state)
	{
switch (state)
		{
case MultiplayServerSubscriptionState.Unsubscribed: /* The Server Events subscription has been unsubscribed from. */ break;
case MultiplayServerSubscriptionState.Synced: /* The Server Events subscription is up to date and active. */ break;
case MultiplayServerSubscriptionState.Unsynced: /* The Server Events subscription has fallen out of sync, the subscription will attempt to automatically recover. */ break;
case MultiplayServerSubscriptionState.Error: /* The Server Events subscription has fallen into an errored state and will not recover automatically. */ break;
case MultiplayServerSubscriptionState.Subscribing: /* The Server Events subscription is attempting to sync. */ break;
		}
	}
}