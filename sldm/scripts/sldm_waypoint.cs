package sldm_waypoint
{


function ShocklanceDMGame::showLeaderWaypoint(%game)
{
	// wait until the match starts
	if (!$MatchStarted)
		return;

	// get the leader
	%leader = $TeamRank[0, 0];

	// set a waypoint for appropriate clients
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);

		// don't set a waypoint for bots or observers
		if (%client.isAIControlled() || %client.team == 0)
			continue;

//		if (!%client.isAIControlled() && %client.team != 0)

		// is the client the leader?
		if (%client != %leader)
		{
			// the client is not the leader --
			// if the client does not have a waypoint set,
			// or we have a new leader, then set a waypoint
			if (!%client.hasWaypoint || %leader != $ShocklanceDM::Leader)
			{
				// scope the client
				%leader.player.scopeToClient(%client);

				// issue a command to kill the target
				%client.setTargetId(%leader.target);
				commandToClient(%client, 'TaskInfo', %client, -1, false, "Lance the leader!");
				%client.sendTargetTo(%client, true);

				// cancel existing waypoint scheduling thread
				if (%client.hasWaypoint)
					cancel(%client.waypointSchedule);

				// show the waypoint
				%client.waypointSchedule = %game.schedule(3000, "showTargetWaypoint", %client);
				%client.hasWaypoint = true;

				// announce the new leader
				messageClient(%client, 'MsgRabbitWaypoint', '\c2%1 has taken the lead!~wfx/misc/target_waypoint.wav', %leader.name);
			}
		}
		else
		{
			// the client is the leader --
			// if the leader has changed, then
			// do some waypoint cleanup
			if (%leader != $ShocklanceDM::Leader)
			{
				messageClient(%client, '', '\c2You have taken the lead!~wfx/misc/flipflop_taken.wav');
				// remove the old waypoint
				cancel(%client.waypointSchedule);
				removeClientTargetType(%client, "AssignedTask");
				%client.hasWaypoint = false;
			}
		}
	}

	// remember who the last leader was
	if (%leader != $ShocklanceDM::Leader)
		$ShocklanceDM::Leader = %leader;
}


};
