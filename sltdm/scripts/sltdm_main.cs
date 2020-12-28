package sltdm_main
{


function SLTDMGame::missionLoadDone(%game)
{
	DefaultGame::missionLoadDone(%game);
	%game.setPrefs(%game);
}

function SLTDMGame::setUpTeams(%game)
{
	DefaultGame::setUpTeams(%game);

	// allow all teams to "listen" to (i.e., see) each other
	for(%i = 1; %i < 32; %i++)
	{
		setSensorGroupAlwaysVisMask(%i, 0xffffffff);
		setSensorGroupListenMask(%i, 0xffffffff);
	}
}

function SLTDMGame::initGameVars(%game)
{
	SLTDMGame::initInventory();

	%game.SCORE_PER_KILL = 1;
	%game.SCORE_PER_DEATH = -1;
	%game.SCORE_PER_SUICIDE = -1;
	%game.SCORE_PER_INVO = -1;
	%game.SCORE_PER_TEAMKILL = -1;
	%game.SCORE_PER_LEADERKILL = 2; // bonus for killing leader

	for(%team = 1; %team <= 2; %team++)
	{
		$SLTDM::Leader[%team] = 0;
		$TeamScore[%team] = 0;
	}

	if ($SLTDM::PrefsLoaded != $SLTDM::Version)
		SLTDMGame::loadPrefs();
}

function SLTDMGame::allowsProtectedStatics(%game)
{
	// prevent players from destroying gens, sensors, or inventory stations
	return true;
}

function SLTDMGame::dropFlag(%game, %player)
{
	// to reduce console spam
}

function SLTDMGame::playerSpawned(%game, %player)
{
	DefaultGame::playerSpawned(%game, %player);
	if ($SLTDM::Pref::SpawnWithFaves > 3)
		buyFavorites(%player.client);
	%player.selectWeaponSlot(0);
}

function SLTDMGame::assignClientTeam(%game, %client)
{
	DefaultGame::assignClientTeam(%game, %client, %respawn);

	// make sure the objective HUD indicates your team on top and in green...
	if (%client.team > 0)
	{
		if(%client.hasSLTDMClient)
			messageClient(%client, 'MsgSLTDMCheckTeamLines', "", %client.team);
		else
			messageClient(%client, 'MsgCheckTeamLines', "", %client.team);
	}

	// show the billboards
	if (!%client.isAIControlled())
	{
		cancel(%client.billboardThread);
		%game.showBillboard(%client);
	}
}

function DefaultGame::clientChangeTeam(%game, %client, %team, %fromObs)
{
	//first, remove the client from the team rank array
	//the player will be added to the new team array as soon as he respawns...
	%game.removeFromTeamRankArray(%client);

	%pl = %client.player;
	if(isObject(%pl))
	{
		if(%pl.isMounted())
		%pl.getDataBlock().doDismount(%pl);
		%pl.scriptKill(0);
	}

	// reset the client's targets and tasks only
	clientResetTargets(%client, true);

	// give this client a new handle to disassociate ownership of deployed objects
	if( %team $= "" && (%team > 0 && %team <= %game.numTeams))
	{
		if( %client.team == 1 )
			%client.team = 2;
		else
			%client.team = 1;
	}
	else
		%client.team = %team;

	// Set the client's skin
	if (!%client.isAIControlled())
		setTargetSkin( %client.target, %game.getTeamSkin(%client.team) );
	setTargetSensorGroup( %client.target, %client.team );
	%client.setSensorGroup( %client.team );

	// Spawn the player
	%client.lastSpawnPoint = %game.pickPlayerSpawn( %client );

	%game.createPlayer( %client, %client.lastSpawnPoint, $MatchStarted );

	if($MatchStarted)
		%client.setControlObject(%client.player);
	else
	{
		%client.camera.getDataBlock().setMode(%client.camera, "pre-game", %client.player);
		%client.setControlObject(%client.camera);
	}

	// call the onEvent for this game type
	%game.onClientEnterObserverMode(%client);

	if(%fromObs $= "" || !%fromObs)
	{
		messageAllExcept( %client, -1, 'MsgClientJoinTeam', '\c1%1 switched to team %2.', %client.name, %game.getTeamName(%client.team), %client, %client.team );
	messageClient( %client, 'MsgClientJoinTeam', '\c1You switched to team %2.', $client.name, %game.getTeamName(%client.team), %client, %client.team );
	}
	else
	{
		messageAllExcept( %client, -1, 'MsgClientJoinTeam', '\c1%1 joined team %2.', %client.name, %game.getTeamName(%client.team), %client, %team );
		messageClient( %client, 'MsgClientJoinTeam', '\c1You joined team %2.', $client.name, %game.getTeamName(%client.team), %client, %client.team );
	}

	updateCanListenState( %client );

	// switch objective hud lines when client switches teams
	if (%client.team > 0)
	{
		if(%client.hasSLTDMClient)
			messageClient(%client, 'MsgSLTDMCheckTeamLines', "", %client.team);
		else
			messageClient(%client, 'MsgCheckTeamLines', "", %client.team);
	}

	logEcho(%client.nameBase@" (cl "@%client@") switched to team "@%client.team);

	// reset the client's score when they switch
	%game.resetScore(%client);
}

function SLTDMGame::clientMissionDropReady(%game, %client)
{
	if (%client.hasSLTDMClient)
		messageClient(%client, 'MsgSLTDMVersion', "", $SLTDM::Version, $SLTDM::VersionString);
	messageClient(%client, 'MsgMissionDropInfo', '\c0You are in mission %1 (%2).', $MissionDisplayName, $MissionTypeDisplayName);

	// show our fancy improved hud if possible
	// otherwise, fall back to the CTF hud
	if (%client.hasSLTDMClient)
		messageClient(%client, 'MsgClientReady', "", SLTDMGame);
	else
		messageClient(%client, 'MsgClientReady', "", CTFGame);

	%game.resetScore(%client);

	%client.lastBillboard = 0;
	%client.hasWaypoint = false;

	// initialize the hud values
	for(%i = 1; %i <= %game.numTeams; %i++)
	{
		if (%client.hasSLTDMClient)
			messageClient(%client, 'MsgSLTDMAddTeam', "", %i, %game.getTeamName(%i), $TeamScore[%i], $TeamRank[%i,0].name);
		else
			messageClient(%client, 'MsgCTFAddTeam', "", %i, %game.getTeamName(%i), "", $TeamScore[%i]);
	}

	//synchronize the clock HUD
	messageClient(%client, 'MsgSystemClock', "", 0, 0);

	%game.sendClientTeamList( %client );
	%game.setupClientHuds( %client );

	%observer = false;
	if( !$Host::TournamentMode )
	{
		if( %client.camera.mode $= "observerFly" || %client.camera.mode $= "justJoined")
		{
			%observer = true;
			%client.observerStartTime = getSimTime();
			commandToClient(%client, 'setHudMode', 'Observer');
			%client.setControlObject( %client.camera );
			updateObserverFlyHud(%client);
		}

		if( !%observer )
		{
			if(!$MatchStarted && !$CountdownStarted) // server has not started anything yet
			{
				%client.setControlObject( %client.camera );
				commandToClient(%client, 'setHudMode', 'Observer');
			}
			else if(!$MatchStarted && $CountdownStarted) // server has started the countdown
			{
				commandToClient(%client, 'setHudMode', 'Observer');
				%client.setControlObject( %client.camera );
			}
			else
			{
				commandToClient(%client, 'setHudMode', 'Standard'); // the game has already started
				%client.setControlObject( %client.player );
			}
		}
	}
	else
	{
		// set all players into obs mode. setting the control object will handle further procedures...
		%client.camera.getDataBlock().setMode( %client.camera, "ObserverFly" );
		commandToClient(%client, 'setHudMode', 'Observer');
		%client.setControlObject( %client.camera );
		messageAll( 'MsgClientJoinTeam', "",%client.name, $teamName[0], %client, 0 );
		%client.team = 0;

		if( !$MatchStarted && !$CountdownStarted)
		{
			if($TeamDamage)
				%damMess = "ENABLED";
			else
				%damMess = "DISABLED";

			if(%game.numTeams > 1)
				BottomPrint(%client, "Server is Running in Tournament Mode.\nPick a Team\nTeam Damage is " @ %damMess, 0, 3 );
		}
		else
		{
			BottomPrint( %client, "\nServer is Running in Tournament Mode", 0, 3 );
		}
	}

	// make sure the objective HUD indicates your team on top and in green...
	if (%client.team > 0)
	{
		if(%client.hasSLTDMClient)
			messageClient(%client, 'MsgSLTDMCheckTeamLines', "", %client.team);
		else
			messageClient(%client, 'MsgCheckTeamLines', "", %client.team);
	}

	// we're ready to go
	%client.matchStartReady = true;
	echo("Client" SPC %client SPC "is ready.");

	if ( isDemo() )
	{
		if ( %client.demoJustJoined )
		{
			%client.demoJustJoined = false;
			centerPrint( %client, "Welcome to the Tribes 2 Demo." NL "You have been assigned the name \"" @ %client.nameBase @ "\"." NL "Press FIRE to join the game.", 0, 3 );
		}
	}
}

function SLTDMGame::AIHasJoined(%game, %client)
{
	// no additional messages
}

function SLTDMGame::createPlayer(%game, %client, %spawnLoc, %respawn)
{
	DefaultGame::createPlayer(%game, %client, %spawnLoc, %respawn);
	// make everything on your team part of your sensor network
	%client.setSensorGroup(%client.team);
}

function SLTDMGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLoc)
{
	DefaultGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLoc);
}

