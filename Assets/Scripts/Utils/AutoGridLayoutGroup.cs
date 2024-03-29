using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class AutoGridLayoutGroup : MonoBehaviour
{
    [Header(" ол-во ожидаемых элементов по определенному направлению")]
    public int countRow = 2;
    public int countColumn = 2;

    [Header("явл€етс€ ли €чейка квадратной")]
    public bool squareCell = false;

    [Header("Ўирина пространства от крайнего элемента до кра€ панели")]
    public int widthPadding;
    [Header("Ўирина пространства между элементами панели")]
    public int widthSpacing;

    [Header("“естовые данные")]
    [SerializeField] float width;
    [SerializeField] float height;
    [SerializeField] Rect testR;

    private GridLayoutGroup _gridLayoutGroup;
    private RectTransform _rectTransform;

    private void OnValidate()
    {
        if (_gridLayoutGroup == null)
        {
            _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        }
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }

    private void Awake()
    {
        UpdateGrid();
    }

    public void UpdateCells(int xcells, int ycells)
    {
        countColumn = xcells;
        countRow = ycells;
        UpdateGrid();
    }

    public void UpdateCells(int count, bool square = true)
    {
        if (square)
        {
            var sqrtResult = Mathf.Sqrt(count);
            countRow = (int)sqrtResult;
            countColumn = countRow + ((sqrtResult - (int)sqrtResult == 0) ? 0 : 1);
        }
        else
        {
            if (count is >= 0 and < 11)
            {
                countRow = 1;
            }
            else if (count is > 10 and < 21)
            {
                countRow = 2;
            }
            else if (count is > 20 and < 41)
            {
                countRow = 3;
            }
            else if (count is > 50 and < 61)
            {
                countRow = 4;
            }
            countColumn = count / countRow;
        }

        UpdateGrid();
    }

    public void UpdateGrid()
    {
        testR = _rectTransform.rect;

        _gridLayoutGroup.padding = new RectOffset(widthPadding, widthPadding, widthPadding, widthPadding);
        _gridLayoutGroup.spacing = new Vector2(widthSpacing, widthSpacing);

        width = _rectTransform.rect.width;
        height = _rectTransform.rect.height;

        var widthCell = (width - widthPadding * 2 - widthSpacing * (countColumn - 1)) / countColumn;
        var heightCell = squareCell ? widthCell : (height - widthPadding * 2 - widthSpacing * (countRow - 1)) / countRow;
        if (heightCell * countRow + widthSpacing * (countRow - 1) + widthPadding * 2 > height)
        {
            heightCell = (height - widthPadding * 2 - widthSpacing * (countRow - 1)) / countRow;
            widthCell = squareCell ? heightCell : (width - widthPadding * 2 - widthSpacing * (countColumn - 1)) / countColumn;
        }

        _gridLayoutGroup.cellSize = new Vector2(widthCell, heightCell);
    }
}