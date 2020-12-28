package sldm_ai
{


function ShocklanceDMGame::AIInit(%game)
{
	//call the default AIInit() function
	AIInit();
}

function ShocklanceDMGame::onAIRespawn(%game, %client)
{
	//add the default task
	if (! %client.defaultTasksAdded)
	{
		%client.defaultTasksAdded = true;
		%client.addTask(AIEngageTask);
		%client.addTask(AIPickupItemTask);
		%client.addTask(AITauntCorpseTask);
		%client.addTask(AIPatrolTask);
		%client.addTask(AIAttackPlayer);
	}

	//set the inv flag
	%client.spawnUseInv = false;
}


};
