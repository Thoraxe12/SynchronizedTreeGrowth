using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace SynchronizedTreeGrowth;

public class ModEntry : Mod
{
    private const string SynchronizeFarmOnlyId = "SynchronizeFarmOnly";
    private const string GrowthChanceId = "GrowthChance";
    private readonly int _fullyGrownTreeStage = 5;
    private readonly Dictionary<Tree, int> _treeGrowthStages = new();
    private float _growthChance = 0.20f;
    private ModConfig _modConfig = new();
    private bool _synchronizeFarmOnly = true;

    // On day ending, get all farm trees
    // Right before it saves, revert growth stage of trees
    // Check if should grow, update growth stage of trees if should.
    public override void Entry(IModHelper helper)
    {
        _modConfig = helper.ReadConfig<ModConfig>();
        _growthChance = _modConfig.GrowthChance;
        _synchronizeFarmOnly = _modConfig.SynchronizeFarmOnly;

        // When the day starts, reset the should grow
        helper.Events.GameLoop.DayEnding += OnDayEnding;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.GameLoop.GameLaunched += (sender, args) => SetupConfigMenu();
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs dayEndingEventArgs)
    {
        if (Game1.currentSeason == "winter") return;

        IEnumerable<Tree> trees = new List<Tree>();

        if (_synchronizeFarmOnly)
            trees = Game1.getFarm()._activeTerrainFeatures.OfType<Tree>();
        else
            trees = Game1.locations.Aggregate(trees,
                (current, gameLocation) => current.Concat(gameLocation._activeTerrainFeatures.OfType<Tree>()));

        trees = trees
            .Where(tree => !tree.stump.Value // Tree can't be a stump
                           && !tree.IsGrowthBlockedByNearbyTree() // Tree can't be blocked by another fully grown tree
                           && tree.growthStage.Value < _fullyGrownTreeStage // Tree can't be fully grown yet
                           && !tree.fertilized.Value); // Let the original code handle fertilized trees

        _treeGrowthStages.Clear();

        foreach (var tree in trees) _treeGrowthStages.Add(tree, tree.growthStage.Value);
    }

    private void OnSaving(object? sender, SavingEventArgs savingEventArgs)
    {
        if (Game1.currentSeason == "winter") return;

        var gameId = (int)Game1.uniqueIDForThisGame;
        var dayOfMonth = Game1.dayOfMonth;

        var random = new Random(gameId + dayOfMonth);

        if (random.NextDouble() > _growthChance) RestoreGrowthStages();
        else UpdateGrowthStages();
    }

    private void RestoreGrowthStages()
    {
        Monitor.Log("Restoring old growth stages");
        foreach (var (tree, oldGrowthValue) in _treeGrowthStages) tree.growthStage.Set(oldGrowthValue);
    }

    private void UpdateGrowthStages()
    {
        Monitor.Log("Updating growth stages");
        foreach (var (tree, oldGrowthValue) in _treeGrowthStages) tree.growthStage.Set(oldGrowthValue + 1);
    }

    private void SetupConfigMenu()
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        configMenu.Register(
            ModManifest,
            () => _modConfig = new ModConfig(),
            () => Helper.WriteConfig(_modConfig)
        );

        configMenu.AddBoolOption(
            ModManifest,
            () => _modConfig.SynchronizeFarmOnly,
            value => _modConfig.SynchronizeFarmOnly = value,
            () => "Run synchronize on farm only",
            () => "When this is enabled, the mod will only affect the trees on the player's farm",
            SynchronizeFarmOnlyId
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => _modConfig.GrowthChance,
            value => _modConfig.GrowthChance = value,
            () => "Growth chance",
            () => "Sets the chance that trees will grow at the end of each day",
            0.0f,
            1.0f,
            fieldId: GrowthChanceId
        );

        configMenu.OnFieldChanged(
            ModManifest,
            (key, o) =>
            {
                switch (key)
                {
                    case SynchronizeFarmOnlyId when o is bool b:
                        _synchronizeFarmOnly = b;
                        break;
                    case GrowthChanceId when o is float f:
                        _growthChance = f;
                        break;
                }

                Monitor.Log($"{key} was changed to {o}");
            }
        );
    }
}