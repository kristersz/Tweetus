using System;
using System.Collections.Generic;
using System.Security.Claims;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNet.Identity3.MongoDB
{
    public class IdentityUser
    {
        public IdentityUser()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        public IdentityUser(string userName) : this()
        {
            UserName = userName;
        }

        /// <summary>
        /// User Id
        /// </summary>
        [BsonId]
        public virtual string Id { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [BsonElement("UserName")]
        public virtual string UserName { get; set; }
        [BsonElement("NormalizedUserName")]
        public virtual string NormalizedUserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [BsonElement("Email")]
        public virtual string Email { get; set; }
        [BsonElement("NormalizedEmail")]
        public virtual string NormalizedEmail { get; set; }

        /// <summary>
        /// True if the email is confirmed, default is false
        /// </summary>
        [BsonElement("EmailConfirmed")]
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// The salted/hashed form of the user password
        /// </summary>
        [BsonElement("PasswordHash")]
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// A random value that should change whenever a users credentials change (password changed, login removed)
        /// </summary>
        [BsonElement("SecurityStamp")]
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        /// A random value that should change whenever a user is persisted to the store
        /// </summary>
        [BsonElement("ConcurrencyStamp")]
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// PhoneNumber for the user
        /// </summary>
        [BsonElement("PhoneNumber")]
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// True if the phone number is confirmed, default is false
        /// </summary>
        [BsonElement("PhoneNumberConfirmed")]
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Is two factor enabled for the user
        /// </summary>
        [BsonElement("TwoFactorEnabled")]
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        [BsonElement("LockoutEnd")]
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Is lockout enabled for this user
        /// </summary>
        [BsonElement("LockoutEnabled")]
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// Used to record failures for the purposes of lockout
        /// </summary>
        [BsonElement("AccessFailedCount")]
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// Navigation property for users in the role
        /// </summary>
        [BsonElement("Roles")]
        public virtual List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();

        /// <summary>
        /// Navigation property for users claims
        /// </summary>
        [BsonElement("Claims")]
        public virtual List<IdentityClaim> Claims { get; set; } = new List<IdentityClaim>();

        /// <summary>
        /// Navigation property for users logins
        /// </summary>
        [BsonElement("Logins")]
        public virtual List<IdentityUserLogin> Logins { get; set; } = new List<IdentityUserLogin>();

        public virtual void AddClaim(Claim claim)
        {
            Claims.Add(new IdentityClaim(claim));
        }

        public virtual void RemoveClaim(Claim claim)
        {
            Claims.RemoveAll(c =>
                c.ClaimType == claim.Type &&
                c.ClaimValue == claim.Value);
        }

        public virtual void ReplaceClaim(Claim claim, Claim newClaim)
        {
            foreach (var userClaim in Claims)
            {
                if (userClaim.ClaimType == claim.Type &&
                    userClaim.ClaimValue == claim.Value)
                {
                    userClaim.ClaimType = newClaim.Type;
                    userClaim.ClaimValue = newClaim.Value;
                }
            }
        }
    }
}