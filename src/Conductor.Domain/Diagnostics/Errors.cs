using ErrorOr;

namespace Conductor.Domain.Diagnostics;

public static partial class DomainErrors
{
    public static partial class Process
    {
        public static readonly Error NameCanNotBeNullOrWhitespace =
        Error.Validation(
            code: "10_001",
            description: "Name can not be null or whitespace");

        public static readonly Error NameLengthMustBeGreater2 =
            Error.Validation(
                "10_002",
                "Name length must be greater 2");

        public static readonly Error NameFirstCharMustBeLetter =
            Error.Validation(
                "10_003",
                "Name first char must be letter");

        public static readonly Error NameAllCharsMustBeLetterOrDigitOrUnderscore =
            Error.Validation(
                "10_004",
                "Name all chars must be letter or digit or underscore");

        public static readonly Error DisplaynameCanNotBeNullOrWhitespace =
            Error.Validation(
                "10_005",
                "Displayname can not be null or whitespace");

        public static readonly Error DisplaynameLengthMustBeGreater2 =
            Error.Validation(
                "10_006",
                "Displayname length must be greater 2");

        public static readonly Error NowValueIsOutOfRange =
            Error.Validation(
                "10_007",
                "Now value is out of range");

        public static readonly Error ProcessNumberValueIsOutOfRange =
            Error.Validation(
                "10_008",
                "Process number value is out of range");
    }
}
