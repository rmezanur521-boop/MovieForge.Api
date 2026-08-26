namespace MovieForge.Api.Models;

public class TrailerSpec
{
    public string AnimationStyle { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public int DurationMs { get; set; }
    public string MusicTrack { get; set; } = string.Empty;
    public string CameraStyle { get; set; } = string.Empty;
    public int RayCount { get; set; }
    public double RaySpin { get; set; }
    public double SweepAngleDeg { get; set; }
}