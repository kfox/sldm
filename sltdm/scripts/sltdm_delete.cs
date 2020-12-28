package sltdm_delete
{


function sltdmDeleteObjectsFromGroupByType(%group, %type)
{
	%noObjectsLeft = 0;

	while (%noObjectsLeft == 0)
	{
		%i = 0;
		%noObjectsLeft = 1;
		%loop = 1;
		%numObjects = %group.getCount();

		while ((%loop == 1) && (%i < %numObjects))
		{
			%obj = %group.getObject(%i);

			if (%obj.getClassName() $= "SimGroup")
				SLTDMGame::deleteObjectsFromGroupByType(%obj, %type);

			if (%obj.getClassName() $= %type)
			{
				%obj.delete();
				%loop = 0;
				%noObjectsLeft = 0;
			}
			else
				%i++;
		}
	}
}

function sltdmDeleteNonGameObjectsFromMap(%target)
{
	sltdmDeleteObjectsFromGroupByType(MissionGroup, %target);
}


};
