using System;
using System.Reflection;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
    public class ComplaintAddedImpl :
        ComplaintAdded
    {
        public ComplaintAddedImpl(User addedBy, string subject, BusinessArea area)
        {
            var dateTime = DateTime.UtcNow;
            AddedAt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
                dateTime.Second,
                dateTime.Millisecond, DateTimeKind.Utc);

            AddedBy = addedBy;
            Subject = subject;
            Area = area;
            Body = string.Empty;
        }

        protected ComplaintAddedImpl()
        {
        }

        public User AddedBy { get; set; }

        public DateTime AddedAt { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public BusinessArea Area { get; set; }

        public bool Equals(ComplaintAdded other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return AddedBy.Equals(other.AddedBy) && other.AddedAt.Equals(AddedAt) && Equals(other.Subject, Subject) &&
                   Equals(other.Body, Body)
                   && Equals(other.Area, Area);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!typeof(ComplaintAdded).GetTypeInfo().IsAssignableFrom(obj.GetType()))
                return false;
            return Equals((ComplaintAdded) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = AddedBy != null ? AddedBy.GetHashCode() : 0;
                result = (result * 397) ^ AddedAt.GetHashCode();
                result = (result * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                result = (result * 397) ^ (Body != null ? Body.GetHashCode() : 0);
                result = (result * 397) ^ Area.GetHashCode();
                return result;
            }
        }
    }
}