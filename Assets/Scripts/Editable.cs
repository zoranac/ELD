using UnityEngine;
using System.Collections;

public class Editable : System.Attribute 
{
	public bool editable;
	public bool RequiresToggleGroup = false;
	public Editable(bool CanEdit)
	{
		editable = CanEdit;
	}
	public Editable(bool CanEdit,bool requiresToggleGroup)
	{
		editable = CanEdit;
		RequiresToggleGroup = requiresToggleGroup;
	}
}
