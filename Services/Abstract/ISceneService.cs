using System.Threading.Tasks;

public interface ISceneService : IAutoLoad
{
    public void ToMainMenu(bool skipTransitions = false);
    public void InitService();

    public Task SetCurrentStage(
        StageConfiguration stageConfiguration,
        bool transitionOut = true,
        bool transitionIn = true);

    public Stage GetCurrentStage();

    public StageConfiguration GetCurrentStageConfiguration();
}