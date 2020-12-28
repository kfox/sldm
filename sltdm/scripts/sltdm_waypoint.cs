package sltdm_waypoint
{


function SLTDMGame::leaderCheck(%game, %client)
{
	// leave if the match hasn't started yet
	if (!$MatchStarted) return;

	if (%client == $TeamRank[%client.team, 0] && %client != $SLTDM::Leader[%client.team])
	{
		// this client is a leader
		%leader = %client;
		%game.showOtherTeamTargetWaypoint(%leader.team);

		// get the other team
		%otherTeam = %leader.team == 1 ? 2 : 1;

		// let everyone know a new leader is in the hizzouse
		messageClient(%leader, '', '\c2You are now the %1 team leader!~wfx/misc/flipflop_taken.wav', $TeamName[%leader.team]);
		messageTeamExcept(%leader, '', '\c2%1 is now your team leader!~wfx/misc/flipflop_lost.wav', %leader.name);
		messageTeam(%otherTeam, 'MsgRabbitWaypoint', '\c2%1 has become the %2 team leader!~wfx/misc/target_waypoint.wav', %leader.name, $TeamName[%leader.team]);

		// we'll use this to check next time
		$SLTDM::Leader[%leader.team] = %leader;

		// update huds with the new leader
		messageAll('MsgSLTDMTeamLeaderIs', "", %leader.team, %leader.name);
	}
	else if (%client != $TeamRank[%client.team, 0] && %client == $SLTDM::Leader[%client.team])
	{
		%game.recalcTeamRanks($TeamRank[%client.team, 0]);
	}
	else
	{
		%leaderTeam = %client.team == 1 ? 2 : 1;
		%game.showClientTargetWaypoint($TeamRank[%leaderTeam, 0], %client);
	}
}

function SLTDMGame::showOtherTeamTargetWaypoint(%game, %leaderTeam)
{
	%team = %leaderTeam == 1 ? 2 : 1;
	%leader = $TeamRank[%leaderTeam, 0];

	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		if (%client.team == %team)
		{
			//echo("client:" @ %client SPC "leader:" @ %leader);
			%game.showClientTargetWaypoint(%leader, %client);
		}
	}
}

function SLTDMGame::showClientTargetWaypoint(%game, %leader, %client)
{
	// wait until the match starts
	if (!$MatchStarted) return;

	// skip bots
	if (%client.isAIControlled()) return;

	// don't waypoint yourself
	if (%leader == %client) return;

	// remove any old waypoints
	%game.hideClientTargetWaypoint(%client);

	// scope the client
	%leader.player.scopeToClient(%client);

	// issue a command to kill the target
	%client.setTargetId(%leader.target);
	commandToClient(%client, 'TaskInfo', %client, -1, false, "Lance the leader!");
	%client.sendTargetTo(%client, true);

	// show the waypoint
	%client.targetWaypointSchedule = %game.schedule(3000, "showTargetWaypoint", %client);

	// remember this
	%client.hasWaypoint = true;
}

function SLTDMGame::hideOtherTeamTargetWaypoint(%game, %leaderTeam)
{
	%team = %leaderTeam == 1 ? 2 : 1;

	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);

		if (%client.team == %team)
			%game.hideClientTargetWaypoint(%client);
	}
}

function SLTDMGame::hideClientTargetWaypoint(%game, %client)
{
	// cancel existing waypoint scheduling thread
	if (isObject(%client.targetWaypointSchedule))
		cancel(%client.targetWaypointSchedule);

	// remove the waypoint
	removeClientTargetType(%client, "AssignedTask");

	// remember this
	%client.hasWaypoint = false;
}


};
