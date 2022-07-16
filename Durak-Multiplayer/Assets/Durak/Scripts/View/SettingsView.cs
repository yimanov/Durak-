using Durak;
using Meta.Settings;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : WindowView
{
#pragma warning disable 649
    [SerializeField] private UIManagerComponent _uiManagerComponent;
    [Space, SerializeField] private Dropdown _playersCountDropDown;
    [SerializeField] private Dropdown _cardsPerHandDropDown;
    [SerializeField] private Dropdown _difficultyDropDown;
    [SerializeField] private Dropdown _transferDropDown;
    [SerializeField] private Dropdown _gameTypeDropDown;
    [SerializeField] private Toggle _hideAiCardsToggle;
    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Toggle _sfxToggle;
    [SerializeField] private Toggle _vibrationToggle;
#pragma warning restore 649

//    private AdsController _adsController;
    private AudioController _audioController;
    private VibrationController _vibrationController;

    protected override void PreInitialize()
    {
      ///  _adsController = LazySingleton<AdsController>.Instance;
        _audioController = LazySingleton<AudioController>.Instance;
        _vibrationController = LazySingleton<VibrationController>.Instance;

        _musicToggle.SetIsOnWithoutNotify(!_audioController.IsMainMute);
        _sfxToggle.SetIsOnWithoutNotify(!_audioController.IsSfxMute);
        _vibrationToggle.SetIsOnWithoutNotify(_vibrationController.IsEnabled);
    }

    private void OnEnable()
    {
       // _adsController.ShowBanner();
    }

    public override void Show(WindowData data)
    {
        base.Show(data);
    }

    /// <summary>
    /// Called when user click NewGame button on this view.
    /// </summary>
    public void StartGame()
    {
        GameSettings settings = new GameSettings()
        {
            CardsPerHand = ParseDropDownValue(_cardsPerHandDropDown),
            PlayersCount = ParseDropDownValue(_playersCountDropDown),
            GameDifficulty = ParseDifficulty(_difficultyDropDown),
            GameTransferType = ParseDurakTransferType(_transferDropDown),
            GameType = ParseDurakType(_gameTypeDropDown),
            HideAiCards = _hideAiCardsToggle.isOn
        };
        _uiManagerComponent.GameField.StartGame(settings);

        Hide();
    }

    public void SwitchMusic()
    {
        _audioController.SwitchSourceMuteState(AudioSourceType.Main);
        _audioController.SwitchSourceMuteState(AudioSourceType.MainAdditional);
    }

    public void SwitchSounds()
    {
        _audioController.SwitchSourceMuteState(AudioSourceType.SFX);
    }

    public void SwitchVibration()
    {
        _vibrationController.SwitchVibration();
    }

    /// <summary>
    /// Parse values from default dropdowns with integers
    /// </summary>
    /// <param name="dropDown"></param>
    /// <returns></returns>
    private int ParseDropDownValue(Dropdown dropDown)
    {
        return int.Parse(dropDown.options[(dropDown.value)].text);
    }

    /// <summary>
    /// Parse value from text Game transfer type dropdown.
    /// </summary>
    private DurakTransferType ParseDurakTransferType(Dropdown dropDown)
    {
        return (DurakTransferType)dropDown.value;
    }

    /// <summary>
    /// Parse value from text Game type dropdown.
    /// </summary>
    private DurakType ParseDurakType(Dropdown dropDown)
    {
        return (DurakType)dropDown.value;
    }

    /// <summary>
    /// Parse value from text Difficulty dropdown.
    /// </summary>
    private Difficulty ParseDifficulty(Dropdown dropDown)
    {
        return (Difficulty)dropDown.value;
    }

    /// <summary>
    /// Called when user change difficulty from Settings Wnidow.
    /// </summary>
    public void OnDifficultyChanged()
    {
        Difficulty currentDiff = ParseDifficulty(_difficultyDropDown);

        _hideAiCardsToggle.interactable = currentDiff != Difficulty.Hard;
        _hideAiCardsToggle.isOn = currentDiff == Difficulty.Hard ? true : _hideAiCardsToggle.isOn;
    }
}

public class SettingsData : WindowData
{
    public int PlayersCount;
    public int CardsPerHand;
    public bool IsHideAiCards;
}