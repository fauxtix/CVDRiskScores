using CVDRiskScores.Enums;

namespace CVDRiskScores.Models.SCORE2;

public class Score2Model
{
    // make numeric fields nullable so Entry placeholder shows until user types
    public int? Age { get; set; }
    public Genero Gender { get; set; }
    public int? SystolicBloodPressure { get; set; }
    public int? TotalCholesterol { get; set; }
    public int? HDLCholesterol { get; set; }
    public bool IsSmoker { get; set; }

    public int? NonHDLCholesterol => (TotalCholesterol.HasValue && HDLCholesterol.HasValue)
        ? TotalCholesterol.Value - HDLCholesterol.Value
        : null;

    public double AgePoints { get; set; }
    public double NonHDLPoints { get; set; }
    public double SBPPoints { get; set; }
    public double SmokingPoints { get; set; }
    public double RiskScore { get; set; }

    public string RiskCategory { get; set; } = string.Empty;
    public Color RiskColor { get; set; } = Colors.DarkGreen;
    public string ClinicalAdvice { get; set; } = string.Empty;
    public string? ValidationError { get; set; }
}