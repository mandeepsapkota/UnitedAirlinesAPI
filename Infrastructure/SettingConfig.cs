namespace UnitedAirlinesAPI.Infrastructure;

public class APIConfig
{
    public string APIKey { get; set; }
    public string MockUrl { get; set; }

    public int ServiceTimeoutMin { get; set; }

    public string CertificateName { get; set; }

    public string CertificatePassword { get; set; }
}