function SLTDMGame::gameOver(%game)
{
	DefaultGame::gameOver(%game);

	// remove any leftover billboards
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		cancel(%client.billboardThread);
		clearBottomPrint(%client);
	}

	//send the winner message
	%winner = "";
	if ($teamScore[1] > $teamScore[2])
		%winner = %game.getTeamName(1);
	else if ($teamScore[2] > $teamScore[1])
		%winner = %game.getTeamName(2);

	if (%winner $= 'Storm')
		messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.stowins.wav" );
	else if (%winner $= 'Inferno')
		messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.infwins.wav" );
	else if (%winner $= 'Starwolf')
		messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.swwin.wav" );
	else if (%winner $= 'Blood Eagle')
		messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.bewin.wav" );
	else if (%winner $= 'Diamond Sword')
		messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.dswin.wav" );
	else if (%winner $= 'Phoenix')
		messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.pxwin.wav" );
	else
		messageAll('MsgGameOver', "Match has ended.~wvoice/announcer/ann.gameover.wav" );

	// clear the objective hud
	messageAll('MsgClearObjHud', "");

	// reset each team's score to zero
	for(%j = 1; %j <= %game.numTeams; %j++)
		$TeamScore[%j] = 0;
}

function SLTDMGame::enterMissionArea(%game, %playerData, %player)
{
	DMGame::enterMissionArea(%game, %playerData, %player);
}

