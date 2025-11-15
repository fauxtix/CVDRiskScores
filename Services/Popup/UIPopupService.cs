using CommunityToolkit.Maui.Extensions;
using CVDRiskScores.MVVM.Views.Shared;
using CVDRiskScores.Services.Popup;

namespace CVDRiskScores.Services.UI
{
    public class UIPopupService : IUIPopupService
    {
        public async Task ShowSimulationResultAsync(object model, string title = "Resultado", string subtitle = "", string badge = "")
        {
            if (model == null) return;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var popup = new SimulationResultPopupGeneric(model, title: title, subtitle: subtitle, badge: badge);
                // show/await on UI thread
                await Shell.Current.ShowPopupAsync(popup);
            });
        }
    }
}
