package sltdm_ai
{


function SLTDMGame::AIInit(%game)
{
	//call the default AIInit() function
	AIInit();
}

function SLTDMGame::onAIRespawn(%game, %client)
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
