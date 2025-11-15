namespace CVDRiskScores.Services.Popup
{
    public interface IUIPopupService
    {
        Task ShowSimulationResultAsync(object model, string title = "Resultado", string subtitle = "", string badge = "");
    }
}
