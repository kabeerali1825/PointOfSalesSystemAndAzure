﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PointOfSales.Entities
{
    public class Users
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }=string.Empty;

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }= string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }=string.Empty ;

        [Required(ErrorMessage = "UserRole is required")]
        public string UserRole { get; set; }=string.Empty;

        // Parameterless constructor
        public Users()
        {
        }

        // JsonConstructor
        //[JsonConstructor]
        protected Users(string name, string email, string password, string userRole)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            UserRole = userRole ?? throw new ArgumentNullException(nameof(userRole));
        }

        public string GetName() => Name;
        public void SetName(string name) => Name = name;
        public string GetEmail() => Email;
        public void SetEmail(string email) => Email = email;
        public string GetPassword() => Password;
        public void SetPassword(string password) => Password = password;
        public void SetUserRole(string userRole) => UserRole = userRole;

        public override string ToString() => $"Name: {Name}, Email: {Email}, UserRole: {UserRole}";
    }
}
