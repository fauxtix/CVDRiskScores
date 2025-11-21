using CommunityToolkit.Maui.Views;
using CVDRiskScores.MVVM.ViewModels.SCORE2;

namespace CVDRiskScores.MVVM.Views.SCORE2;

public partial class Score2LearnMorePopup : Popup
{
    public Score2LearnMorePopup(Score2LearnMoreViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        TryLimitToScreen();

        // entry animation (optional)
        this.Opacity = 0;
        this.Scale = 0.8;
        _ = AnimatePopup();
    }

    void TryLimitToScreen()
    {
        try
        {
            var info = DeviceDisplay.MainDisplayInfo;
            var density = info.Density;
            var screenWidth = info.Width / density;
            var screenHeight = info.Height / density;
            var maxW = screenWidth * 0.8;
            var maxH = screenHeight * 0.8;

            if (RootBorder != null)
            {
                RootBorder.WidthRequest = Math.Min(400, (double)maxW);
                RootBorder.MaximumHeightRequest = (double)maxH;
            }
        }
        catch { }
    }

    private async Task AnimatePopup()
    {
        await this.FadeTo(1, 250, Easing.CubicOut);
        await this.ScaleTo(1, 250, Easing.CubicOut);
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        await this.ScaleTo(0.8, 150, Easing.CubicIn);
        await this.FadeTo(0, 150, Easing.CubicIn);
        await CloseAsync();
    }
}
