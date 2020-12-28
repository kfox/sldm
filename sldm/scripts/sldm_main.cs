package sldm_main
{


function ShocklanceDMGame::missionLoadDone(%game)
{
	DefaultGame::missionLoadDone(%game);
	%game.setPrefs(%game);
}

function ShocklanceDMGame::setUpTeams(%game)
{
	%group = nameToID("MissionGroup/Teams");
	if(%group == -1)
		return;

	// create a team0 if it does not exist
	%team = nameToID("MissionGroup/Teams/team0");
	if(%team == -1)
	{
		%team = new SimGroup("team0");
		%group.add(%team);
	}

	// 'team0' is not counted as a team here
	%game.numTeams = 0;
	while(%team != -1)
	{
		// create drop set and add all spawnsphere objects into it
		%dropSet = new SimSet("TeamDrops" @ %game.numTeams);
		MissionCleanup.add(%dropSet);

		%spawns = nameToID("MissionGroup/Teams/team" @ %game.numTeams @ "/SpawnSpheres");
		if(%spawns != -1)
		{
			%count = %spawns.getCount();
			for(%i = 0; %i < %count; %i++)
				%dropSet.add(%spawns.getObject(%i));
		}

		// set the 'team' field for all the objects in this team
		%team.setTeam(0);

		clearVehicleCount(%team+1);
		// get next group
		%team = nameToID("MissionGroup/Teams/team" @ %game.numTeams + 1);
		if (%team != -1)
			%game.numTeams++;
	}

	// set the number of sensor groups (including team0) that are processed
	setSensorGroupCount(%game.numTeams + 1);

	%game.numTeams = 1;

	// allow all teams to "listen" to (i.e., see) each other
	for(%i = 1; %i < 32; %i++)
	{
		setSensorGroupAlwaysVisMask(%i, 0xffffffff);
		setSensorGroupListenMask(%i, 0xffffffff);
	}
}

function ShocklanceDMGame::initGameVars(%game)
{
	DMGame::initGameVars(%game);
	ShocklanceDMGame::initInventory();
	%game.SCORE_PER_INVO = -1;
	%game.SCORE_PER_HIT = 1; // hit on an opponent
	%game.SCORE_PER_REARSHOT = 1; // bonus for a shocklance rearshot
	%game.SCORE_PER_LEADERKILL = 2; // bonus for killing leader
	$ShocklanceDM::Leader = 0;

	if ($ShocklanceDM::PrefsLoaded != $ShocklanceDM::Version)
		ShocklanceDMGame::loadPrefs();
}

function ShocklanceDMGame::allowsProtectedStatics(%game)
{
	// prevent players from destroying gens, sensors, or inventory stations
	return true;
}

function ShocklanceDMGame::playerSpawned(%game, %player)
{
	DefaultGame::playerSpawned(%game, %player);
	if ($ShocklanceDM::Pref::SpawnWithFaves > 3)
		buyFavorites(%player.client);
	%player.selectWeaponSlot(0);
}

function ShocklanceDMGame::pickPlayerSpawn(%game, %client, %respawn)
{
	DMGame::pickPlayerSpawn(%game, %client, %respawn);
}

function ShocklanceDMGame::clientJoinTeam( %game, %client, %team, %respawn )
{
	DMGame::clientJoinTeam( %game, %client, %team, %respawn );
}

function ShocklanceDMGame::assignClientTeam(%game, %client)
{
	for(%i = 1; %i < 32; %i++)
		$ShocklanceDMTeamArray[%i] = false;

	%maxSensorGroup = 0;
	%count = ClientGroup.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%cl = ClientGroup.getObject(%i);
		if(%cl != %client)
		{
			$ShocklanceDMTeamArray[%cl.team] = true;
			if (%cl.team > %maxSensorGroup)
				%maxSensorGroup = %cl.team;
		}
	}

	// loop through the team array, looking for an empty team
	for(%i = 1; %i < 32; %i++)
	{
		if (! $ShocklanceDMTeamArray[%i])
		{
			%client.team = %i;
			if (%client.team > %maxSensorGroup)
				%maxSensorGroup = %client.team;
			break;
		}
	}

	// set player's skin pref here
	setTargetSkin(%client.target, %client.skin);

	// let everybody know you are no longer an observer
	messageAll( 'MsgClientJoinTeam', '\c1%1 is ready to lance or be lanced.', %client.name, "", %client, 1 );
	updateCanListenState( %client );

	// set the max number of sensor groups...
	setSensorGroupCount(%maxSensorGroup + 1);

	// show the billboards
	if (!%client.isAIControlled())
	{
		cancel(%client.billboardThread);
		%game.showBillboard(%client);
	}
}

