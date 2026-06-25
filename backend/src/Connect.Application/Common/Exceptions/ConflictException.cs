namespace Connect.Application.Common.Exceptions;

/// <summary>Thrown when a request conflicts with current state (e.g. duplicate). Mapped to HTTP 409.</summary>
public class ConflictException(string message) : Exception(message);
