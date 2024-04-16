using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static GameEvents;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    private EventBus _eventBus;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private TextMeshProUGUI PassangerBoosterCountText;
    [SerializeField] private TextMeshProUGUI FreezeTimeBoosterCountText;
    [SerializeField] private TextMeshProUGUI PassangerBoosterBuyPriceText;
    [SerializeField] private TextMeshProUGUI FreezeTimeBoosterBuyPriceText;
    [SerializeField] private TextMeshProUGUI hardnessText;


    [SerializeField] private Button PassangerBoosterButton;
    [SerializeField] private Button FreezeTimeBoosterButton;
    [SerializeField] private Button PassangerBoosterBuyButton;
    [SerializeField] private Button FreezeTimeBoosterBuyButton;


    [SerializeField] private GameObject PassangerBoosterCountObject;
    [SerializeField] private GameObject FreezeTimeBoosterCountObject;
    [SerializeField] private GameObject FreezePanel;

    [SerializeField] private Image levelBadge;
    [SerializeField] private Image levelDifficultyInfoImage;
    [SerializeField] private Image levelDifficultyInfoBg;
    [SerializeField] private Image pauseButtonBadge;
    [SerializeField] private Image timeBadge;
    [SerializeField] private Image coinBadge;
    [SerializeField] private Image retryButtonBadge;
    [SerializeField] private Image hardnessBadge;
    [SerializeField] private Image freezeFillImage;
    [SerializeField] private Image timeCountFreezeImage;


    public List<Sprite> levelBadgeList = new List<Sprite>();
    public List<Sprite> levelDifficultyInfoBgList = new List<Sprite>();
    public List<Sprite> pauseButtonBadgeList = new List<Sprite>();
    public List<Sprite> timeBadgeList = new List<Sprite>();
    public List<Sprite> retryButtonBadgeList = new List<Sprite>();
    public List<Sprite> timeCountFreezeBadgeList = new List<Sprite>();

    public List<Image> levelDifficultInfoStars = new List<Image>();
    public List<Sprite> levelDifficultInfoSpriteList = new List<Sprite>();

    public List<Color32> difficultyInfoTextColorList = new List<Color32>();

    public GameObject levelDifficultyInfoObject;
    public GameObject levelDifficultyInfoStarsParent;

    [SerializeField] private TextMeshProUGUI levelDifficuiltyInfoText;


    private int FreezeTimeBoosterCount;
    private int PassangerBoosterCount;
    private int FreezeTimeBoosterPrice;
    private int PassangerBoosterPrice;



    public int time;
    StringBuilder stringBuilder = new StringBuilder();
    private Coroutine timerCoroutine;
    public bool isMoveFinish;
    private int extraTimeBoosterValue;

    public void Initialize()
    {
        Instance = this;

        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
        _eventBus.Subscribe<GameEvents.OnLevelLoaded>(OnLevelLoaded);
        _eventBus.Subscribe<GameEvents.OnLevelFailed>(OnLevelEnded);
        _eventBus.Subscribe<GameEvents.OnLevelCompleted>(OnLevelEnded);

        PassangerBoosterCount = PlayerPrefs.GetInt("unscrewBoosterCount", 2);
        FreezeTimeBoosterCount = PlayerPrefs.GetInt("extraMoveBoosterCount", 2);

        PassangerBoosterPrice = RemoteManager.instance.PassangerBoosterPrice;
        FreezeTimeBoosterPrice = RemoteManager.instance.FreezeTimeBoosterValue;

        SetBoosterButtons();
        SetBoosterPriceText();

        FreezeTimeBoosterButton.onClick.AddListener(FreezeTimeBoosterClicked);
        FreezeTimeBoosterBuyButton.onClick.AddListener(BuyFreezeTimeBoosterClicked);

        PassangerBoosterButton.onClick.AddListener(PassangerBoosterClicked);
        PassangerBoosterBuyButton.onClick.AddListener(BuyPassangerBoosterClicked);

    }
    public void StartTimer()
    {
        if (timerCoroutine != null)
            return;

        timerCoroutine = StartCoroutine(TimerCoroutine());
    }
    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }
    public IEnumerator TimerCoroutine()
    {
        SetTimeText(time);

        while (true)
        {
            yield return new WaitForSeconds(1f);
            --time;

            SetTimeText(time);

            if (time == 0)
            {
                StopTimer();
                _eventBus.Fire(new GameEvents.OnTimeOver());

                break;
            }
        }
    }
    public void SetTimeText(int time)
    {
        stringBuilder.Clear();
        stringBuilder.Append(string.Format("{0:00}:{1:00}", TimeSpan.FromSeconds(time).Minutes, TimeSpan.FromSeconds(time).Seconds));

        timeText.text = stringBuilder.ToString(); ;


        if (time <= 5)
        {
            timeText.transform.DOScale(1.5f, .25f).SetLoops(2, LoopType.Yoyo);
            timeText.DOColor(Color.red, .25f).SetLoops(2, LoopType.Yoyo);
        }
    }
    public void ResetTimeText()
    {
        DOTween.Kill(timeText);
        DOTween.Kill(timeText.transform);

        timeText.transform.localScale = Vector3.one;
        timeText.DOColor(Color.white, 0);
    }
    private void OnLevelLoaded(GameEvents.OnLevelLoaded p)
    {
        levelText.text = "Level "+(p.level+1).ToString();
        time = p.time;

        StartTimer();

        SetLevelDifficultInfoObject(p.difficulty);
        SetLevelBadge(p.difficulty);
        SetLevelDifficultInfoBg(p.difficulty);
        SetLevelDifficultInfoText(p.difficulty);
        SetPauseButton(p.difficulty);
        SetTimeBadge(p.difficulty);
        SetCoinBadge(p.difficulty);
        SetRetryButton(p.difficulty);
        SetHardnessBadge(p.difficulty);
        SetTimeCountFreezeImageBadge(p.difficulty);
    }
    private void OnLevelEnded()
    {
        StopTimer();
    }
    private void FreezeTimeBoosterClicked()
    {
        FreezeTimeBoosterCount--;
        SetBoosterButtons();

        StopTimer();

        float fillValue = 1f;

        FreezeScreen();
        freezeFillImage.gameObject.SetActive(true);
        timeCountFreezeImage.gameObject.SetActive(true);

        DOTween.To(() => fillValue, x => fillValue = x, 0, 5f).SetEase(Ease.Linear)
            .OnUpdate(() =>
        {
            freezeFillImage.fillAmount = fillValue;
        }).OnComplete(() =>
        {
            freezeFillImage.gameObject.SetActive(false);
            timeCountFreezeImage.gameObject.SetActive(false);
            UnFreezeScreen();
            StartTimer();
        });

    }
    private void FreezeScreen()
    {
        foreach (Transform item in FreezePanel.transform)
        {
            item.GetComponent<Image>().DOFade(1f, 1f).OnStart(() =>
            {
                item.GetComponent<Image>().DOFade(0f, 0f);
            });
        }
        FreezePanel.SetActive(true);
    }
    private void UnFreezeScreen()
    {
        foreach (Transform item in FreezePanel.transform)
        {
            item.GetComponent<Image>().DOFade(0f, 1f).OnStart(() =>
            {
                item.GetComponent<Image>().DOFade(1f, 0f);
            }).OnComplete(() =>
            {
                FreezePanel.SetActive(false);
            });
        }
        
    }
    private void BuyFreezeTimeBoosterClicked()
    {
        if (GameManager.Instance.coin >= FreezeTimeBoosterPrice)
        {
            GameManager.Instance.OnCoinSpent(FreezeTimeBoosterPrice);
         //   EventDataManager.SendVirtualCurrencyPaymentEvent("add_extramove_booster", "add_extramove_booster", 1, extraMoveBoosterPrice);
            AddFreezeTimeBooster(1);
            SaveBoosters();
            FreezeTimeBoosterClicked();
        }
        else
        {
            UIManager.Instance.OpenStore(true);
        }
    }
    private void PassangerBoosterClicked()
    {
        PassangerBoosterCount--;
        QueueManager.instance.PassangerBoosterClicked();
        SetBoosterButtons();
        SaveBoosters();
    }
    private void BuyPassangerBoosterClicked()
    {
        if (GameManager.Instance.coin >= PassangerBoosterPrice)
        {
            GameManager.Instance.OnCoinSpent(PassangerBoosterPrice);
         //   EventDataManager.SendVirtualCurrencyPaymentEvent("add_unscrew_booster", "add_unscrew_booster", 1, unscrewBoosterPrice);

            AddPassangerBooster(1);
            SaveBoosters();
            PassangerBoosterClicked();
        }
        else
        {
            UIManager.Instance.OpenStore(true);
        }
    }
    private void SetBoosterButtons()
    {
        FreezeTimeBoosterBuyButton.gameObject.SetActive(FreezeTimeBoosterCount <= 0);
        FreezeTimeBoosterCountObject.SetActive(!(FreezeTimeBoosterCount <= 0));
        FreezeTimeBoosterButton.interactable = !(FreezeTimeBoosterCount <= 0);
        FreezeTimeBoosterCountText.text = FreezeTimeBoosterCount.ToString();



        PassangerBoosterBuyButton.gameObject.SetActive(PassangerBoosterCount <= 0);
        PassangerBoosterCountObject.SetActive(!(PassangerBoosterCount <= 0));
        PassangerBoosterButton.interactable = !(PassangerBoosterCount <= 0);
        PassangerBoosterCountText.text = PassangerBoosterCount.ToString();


    }
    private void SetBoosterPriceText()
    {
        FreezeTimeBoosterBuyPriceText.text = FreezeTimeBoosterPrice.ToString();
        PassangerBoosterBuyPriceText.text = PassangerBoosterPrice.ToString();
    }
    public void AddPassangerBooster(int i)
    {
        PassangerBoosterCount += i;
        SetBoosterButtons();
        SaveBoosters();
    }
    public void AddFreezeTimeBooster(int i)
    {
        FreezeTimeBoosterCount += i;
        SetBoosterButtons();
        SaveBoosters();
    }
    public void SaveBoosters()
    {
        PlayerPrefs.SetInt("unscrewBoosterCount", PassangerBoosterCount);
        PlayerPrefs.SetInt("FreezeTimeBoosterCount", FreezeTimeBoosterCount);
    }
    private void SetHardnessBadge(int difficulty)
    {
        if (difficulty == 0)
        {
            hardnessBadge.gameObject.SetActive(false);
            return;
        }
        else
        {
            if (difficulty == 1)
            {
                hardnessText.text = "Hard";
            }
            else
            {
                hardnessText.text = "Impossible";
            }
        }

        hardnessBadge.sprite = levelBadgeList[difficulty];
    }
    private void SetTimeBadge(int difficulty)
    {
        timeBadge.sprite = timeBadgeList[difficulty];
    }
    private void SetTimeCountFreezeImageBadge(int difficulty)
    {
        timeCountFreezeImage.sprite = timeCountFreezeBadgeList[difficulty];
    }
    private void SetRetryButton(int difficulty)
    {
        retryButtonBadge.sprite = retryButtonBadgeList[difficulty];
    }
    private void SetPauseButton(int difficulty)
    {
        pauseButtonBadge.sprite = pauseButtonBadgeList[difficulty];
    }
    private void SetCoinBadge(int difficulty)
    {
        coinBadge.sprite = levelBadgeList[difficulty];
    }
    private void SetLevelBadge(int difficulty)
    {
        levelBadge.sprite = levelBadgeList[difficulty];
    }
    private void SetLevelDifficultInfoBg(int difficulty)
    {
        levelDifficultyInfoBg.sprite = levelDifficultyInfoBgList[difficulty];
    }
    private void SetLevelDifficultSprite(int difficulty)
    {
        levelDifficultyInfoImage.sprite = levelDifficultInfoSpriteList[difficulty];
    }
    private void SetLevelDifficultInfoText(int difficulty)
    {
        if (difficulty == 1)
            levelDifficuiltyInfoText.text = "HARD\nLEVEL";
        if (difficulty == 2)
            levelDifficuiltyInfoText.text = "IMPOSSIBLE\nLEVEL";

        levelDifficuiltyInfoText.color = difficultyInfoTextColorList[difficulty];

    }
    private void SetLevelDifficultInfoObject(int difficulty)
    {
        SetLevelDifficultSprite(difficulty);

        if (difficulty == 1 || difficulty == 2)
        {
            levelDifficultyInfoBg.transform.DOKill();
            levelDifficultyInfoObject.transform.localScale = Vector3.zero;
            levelDifficultyInfoStarsParent.transform.localScale = Vector3.one * .5f;
            levelDifficultyInfoStarsParent.SetActive(false);

            foreach (var item in levelDifficultInfoStars)
            {
                item.transform.localScale = Vector3.one;
            }

            levelDifficultyInfoObject.SetActive(true);

            levelDifficultyInfoObject.transform.DOScale(Vector3.one * 1.25f, 1f).SetEase(Ease.Linear);
            levelDifficultyInfoBg.transform.DOLocalRotate(new Vector3(0, 0, -360F), 3F, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);


            levelDifficultyInfoObject.transform.DOScale(Vector3.one * 1f, .25f).SetDelay(1f).SetEase(Ease.Linear);
            levelDifficultyInfoStarsParent.transform.DOScale(Vector3.one * 1.25f, 1f).SetDelay(.25f).SetEase(Ease.Linear).OnStart(() =>
            {
                levelDifficultyInfoStarsParent.SetActive(true);
            });

            foreach (var item in levelDifficultInfoStars)
            {
                item.transform.DOScale(Vector3.zero, .65f).SetDelay(1.35f).SetEase(Ease.Linear);
            }

            levelDifficultyInfoObject.transform.DOScale(Vector3.zero, .6f).SetDelay(2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                levelDifficultyInfoObject.SetActive(false);
            });
        }
    }
}
