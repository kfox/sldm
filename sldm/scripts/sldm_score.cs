package sldm_score
{


function ShocklanceDMGame::recalcScore(%game, %client)
{
	if (%client <= 0)
		return;

	%hitValue = %client.hitscore;
	%killValue = %client.kills * %game.SCORE_PER_KILL;
	%deathValue = %client.deaths * %game.SCORE_PER_DEATH;
	%suicideValue = %client.suicides * %game.SCORE_PER_SUICIDE;
	%leaderKillValue = %client.leaderKills * %game.SCORE_PER_LEADERKILL;
	%rearshotValue = %client.rearshots * %game.SCORE_PER_REARSHOT;

	if ($ShocklanceDM::Pref::InventoryStationAccessPenalty > 3)
		%invoValue = %client.invos * %game.SCORE_PER_INVO;
	else
		%invoValue = 0;

	if (%killValue - %deathValue == 0)
		%client.efficiency = %hitValue + %suicideValue + %invoValue + %leaderKillValue;
	else
		%client.efficiency = ((%killValue * %killValue) / (%killValue - %deathValue)) + %hitValue + %suicideValue + %invoValue + %leaderKillValue + %rearshotValue;

	%client.score = mFloatLength(%client.efficiency, 1);
	messageClient(%client, 'MsgYourScoreIs', "", %client.score);
	%game.recalcTeamRanks(%client);
}

function ShocklanceDMGame::recalcTeamRanks(%game, %client)
{
	Parent::recalcTeamRanks(%game, %client);

	// set the waypoint
	%game.showLeaderWaypoint();
}

function ShocklanceDMGame::recalcAllScores(%game)
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
		%game.recalcScore(ClientGroup.getObject(%i));
}

function ShocklanceDMGame::updateKillScores(%game, %clVictim, %clKiller, %damageType, %implement)
{
	if (%game.testKill(%clVictim, %clKiller)) //verify victim was an enemy
	{
		%leader = $ShocklanceDM::Leader;

		%game.awardScoreKill(%clKiller);
		messageClient(%clKiller, 'MsgDMKill', "", %clKiller.kills);

		// award a bonus if the victim was the leader
		if (%clVictim == %leader)
		{
			%game.awardScoreLeaderKill(%clKiller);
			if (%clKiller.hasSLDMClient)
				messageClient(%clKiller, 'MsgSLDMBonus', "", %clKiller.leaderKills * %game.SCORE_PER_LEADERKILL);
		}

		if (%clVictim.rearshot)
		{
			%game.awardScoreRearShotKill(%clKiller);
			if (%clKiller.hasSLDMClient)
				messageClient(%clKiller, 'MsgSLDMRearShot', "", %clKiller.rearshots * %game.SCORE_PER_REARSHOT);
		}

		%game.awardScoreDeath(%clVictim);
	}
	else if (%game.testSuicide(%clVictim, %clKiller, %damageType)) // test for suicide
		%game.awardScoreSuicide(%clVictim);

	messageClient(%clVictim, 'MsgDMPlayerDies', "", %clVictim.deaths + %clVictim.suicides);
}

function ShocklanceDMGame::updateInvoScores(%game, %client)
{
	%game.awardScoreInvo(%client);
	if (%client.hasSLDMClient)
		messageClient(%client, 'MsgSLDMInvo', "", %client.invos);
}

function ShocklanceDMGame::awardScoreDeath(%game, %victimID)
{
	%victimID.deaths++;
	%victimID.streak = 0; // the winning streak is over!
	if (%victimID.hasSLDMClient)
		messageClient(%victimID, 'MsgSLDMStreak', "", %victimID.streak);
	%game.recalcScore(%victimID);
}

function ShocklanceDMGame::awardScoreKill(%game, %killerID)
{
	%killerID.kills++;
	%killerID.streak++; // how many kills since last death?
	if (%killerID.hasSLDMClient)
		messageClient(%killerID, 'MsgSLDMStreak', "", %killerID.streak);
	%game.recalcScore(%killerID);
}

