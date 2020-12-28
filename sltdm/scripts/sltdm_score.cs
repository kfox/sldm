package sltdm_score
{


function SLTDMGame::recalcScore(%game, %client)
{
	if (%client <= 0)
		return;

	%killValue = %client.kills * %game.SCORE_PER_KILL;
	%deathValue = %client.deaths * %game.SCORE_PER_DEATH;
	%suicideValue = %client.suicides * %game.SCORE_PER_SUICIDE;
	%teamKillValue = %client.teamKills * %game.SCORE_PER_TEAMKILL;
	%leaderKillValue = %client.leaderKills * %game.SCORE_PER_LEADERKILL;

	if ($SLTDM::Pref::InventoryStationAccessPenalty > 3)
		%invoValue = %client.invos * %game.SCORE_PER_INVO;
	else
		%invoValue = 0;

	%client.score = %killValue + %deathValue + %suicideValue + %teamKillValue + %leaderKillValue;

	%game.recalcTeamRanks(%client);
	%game.checkScoreLimit(%client);
}

function SLTDMGame::recalcTeamRanks(%game, %client)
{
        DefaultGame::recalcTeamRanks(%game, %client);

	// set waypoints
	%game.leaderCheck(%client);
}

function SLTDMGame::recalcAllScores(%game)
{
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
		%game.recalcScore(ClientGroup.getObject(%i));
}

function SLTDMGame::updateKillScores(%game, %clVictim, %clKiller, %damageType, %implement)
{
	// verify victim was an enemy
	if (%game.testKill(%clVictim, %clKiller))
	{
		%leader = $SLTDM::Leader[%clVictim.team];
		%game.awardScoreKill(%clKiller);

		// award a bonus if the victim was the enemy leader
		if (%clVictim == %leader)
			%game.awardScoreLeaderKill(%clKiller);

		%game.awardScoreDeath(%clVictim);
	}
	else if (%game.testSuicide(%clVictim, %clKiller, %damageType)) // test for suicide
		%game.awardScoreSuicide(%clVictim);
	else if (%game.testTeamKill(%clVictim, %clKiller)) // test for teamkill
		%game.awardScoreTeamKill(%clVictim, %clKiller);
}

function SLTDMGame::updateInvoScores(%game, %client)
{
	%game.awardScoreInvo(%client);
}

function SLTDMGame::awardScoreDeath(%game, %victimID)
{
	%victimID.deaths++;
	%game.recalcScore(%victimID);
}

function SLTDMGame::awardScoreKill(%game, %killerID)
{
	%killerID.kills++;
	%team = %killerID.team;
	$TeamScore[%team] += %game.SCORE_PER_KILL;
	messageAll('MsgTeamScoreIs', "", %team, $TeamScore[%team]);
	%game.recalcScore(%killerID);
}

function SLTDMGame::awardScoreTeamKill(%game, %victimID, %killerID)
{
	%killerID.teamKills++;
	if (%game.SCORE_PER_TEAMKILL != 0)
		messageClient(%killerID, 'MsgScoreTeamkill', '\c0You have been penalized for killing teammate %1.', %victimID.name);
	%game.recalcScore(%killerID);
}

function SLTDMGame::awardScoreLeaderKill(%game, %killerID)
{
	%killerID.leaderKills++;
	%team = %killerID.team;
	%otherTeam = %killerID.team & 1 ? 2 : 1;
	messageAll('','\c2%1 gets a %2-point BONUS for killing the %3 leader!~wfx/misc/bounty_objRem1.wav', %killerID.name, %game.SCORE_PER_LEADERKILL, $TeamName[%otherTeam]);
	$TeamScore[%team] += %game.SCORE_PER_LEADERKILL;
	messageAll('MsgTeamScoreIs', "", %team, $TeamScore[%team]);
	%game.recalcScore(%killerID);
}

function SLTDMGame::awardScoreInvo(%game, %client)
{
	%client.invos++;
	%game.recalcScore(%client);
}

function SLTDMGame::resetScore(%game, %client)
{
	%client.kills = 0;
	%client.deaths = 0;
	%client.invos = 0;
	%client.suicides = 0;
	%client.leaderKills = 0;
	%client.score = 0;
}

function SLTDMGame::checkScoreLimit(%game, %client)
{
	// there's no score limit in SLTDM
}

function SLTDMGame::updateScoreHud(%game, %client, %tag)
{
	// clear the hud
	messageClient( %client, 'ClearHud', "", %tag, 0 );

	// send header
	messageClient( %client, 'SetScoreHudHeader', "", '<tab:15,315>\t%1<rmargin:260><just:right>%2<rmargin:560><just:left>\t%3<just:right>%4', $teamName[1], $teamScore[1], $teamName[2], $teamScore[2] );

	// send subheader
	messageClient( %client, 'SetScoreHudSubheader', "", '<tab:5,305>\tPLAYER (%1)<rmargin:210><just:right>SCORE<rmargin:270><just:right>KILLS<rmargin:510><just:left>\tPLAYER (%2)<just:right>SCORE<rmargin:570><just:right>KILLS', $TeamRank[1, count], $TeamRank[2, count] );

	// find out who on each team has the most kills
	%team1ClientMostKills = -1;
	%team1ClientMostKillsCount = -1;
	for (%i = 0; %i < $TeamRank[1, count]; %i++)
	{
		%cl = $TeamRank[1, %i];
		if (%cl.kills > %team1ClientMostKillsCount)
		{
			%team1ClientMostKillsCount = %cl.kills;
			%team1ClientMostKills = %cl;
		}
	}
	if (%team1ClientMostKillsCount <= 1)
		%team1ClientMostKills = -1;

	%team2ClientMostKills = -1;
	%team2ClientMostKillsCount = -1;
	for (%i = 0; %i < $TeamRank[2, count]; %i++)
	{
		%cl = $TeamRank[2, %i];
		if (%cl.kills > %team2ClientMostKillsCount)
		{
			%team2ClientMostKillsCount = %cl.kills;
			%team2ClientMostKills = %cl;
		}
	}
	if (%team2ClientMostKillsCount <= 1)
		%team2ClientMostKills = -1;

	%index = 0;
	while (true)
	{
		if (%index >= $TeamRank[1, count] && %index >= $TeamRank[2, count])
			break;

		// get the team1 client info
		%team1Client = "";
		%team1ClientScore = "";
		%team1ClientKills = "";
		%col1Style = "";
		if (%index < $TeamRank[1, count])
		{
			%team1Client = $TeamRank[1, %index];
			%team1ClientScore = %team1Client.score $= "" ? 0 : %team1Client.score;
			%team1ClientKills = %team1Client.kills;
			if (%team1ClientKills <= 0)
				%team1ClientKills = "0";
			if ( %team1Client == %team1ClientMostKills )
				%col1Style = "<color:00dc00>";
			else if ( %team1Client == %client )
				%col1Style = "<color:dcdcdc>";
		}

		// get the team2 client info
		%team2Client = "";
		%team2ClientScore = "";
		%team2ClientKills = "";
		%col2Style = "";
		if (%index < $TeamRank[2, count])
		{
			%team2Client = $TeamRank[2, %index];
			%team2ClientScore = %team2Client.score $= "" ? 0 : %team2Client.score;
			%team2ClientKills = %team2Client.kills;
			if (%team2ClientKills <= 0)
				%team2ClientKills = "0";
			if ( %team2Client == %team2ClientMostKills )
				%col2Style = "<color:00dc00>";
			else if ( %team2Client == %client )
				%col2Style = "<color:dcdcdc>";
		}

		// if the client is not an observer, send the message
		if (%client.team != 0)
		{
			messageClient( %client, 'SetLineHud', "", %tag, %index, '<tab:10, 310><spush>%7\t<clip:150>%1</clip><rmargin:200><just:right>%2<rmargin:260><just:right>%3<spop><rmargin:500><just:left>\t%8<clip:150>%4</clip><just:right>%5<rmargin:560><just:right>%6', %team1Client.name, %team1ClientScore, %team1ClientKills, %team2Client.name, %team2ClientScore, %team2ClientKills, %col1Style, %col2Style );
		}
		// else for observers, create an anchor around the
		// player name so they can be observed
		else
		{
			// this is lame, but we can only have up to %9 args
			if (%team2Client == %team2ClientMostKills)
			{
				messageClient( %client, 'SetLineHud', "", %tag, %index, '<tab:10, 310><spush>%7\t<clip:150><a:gamelink\t%8>%1</a></clip><rmargin:200><just:right>%2<rmargin:260><just:right>%3<spop><rmargin:500><just:left>\t<color:00dc00><clip:150><a:gamelink\t%9>%4</a></clip><just:right>%5<rmargin:560><just:right>%6', %team1Client.name, %team1ClientScore, %team1ClientKills, %team2Client.name, %team2ClientScore, %team2ClientKills, %col1Style, %team1Client, %team2Client );
			}
			else if (%team2Client == %client)
			{
				messageClient( %client, 'SetLineHud', "", %tag, %index, '<tab:10, 310><spush>%7\t<clip:150><a:gamelink\t%8>%1</a></clip><rmargin:200><just:right>%2<rmargin:260><just:right>%3<spop><rmargin:500><just:left>\t<color:dcdcdc><clip:150><a:gamelink\t%9>%4</a></clip><just:right>%5<rmargin:560><just:right>%6', %team1Client.name, %team1ClientScore, %team1ClientKills, %team2Client.name, %team2ClientScore, %team2ClientKills, %col1Style, %team1Client, %team2Client );
			}
			else
			{
			messageClient( %client, 'SetLineHud', "", %tag, %index, '<tab:10, 310><spush>%7\t<clip:150><a:gamelink\t%8>%1</a></clip><rmargin:200><just:right>%2<rmargin:260><just:right>%3<spop><rmargin:500><just:left>\t<clip:150><a:gamelink\t%9>%4</a></clip><just:right>%5<rmargin:560><just:right>%6', %team1Client.name, %team1ClientScore, %team1ClientKills, %team2Client.name, %team2ClientScore, %team2ClientKills, %col1Style, %team1Client, %team2Client );
			}
		}

		%index++;
	}

	// tack on the list of observers
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
			// if this is an observer
			if (%cl.team == 0)
			{
				%obsTime = getSimTime() - %cl.observerStartTime;
				%obsTimeStr = %game.formatTime(%obsTime, false);
				messageClient( %client, 'SetLineHud', "", %tag, %index, '<tab:20, 310>\t<clip:150>%1</clip><rmargin:260><just:right>%2', %cl.name, %obsTimeStr );
				%index++;
			}
		}
	}

	// clear the rest of Hud so we don't get old lines hanging around...
	messageClient( %client, 'ClearHud', "", %tag, %index );
}


};
