using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Foundations.UIModules.UIManager.UICanvases
{
    [CreateAssetMenu(fileName = "CanvasDefinition", menuName = "UI/Canvas Definition")]
    public class CanvasDefinitionSettings : ScriptableObject
    {
        [SerializeField] public Canvas canvasPrefab;
        [SerializeField] private List<CanvasSettings> canvasSettings = new();

        public Dictionary<UICanvasType, CanvasSettings> CanvasSettings { get; private set; }

        public void Initialize() =>
            CanvasSettings ??= canvasSettings.ToDictionary(key => key.canvasType, value => value);
    }
}
