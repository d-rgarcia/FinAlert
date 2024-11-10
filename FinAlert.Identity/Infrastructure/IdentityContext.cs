using FinAlert.Identity.Core.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinAlert.Identity.Infrastructure;

public class IndentityContext : IdentityDbContext<User>
{
    public IndentityContext(DbContextOptions<IndentityContext> options) : base(options)
    {
    }
}
