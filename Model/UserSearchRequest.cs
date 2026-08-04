public class UserSearchRequest
{
	public string? Search { get; set; }

	public int? DepartmentId { get; set; }

	public string? Role { get; set; }

	public string? Status { get; set; }

	public string? SortBy { get; set; }

	public string? SortOrder { get; set; }

	public int Page { get; set; } = 1;

	public int PageSize { get; set; } = 10;
}