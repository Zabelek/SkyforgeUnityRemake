using System.Linq;
using System.Threading.Tasks;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public static class SkyforgeLoader
{
    //A class designed to manage all processes that, logically, keep the whole game session together. For now it's basically: managing scene/asset loading and player profile.
    #region Variables
    private static string _targetSceneName;
    public static UserProfile CurrentProfile;
    //Used when the full access to all [you name it] available in the game is needed
    public static OutfitRegistry OutfitRegistry;
    public static PerkRegistry PerkRegistry;
    public static ClassRegistry ClassRegistry;
    //Used so that both loaded scene and the loading screen can notify each other they're ready to switch
    public static bool LoadingScreenReady, LoadedSceneReady;
    public static SettingsSet SettingsSet;
    //for switching between game menu and gameplay scene
    public static GUIGameMenu GUIGameMenu;
    public static GUIGameplayControls GUIGameplayControls;
    public static bool SettingsChanged;
    public static bool PerksChanged;
    //variables used only for the main menu to see of the player just finished the game, so that the Argus end lines can play
    public static bool HardestDiffBeaten = false;
    public static bool GameBeaten = false;
    #endregion

    #region Methods
    public static async Task LoadScene(string currentScene, string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("WARNING! Empty name scene loading attempt!");
            return;
        }
        LoadingScreenReady = false;
        LoadedSceneReady = false;
        await SceneManager.LoadSceneAsync("LoadingScene");
        _targetSceneName = sceneName;
        await LoadPerkRegistry();
        await LoadClassRegistry();
    }
    public static async Task LoadScene(string currentScene, MapSO map)
    {
        GUILoadingScreen.HintText = "Divine Observatory dungeon is just a test map. Holes and collision bugs are expected, so don't bother reporting bugs related to map itself. All other bug reports will be, obviously, highly appreciated.";
        GUILoadingScreen.MapThumbnail = map.Thumbnail;
        await LoadScene(currentScene, map.SceneName);
        await SceneManager.LoadSceneAsync("GameMenuScene", LoadSceneMode.Additive);
    }
    public static async Task LoadScene()
    {
        if (string.IsNullOrWhiteSpace(_targetSceneName))
        {
            Debug.LogError("WARNING! No target scene to load!");
            return;
        }
        await SceneManager.LoadSceneAsync(_targetSceneName, LoadSceneMode.Additive);
    }
    public static async Task UnloadLoadingScene()
    {
        if (SceneManager.GetSceneByName("LoadingScene").isLoaded)
        {
            await SceneManager.UnloadSceneAsync("LoadingScene");
        }
        LoadingScreenReady = true;
    }
    private static async Task LoadOutfitRegistry()
    {
        //First check if the outfit registry was initialized
        if (OutfitRegistry == null)
        {
            var handle = await Addressables.LoadAssetAsync<GameObject>("OutfitRegistry").Task;
            if (handle != null && handle.TryGetComponent<OutfitRegistry>(out OutfitRegistry registry) == true)
            {
                OutfitRegistry = registry;
            }
            else
            {
                Debug.LogError("Unable to load the OutfitRegistry addressable!");
            }
        }
    }
    private static async Task LoadPerkRegistry()
    {
        //First check if the perk registry was initialized
        if (PerkRegistry == null)
        {
            var handle = await Addressables.LoadAssetAsync<GameObject>("PerkRegistry").Task;
            if (handle != null && handle.TryGetComponent<PerkRegistry>(out PerkRegistry registry) == true)
            {
                PerkRegistry = registry;
            }
        }
    }
    private static async Task LoadClassRegistry()
    {
        //First check if the class registry was initialized
        if (ClassRegistry == null)
        {
            var handle = await Addressables.LoadAssetAsync<GameObject>("ClassRegistry").Task;
            if (handle != null && handle.TryGetComponent<ClassRegistry>(out ClassRegistry registry) == true)
            {
                ClassRegistry = registry;
            }
        }
    }
    public static async Task<OutfitBehaviour> LoadOutfit(int outfitID, OutfitSO.OutfitSlot slot, Transform parent)
    {
        await LoadOutfitRegistry();
        string address = OutfitRegistry.Outfits.FirstOrDefault(o => o.ObjectID == outfitID && o.Slot == slot).Address;
        if (address != null && address.Length > 0)
        {
            var handle = await Addressables.InstantiateAsync(address, parent).Task;
            if (handle != null && handle.TryGetComponent<OutfitBehaviour>(out OutfitBehaviour outfit)==true)
            {
                return outfit;
            }
        }
        Debug.Log("No outfit found for address" + address + "!");
        return null;
    }
    public static async Task UnloadGameMenu()
    {
        await SceneManager.UnloadSceneAsync("GameMenuScene");
    }
    public static async Task SetMenuOpen(bool menuOpen)
    {
        if(GUIGameMenu != null && GUIGameplayControls != null)
        {
            if (menuOpen)
            {
                await GUIGameplayControls.OpenMenu();
                await GUIGameMenu.OpenMenu();
            }
            else
            {
                await GUIGameMenu.CloseMenu();
                await GUIGameplayControls.CloseMenu();
            }
            SettingsChanged = false;
            PerksChanged = false;
        }
    }
    public static void EnablePerk(PerkSO perkSO)
    {
        if(CurrentProfile != null)
        {
            var perkToEnable = CurrentProfile.AcquiredPerks.FirstOrDefault(p => p.PerkID == perkSO.ID);
            if (perkToEnable != null)
                perkToEnable.Enabled = true;
            var perkSet = PerkRegistry.PerkSets.FirstOrDefault(ps => ps.Perks.Any(p => p.ID == perkSO.ID));
            if (perkSet != null)
            {
                foreach(var otherPerkSO in perkSet.Perks)
                {
                    if(otherPerkSO.ID != perkSO.ID)
                    {
                        var perkToDisable = CurrentProfile.AcquiredPerks.FirstOrDefault(p => p.PerkID == otherPerkSO.ID);
                        if(perkToDisable != null && perkToDisable.Enabled == true)
                        {
                            perkToDisable.Enabled = false;
                        }
                    }
                }
            }
            PerksChanged = true;
        }
    }
    #endregion
}
