namespace PayrollSystem.Domain.ValueObjects;

public class EmployeeNumberGenerator
{
    public static string Generate(string employeeName, DateTime dateOfBirth)
    {
        // Get first 3 letters of the name (uppercase)
        var namePrefix = GetNamePrefix(employeeName);
        
        // Generate 5-digit random number
        var randomNumber = new Random().Next(0, 100000).ToString("D5");
        
        // Format date of birth as ddMMMyyyy (e.g., 10JAN1994)
        var dateFormatted = dateOfBirth.ToString("ddMMMyyyy").ToUpper();
        
        return $"{namePrefix}-{randomNumber}-{dateFormatted}";
    }

    private static string GetNamePrefix(string name)
    {
        // Remove special characters and get only letters
        var cleanName = new string(name.Where(char.IsLetter).ToArray());
        
        // Get first 3 letters, uppercase
        return cleanName.Length >= 3 
            ? cleanName.Substring(0, 3).ToUpper() 
            : cleanName.ToUpper().PadRight(3, 'X');
    }
}
