namespace PicpaySimplificado.DTO;

public record ApiResponse<T>(
    bool Success,
    string? Message = null,
    int StatusCode = 200,
    T? Data = default
);
