﻿namespace Eshop.Infrastructure.Events.User
{
    public class UserCreated
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Password { get; set; }
    }
}
