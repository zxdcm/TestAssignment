namespace Feed.Core;

public class FeedException(string message, Exception ex) : Exception(message, ex);