function ShocklanceDMGame::awardScoreLeaderKill(%game, %killerID)
{
	%killerID.leaderKills++;
	messageAll('','\c2%1 gets a %2-point BONUS for killing the leader!~wfx/misc/bounty_objRem1.wav', %killerID.name, %game.SCORE_PER_LEADERKILL);
	%game.recalcScore(%killerID);
}

function ShocklanceDMGame::awardScoreRearShotKill(%game, %killerID)
{
	%killerID.rearshots++;
	messageClient(%killerID, '', '\c2You get a %2-point BONUS for a shocklance rearshot!~wfx/misc/bounty_objRem2.wav', %killerID.name, %game.SCORE_PER_REARSHOT);
	%game.recalcScore(%killerID);
}

function ShocklanceDMGame::awardScoreInvo(%game, %client)
{
	%client.invos++;
	%game.recalcScore(%client);
}

function ShocklanceDMGame::resetScore(%game, %client)
{
	%client.kills = 0;
	%client.rearshots = 0;
	%client.deaths = 0;
	%client.streak = 0;
	%client.invos = 0;
	%client.suicides = 0;
	%client.leaderKills = 0;
	%client.score = 0;
	%client.efficiency = 0;
}

function ShocklanceDMGame::updateScoreHud(%game, %client, %tag)
{
	// Clear the header:
	messageClient( %client, 'SetScoreHudHeader', "", "" );

	// Send the subheader:
	messageClient(%client, 'SetScoreHudSubheader', "", '<tab:15,235,320,370,455,520>\tPLAYER\tRATING\tKILLS\tDEATHS\tINVOS\tBONUS');

	for (%index = 0; %index < $TeamRank[0, count]; %index++)
	{
		//get the client info
		%cl = $TeamRank[0, %index];

		//get the score
		%clScore = mFloatLength( %cl.efficiency, 1 );

		%clKills = mFloatLength( %cl.kills, 0 );
		%clDeaths = mFloatLength( %cl.deaths + %cl.suicides, 0 );
		%clInvos = mFloatLength( %cl.invos, 0 );
		%clBonus =
			mFloatLength( %cl.leaderKills * %game.SCORE_PER_LEADERKILL, 0 ) +
			mFloatLength( %cl.rearshots * %game.SCORE_PER_REARSHOT, 0 ) +
			%cl.hitscore;
		%clStyle = %cl == %client ? "<color:dcdcdc>" : "";

		//if the client is not an observer, send the message
		if (%client.team != 0)
		{
			messageClient( %client, 'SetLineHud', "", %tag, %index, '%7<tab:20, 450>\t<clip:200>%1</clip><rmargin:280><just:right>%2<rmargin:350><just:right>%3<rmargin:420><just:right>%4<rmargin:490><just:right>%5<rmargin:560><just:right>%6', %cl.name, %clScore, %clKills, %clDeaths, %clInvos, %clBonus, %clStyle );
		}
		//else for observers, create an anchor around the player name so they can be observed
		else
		{
			messageClient( %client, 'SetLineHud', "", %tag, %index, '%7<tab:20, 450>\t<clip:200><a:gamelink\t%8>%1</a></clip><rmargin:280><just:right>%2<rmargin:350><just:right>%3<rmargin:420><just:right>%4<rmargin:490><just:right>%5<rmargin:560><just:right>%6', %cl.name, %clScore, %clKills, %clDeaths, %clInvos, %clBonus, %clStyle, %cl );
		}
	}

	// Tack on the list of observers:
	%observerCount = 0;
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%cl = ClientGroup.getObject(%i);
		if (%cl.team == 0)
			%observerCount++;
	}

	if (%observerCount > 0)
	{
		messageClient( %client, 'SetLineHud', "", %tag, %index, "");
		%index++;
		messageClient(%client, 'SetLineHud', "", %tag, %index, '<tab:10, 310><spush><font:Univers Condensed:22>\tOBSERVERS (%1)<rmargin:260><just:right>TIME<spop>', %observerCount);
		%index++;
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%cl = ClientGroup.getObject(%i);
			//if this is an observer
			if (%cl.team == 0)
			{
				%obsTime = getSimTime() - %cl.observerStartTime;
				%obsTimeStr = %game.formatTime(%obsTime, false);
				messageClient( %client, 'SetLineHud', "", %tag, %index, '<tab:20, 310>\t<clip:150>%1</clip><rmargin:260><just:right>%2', %cl.name, %obsTimeStr );
				%index++;
			}
		}
	}

	//clear the rest of Hud so we don't get old lines hanging around...
	messageClient( %client, 'ClearHud', "", %tag, %index );
}

