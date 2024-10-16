using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using UnityEngine.Events;

public class HomeManager : OverwritableMonoSingleton<HomeManager>
{
    public static UnityAction EVCancel;
    public static UnityAction<HomeMenuSelector.HomeMenuResult> EVConfirmHomeMenuResult;
    public static UnityAction<PlayerData> EVConfirmPlayerSelect;
    public static UnityAction<AbstractSingle> RequestSetPracticePattern;

    private List<AbstractHomeSelector> homeSelectors = new();
    [SerializeField]
    private HomeMenuSelector homeMenuSelector;
    [SerializeField]
    private PlayerSelector playerSelector;
    [SerializeField]
    private PracticeSelector practiceSelector;
    [SerializeField]
    private InertPanel manualPanel;

    private const int SELECTOR_SWITCH_DURATION = 15;

    private bool shouldRespondToInput = true;

    private void OnEnable()
    {
        EVCancel += ResolveCancel;
        EVConfirmHomeMenuResult += ResolveHomeMenu;
        EVConfirmPlayerSelect += ResolvePlayerData;
        RequestSetPracticePattern += ResolveSetPracticePattern;
    }

    private void OnDisable()
    {
        EVCancel -= ResolveCancel;
        EVConfirmHomeMenuResult -= ResolveHomeMenu;
        EVConfirmPlayerSelect -= ResolvePlayerData;
        RequestSetPracticePattern -= ResolveSetPracticePattern;
    }

    protected override void Awake()
    {
        base.Awake();
        homeSelectors.Add(homeMenuSelector);
    }

    private void Start()
    {
        BGMPlayer.RequestPlayHomeBGM?.Invoke();
    }

    private void PushNextSelector(AbstractHomeSelector newSelector)
    {
        AbstractHomeSelector oldSelector = homeSelectors.Last();
        homeSelectors.Add(newSelector);
        Timing.RunCoroutine(_PushSelector(oldSelector, newSelector));
    }

    private void PopSelector()
    {
        if (homeSelectors.Count <= 1)
        {
            return;
        }
        AbstractHomeSelector oldSelector = homeSelectors.Last();
        homeSelectors.RemoveAt(homeSelectors.Count - 1);
        homeSelectors.Last();
        Timing.RunCoroutine(_PopSelector(oldSelector, homeSelectors.Last()));

    }

    private void ResolveCancel()
    {
        if (!shouldRespondToInput)
        {
            return;
        }
        SFXPlayer.EVPlayCancelSound?.Invoke();
        PopSelector();
    }

    private void ResolveHomeMenu(HomeMenuSelector.HomeMenuResult homeMenuResult)
    {
        if (!shouldRespondToInput)
        {
            return;
        }
        switch (homeMenuResult)
        {
            case HomeMenuSelector.HomeMenuResult.Start:
                SFXPlayer.EVPlayConfirmSound?.Invoke();
                // set patterns to use all listed
                RuntimeGameData.SelectedPatterns = DefaultGameData.AllPatterns;
                RuntimeGameData.IsPractice = false;
                PushNextSelector(playerSelector);
                break;
            case HomeMenuSelector.HomeMenuResult.Practice:
                SFXPlayer.EVPlayConfirmSound?.Invoke();
                RuntimeGameData.IsPractice = true;
                PushNextSelector(practiceSelector);
                break;
            case HomeMenuSelector.HomeMenuResult.Settings:
                SFXPlayer.RequestPlayInvalidSound?.Invoke();
                break;
            case HomeMenuSelector.HomeMenuResult.Manual:
                SFXPlayer.EVPlayConfirmSound?.Invoke();
                PushNextSelector(manualPanel);
                break;
            case HomeMenuSelector.HomeMenuResult.Quit:
                SFXPlayer.EVPlayCancelSound?.Invoke();
#if UNITY_EDITOR
                Debug.Log("Can't quit in editor, idiot.");
#else
                Application.Quit();
#endif
                break;
        }
    }

    private void ResolvePlayerData(PlayerData selectedPlayerData)
    {
        if (!shouldRespondToInput)
        {
            return;
        }
        RuntimeGameData.SelectedPlayerData = selectedPlayerData;
        SFXPlayer.EVPlayConfirmSound?.Invoke();
        SceneUtil.LoadSceneAsync("Stage");
        shouldRespondToInput = false;
    }

