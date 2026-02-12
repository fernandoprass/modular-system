public sealed record CustomerCreateRequest
{
    public CustomerType Type { get; init; }
    public string CustomerName { get; init; }
    public string CustomerCode { get; init; }
    public CustomerUserCreateResquest User  { get; init; }
}