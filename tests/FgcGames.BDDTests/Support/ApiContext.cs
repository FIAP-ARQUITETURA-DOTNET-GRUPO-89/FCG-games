namespace FgcGames.BDDTests.Support;

public class ApiContext
{
    public HttpClient Client { get; set; } = default!;
    public HttpResponseMessage? LastResponse { get; set; }
    public string? Token { get; set; }
    public Guid LastCreatedId { get; set; }
}