function ShocklanceDMGame::clientMissionDropReady(%game, %client)
{
	messageClient(%client, 'MsgSLDMVersion', "", $ShocklanceDM::Version, $ShocklanceDM::VersionString);
	messageClient(%client, 'MsgMissionDropInfo', '\c0You are in mission %1 (%2).', $MissionDisplayName, $MissionTypeDisplayName);

	// show our fancy improved hud if possible
	// otherwise, fall back to the normal DM hud
	if (%client.hasSLDMClient)
		messageClient(%client, 'MsgClientReady', "", %game.class);
	else
		messageClient(%client, 'MsgClientReady', "", DMGame);

	%game.resetScore(%client);

	messageClient(%client, 'MsgYourScoreIs', "", mFloatLength(0, 1));
	messageClient(%client, 'MsgDMPlayerDies', "", 0);
	messageClient(%client, 'MsgDMKill', "", 0);

	if (%client.hasSLDMClient)
	{
		messageClient(%client, 'MsgSLDMInvo', "", 0);
		messageClient(%client, 'MsgSLDMStreak', "", 0);
		messageClient(%client, 'MsgSLDMBonus', "", 0);
	}

	%client.lastBillboard = 0;
	%client.hasWaypoint = false;

	DefaultGame::clientMissionDropReady(%game, %client);
}

function ShocklanceDMGame::AIHasJoined(%game, %client)
{
	// no additional messages
}

function ShocklanceDMGame::createPlayer(%game, %client, %spawnLoc, %respawn)
{
	DMGame::createPlayer(%game, %client, %spawnLoc, %respawn);
}

function ShocklanceDMGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLoc)
{
	DefaultGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLoc);
}

function ShocklanceDMGame::timeLimitReached(%game)
{
	DMGame::timeLimitReached(%game);
}

function ShocklanceDMGame::gameOver(%game)
{
	// remove any dangling billboards
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		cancel(%client.billboardThread);
		clearBottomPrint(%client);
	}

	// do the regular stuff
	DMGame::gameOver(%game);
}

function ShocklanceDMGame::enterMissionArea(%game, %playerData, %player)
{
	DMGame::enterMissionArea(%game, %playerData, %player);
}

function ShocklanceDMGame::leaveMissionArea(%game, %playerData, %player)
{
	if (%player.getState() $= "Dead") return;

	%player.client.outOfBounds = true;
	messageClient(%player.client, 'LeaveMissionArea', '\c1You have left the mission area. Return or take damage.~wfx/misc/warning_beep.wav');
	logEcho(%player.client.nameBase@" (pl "@%player@"/cl "@%player.client@") left mission area");
	%player.alertThread = %game.schedule(1000, "ShocklanceDMAlertPlayer", 3, %player);
}

function ShocklanceDMGame::ShocklanceDMAlertPlayer(%game, %count, %player)
{
	if (%count > 1)
		%player.alertThread = %game.schedule(1000, "ShocklanceDMAlertPlayer", %count - 1, %player);
	else
		%player.alertThread = %game.schedule(1000, "MissionAreaDamage", %player);
}

function ShocklanceDMGame::MissionAreaDamage(%game, %player)
{
	if (%player.getState() !$= "Dead")
	{
		%player.setDamageFlash(0.1);
		%damageRate = 0.05;
		%prevHurt = %player.getDamageLevel();
		%player.setDamageLevel(%prevHurt + %damageRate);

		// a little redundancy to see if the lastest damage killed the player
		if (%player.getState() $= "Dead")
			%game.onClientKilled(%player.client, 0, $DamageType::OutOfBounds);
		else
			%player.alertThread = %game.schedule(1000, "MissionAreaDamage", %player);
	}
	else
	{
		%game.onClientKilled(%player.client, 0, $DamageType::OutOfBounds);
	}
}

function ShocklanceDMGame::applyConcussion(%game, %player)
{
}

