using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace CrosshairColorChanger
{
    [HarmonyPatch]
    public class ColorChanger : MelonMod
    {
        private static MelonPreferences_Category _crosshairCategory;
        private static MelonPreferences_Entry<bool> _enabled;
        private static MelonPreferences_Entry<Color> _mainColor;
        private static MelonPreferences_Entry<Color> _telefragOn;
        private static MelonPreferences_Entry<Color> _telefragOff;
        private static MelonPreferences_Entry<Color> _overheat;
        private static MelonPreferences_Entry<Color> _ziplineInner;
        private static MelonPreferences_Entry<Color> _ziplineOuter;
        private static MelonPreferences_Entry<Color> _ziplineOn;

        public override void OnLateInitializeMelon()
        {
            _crosshairCategory = MelonPreferences.CreateCategory("Crosshair");
            _enabled = _crosshairCategory.CreateEntry<bool>("Enabled", true);
            _mainColor = _crosshairCategory.CreateEntry<Color>("Main Crosshair", Color.grey);
            _ziplineInner = _crosshairCategory.CreateEntry<Color>("Zipline Inner", Color.grey, description:"Inner hexagon (Requires level restart)");
            _ziplineOuter = _crosshairCategory.CreateEntry<Color>("Zipline Outer", Color.white, description:"Outer hexagon. Will be darker at range -INF, same as base game (Requires level restart)");
            _ziplineOn = _crosshairCategory.CreateEntry<Color>("Zipline Active", Color.green, description:"When in zipline range (Requires level restart)");
            _telefragOn = _crosshairCategory.CreateEntry<Color>("Boof Active", new Color(1f, 0f, 0.4308f, 1f), description:"When targeting enemy");
            _telefragOff = _crosshairCategory.CreateEntry<Color>("Boof Inactive", Color.grey, description:"When not targeting enemy");
            _overheat = _crosshairCategory.CreateEntry<Color>("Overheat", Color.grey, description:"Purify/Stomp/Fireball outer hexagon (Very dark)");

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
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIAbilityIndicator_Zipline), "Start")]
        public static void SetIndicatorUI(ref Color ____colorGray, ref Color ____colorWhite, ref Color ____colorGreen)
        {
            if (!_enabled.Value) return;
            
            ____colorGray = ClampColor(_ziplineInner.Value);
            ____colorWhite = ClampColor(_ziplineOuter.Value);
            ____colorGreen = ClampColor(_ziplineOn.Value);
        }

        private void ChangeColor()
        {
            if (!_enabled.Value) return;
            
            RM.ui.crosshair.GetComponent<MeshRenderer>().material.color = ClampColor(_mainColor.Value);
            RM.ui._telefragIndicator.indicatorUIOn.GetComponent<MeshRenderer>().material.color = ClampColor(_telefragOn.Value);
            RM.ui._telefragIndicator.indicatorUIOff.GetComponent<MeshRenderer>().material.color = ClampColor(_telefragOff.Value);
            RM.ui.crosshairOverheatIndicator.GetComponent<MeshRenderer>().material.color = ClampColor(_overheat.Value);
        }

        private static Color ClampColor(Color color)
        {
            color.r = Mathf.Clamp01(color.r);
            color.g = Mathf.Clamp01(color.g);
            color.b = Mathf.Clamp01(color.b);
            color.a = Mathf.Clamp01(color.a);
            
            return color;
        }
    }
}
