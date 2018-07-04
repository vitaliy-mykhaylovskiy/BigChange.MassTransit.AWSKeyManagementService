namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
	public class UserImpl : User
	{
		public UserImpl(string name, string email)
		{
			Name = name;
			Email = email;
		}

		protected UserImpl()
		{
		}

		public string Name { get; set; }

		public string Email { get; set; }

		public bool Equals(User other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return Equals(other.Name, Name) && Equals(other.Email, Email);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (!typeof(User).IsAssignableFrom(obj.GetType()))
				return false;
			return Equals((User)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Email != null ? Email.GetHashCode() : 0);
			}
		}
	}
}