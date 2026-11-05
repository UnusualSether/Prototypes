/// <summary>
/// Todos os nomes usados para buscar elementos do UXML e classes do USS.
/// Se algo mudar no UI Builder, basta alterar aqui.
/// </summary>
public static class UINames
{
    // ---------- Elementos (atributo "name" no UXML) ----------
    public const string Play = "Play";
    public const string TapToStart = "TapToStart";

    // Hub inferior / Bottom sheet
    public const string OpenConfig = "openConfig";
    public const string OpenSound = "openSound";
    public const string OpenEvents = "openEvents";
    public const string OpenShop = "openShop";
    public const string BottomSheet = "BottomSheet";
    public const string Scrim = "Scrim";
    public const string CloseMenu = "closeMenu";
    public const string LanguageContainer = "Language";
    public const string SoundController = "SoundController";

    // Idioma
    public const string LeftLanguage = "LeftLanguage";
    public const string RightLanguage = "RightLanguage";
    public const string LanguageName = "LanguageName";

    // Janela Trinked
    public const string TrinkedWindow = "trinked_window";
    public const string TrinkedReturn = "TReturn";
    public const string DifficultyButton = "Difficulty_btn";

    // Janela Level
    public const string LevelWindow = "level_window";
    public const string LevelReturn = "return_to_trinked";
    public const string Level1Button = "level1";

    // Tutorial
    public const string TutorialQuestion = "tutorial_window";
    public const string TutorialPages = "TutorialWindows";
    public const string TutorialPagePrefix = "TutorialWindow"; // TutorialWindow1, TutorialWindow2...
    public const int TutorialPageCount = 6;
    public const string TutorialYes = "TutorialYes";
    public const string TutorialNo = "TutorialNo";
    public const string NextPage = "NextPage";

    // ---------- Classes USS ----------
    public const string BottomSheetUp = "bottom_Sheet--UP";
    public const string ScrimFadeIn = "scrim_fadein";
    public const string StartOff = "starOff";
    public const string CloseMenuOn = "close_menu_on";

    public const string TrinkedOn = "trinked_select_on";
    public const string TrinkedOff = "trinked_select_off";

    public const string LevelOn = "level_select_on";
    public const string LevelOff = "level_select_off";

    public const string TutorialOn = "Tutorial-Menu_on";
    public const string TutorialOff = "Tutorial-Menu_off";
    // Atenção: o código original usava "tutorial-Menu_on" (t minúsculo) na janela da pergunta.
    // Mantido separado para não alterar o visual; unifique com TutorialOn se o USS usar um só nome.
    public const string TutorialQuestionOn = "tutorial-Menu_on";
}
