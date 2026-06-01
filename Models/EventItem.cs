using System;
using System.ComponentModel.DataAnnotations;

namespace EventEaseApp.Models
{
    public class EventItem
    {
        private const int MinNameLength = 1;
        private const int MaxNameLength = 100;
        private const int MinLocationLength = 1;
        private const int MaxLocationLength = 150;

        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(MaxNameLength, MinimumLength = MinNameLength, 
            ErrorMessage = "Event name must be between 1 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-zA-Z0-9]).*$", 
            ErrorMessage = "Event name must contain at least one letter or number.")]
        public string Name { get; set; } = string.Empty;
        
        [Range(typeof(DateTime), "1900-01-01", "2099-12-31", 
            ErrorMessage = "Event date must be a valid date.")]
        public DateTime Date { get; set; } = DateTime.Today;
        
        [Required(ErrorMessage = "Location is required.")]
        [StringLength(MaxLocationLength, MinimumLength = MinLocationLength, 
            ErrorMessage = "Location must be between 1 and 150 characters.")]
        public string Location { get; set; } = string.Empty;
        
        public int AttendanceCount { get; private set; }
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Registers a new attendee for the event.
        /// </summary>
        public void RegisterAttendance() => AttendanceCount++;

        /// <summary>
        /// Validates the event item data
        /// </summary>
        /// <returns>True if the event item is valid; otherwise false</returns>
        public bool IsValid()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        }

        /// <summary>
        /// Gets all validation errors for this event item
        /// </summary>
        /// <returns>List of validation error messages</returns>
        public IEnumerable<string> GetValidationErrors()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, results, validateAllProperties: true);
            return results.Select(r => r.ErrorMessage ?? "Unknown error");
        }

        /// <summary>
        /// Marks the event as completed
        /// </summary>
        public void MarkComplete() => IsCompleted = true;

        /// <summary>
        /// Sanitizes and validates input strings
        /// </summary>
        public void SanitizeInputs()
        {
            Name = Name?.Trim() ?? string.Empty;
            Location = Location?.Trim() ?? string.Empty;
        }
    }
}
