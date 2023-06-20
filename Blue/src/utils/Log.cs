using System.Diagnostics;

namespace BlueFw.Utils;

public static class Log {

    public static void Message(string message) {
        Debug.WriteLine($"Message: {message}");
    }

    public static void Warning(string message) {
        Debug.WriteLine($"Warning: {message}");
    }

    public static void Error(string message) {
        Debug.WriteLine($"Error: {message}");
    }
}
