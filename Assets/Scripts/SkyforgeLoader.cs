using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public static class SkyforgeLoader
{
    //A class designed to manage all processes that, logically, keep the whole game session together. For now it's basically: managing scene/asset loading and player profile.
    #region Variables
    private static string _targetSceneName, _previousSceneName;
    public static UserProfile CurrentProfile;
    public static OutfitRegistry OutfitRegistry;
    //Used so that both loaded scene and the loading screen can notify each other they're ready to switch
    public static bool LoadingScreenReady, LoadedSceneReady;
    public static SettingsSet SettingsSet;
    //variables used only for the main menu to see of the player just finished the game, so that the Argus end lines can play
    public static bool HardestDiffBeaten = false;
    public static bool GameBeaten = false;
    #endregion

    #region Methods
    public static async Task LoadScene(string currentScene, string sceneName)
    {
        LoadingScreenReady = false;
        LoadedSceneReady = false;
        await SceneManager.LoadSceneAsync("LoadingScene");
        _previousSceneName = currentScene;
        _targetSceneName = sceneName;
    }
    public static async Task LoadScene(string currentScene, MapSO map)
    {
        GUILoadingScreen.HintText = "Divine Observatory dungeon is just a test map. Holes and collision bugs are expected, so don't bother reporting bugs related to map itself. All other bug reports will be, obviously, highly appreciated.";
        GUILoadingScreen.MapThumbnail = map.Thumbnail;
        await LoadScene(currentScene, map.SceneName);
    }
    public static async Task LoadScene()
    {
        await SceneManager.LoadSceneAsync(_targetSceneName, LoadSceneMode.Additive);
    }
    public static async Task UnloadLoadingScene()
    {
        await SceneManager.UnloadSceneAsync("LoadingScene");
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
        return null;
    }
    #endregion
}
