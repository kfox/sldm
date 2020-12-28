package sldm_delete
{


function sldmDeleteObjectsFromGroupByType(%group, %type)
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
				sldmDeleteObjectsFromGroupByType(%obj, %type);

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

function sldmDeleteNonGameObjectsFromMap(%target)
{
	sldmDeleteObjectsFromGroupByType(MissionGroup, %target);
}


};
