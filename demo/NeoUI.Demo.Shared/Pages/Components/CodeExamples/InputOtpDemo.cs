namespace NeoUI.Demo.Shared.Pages.Components
{
    partial class InputOtpDemo
    {

        private static readonly IReadOnlyList<DemoPropRow> _inputOtpProps =
            [
                new("Length",      "int",                    "6",      "Total number of OTP character slots."),
                new("Value",       "string",                 "\"\"",   "The current OTP value. Supports two-way binding with @bind-Value."),
                new("Pattern",     "string?",                null,     "Regex pattern to validate each character input."),
                new("Disabled",    "bool",                   "false",  "When true, the input cannot be interacted with."),
                new("AriaInvalid", "bool",                   "false",  "When true, applies destructive (red) error styling to all slots."),
                new("OnComplete",  "EventCallback<string>",  null,     "Fired when all slots have been filled."),
            ];

        private const string _basicOtpCode =
                """
                <InputOtp Length="6" OnComplete="HandleComplete">
                    <InputOtpGroup>
                        <InputOtpSlot Index="0" />
                        <InputOtpSlot Index="1" />
                        <InputOtpSlot Index="2" />
                    </InputOtpGroup>
                    <InputOtpSeparator />
                    <InputOtpGroup>
                        <InputOtpSlot Index="3" />
                        <InputOtpSlot Index="4" />
                        <InputOtpSlot Index="5" />
                    </InputOtpGroup>
                </InputOtp>
                """;

        private const string _pinCode =
                """
                <InputOtp Length="4">
                    <InputOtpGroup>
                        <InputOtpSlot Index="0" />
                        <InputOtpSlot Index="1" />
                        <InputOtpSlot Index="2" />
                        <InputOtpSlot Index="3" />
                    </InputOtpGroup>
                </InputOtp>
                """;

        private const string _alphanumericCode =
                """
                <InputOtp Length="6" Pattern="[A-Za-z0-9]">
                    <InputOtpGroup>
                        <InputOtpSlot Index="0" />
                        <InputOtpSlot Index="1" />
                        <InputOtpSlot Index="2" />
                    </InputOtpGroup>
                    <InputOtpSeparator />
                    <InputOtpGroup>
                        <InputOtpSlot Index="3" />
                        <InputOtpSlot Index="4" />
                        <InputOtpSlot Index="5" />
                    </InputOtpGroup>
                </InputOtp>
                """;

        private const string _disabledCode =
                """
                <InputOtp Length="6" Disabled="true" Value="123456">
                    <InputOtpGroup>
                        <InputOtpSlot Index="0" />
                        <InputOtpSlot Index="1" />
                        <InputOtpSlot Index="2" />
                    </InputOtpGroup>
                    <InputOtpSeparator />
                    <InputOtpGroup>
                        <InputOtpSlot Index="3" />
                        <InputOtpSlot Index="4" />
                        <InputOtpSlot Index="5" />
                    </InputOtpGroup>
                </InputOtp>
                """;

        private const string _verificationCode =
                """
                <InputOtp Length="6"
                          OnComplete="HandleVerification"
                          AriaInvalid="@(verificationResult == false)">
                    <InputOtpGroup>
                        <InputOtpSlot Index="0" />
                        <InputOtpSlot Index="1" />
                        <InputOtpSlot Index="2" />
                    </InputOtpGroup>
                    <InputOtpSeparator />
                    <InputOtpGroup>
                        <InputOtpSlot Index="3" />
                        <InputOtpSlot Index="4" />
                        <InputOtpSlot Index="5" />
                    </InputOtpGroup>
                </InputOtp>
                """;

        private const string _errorCode =
                """
                <InputOtp Length="6" AriaInvalid="true">
                    <InputOtpGroup>
                        <InputOtpSlot Index="0" />
                        <InputOtpSlot Index="1" />
                        <InputOtpSlot Index="2" />
                    </InputOtpGroup>
                    <InputOtpSeparator />
                    <InputOtpGroup>
                        <InputOtpSlot Index="3" />
                        <InputOtpSlot Index="4" />
                        <InputOtpSlot Index="5" />
                    </InputOtpGroup>
                </InputOtp>
                """;

        private const string _keyboardNavCode =
                """
                <InputOtp Length="6">
                    <InputOtpGroup>
                        <InputOtpSlot Index="0" />
                        <InputOtpSlot Index="1" />
                        <InputOtpSlot Index="2" />
                        <InputOtpSlot Index="3" />
                        <InputOtpSlot Index="4" />
                        <InputOtpSlot Index="5" />
                    </InputOtpGroup>
                </InputOtp>
                """;
    }
}
