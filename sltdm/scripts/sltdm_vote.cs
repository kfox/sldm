package sltdm_vote
{


function DefaultGame::sendGameVoteMenu(%game, %client, %key)
{
	Parent::sendGameVoteMenu(%game, %client, %key);

	if (%game.scheduleVote $= "")
	{

		// if invos are removed, don't bother with these options
		if ($SLTDM::Pref::InventoryStationAccess != 8)
		{

			//
			// inventory station access
			//

			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::InventoryStationAccess & 2))
			{
				if ($SLTDM::Pref::InventoryStationAccess > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationAccess', 'disable inventory station access', 'Disable Inventory Station Access');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationAccess', 'enable inventory station access', 'Enable Inventory Station Access');
			}
			else if ($SLTDM::Pref::InventoryStationAccess & 1)
			{
				if ($SLTDM::Pref::InventoryStationAccess > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationAccess', 'disable inventory station access', 'Vote to Disable Inventory Station Access');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationAccess', 'enable inventory station access', 'Vote to Enable Inventory Station Access');
			}

			//
			// inventory station teleportation
			//

			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::InventoryStationTeleportation & 2))
			{
				if ($SLTDM::Pref::InventoryStationTeleportation > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationTeleportation', 'disable inventory station teleportation', 'Disable Inventory Station Teleportation');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationTeleportation', 'enable inventory station teleportation', 'Enable Inventory Station Teleportation');
			}
			else if ($SLTDM::Pref::InventoryStationTeleportation & 1)
			{
				if ($SLTDM::Pref::InventoryStationTeleportation > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationTeleportation', 'disable inventory station teleportation', 'Vote to Disable Inventory Station Teleportation');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationTeleportation', 'enable inventory station teleportation', 'Vote to Enable Inventory Station Teleportation');
			}

			// only show these options if invo access
			// or teleportation are enabled
			if ($SLTDM::Pref::InventoryStationAccess > 3 || $SLTDM::Pref::InventoryStationTeleportation > 3)
			{

				//
				// inventory station penalty
				//

				if (%client.isSuperAdmin ||
					(%client.isAdmin && $SLTDM::Pref::InventoryStationAccessPenalty & 2))
				{
					if ($SLTDM::Pref::InventoryStationAccessPenalty > 3)
						messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationAccessPenalty', 'disable the inventory station access penalty', 'Disable the Inventory Station Access Penalty');
					else
						messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationAccessPenalty', 'enable the inventory station access penalty', 'Enable the Inventory Station Access Penalty');
				}
				else if ($SLTDM::Pref::InventoryStationAccessPenalty & 1)
				{
					if ($SLTDM::Pref::InventoryStationAccessPenalty > 3)
						messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationAccessPenalty', 'disable the inventory station access penalty', 'Vote to Disable the Inventory Station Access Penalty');
					else
						messageClient(%client, 'MsgVoteItem', "", %key, 'VoteStationAccessPenalty', 'enable the inventory station access penalty', 'Vote to Enable the Inventory Station Access Penalty');
				}
			}

		}

		// only show these if players have a chance
		// of being able to select the grenades
		if (($SLTDM::Pref::InventoryStationAccess > 3 && $SLTDM::Pref::InventoryStationAccess < 8) || ($SLTDM::Pref::SpawnWithFaves > 3))
		{

			//
			// whiteout grenades
			//

			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::AllowWhiteoutGrenades & 2))
			{
				if ($SLTDM::Pref::AllowWhiteoutGrenades > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteWhiteoutGrenades', 'disallow whiteout grenades', 'Disallow Whiteout Grenades');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteWhiteoutGrenades', 'allow whiteout grenades', 'Allow Whiteout Grenades');
			}
			else if ($SLTDM::Pref::AllowWhiteoutGrenades & 1)
			{
				if ($SLTDM::Pref::AllowWhiteoutGrenades > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteWhiteoutGrenades', 'disallow whiteout grenades', 'Vote to Disallow Whiteout Grenades');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteWhiteoutGrenades', 'allow whiteout grenades', 'Vote to Allow Whiteout Grenades');
			}

			//
			// concussion grenades
			//

			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::AllowConcussionGrenades & 2))
			{
				if ($SLTDM::Pref::AllowConcussionGrenades > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteConcussionGrenades', 'disallow concussion grenades', 'Disallow Concussion Grenades');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteConcussionGrenades', 'allow concussion grenades', 'Allow Concussion Grenades');
			}
			else if ($SLTDM::Pref::AllowConcussionGrenades & 1)
			{
				if ($SLTDM::Pref::AllowConcussionGrenades > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteConcussionGrenades', 'disallow concussion grenades', 'Vote to Disallow Concussion Grenades');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteConcussionGrenades', 'allow concussion grenades', 'Vote to Allow Concussion Grenades');
			}

			//
			// normal grenades
			//

			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::AllowNormalGrenades & 2))
			{
				if ($SLTDM::Pref::AllowNormalGrenades > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteNormalGrenades', 'disallow normal grenades', 'Disallow Normal Grenades');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteNormalGrenades', 'allow normal grenades', 'Allow Normal Grenades');
			}
			else if ($SLTDM::Pref::AllowNormalGrenades & 1)
			{
				if ($SLTDM::Pref::AllowNormalGrenades > 3)
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteNormalGrenades', 'disallow normal grenades', 'Vote to Disallow Normal Grenades');
				else
					messageClient(%client, 'MsgVoteItem', "", %key, 'VoteNormalGrenades', 'allow normal grenades', 'Vote to Allow Normal Grenades');
			}

		}

		//
		// spawn loadout option
		//

		if (%client.isSuperAdmin ||
			(%client.isAdmin && $SLTDM::Pref::SpawnWithFaves & 2))
		{
			if ($SLTDM::Pref::SpawnWithFaves > 3)
				messageClient(%client, 'MsgVoteItem', "", %key, 'VoteSpawnWithFaves', 'make players spawn using default loadout', 'Spawn Players Using Default Loadout');
			else
				messageClient(%client, 'MsgVoteItem', "", %key, 'VoteSpawnWithFaves', 'make players spawn using selected favorite', 'Spawn Players Using Favorites');
		}
		else if ($SLTDM::Pref::SpawnWithFaves & 1)
		{
			if ($SLTDM::Pref::SpawnWithFaves > 3)
				messageClient(%client, 'MsgVoteItem', "", %key, 'VoteSpawnWithFaves', 'make players spawn using default loadout', 'Vote to Spawn Players Using Default Loadout');
			else
				messageClient(%client, 'MsgVoteItem', "", %key, 'VoteSpawnWithFaves', 'make players spawn using selected favorite', 'Vote to Spawn Players Using Favorites');
		}

	}
}

