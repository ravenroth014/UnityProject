//  MenuItemUI.cs
//  By Atid Puwatnuttasit

using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class MenuItemUI : MonoBehaviour
{
    #region Public Properties

    public MenuScene MenuScene => _MenuType;                                    // The menu scene destination for this menu option.
    public List<MenuScene> MenuSceneList => _MenuSceneList;                     // The menu scene destination list for this menu option list.
    public int HorizontalIconCount => _SelectIconHorizontalList.Count;          // Horizontal option count.

    #endregion

    #region Inspector Properties

    [Header("UI Elements")]
    [SerializeField] private GameObject _SelectIcon;
    [SerializeField] private GameObject _DecreaseAdjustment;
    [SerializeField] private GameObject _IncreaseAdjustment;
    [SerializeField] private TextMeshPro _ValueText;
    [SerializeField] private MenuScene _MenuType;

    [SerializeField] private List<GameObject> _SelectIconHorizontalList;
    [SerializeField] private List<MenuScene> _MenuSceneList;

    #endregion

    #region Methods

    /// <summary>
    /// Call this method to set active on selected option.
    /// </summary>
    /// <param name="caller">UI Manager</param>
    public void OnSelect(object caller)
    {
        if (caller.GetType() != typeof(UIManager)) return;
        if (_SelectIcon != null) _SelectIcon.SetActive(true);
    }

    /// <summary>
    /// Call this method to set inactive on selected option.
    /// </summary>
    /// <param name="caller">UI Manager</param>
    public void OnDeSelect(object caller)
    {
        if (caller.GetType() != typeof(UIManager)) return;
        if (_SelectIcon != null) _SelectIcon.SetActive(false);
    }

    /// <summary>
    /// Call this method to set state on decrease arrow.
    /// </summary>
    /// <param name="caller">UI Manager</param>
    /// <param name="value">State value</param>
    public void SetDecreaseState(object caller, bool value)
    {
        if (caller.GetType() != typeof(UIManager)) return;
        _DecreaseAdjustment.SetActive(value);
    }

    /// <summary>
    /// Call this method to set state on increase arrow.
    /// </summary>
    /// <param name="caller">UI Manager</param>
    /// <param name="value">State value</param>
    public void SetIncreaseState(object caller, bool value)
    {
        if (caller.GetType() != typeof(UIManager)) return;
        _IncreaseAdjustment.SetActive(value);
    }

    /// <summary>
    /// Call this method to update value text of that option.
    /// </summary>
    /// <param name="caller">UI Manager</param>
    /// <param name="text">Text value</param>
    public void UpdateValueText(object caller, string text)
    {
        if (caller.GetType() != typeof(UIManager)) return;
        _ValueText.text = text;
    }

    /// <summary>
    /// Call this method to update arrow on horizontal selection.
    /// </summary>
    /// <param name="caller">UI Manager</param>
    /// <param name="index">Arrow index</param>
    public void UpdateHorizontalSelection(object caller, int index)
    {
        for (int i = 0; i < HorizontalIconCount; i++)
        {
            if(index == -1)
                _SelectIconHorizontalList[i].SetActive(false);
            else
                _SelectIconHorizontalList[i].SetActive(i == index);
        }
    }

    /// <summary>
    /// Call this method to set text active state.
    /// </summary>
    /// <param name="caller">UI Manager</param>
    /// <param name="state">Active state</param>
    public void SetTextActiveState(object caller, bool state)
    {
        _ValueText.gameObject.SetActive(state);
    }

    #endregion
}
