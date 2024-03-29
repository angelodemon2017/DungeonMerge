using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Editor
{
    public class MenuHelper : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("HELPER/Test func")]
        static void Random_value()
        {
            float _timer = 5.5123f;
            var result = _timer - ((int)(_timer * 10)) / 10f;

            Debug.Log($"Test value:{result}");
        }
#endif
    }

}