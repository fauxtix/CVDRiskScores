using CommunityToolkit.Maui;
using CVDRiskScores.MVVM.ViewModels.Framingham;
using CVDRiskScores.MVVM.ViewModels.SCORE2;
using CVDRiskScores.MVVM.Views.Framingham;
using CVDRiskScores.MVVM.Views.SCORE2;
using CVDRiskScores.Services.Framingham;
using CVDRiskScores.Services.SCORE2;
using Microsoft.Extensions.Logging;

namespace CVDRiskScores
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                }).UseMauiCommunityToolkit();
            ;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<FraminghamIntroPage>();
            builder.Services.AddSingleton<FraminghamRiskScorePage>();
            builder.Services.AddSingleton<FraminghamResultsPage>();

            builder.Services.AddSingleton<FraminghamRiskScoreViewModel>();
            builder.Services.AddSingleton<IFRS_Service, FRS_Service>();

            builder.Services.AddTransient<ISCORE2_Service, SCORE2_Service>();
            builder.Services.AddTransient<Score2ViewModel>();
            builder.Services.AddTransient<Score2RiskScorePage>();

            return builder.Build();
        }
    }
}
