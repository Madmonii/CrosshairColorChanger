using MelonLoader;
using UnityEngine;

namespace CrosshairColorChanger
{
    public class ColorChanger : MelonMod
    {
        private MelonPreferences_Category _crosshairCategory;
        private MelonPreferences_Entry<Color> _colorEntry;

        public override void OnLateInitializeMelon()
        {
            _crosshairCategory = MelonPreferences.CreateCategory("Crosshair");
            _colorEntry = _crosshairCategory.CreateEntry<Color>("Color", Color.grey);

            Singleton<Game>.Instance.OnLevelLoadComplete += OnLevelLoadComplete;
        } 
        
        private void OnLevelLoadComplete()
        {
            if (Singleton<Game>.Instance.GetCurrentLevelType() == LevelData.LevelType.Hub) return;
            
            ChangeColor();
        }

        public override void OnPreferencesSaved()
        {
            if (!RM.ui) return;
            
            ChangeColor();
        }

        private void ChangeColor()
        {
            RM.ui.crosshair.GetComponent<MeshRenderer>().material.color = _colorEntry.Value;
        }
    }
}