function ShocklanceDMGame::sendDebriefing(%game, %client)
{
	// Mission result:
	%winner = $TeamRank[0, 0];
	if ( %winner.score > 0 )
		messageClient(%client, 'MsgDebriefResult', "", '<just:center>%1 wins!', $TeamRank[0, 0].name);
	else
		messageClient(%client, 'MsgDebriefResult', "", '<just:center>Nobody wins.');

	// Player scores:
	%count = $TeamRank[0, count];
	messageClient(%client, 'MsgDebriefAddLine', "", '<spush><color:00dc00><font:univers condensed:18>PLAYER<lmargin%%:40>SCORE<lmargin%%:55>KILLS<lmargin%%:70>DEATHS<lmargin%%:85>BONUS<spop>');
	for ( %i = 0; %i < %count; %i++ )
	{
		%cl = $TeamRank[0, %i];

		if ( %cl.score $= "" )
			%score = 0;
		else
			%score = %cl.score;

		if ( %cl.kills $= "" )
			%kills = 0;
		else
			%kills = %cl.kills;

		if ( %cl.deaths $= "" )
			%deaths = 0;
		else
			%deaths = %cl.deaths;

		%bonus =
			mFloatLength( %cl.leaderKills * %game.SCORE_PER_LEADERKILL, 0 ) +
			mFloatLength( %cl.rearshots * %game.SCORE_PER_REARSHOT, 0 ) +
			%cl.hitscore;

		messageClient(%client, 'MsgDebriefAddLine', "", '<lmargin:0><clip%%:40> %1</clip><lmargin%%:40><clip%%:15> %2</clip><lmargin%%:55><clip%%:15> %3<lmargin%%:70><clip%%:15> %4<lmargin%%:85><clip%%:15> %5', %cl.name, %score, %kills, %deaths, %bonus);
	}

	// list all observers
	%printedHeader = false;
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%cl = ClientGroup.getObject(%i);
		if (%cl.team <= 0)
		{
			// print the header only if we actually find an observer
			if (!%printedHeader)
			{
				%printedHeader = true;
				messageClient(%client, 'MsgDebriefAddLine', "", '\n<lmargin:0><spush><color:00dc00><font:univers condensed:18>OBSERVERS<lmargin%%:60>SCORE<spop>');
			}

			// print out the client info
			%score = %cl.score $= "" ? 0 : %cl.score;
			messageClient( %client, 'MsgDebriefAddLine', "", '<lmargin:0><clip%%:60> %1</clip><lmargin%%:60><clip%%:40> %2</clip>', %cl.name, %score);
		}
	}
}

function ShocklanceDMGame::onClientDamaged(%game, %clVictim, %clAttacker, %damageType, %implement, %damageLoc)
{
	%game.awardScoreHit();
	// record another hit by the attacker
	%clVictim.player.hitsBy[%clAttacker]++;

	// attacker's hit score increases by number of hits on that same player
	%clAttacker.hitscore += %clVictim.player.hitsBy[%clAttacker];

	if (%clVictim.player.hitsBy[%clAttacker] > 1)
		messageClient(%clAttacker, '', '\c2You receive a %1 point BONUS for hitting %2 again.~wfx/misc/coin.wav', %clVictim.player.hitsBy[%clAttacker], %clVictim.name);

	%game.recalcScore(%clAttacker);

	DefaultGame::onClientDamaged(%game, %clVictim, %clAttacker, %damageType, %implement, %damageLoc);
}


};