function ShocklanceDMGame::showBillboard(%game, %client)
{
	// No billboards in tourney mode
	if ($Host::TournamentMode)
		return;

	// No billboards for observers
	if (%client.team == 0)
		return;

	// vars stored on client
	// %client.lastBillboard - id of last billboard (initialized to 0 elsewhere)

	// Billboards
	//  1 spawn with/without faves
	//  2 invo access
	//  3 teleporters
	//  4 invo penalty
	//  5 client version update

	%billboardCount = 5;
	%timeShow = 7;
	%didBillboard = false;

	// clear any existing thread for this client
	cancel(%client.billboardThread);

	// Show next billboard
	while (!%didBillboard && %client.lastBillboard < %billboardCount)
	{
		%client.lastBillboard++;

		switch (%client.lastBillboard)
		{

			case 1:
				if ($ShocklanceDM::Pref::SpawnWithFaves < 4)
					%status = "You will spawn using the default loadout";
				else
					%status = "You will spawn using your selected favorite loadout";
				%modifiability = %game.describePrefs($ShocklanceDM::Pref::SpawnWithFaves);
				bottomPrint(%client, %status NL %modifiability, %timeShow, 2);
				%didBillboard = true;

			case 2:
				if ($ShocklanceDM::Pref::InventoryStationAccess == 8)
				{
					%status = "Inventory stations have been removed";
					%modifiability = "No in-game change possible";
				}
				else
				{
					if ($ShocklanceDM::Pref::InventoryStationAccess < 4)
						%status = "Inventory station access is disabled";
					else
						%status = "Inventory station access is enabled";
					%modifiability = %game.describePrefs($ShocklanceDM::Pref::InventoryStationAccess);
				}
				bottomPrint(%client, %status NL %modifiability, %timeShow, 2);
				%didBillboard = true;

			case 3:
				if ($ShocklanceDM::Pref::InventoryStationAccess < 8)
				{
					if ($ShocklanceDM::Pref::InventoryStationTeleportation < 4)
					        %status = "Inventory stations will not teleport you";
					else
						%status = "Inventory stations will teleport you";
					%modifiability = %game.describePrefs($ShocklanceDM::Pref::InventoryStationTeleportation);
					bottomPrint(%client, %status NL %modifiability, %timeShow, 2);
					%didBillboard = true;
				}

			case 4:
				if ($ShocklanceDM::Pref::InventoryStationAccess > 3 &&
				    $ShocklanceDM::Pref::InventoryStationAccess < 8)
				{
					if ($ShocklanceDM::Pref::InventoryStationAccessPenalty < 4)
					        %status = "There is no penalty for inventory station access";
					else
					        %status = "You will be penalized for inventory station access";
					%modifiability = %game.describePrefs($ShocklanceDM::Pref::InventoryStationAccessPenalty);
					bottomPrint(%client, %status NL %modifiability, %timeShow, 2);
					%didBillboard = true;
				}

			case 5:
				if (%client.hasSLDMClient)
				{
					if (%client.SLDMClientVersion < $ShocklanceDM::latestClientVersion)
					{
						%firstLine = "A newer version of the SLDM Client is available.";
						%secondLine = "The current version is v" @ $ShocklanceDM::latestClientVersion @ ", your version is v" @ %client.SLDMClientVersion @ ".";
						bottomPrint(%client, %firstLine NL %secondLine, %timeShow, 2);
						%didBillboard = true;
					}
				}
				else
				{
					%firstLine = "An optional SLDM Client is available from https://github.com/kfox/sldm";
					%secondLine = "The client provides an improved objective hud.";
					bottomPrint(%client, %firstLine NL %secondLine, %timeShow, 2);
					%didBillboard = true;
				}

		}

	}

	// schedule the next billboard if more remain
	if (%client.lastBillboard < %billboardCount)
		%client.billboardThread = %game.schedule((%timeShow - 1) * 1000, "showBillboard", %client);
}

function GameConnection::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch)
{
	Parent::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch);
	messageClient(%client, 'MsgSLDMClientJoin');
}

function serverCmdSLDMRegisterClient(%client, %version, %versionString)
{
	%client.hasSLDMClient = true;
	%client.SLDMClientVersion = %version;

	if (%version >= $ShocklanceDM::latestClientVersion)
		messageClient(%client, '', '\c2SLDM Client (v%1) successfully registered.', %versionString);
	else
		messageClient(%client, '', '\c2A newer SLDM Client (v%1) is available.', $ShocklanceDM::latestClientVersion);
}

function ShocklanceDMGame::onClientEnterObserverMode(%game, %client)
{
	// to prevent folks from entering obs mode to avoid death penalty
	%client.kills = 0;
	%client.leaderKills = 0;
	%client.rearshots = 0;
	%client.streak = 0;

	%game.recalcScore(%client);
}


};
