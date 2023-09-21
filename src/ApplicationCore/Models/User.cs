using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Models;

public class User : IdentityUser, IAggregateRoot
{	
	public string Name { get; set; } = String.Empty;
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public virtual RefreshToken? RefreshToken { get; set; }

	public virtual ICollection<OAuth>? OAuthList { get; set; }

}
