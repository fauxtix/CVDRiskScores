using CommunityToolkit.Maui.Views;
using CVDRiskScores.MVVM.ViewModels.SCORE2;

namespace CVDRiskScores.MVVM.Views.SCORE2;

public partial class CalibrationExamplesPopup : Popup
{
    public CalibrationExamplesPopup(CalibrationExamplesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        TryLimitToScreen();

        // Animação de entrada
        this.Opacity = 0;
        this.Scale = 0.8;
        this.AnimatePopup();
    }

    void TryLimitToScreen()
    {
        try
        {
            var info = DeviceDisplay.MainDisplayInfo;
            var density = info.Density; // pixels per DIP
            var screenWidth = info.Width / density;
            var screenHeight = info.Height / density;

            // limit to 90% of available screen space
            var maxW = screenWidth * 0.9;
            var maxH = screenHeight * 0.9;

            if (RootFrame != null)
            {
                RootFrame.MaximumWidthRequest = maxW;
                RootFrame.MaximumHeightRequest = maxH;
            }
        }
        catch { }
    }

    private async void AnimatePopup()
    {
        await this.FadeTo(1, 250, Easing.CubicOut);
        await this.ScaleTo(1, 250, Easing.CubicOut);
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        // Animação de saída
        await this.ScaleTo(0.8, 150, Easing.CubicIn);
        await this.FadeTo(0, 150, Easing.CubicIn);
        await CloseAsync();
    }
}