function SLTDMGame::leaveMissionArea(%game, %playerData, %player)
{
	DMGame::leaveMissionArea(%game, %playerData, %player);
	%player.alertThread = %game.schedule(1000, "SLTDMAlertPlayer", 3, %player);
}

function SLTDMGame::SLTDMAlertPlayer(%game, %count, %player)
{
	if(%count > 1)
		%player.alertThread = %game.schedule(1000, "SLTDMAlertPlayer", %count - 1, %player);
	else
		%player.alertThread = %game.schedule(1000, "MissionAreaDamage", %player);
}

function SLTDMGame::MissionAreaDamage(%game, %player)
{
	DMGame::MissionAreaDamage(%game, %player);
}

function SLTDMGame::showBillboard(%game, %client)
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
				if ($SLTDM::Pref::SpawnWithFaves < 4)
					%status = "You will spawn using the default loadout";
				else
					%status = "You will spawn using your selected favorite loadout";
				%modifiability = %game.describePrefs($SLTDM::Pref::SpawnWithFaves);
				bottomPrint(%client, %status NL %modifiability, %timeShow, 2);
				%didBillboard = true;

			case 2:
				if ($SLTDM::Pref::InventoryStationAccess == 8)
				{
					%status = "Inventory stations have been removed";
					%modifiability = "No in-game change possible";
				}
				else
				{
					if ($SLTDM::Pref::InventoryStationAccess < 4)
						%status = "Inventory station access is disabled";
					else
						%status = "Inventory station access is enabled";
					%modifiability = %game.describePrefs($SLTDM::Pref::InventoryStationAccess);
				}
				bottomPrint(%client, %status NL %modifiability, %timeShow, 2);
				%didBillboard = true;

			case 3:
				if ($SLTDM::Pref::InventoryStationAccess < 8)
				{
					if ($SLTDM::Pref::InventoryStationTeleportation < 4)
					        %status = "Inventory stations will not teleport you";
					else
						%status = "Inventory stations will teleport you";
					%modifiability = %game.describePrefs($SLTDM::Pref::InventoryStationTeleportation);
					bottomPrint(%client, %status NL %modifiability, %timeShow, 2);
					%didBillboard = true;
				}

			case 4:
				if ($SLTDM::Pref::InventoryStationAccess > 3 &&
				    $SLTDM::Pref::InventoryStationAccess < 8)
				{
					if ($SLTDM::Pref::InventoryStationAccessPenalty < 4)
					        %status = "There is no penalty for inventory station access";
					else
					        %status = "You will be penalized for inventory station access";
					%modifiability = %game.describePrefs($SLTDM::Pref::InventoryStationAccessPenalty);
					bottomPrint(%client, %status NL %modifiability, %timeShow, 2);
					%didBillboard = true;
				}

			case 5:
				if (%client.hasSLTDMClient)
				{
					if (%client.SLTDMClientVersion < $SLTDM::latestClientVersion)
					{
						%firstLine = "A newer version of the SLTDM Client is available.";
						%secondLine = "The current version is v" @ $SLTDM::latestClientVersion @ ", your version is v" @ %client.SLTDMClientVersion @ ".";
						bottomPrint(%client, %firstLine NL %secondLine, %timeShow, 2);
						%didBillboard = true;
					}
				}
				else
				{
					%firstLine = "An optional SLTDM Client is available from https://github.com/kfox/sldm";
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

function GameConnection::onDrop(%client, %reason)
{
	Parent::onDrop(%client, %reason);
}

function DefaultGame::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation)
{
	parent::onClientKilled(%game, %clVictim, %clKiller, %damageType, %implement, %damageLocation);
}

function SLTDMGame::onClientEnterObserverMode(%game, %client)
{
	// cancel the respawn schedule
	cancel(%client.forceRespawnThread);
}

function serverCmdSLTDMRegisterClient(%client, %version, %versionString)
{
	%client.hasSLTDMClient = true;
	%client.SLTDMClientVersion = %version;

	if (%version >= $SLTDM::latestClientVersion)
		messageClient(%client, '', '\c2SLTDM Client (v%1) successfully registered.', %versionString);
	else
		messageClient(%client, '', '\c2A newer SLTDM Client (v%1) is available.', $SLTDM::latestClientVersion);
}

function SLTDMGame::startMatch(%game)
{
	DefaultGame::startMatch(%game);
	%game.recalcAllScores();
}

function SLTDMGame::timeLimitReached(%game)
{
	logEcho("game over (timelimit)");
	%game.gameOver();
	cycleMissions();
}


};