function serverCmdStartNewVote(%client, %typeName, %arg1, %arg2, %arg3, %arg4, %playerVote)
{
	Parent::serverCmdStartNewVote(%client, %typeName, %arg1, %arg2, %arg3, %arg4, %playerVote);

	switch$ (%typeName)
	{

		case "VoteStationAccess":
			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::InventoryStationAccess & 2))
			{
				SLTDMGame::adminForceCommand(%client, %typename, %arg1, %arg2, %arg3, %arg4);
			}
			else if ($SLTDM::Pref::InventoryStationAccess & 1)
			{
				if (Game.scheduleVote !$= "")
				{
					messageClient(%client, 'voteAlreadyRunning', '\c2A vote is already in progress.');
					return;
				}
				%actionMsg = $SLTDM::Pref::InventoryStationAccess > 3 ? "disable inventory station access" : "enable inventory station access";
				for (%idx = 0; %idx < ClientGroup.getCount(); %idx++)
				{
					%cl = ClientGroup.getObject(%idx);
					if (!%cl.isAIControlled())
					{
						messageClient( %cl, 'VoteStarted', '\c2%1 initiated a vote to %2.', %client.name, %actionMsg);
						%clientsVoting++;
					}
				}
				Game.playerVote(%client, %typename, %arg1, %arg2, %arg3, %arg4, %clientsVoting);
			}
			else
			{
				return;
			}

		case "VoteStationAccessPenalty":
			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::InventoryStationAccessPenalty & 2))
			{
				SLTDMGame::adminForceCommand(%client, %typename, %arg1, %arg2, %arg3, %arg4);
			}
			else if ($SLTDM::Pref::InventoryStationAccessPenalty & 1)
			{
				if (Game.scheduleVote !$= "")
				{
					messageClient(%client, 'voteAlreadyRunning', '\c2A vote is already in progress.');
					return;
				}
				%actionMsg = $SLTDM::Pref::InventoryStationAccessPenalty > 3 ? "disable the penalty for inventory station access" : "enable the penalty for inventory station access";
				for (%idx = 0; %idx < ClientGroup.getCount(); %idx++)
				{
					%cl = ClientGroup.getObject(%idx);
					if (!%cl.isAIControlled())
					{
						messageClient( %cl, 'VoteStarted', '\c2%1 initiated a vote to %2.', %client.name, %actionMsg);
						%clientsVoting++;
					}
				}
				Game.playerVote(%client, %typename, %arg1, %arg2, %arg3, %arg4, %clientsVoting);
			}
			else
			{
				return;
			}

		case "VoteStationTeleportation":
			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::InventoryStationTeleportation & 2))
			{
				SLTDMGame::adminForceCommand(%client, %typename, %arg1, %arg2, %arg3, %arg4);
			}
			else if ($SLTDM::Pref::InventoryStationTeleportation & 1)
			{
				if (Game.scheduleVote !$= "")
				{
					messageClient(%client, 'voteAlreadyRunning', '\c2A vote is already in progress.');
					return;
				}
				%actionMsg = $SLTDM::Pref::InventoryStationTeleportation > 3 ? "disable inventory station teleportation" : "enable inventory station teleportation";
				for (%idx = 0; %idx < ClientGroup.getCount(); %idx++)
				{
					%cl = ClientGroup.getObject(%idx);
					if (!%cl.isAIControlled())
					{
						messageClient( %cl, 'VoteStarted', '\c2%1 initiated a vote to %2.', %client.name, %actionMsg);
						%clientsVoting++;
					}
				}
				Game.playerVote(%client, %typename, %arg1, %arg2, %arg3, %arg4, %clientsVoting);
			}
			else
			{
				return;
			}

		case "VoteWhiteoutGrenades":
			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::AllowWhiteoutGrenades & 2))
			{
				SLTDMGame::adminForceCommand(%client, %typename, %arg1, %arg2, %arg3, %arg4);
			}
			else if ($SLTDM::Pref::AllowWhiteoutGrenades & 1)
			{
				if (Game.scheduleVote !$= "")
				{
					messageClient(%client, 'voteAlreadyRunning', '\c2A vote is already in progress.');
					return;
				}
				%actionMsg = $SLTDM::Pref::AllowWhiteoutGrenades > 3 ? "disallow whiteout grenades" : "allow whiteout grenades";
				for (%idx = 0; %idx < ClientGroup.getCount(); %idx++)
				{
					%cl = ClientGroup.getObject(%idx);
					if (!%cl.isAIControlled())
					{
						messageClient( %cl, 'VoteStarted', '\c2%1 initiated a vote to %2.', %client.name, %actionMsg);
						%clientsVoting++;
					}
				}
				Game.playerVote(%client, %typename, %arg1, %arg2, %arg3, %arg4, %clientsVoting);
			}
			else
			{
				return;
			}

		case "VoteConcussionGrenades":
			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::AllowConcussionGrenades & 2))
			{
				SLTDMGame::adminForceCommand(%client, %typename, %arg1, %arg2, %arg3, %arg4);
			}
			else if ($SLTDM::Pref::AllowConcussionGrenades & 1)
			{
				if (Game.scheduleVote !$= "")
				{
					messageClient(%client, 'voteAlreadyRunning', '\c2A vote is already in progress.');
					return;
				}
				%actionMsg = $SLTDM::Pref::AllowConcussionGrenades > 3 ? "disallow concussion grenades" : "allow concussion grenades";
				for (%idx = 0; %idx < ClientGroup.getCount(); %idx++)
				{
					%cl = ClientGroup.getObject(%idx);
					if (!%cl.isAIControlled())
					{
						messageClient( %cl, 'VoteStarted', '\c2%1 initiated a vote to %2.', %client.name, %actionMsg);
						%clientsVoting++;
					}
				}
				Game.playerVote(%client, %typename, %arg1, %arg2, %arg3, %arg4, %clientsVoting);
			}
			else
			{
				return;
			}

		case "VoteNormalGrenades":
			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::AllowNormalGrenades & 2))
			{
				SLTDMGame::adminForceCommand(%client, %typename, %arg1, %arg2, %arg3, %arg4);
			}
			else if ($SLTDM::Pref::AllowNormalGrenades & 1)
			{
				if (Game.scheduleVote !$= "")
				{
					messageClient(%client, 'voteAlreadyRunning', '\c2A vote is already in progress.');
					return;
				}
				%actionMsg = $SLTDM::Pref::AllowNormalGrenades > 3 ? "disallow normal grenades" : "allow normal grenades";
				for (%idx = 0; %idx < ClientGroup.getCount(); %idx++)
				{
					%cl = ClientGroup.getObject(%idx);
					if (!%cl.isAIControlled())
					{
						messageClient( %cl, 'VoteStarted', '\c2%1 initiated a vote to %2.', %client.name, %actionMsg);
						%clientsVoting++;
					}
				}
				Game.playerVote(%client, %typename, %arg1, %arg2, %arg3, %arg4, %clientsVoting);
			}
			else
			{
				return;
			}

		case "VoteSpawnWithFaves":
			if (%client.isSuperAdmin ||
				(%client.isAdmin && $SLTDM::Pref::SpawnWithFaves & 2))
			{
				SLTDMGame::adminForceCommand(%client, %typename, %arg1, %arg2, %arg3, %arg4);
			}
			else if ($SLTDM::Pref::SpawnWithFaves & 1)
			{
				if (Game.scheduleVote !$= "")
				{
					messageClient(%client, 'voteAlreadyRunning', '\c2A vote is already in progress.');
					return;
				}
				%actionMsg = $SLTDM::Pref::SpawnWithFaves > 3 ? "make players spawn using default loadout" : "make players spawn using selected favorite";
				for (%idx = 0; %idx < ClientGroup.getCount(); %idx++)
				{
					%cl = ClientGroup.getObject(%idx);
					if (!%cl.isAIControlled())
					{
						messageClient( %cl, 'VoteStarted', '\c2%1 initiated a vote to %2.', %client.name, %actionMsg);
						%clientsVoting++;
					}
				}
				Game.playerVote(%client, %typename, %arg1, %arg2, %arg3, %arg4, %clientsVoting);
			}
			else
			{
				return;
			}

	}
}

