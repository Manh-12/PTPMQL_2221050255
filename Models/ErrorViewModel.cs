namespace DemoMvc.Models; // Đảm bảo ghi đúng chữ namespace có chữ n ở đầu

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
