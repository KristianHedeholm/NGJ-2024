
using Shapes;
using UnityEngine;

public class LineManager : Singleton<LineManager>
{
    public enum Line
    {
        AimLine,
    }

    [SerializeField]
    private GameObject _aimLine;

    public GameObject GetLine(Line line)
    {
        switch (line)
        {
            case Line.AimLine:
                return _aimLine;

            default:
                return null;
        }
    }
}
