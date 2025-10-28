﻿using FluentResults;

namespace Domain.Errors;

public class ConflictError : Error
{
    public ConflictError(string message) : base(message)
    {
    }
}