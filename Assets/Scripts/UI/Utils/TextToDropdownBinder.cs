using TMPro;
using UnityEngine;

public class TextToDropdownBinder : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    public void UseDropdown()
    {
        if (!dropdown.IsExpanded)
        {
            dropdown.Show();
        }
        else
            dropdown.Hide();
    }
}
