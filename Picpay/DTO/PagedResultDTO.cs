namespace Picpay.DTO;

public record PagedResultDTO(int Page, int PageSize, int TotalItems, int TotalPages);