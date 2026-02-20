using IAM.Domain.DTOs.Requests;

public sealed record CustomerCreateRequest
{
    public CustomerType Type { get; init; }
    public string Name { get; init; }
    public string Code { get; init; }
    public CustomerUserCreateRequest User  { get; init; }
}