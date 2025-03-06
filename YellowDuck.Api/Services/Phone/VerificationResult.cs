namespace YellowDuck.Api.Services.Phone
{
  public class VerificationResult
  {
    public bool Success { get; set; }
    public string ErrorCode { get; set; }

    public static VerificationResult Succeeded() => new VerificationResult { Success = true };
    public static VerificationResult Failed(string errorCode) => new VerificationResult
    {
      Success = false,
      ErrorCode = errorCode
    };
  }
}