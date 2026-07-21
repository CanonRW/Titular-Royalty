//using System; 

namespace TitularRoyalty;

[UsedImplicitly]
public class GameComponent_TitularRoyalty : GameComponent
{
    public static GameComponent_TitularRoyalty Current { get; private set; }
    public GameComponent_TitularRoyalty(Game game) { Current = this; } // Needs this or else Rimworld throws a fit and errors.
    
    public RealmTypeDef realmTypeDef;

    // Custom Titles
    private Dictionary<PlayerTitleDef, RoyalTitleOverride> customTitles;
    private Dictionary<PlayerTitleDef, RoyalTitleOverride> CustomTitles 
        => customTitles ??= TitleDatabase.TitlesBySeniority.ToDictionary(x => x, _ => new RoyalTitleOverride());

    // Required for ExposeData
    private List<PlayerTitleDef> customTitles_List1;
    private List<RoyalTitleOverride> customTitles_List2;

    /// <summary>
    /// Passes on the Realmtype 
    /// </summary>
    public void SetupAllTitles() => TitleDatabase.TitlesBySeniority.ForEach(SetupTitle);

    public void SetupTitle(PlayerTitleDef title)
    {
        // Custom Title
        if (CustomTitles.TryGetValue(title, out var titleOverrides) && titleOverrides.HasTitle())
        {
            ApplyTitleOverridesTo(title, titleOverrides);
            return;
        }

        // Realm Type, if there is no Custom title.
        if (realmTypeDef.TitlesWithOverrides.TryGetValue(title, out var realmTypeOverrides))
            ApplyTitleOverridesTo(title, realmTypeOverrides); // Found one
        else
            ApplyTitleOverridesTo(title, title.originalTitleFields); // No override found, create a default one.
    }

    private static void ApplyTitleOverridesTo(PlayerTitleDef title, RoyalTitleOverride overrides)
    {
        if (overrides.HasTitle())
        {
            title.label = overrides.label;
            title.labelFemale = overrides.HasFemaleTitle() ? overrides.labelFemale : null;
        }
        else
        {
            title.label = title.originalTitleFields.label;
            title.labelFemale = title.originalTitleFields.labelFemale;
        }

        title.titleTier = overrides.titleTier ?? title.originalTitleFields.titleTier ?? TitleTiers.Lowborn;
        title.allowDignifiedMeditationFocus = overrides.allowDignifiedMeditationFocus ?? title.originalTitleFields.allowDignifiedMeditationFocus ?? false;

        title.iconName = overrides.iconName.NullOrEmpty() ? null : overrides.iconName;

        title.TRInheritable = overrides.TRInheritable ?? title.originalTitleFields.TRInheritable ?? false;
        title.canBeInherited = TRSettings.inheritanceEnabled && title.TRInheritable;

        title.minExpectation = overrides.minExpectation ?? title.originalTitleFields.minExpectation ?? ExpectationDefOf.ExtremelyLow;

        title.ClearCachedData();
    }

    /// <summary>
    /// Resets all changed titles to their default realmType or Base Value
    /// </summary>
    public void ResetTitles()
    {
        customTitles = null;
        customTitles_List1 = null;
        customTitles_List2 = null;
        TitleDatabase.Notify_TitleSeniorityChanged();

        foreach (var title in TitleDatabase.TitlesBySeniority)
        {
            title.ResetToDefaultValues();
            SetupTitle(title);
        }

        TitleThoughtUtility.Notify_TitlesChanged();
    }

    public RoyalTitleOverride GetCustomTitleOverrideFor(PlayerTitleDef titleDef)
    {
        if (CustomTitles.TryGetValue(titleDef, out var titleOverride))
            return titleOverride;
        
        Log.Error($"Titular Royalty: Could not find custom title override for {titleDef.defName} {titleDef.label}");
        return null;
    }
    
    public void SaveTitleChange(PlayerTitleDef title, RoyalTitleOverride newOverride)
    {
        customTitles[title] = newOverride;
        SetupTitle(title);
        TitleThoughtUtility.Notify_TitlesChanged();
    }

    /// <summary>
    /// Code to be run on both loading or starting a new game
    /// </summary>
    private void OnGameStart(bool newGame)
    {
        Current = this;
        if (realmTypeDef == null)
        {
            if (!newGame) Log.Warning("Realm Type Def was not found, resetting to kingdom, if you are updating to TR 1.9, ignore this.");
            realmTypeDef = DefDatabase<RealmTypeDef>.GetNamed("RealmType_Kingdom");
        }
        Faction.OfPlayer.allowGoodwillRewards = false;
        Faction.OfPlayer.allowRoyalFavorRewards = false;
        StartupSetup.ApplyModSettings();
        SetupAllTitles();
        TitleThoughtUtility.Notify_TitlesChanged();
    }

    public override void LoadedGame() => OnGameStart(false);
    public override void StartedNewGame() => OnGameStart(true);

    /// <summary>
    /// Saves & Loads the title data, if updating from a TR 1.1 save or the values are otherwise not found, generate new ones
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref realmTypeDef, "realmType");
        Scribe_Collections.Look(ref customTitles, "TRCustomTitles", LookMode.Def, LookMode.Deep, ref customTitles_List1, ref customTitles_List2);
    }

}