function SLTDMGame::playerVote(%game, %client, %typename, %arg1, %arg2, %arg3, %arg4, %clientsVoting, %teamSpecific)
{
	// open the vote hud for all clients that will participate in this vote
	if (%teamSpecific)
	{
		for (%clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++)
		{
			%cl = ClientGroup.getObject(%clientIndex);

			if(%cl.team == %client.team && !%cl.isAIControlled())
				messageClient(%cl, 'openVoteHud', "", %clientsVoting, ($Host::VotePassPercent / 100));
		}
	}
	else
	{
		for (%clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++)
		{
			%cl = ClientGroup.getObject(%clientIndex);
			if (!%cl.isAIControlled())
				messageClient(%cl, 'openVoteHud', "", %clientsVoting, ($Host::VotePassPercent / 100));
		}
	}

	clearVotes();
	Game.voteType = %typeName;
	Game.scheduleVote = schedule(($Host::VoteTime * 1000), 0, "calcVotes", %typeName, %arg1, %arg2, %arg3, %arg4);
	%client.vote = true;
	messageAll('addYesVote', "");

	if(!%client.team == 0) clearBottomPrint(%client);

	%client.canVote = false;
	%client.rescheduleVote = schedule(($Host::voteSpread * 1000) + ($Host::voteTime * 1000) , 0, "resetVotePrivs", %client);
}

