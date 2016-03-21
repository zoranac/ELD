using UnityEngine;
using System.Collections;

public class EditableToggleGroup : System.Attribute
{
    bool RequiresToggleGroup = true;
    public EditableToggleGroup(bool CanEdit)
    {
        RequiresToggleGroup = CanEdit;
    }
}
