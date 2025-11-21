using System;
using Microsoft.Extensions.DependencyInjection;
using CVDRiskScores.MVVM.ViewModels.SCORE2;
using CVDRiskScores.MVVM.Views.SCORE2;

namespace CVDRiskScores.MVVM.Services.SCORE2
{
    public class Score2LearnMoreFactory
    {
        readonly IServiceProvider _sp;
        public Score2LearnMoreFactory(IServiceProvider sp) => _sp = sp;

        public Score2LearnMorePopup Create(string htmlOrText, string dataVersion)
        {
            var vm = _sp.GetRequiredService<Score2LearnMoreViewModel>();
            vm.Initialize(htmlOrText, dataVersion);
            return new Score2LearnMorePopup(vm);
        }
    }
}
