using Picpay.Enums;

namespace Picpay.Models;

public class User
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Identifier { get; set; }
    public string Email { get; set; }
    public string Password { get; private set; }
    public UserType Type { get; init; }
    public decimal Balance { get; set; }

    public User(string name, string email, string password, UserType type, string identifier)
    {
        Id = Guid.NewGuid();
        Name = name;
        Identifier = identifier;
        Email = email;
        Password = BCrypt.Net.BCrypt.HashPassword(password);
        Type = type;
        Balance = 0;
    }

    public void Credit(decimal amount)
    {
        Balance += amount;
    }

    public void Debit(decimal amount)
    {
        if (Balance - amount < 0)
            return;
        Balance -= amount;
    }
}