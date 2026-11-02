using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(ShowPlayerOnMap.Mod), "ShowPlayerOnMap", "1.0.0", "KillerGanja", "https://github.com/KillerGanja/TLD-ShowPlayerOnMap")]
[assembly: MelonGame("Hinterland", "TheLongDark")]

namespace ShowPlayerOnMap
{
    // Shows your own position (direction arrow) on the map in Survival mode
    // and centers the map on you when it is opened.
    //
    // Why the arrow is missing in vanilla Survival:
    // The game already tracks the player icon (Panel_Map.m_PlayerIcon) in Survival,
    // but its UISprite points to the atlas "Skills", which is empty in Survival.
    // The sprite "ico_map_directional" exists in the "Base Atlas" used by all other map icons.
    public class Mod : MelonMod
    {
        private const string PlayerSpriteName = "ico_map_directional";
        private const string FallbackAtlasName = "Base Atlas";
        private const int PlayerIconDepth = 40000;   // draw above all other map icons
        private const float CheckInterval = 0.05f;   // seconds between checks
        private const float ErrorBackoff = 30f;      // pause after an error to avoid log spam

        private float nextCheck;
        private bool warnedNoAtlas;
        private bool mapWasOpen;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("loaded - shows your position on the map");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            Panel_Map.s_ForceShowPlayerIcon = true;
        }

        // Harmony patches on Panel_Map.Enable / ResetToNormal are never triggered in TLD 2.55
        // (IL2CPP), so we poll whether the map panel is open.
        public override void OnUpdate()
        {
            if (Time.unscaledTime < nextCheck) return;
            nextCheck = Time.unscaledTime + CheckInterval;
            try
            {
                Panel_Map map = InterfaceManager.GetPanel<Panel_Map>();
                bool open = map != null && map.gameObject.activeInHierarchy;
                if (!open) { mapWasOpen = false; return; }

                FixPlayerIcon(map);
                if (!mapWasOpen)
                {
                    mapWasOpen = true;
                    CenterOnPlayer(map);
                }
            }
            catch (System.Exception e)
            {
                LoggerInstance.Error($"Error: {e.Message}");
                nextCheck = Time.unscaledTime + ErrorBackoff;
            }
        }

        private void FixPlayerIcon(Panel_Map map)
        {
            var icon = map.m_PlayerIcon;
            if (icon == null) return;
            if (!icon.gameObject.activeSelf) icon.gameObject.SetActive(true);

            var sprite = icon.GetComponent<UISprite>();
            if (sprite == null) return;

            if (sprite.depth != PlayerIconDepth) sprite.depth = PlayerIconDepth;

            // Sprite already valid? Nothing to do.
            var atlas = sprite.atlas as UIAtlas;
            if (atlas != null && sprite.spriteName == PlayerSpriteName && atlas.GetSprite(PlayerSpriteName) != null) return;

            // Take the "Base Atlas" from another map icon
            UIAtlas baseAtlas = FindAtlas(icon.parent, FallbackAtlasName);
            if (baseAtlas == null || baseAtlas.GetSprite(PlayerSpriteName) == null)
            {
                if (!warnedNoAtlas) LoggerInstance.Warning($"'{FallbackAtlasName}' containing '{PlayerSpriteName}' not found");
                warnedNoAtlas = true;
                return;
            }

            sprite.atlas = baseAtlas;
            sprite.spriteName = PlayerSpriteName;
            LoggerInstance.Msg($"Player icon: atlas '{(atlas == null ? "?" : atlas.name)}' -> '{baseAtlas.name}', sprite '{PlayerSpriteName}'");
        }

        private void CenterOnPlayer(Panel_Map map)
        {
            var player = GameManager.GetPlayerTransform();
            if (player == null) return;
            map.CenterOnPoint(player.position);   // expects the world position
        }

        private static UIAtlas FindAtlas(Transform parent, string atlasName)
        {
            if (parent == null) return null;
            foreach (var s in parent.GetComponentsInChildren<UISprite>(true))
            {
                var a = s.atlas as UIAtlas;
                if (a != null && a.name == atlasName) return a;
            }
            return null;
        }
    }
}
