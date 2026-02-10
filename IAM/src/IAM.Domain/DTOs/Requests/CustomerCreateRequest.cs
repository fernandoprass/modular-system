public sealed record CustomerCreateRequest
{
    public CustomerType Type { get; set; }
    public string CustomerName { get; set; }
    public string CustomerCode { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string UserPassword { get; set; }
}