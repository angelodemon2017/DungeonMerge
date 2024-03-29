using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cursor : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _textIcon;

    public static HeroField fromField;
    public static int fromIndex;

    public void UpdateCursor(HeroData hero, int indexCell, HeroField field)
    {
        if (hero.IsEmpty)
            return;

        gameObject.SetActive(true);
        _icon.color = hero.HeroColor;
        _textIcon.text = $"{hero.Level}L";
        fromIndex = indexCell;
        fromField = field;
    }

    public void HideCursor()
    {
        Debug.Log("HideCursor");
        gameObject.SetActive(false);
    }

    private void OnMouseUp()
    {
        Debug.Log("Cursor mouse up");
    }
}