function SLTDMGame::adminForceCommand(%client, %typename, %arg1, %arg2, %arg3, %arg4)
{
	if (!%client.isAdmin) return;

	if (Game.scheduleVote !$= "" && Game.voteType $= %typeName)
	{
		messageAll('closeVoteHud', "");
		cancel(Game.scheduleVote);
		Game.scheduleVote = "";
	}

	eval("Game." @ %typeName @ "(true,\"" @ %arg1 @ "\",\"" @ %arg2 @ "\",\"" @ %arg3 @ "\",\"" @ %arg4 @ "\");");
}

function SLTDMGame::voteStationAccess(%game, %admin, %client)
{
	%setto = "";
	%cause = "";

	if (%admin)
	{
		if ($SLTDM::Pref::InventoryStationAccess > 3)
		{
			messageAll('MsgAdminForce', '\c2The Admin has disabled inventory station access.');
			$SLTDM::Pref::InventoryStationAccess -= 4;
			%setto = "disabled";
		}
		else
		{
			messageAll('MsgAdminForce', '\c2The Admin has enabled inventory station access.');
			$SLTDM::Pref::InventoryStationAccess += 4;
			%setto = "enabled";
		}
	}
	else
	{
		%totalVotes = %game.totalVotesFor + %game.totalVotesAgainst;
		if (%totalVotes > 0 && (%game.totalVotesFor / (ClientGroup.getCount() - $HostGameBotCount)) > ($Host::VotePasspercent / 100))
		{
			if ($SLTDM::Pref::InventoryStationAccess > 3)
			{
				messageAll('MsgVotePassed', '\c2Inventory station access was disabled by vote.');
				$SLTDM::Pref::InventoryStationAccess -= 4;
				%setto = "disabled";
			}
			else
			{
				messageAll('MsgVotePassed', '\c2Inventory station access was enabled by vote.');
				$SLTDM::Pref::InventoryStationAccess += 4;
				%setto = "enabled";
			}
		}
		else
		{
			if ($SLTDM::Pref::InventoryStationAccess > 3)
				messageAll('MsgVoteFailed', '\c2Disable inventory station access vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
			else
				messageAll('MsgVoteFailed', '\c2Enable inventory station access vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
		}
	}

	if (%admin)
		%cause = "(admin)";
	else
		%cause = "(vote)";

	if(%setto !$= "")
		logEcho("inventory station access "@%setto SPC %cause);
}

function SLTDMGame::voteStationAccessPenalty(%game, %admin, %client)
{
	%setto = "";
	%cause = "";

	if (%admin)
	{
		if ($SLTDM::Pref::InventoryStationAccessPenalty > 3)
		{
			messageAll('MsgAdminForce', '\c2The Admin has disabled the inventory station access penalty.');
			$SLTDM::Pref::InventoryStationAccessPenalty -= 4;
			%game.recalcAllScores();
			%setto = "disabled";
		}
		else
		{
			messageAll('MsgAdminForce', '\c2The Admin has enabled the inventory station access penalty.');
			$SLTDM::Pref::InventoryStationAccessPenalty += 4;
			%game.recalcAllScores();
			%setto = "enabled";
		}
	}
	else
	{
		%totalVotes = %game.totalVotesFor + %game.totalVotesAgainst;
		if (%totalVotes > 0 && (%game.totalVotesFor / (ClientGroup.getCount() - $HostGameBotCount)) > ($Host::VotePasspercent / 100))
		{
			if ($SLTDM::Pref::InventoryStationAccessPenalty > 3)
			{
				messageAll('MsgVotePassed', '\c2Inventory station access penalty was disabled by vote.');
				$SLTDM::Pref::InventoryStationAccessPenalty -= 4;
				%game.recalcAllScores();
				%setto = "disabled";
			}
			else
			{
				messageAll('MsgVotePassed', '\c2Inventory station access penalty was enabled by vote.');
				$SLTDM::Pref::InventoryStationAccessPenalty += 4;
				%game.recalcAllScores();
				%setto = "enabled";
			}
		}
		else
		{
			if ($SLTDM::Pref::InventoryStationAccessPenalty > 3)
				messageAll('MsgVoteFailed', '\c2Disable inventory station access penalty vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
			else
				messageAll('MsgVoteFailed', '\c2Enable inventory station access penalty vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
		}
	}

	if (%admin)
		%cause = "(admin)";
	else
		%cause = "(vote)";

	if(%setto !$= "")
		logEcho("inventory station access penalty "@%setto SPC %cause);
}

function SLTDMGame::voteStationTeleportation(%game, %admin, %client)
{
	%setto = "";
	%cause = "";

	if (%admin)
	{
		if ($SLTDM::Pref::InventoryStationTeleportation > 3)
		{
			messageAll('MsgAdminForce', '\c2The Admin has disabled inventory station teleportation.');
			$SLTDM::Pref::InventoryStationTeleportation -= 4;
			%setto = "disabled";
		}
		else
		{
			messageAll('MsgAdminForce', '\c2The Admin has enabled inventory station teleportation.');
			$SLTDM::Pref::InventoryStationTeleportation += 4;
			%setto = "enabled";
		}
	}
	else
	{
		%totalVotes = %game.totalVotesFor + %game.totalVotesAgainst;
		if (%totalVotes > 0 && (%game.totalVotesFor / (ClientGroup.getCount() - $HostGameBotCount)) > ($Host::VotePasspercent / 100))
		{
			if ($SLTDM::Pref::InventoryStationTeleportation > 3)
			{
				messageAll('MsgVotePassed', '\c2Inventory station teleportation was disabled by vote.');
				$SLTDM::Pref::InventoryStationTeleportation -= 4;
				%setto = "disabled";
			}
			else
			{
				messageAll('MsgVotePassed', '\c2Inventory station teleportation was enabled by vote.');
				$SLTDM::Pref::InventoryStationTeleportation += 4;
				%setto = "enabled";
			}
		}
		else
		{
			if ($SLTDM::Pref::InventoryStationTeleportation > 3)
				messageAll('MsgVoteFailed', '\c2Disable inventory station teleportation vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
			else
				messageAll('MsgVoteFailed', '\c2Enable inventory station teleportation vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
		}
	}

	if (%admin)
		%cause = "(admin)";
	else
		%cause = "(vote)";

	if(%setto !$= "")
		logEcho("inventory station teleportation "@%setto SPC %cause);
}

function SLTDMGame::voteWhiteoutGrenades(%game, %admin, %client)
{
	%setto = "";
	%cause = "";

	if (%admin)
	{
		if ($SLTDM::Pref::AllowWhiteoutGrenades > 3)
		{
			messageAll('MsgAdminForce', '\c2The Admin has disallowed whiteout grenades.');
			$SLTDM::Pref::AllowWhiteoutGrenades -= 4;
			$InvBanList[SLTDM, "FlashGrenade"] = 1;
			%setto = "disallowed";
		}
		else
		{
			messageAll('MsgAdminForce', '\c2The Admin has allowed whiteout grenades.');
			$SLTDM::Pref::AllowWhiteoutGrenades += 4;
			$InvBanList[SLTDM, "FlashGrenade"] = 0;
			%setto = "allowed";
		}
	}
	else
	{
		%totalVotes = %game.totalVotesFor + %game.totalVotesAgainst;
		if (%totalVotes > 0 && (%game.totalVotesFor / (ClientGroup.getCount() - $HostGameBotCount)) > ($Host::VotePasspercent / 100))
		{
			if ($SLTDM::Pref::AllowWhiteoutGrenades > 3)
			{
				messageAll('MsgVotePassed', '\c2Whiteout grenades were disallowed by vote.');
				$SLTDM::Pref::AllowWhiteoutGrenades -= 4;
				$InvBanList[SLTDM, "FlashGrenade"] = 1;
				%setto = "disallowed";
			}
			else
			{
				messageAll('MsgVotePassed', '\c2Whiteout grenades were allowed by vote.');
				$SLTDM::Pref::AllowWhiteoutGrenades += 4;
				$InvBanList[SLTDM, "FlashGrenade"] = 0;
				%setto = "allowed";
			}
		}
		else
		{
			if ($SLTDM::Pref::AllowWhiteoutGrenades > 3)
				messageAll('MsgVoteFailed', '\c2Disallow whiteout grenades vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
			else
				messageAll('MsgVoteFailed', '\c2Allow whiteout grenades vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
		}
	}

	if (%admin)
		%cause = "(admin)";
	else
		%cause = "(vote)";

	if(%setto !$= "")
		logEcho("whiteout grenades "@%setto SPC %cause);
}

function SLTDMGame::voteConcussionGrenades(%game, %admin, %client)
{
	%setto = "";
	%cause = "";

	if (%admin)
	{
		if ($SLTDM::Pref::AllowConcussionGrenades > 3)
		{
			messageAll('MsgAdminForce', '\c2The Admin has disallowed concussion grenades.');
			$SLTDM::Pref::AllowConcussionGrenades -= 4;
			$InvBanList[SLTDM, "ConcussionGrenade"] = 1;
			%setto = "disallowed";
		}
		else
		{
			messageAll('MsgAdminForce', '\c2The Admin has allowed concussion grenades.');
			$SLTDM::Pref::AllowConcussionGrenades += 4;
			$InvBanList[SLTDM, "ConcussionGrenade"] = 0;
			%setto = "allowed";
		}
	}
	else
	{
		%totalVotes = %game.totalVotesFor + %game.totalVotesAgainst;
		if (%totalVotes > 0 && (%game.totalVotesFor / (ClientGroup.getCount() - $HostGameBotCount)) > ($Host::VotePasspercent / 100))
		{
			if ($SLTDM::Pref::AllowConcussionGrenades > 3)
			{
				messageAll('MsgVotePassed', '\c2Concussion grenades were disallowed by vote.');
				$SLTDM::Pref::AllowConcussionGrenades -= 4;
				$InvBanList[SLTDM, "ConcussionGrenade"] = 1;
				%setto = "disallowed";
			}
			else
			{
				messageAll('MsgVotePassed', '\c2Concussion grenades were allowed by vote.');
				$SLTDM::Pref::AllowConcussionGrenades += 4;
				$InvBanList[SLTDM, "ConcussionGrenade"] = 0;
				%setto = "allowed";
			}
		}
		else
		{
			if ($SLTDM::Pref::AllowConcussionGrenades > 3)
				messageAll('MsgVoteFailed', '\c2Disallow concussion grenades vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
			else
				messageAll('MsgVoteFailed', '\c2Allow concussion grenades vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
		}
	}

	if (%admin)
		%cause = "(admin)";
	else
		%cause = "(vote)";

	if(%setto !$= "")
		logEcho("concussion grenades "@%setto SPC %cause);
}

function SLTDMGame::voteNormalGrenades(%game, %admin, %client)
{
	%setto = "";
	%cause = "";

	if (%admin)
	{
		if ($SLTDM::Pref::AllowNormalGrenades > 3)
		{
			messageAll('MsgAdminForce', '\c2The Admin has disallowed normal grenades.');
			$SLTDM::Pref::AllowNormalGrenades -= 4;
			$InvBanList[SLTDM, "Grenade"] = 1;
			%setto = "disallowed";
		}
		else
		{
			messageAll('MsgAdminForce', '\c2The Admin has allowed normal grenades.');
			$SLTDM::Pref::AllowNormalGrenades += 4;
			$InvBanList[SLTDM, "Grenade"] = 0;
			%setto = "allowed";
		}
	}
	else
	{
		%totalVotes = %game.totalVotesFor + %game.totalVotesAgainst;
		if (%totalVotes > 0 && (%game.totalVotesFor / (ClientGroup.getCount() - $HostGameBotCount)) > ($Host::VotePasspercent / 100))
		{
			if ($SLTDM::Pref::AllowNormalGrenades > 3)
			{
				messageAll('MsgVotePassed', '\c2Normal grenades were disallowed by vote.');
				$SLTDM::Pref::AllowNormalGrenades -= 4;
				$InvBanList[SLTDM, "Grenade"] = 1;
				%setto = "disallowed";
			}
			else
			{
				messageAll('MsgVotePassed', '\c2Normal grenades were allowed by vote.');
				$SLTDM::Pref::AllowNormalGrenades += 4;
				$InvBanList[SLTDM, "Grenade"] = 0;
				%setto = "allowed";
			}
		}
		else
		{
			if ($SLTDM::Pref::AllowNormalGrenades > 3)
				messageAll('MsgVoteFailed', '\c2Disallow normal grenades vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
			else
				messageAll('MsgVoteFailed', '\c2Allow normal grenades vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
		}
	}

	if (%admin)
		%cause = "(admin)";
	else
		%cause = "(vote)";

	if(%setto !$= "")
		logEcho("normal grenades "@%setto SPC %cause);
}

function SLTDMGame::voteSpawnWithFaves(%game, %admin, %client)
{
	%setto = "";
	%cause = "";

	if (%admin)
	{
		if ($SLTDM::Pref::SpawnWithFaves > 3)
		{
			messageAll('MsgAdminForce', '\c2The Admin has forced players to spawn using the default loadout.');
			$SLTDM::Pref::SpawnWithFaves -= 4;
			%setto = "disallowed";
		}
		else
		{
			messageAll('MsgAdminForce', '\c2The Admin has forced players to spawn using their selected favorite.');
			$SLTDM::Pref::SpawnWithFaves += 4;
			%setto = "allowed";
		}
	}
	else
	{
		%totalVotes = %game.totalVotesFor + %game.totalVotesAgainst;
		if (%totalVotes > 0 && (%game.totalVotesFor / (ClientGroup.getCount() - $HostGameBotCount)) > ($Host::VotePasspercent / 100))
		{
			if ($SLTDM::Pref::SpawnWithFaves > 3)
			{
				messageAll('MsgVotePassed', '\c2Spawning with default loadout was enabled by vote.');
				$SLTDM::Pref::SpawnWithFaves -= 4;
				%setto = "disallowed";
			}
			else
			{
				messageAll('MsgVotePassed', '\c2Spawning with selected favorite was enabled by vote.');
				$SLTDM::Pref::SpawnWithFaves += 4;
				%setto = "allowed";
			}
		}
		else
		{
			if ($SLTDM::Pref::SpawnWithFaves > 3)
				messageAll('MsgVoteFailed', '\c2Spawning with default loadout vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
			else
				messageAll('MsgVoteFailed', '\c2Spawning with selected favorite vote did not pass: %1 percent.', mFloor(%game.totalVotesFor/(ClientGroup.getCount() - $HostGameBotCount) * 100));
		}
	}

	if (%admin)
		%cause = "(admin)";
	else
		%cause = "(vote)";

	if(%setto !$= "")
		logEcho("spawning with faves "@%setto SPC %cause);
}


};
