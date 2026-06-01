using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EventEaseApp.Models;

namespace EventEaseApp.Data
{
    /// <summary>
    /// Helper class for validating event data
    /// Provides centralized validation logic for event items
    /// </summary>
    public static class EventValidationHelper
    {
        // Validation constraints
        public const int MinNameLength = 1;
        public const int MaxNameLength = 100;
        public const int MinLocationLength = 1;
        public const int MaxLocationLength = 150;
        public static readonly DateTime MinEventDate = DateTime.Today.AddYears(-1);
        public static readonly DateTime MaxEventDate = DateTime.Today.AddYears(10);

        /// <summary>
        /// Validates an event name
        /// </summary>
        /// <param name="name">The event name to validate</param>
        /// <returns>Validation error message, or null if valid</returns>
        public static string? ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Event name is required.";

            var trimmed = name.Trim();

            if (trimmed.Length < MinNameLength)
                return $"Event name must have at least {MinNameLength} character.";

            if (trimmed.Length > MaxNameLength)
                return $"Event name cannot exceed {MaxNameLength} characters.";

            if (!trimmed.Any(c => char.IsLetterOrDigit(c)))
                return "Event name must contain at least one letter or number.";

            return null; // Valid
        }

        /// <summary>
        /// Validates a location
        /// </summary>
        /// <param name="location">The location to validate</param>
        /// <returns>Validation error message, or null if valid</returns>
        public static string? ValidateLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return "Location is required.";

            var trimmed = location.Trim();

            if (trimmed.Length < MinLocationLength)
                return $"Location must have at least {MinLocationLength} character.";

            if (trimmed.Length > MaxLocationLength)
                return $"Location cannot exceed {MaxLocationLength} characters.";

            return null; // Valid
        }

        /// <summary>
        /// Validates an event date
        /// </summary>
        /// <param name="date">The date to validate</param>
        /// <returns>Validation error message, or null if valid</returns>
        public static string? ValidateDate(DateTime date)
        {
            if (date < MinEventDate || date > MaxEventDate)
                return $"Date must be between {MinEventDate:yyyy-MM-dd} and {MaxEventDate:yyyy-MM-dd}.";

            return null; // Valid
        }

        /// <summary>
        /// Validates a complete event item
        /// </summary>
        /// <param name="eventItem">The event item to validate</param>
        /// <returns>Dictionary of field names to validation error messages</returns>
        public static Dictionary<string, string> ValidateEventItem(EventItem eventItem)
        {
            var errors = new Dictionary<string, string>();

            var nameError = ValidateName(eventItem.Name);
            if (nameError != null)
                errors["Name"] = nameError;

            var locationError = ValidateLocation(eventItem.Location);
            if (locationError != null)
                errors["Location"] = locationError;

            var dateError = ValidateDate(eventItem.Date);
            if (dateError != null)
                errors["Date"] = dateError;

            return errors;
        }

        /// <summary>
        /// Checks if an event item is valid
        /// </summary>
        /// <param name="eventItem">The event item to validate</param>
        /// <returns>True if valid; otherwise false</returns>
        public static bool IsEventValid(EventItem eventItem)
        {
            return ValidateEventItem(eventItem).Count == 0;
        }

        /// <summary>
        /// Sanitizes event data by trimming strings
        /// </summary>
        /// <param name="eventItem">The event item to sanitize</param>
        public static void SanitizeEvent(EventItem eventItem)
        {
            eventItem.Name = eventItem.Name?.Trim() ?? string.Empty;
            eventItem.Location = eventItem.Location?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Validates and sanitizes an event item
        /// </summary>
        /// <param name="eventItem">The event item to validate and sanitize</param>
        /// <returns>Dictionary of validation errors, or empty if valid</returns>
        public static Dictionary<string, string> ValidateAndSanitize(EventItem eventItem)
        {
            SanitizeEvent(eventItem);
            return ValidateEventItem(eventItem);
        }
    }
}
