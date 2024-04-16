using UnityEngine;


public class GameEvents : MonoBehaviour
{

    public struct OnItemClicked : IEvent { }
    public struct OnItemUnclicked : IEvent { }
    public struct OnSplashScreenFinished : IEvent { }

    public struct OnTimeOver : IEvent { }
    public struct OnLevelCompleted : IEvent { }
    public struct OnLevelFailed : IEvent { }
    public struct OnLevelGiveUp : IEvent { }
    public struct OnLevelEnded : IEvent { }
    public struct OnPassangerLand : IEvent { }
    //  public struct OnLevelQuit : IEvent { }
    //  public struct OnSecondChanceButtonPressed : IEvent { }
    //  public struct OnSplashScreenFinished : IEvent { }


    public struct OnLevelLoaded : IEvent
    {
        public int level;
        public int time;
        public int difficulty;

        public OnLevelLoaded(int level, int time, int difficulty)
        {
            this.level = level;
            this.time = time;
            this.difficulty = difficulty;
        }
    }
    public struct OnCoinChanged : IEvent {

        public int coin;

        public OnCoinChanged(int coin)
        {
            this.coin = coin;
        }
    }
    public struct OnLevelStarted : IEvent
    {
        public int numberOfStage;
        public int difficulty;

        public OnLevelStarted(int numberOfStage,int difficulty)
        {
            this.numberOfStage = numberOfStage;
            this.difficulty = difficulty;
        }
    }


}
