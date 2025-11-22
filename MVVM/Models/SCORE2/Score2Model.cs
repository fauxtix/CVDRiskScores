using CVDRiskScores.Enums;

namespace CVDRiskScores.Models.SCORE2;

public class Score2Model
{
    public int? Age { get; set; }
    public Genero Gender { get; set; }
    public int? SystolicBloodPressure { get; set; }

    public double? TotalCholesterol { get; set; }
    public double? HDLCholesterol { get; set; }
    public bool IsSmoker { get; set; }

    public double? NonHDLCholesterol => (TotalCholesterol.HasValue && HDLCholesterol.HasValue)
        ? TotalCholesterol.Value - HDLCholesterol.Value
        : (double?)null;

    public int AgePoints { get; set; }
    public int NonHDLPoints { get; set; }
    public int SBPPoints { get; set; }
    public int SmokingPoints { get; set; }
    public double RiskScore { get; set; }

    public string RiskCategory { get; set; } = string.Empty;
    public Color RiskColor { get; set; } = Colors.DarkGreen;
    public string ClinicalAdvice { get; set; } = string.Empty;
    public string? ValidationError { get; set; }

    public string CalibrationKey { get; set; } = "Low";

    public object? ScoreDetails { get; set; }

    public bool ForceRealCalculation { get; set; } = false;
}