    private void ResolveSetPracticePattern(AbstractSingle practicePattern)
    {
        if (!shouldRespondToInput)
        {
            return;
        }
        RuntimeGameData.SelectedPatterns = new() { practicePattern };
        SFXPlayer.EVPlayConfirmSound?.Invoke();
        PushNextSelector(playerSelector);
    }

    private IEnumerator<float> _PushSelector(AbstractHomeSelector oldSelector, AbstractHomeSelector newSelector)
    {
        shouldRespondToInput = false;
        Vector3 oldSelectorRotation = Vector3.zero;
        Vector3 newSelectorRotation = new Vector3(0f, -90f, 0f);

        oldSelector.enabled = false;
        oldSelector.RectTransform.anchorMin = new Vector2(0f, 0.5f);
        oldSelector.RectTransform.anchorMax = new Vector2(0f, 0.5f);
        oldSelector.RectTransform.pivot = new Vector2(0f, 0.5f);
        // oldSelector.RectTransform.position = Vector3.zero;
        // oldSelector.RectTransform.eulerAngles = oldSelectorRotation;

        newSelector.gameObject.SetActive(true);
        newSelector.enabled = false;
        newSelector.RectTransform.anchorMin = new Vector2(1f, 0.5f);
        newSelector.RectTransform.anchorMax = new Vector2(1f, 0.5f);
        newSelector.RectTransform.pivot = new Vector2(1f, 0.5f);
        // newSelector.RectTransform.position = Vector3.zero;
        // oldSelector.RectTransform.eulerAngles = newSelectorRotation;

        const float ANGLE_CHANGE_VEL = 90f / SELECTOR_SWITCH_DURATION;

        for (int i = 0; i < SELECTOR_SWITCH_DURATION; i++)
        {
            oldSelectorRotation.y -= ANGLE_CHANGE_VEL;
            newSelectorRotation.y += ANGLE_CHANGE_VEL;
            oldSelector.RectTransform.eulerAngles = oldSelectorRotation;
            newSelector.RectTransform.eulerAngles = newSelectorRotation;
            yield return Timing.WaitForOneFrame;
        }

        oldSelector.gameObject.SetActive(false);
        oldSelector.enabled = false;
        newSelector.enabled = true;
        shouldRespondToInput = true;
    }

    private IEnumerator<float> _PopSelector(AbstractHomeSelector oldSelector, AbstractHomeSelector newSelector)
    {
        shouldRespondToInput = false;
        Vector3 oldSelectorRotation = Vector3.zero;
        Vector3 newSelectorRotation = new Vector3(0f, -90f, 0f);

        oldSelector.enabled = false;
        oldSelector.RectTransform.anchorMin = new Vector2(1f, 0.5f);
        oldSelector.RectTransform.anchorMax = new Vector2(1f, 0.5f);
        oldSelector.RectTransform.pivot = new Vector2(1f, 0.5f);
        // oldSelector.RectTransform.position = Vector3.zero;
        // oldSelector.RectTransform.eulerAngles = oldSelectorRotation;

        newSelector.gameObject.SetActive(true);
        newSelector.enabled = false;
        newSelector.RectTransform.anchorMin = new Vector2(0f, 0.5f);
        newSelector.RectTransform.anchorMax = new Vector2(0f, 0.5f);
        newSelector.RectTransform.pivot = new Vector2(0f, 0.5f);
        // newSelector.RectTransform.position = Vector3.zero;
        // oldSelector.RectTransform.eulerAngles = newSelectorRotation;

        const float ANGLE_CHANGE_VEL = 90f / SELECTOR_SWITCH_DURATION;

        for (int i = 0; i < SELECTOR_SWITCH_DURATION; i++)
        {
            oldSelectorRotation.y -= ANGLE_CHANGE_VEL;
            newSelectorRotation.y += ANGLE_CHANGE_VEL;
            oldSelector.RectTransform.eulerAngles = oldSelectorRotation;
            newSelector.RectTransform.eulerAngles = newSelectorRotation;
            yield return Timing.WaitForOneFrame;
        }

        oldSelector.gameObject.SetActive(false);
        oldSelector.enabled = false;
        newSelector.enabled = true;
        shouldRespondToInput = true;
    }
}