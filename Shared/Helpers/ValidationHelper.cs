using System.Text.RegularExpressions;

namespace Shared.Helpers
{
    public static class ValidationHelper
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new Regex(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$", RegexOptions.Compiled);
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", RegexOptions.Compiled);

        public static bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && EmailRegex.IsMatch(email);
        }

        public static bool IsValidPhone(string phone)
        {
            return !string.IsNullOrEmpty(phone) && PhoneRegex.IsMatch(phone);
        }

        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && PasswordRegex.IsMatch(password);
        }

        public static bool PasswordsMatch(string password, string confirmPassword)
        {
            return !string.IsNullOrEmpty(password) && password == confirmPassword;
        }

        public static bool IsValidUsername(string username)
        {
            return !string.IsNullOrEmpty(username) && username.Length >= 3 && username.Length <= 50;
        }

        public static bool IsValidReservationDate(DateTime date)
        {
            return date.Date >= DateTime.Today;
        }

        public static bool IsValidReservationTime(TimeSpan time)
        {
            // Assuming restaurant operates from 10:00 to 22:00
            return time >= new TimeSpan(10, 0, 0) && time <= new TimeSpan(22, 0, 0);
        }

        public static bool IsValidPartySize(int partySize)
        {
            return partySize >= 1 && partySize <= 20;
        }

        public static bool IsValidPrice(decimal price)
        {
            return price > 0 && price <= 1000;
        }

        public static List<string> ValidateLoginRequest(string username, string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(username))
                errors.Add("Username is required.");

            if (string.IsNullOrEmpty(password))
                errors.Add("Password is required.");

            return errors;
        }

        public static List<string> ValidateRegisterRequest(string username, string email, string phone, string password, string confirmPassword)
        {
            var errors = new List<string>();

            if (!IsValidUsername(username))
                errors.Add("Username must be between 3 and 50 characters.");

            if (!IsValidEmail(email))
                errors.Add("Invalid email format.");

            if (!IsValidPhone(phone))
                errors.Add("Invalid phone number format.");

            if (!IsValidPassword(password))
                errors.Add("Password must contain at least 6 characters, including uppercase, lowercase, and numbers.");

            if (!PasswordsMatch(password, confirmPassword))
                errors.Add("Passwords do not match.");

            return errors;
        }

        public static List<string> ValidateReservationRequest(DateTime date, TimeSpan time, int partySize)
        {
            var errors = new List<string>();

            if (!IsValidReservationDate(date))
                errors.Add("Reservation date must be today or a future date.");

            if (!IsValidReservationTime(time))
                errors.Add("Reservation time must be between 10:00 AM and 10:00 PM.");

            if (!IsValidPartySize(partySize))
                errors.Add("Party size must be between 1 and 20 people.");

            return errors;
        }
    }
}