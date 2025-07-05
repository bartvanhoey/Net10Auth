using Net10Auth.Shared.Infrastructure;

public static class HttpContextExtensions
{
    public static string GetUserEmailFromAuthorizationHeader(this HttpContext context, ITokenHandler tokenHandler)
    {
        var userEmail = Empty;
        // Extract JWT token from the Authorization header
        var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ")) 
            userEmail = tokenHandler.GetEmailFromToken(authorizationHeader["Bearer ".Length..].Trim());
        return userEmail;
    }
}