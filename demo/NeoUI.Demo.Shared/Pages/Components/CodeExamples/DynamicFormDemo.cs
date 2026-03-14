namespace NeoUI.Demo.Shared.Pages.Components;

partial class DynamicFormDemo
{
    private static class DynamicFormDemoCode
    {
        public const string Basic =
            """
            @code {
                private Dictionary<string, object?> _values = new();

                private static readonly FormSchema _schema = new()
                {
                    Fields =
                    [
                        new() { Name = "name",    Label = "Full Name", Type = FieldType.Text,     Required = true },
                        new() { Name = "email",   Label = "Email",     Type = FieldType.Email,    Required = true },
                        new() { Name = "message", Label = "Message",   Type = FieldType.Textarea, Required = true },
                    ]
                };
            }

            <DynamicForm Schema="@_schema"
                         @bind-Values="_values"
                         OnValidSubmit="HandleSubmit"
                         SubmitText="Send Message" />
            """;

        public const string TwoColumn =
            """
            <DynamicForm Schema="@_schema" @bind-Values="_values" Columns="2" SubmitText="Create Account" />

            @code {
                private static readonly FormSchema _schema = new()
                {
                    Fields =
                    [
                        new() { Name = "firstName", Label = "First Name", Type = FieldType.Text,  Required = true },
                        new() { Name = "lastName",  Label = "Last Name",  Type = FieldType.Text,  Required = true },
                        new() { Name = "email",     Label = "Email",      Type = FieldType.Email, Required = true, ColSpan = 2 },
                    ]
                };
            }
            """;

        public const string Sections =
            """
            <DynamicForm Schema="@_schema" @bind-Values="_values" />

            @code {
                private static readonly FormSchema _schema = new()
                {
                    Sections =
                    [
                        new()
                        {
                            Title = "Profile",
                            Fields =
                            [
                                new() { Name = "displayName", Label = "Display Name", Type = FieldType.Text     },
                                new() { Name = "bio",         Label = "Bio",          Type = FieldType.Textarea },
                            ]
                        },
                        new()
                        {
                            Title = "Notifications",
                            Collapsible = true,
                            Fields =
                            [
                                new() { Name = "emailMarketing", Label = "Marketing emails",  Type = FieldType.Switch },
                                new() { Name = "emailProduct",   Label = "Product updates",    Type = FieldType.Switch, DefaultValue = true },
                            ]
                        }
                    ]
                };
            }
            """;

        public const string FieldTypes =
            """
            <DynamicForm Schema="@_schema" @bind-Values="_values" Columns="2" ShowSubmitButton="false" />

            @code {
                private static readonly FormSchema _schema = new()
                {
                    Fields =
                    [
                        new() { Name = "text",     Label = "Text",       Type = FieldType.Text      },
                        new() { Name = "email",    Label = "Email",      Type = FieldType.Email     },
                        new() { Name = "password", Label = "Password",   Type = FieldType.Password  },
                        new() { Name = "number",   Label = "Number",     Type = FieldType.Number    },
                        new() { Name = "textarea", Label = "Textarea",   Type = FieldType.Textarea  },
                        new() { Name = "select",   Label = "Select",     Type = FieldType.Select,
                            Options = [ new() { Value="a", Label="Option A" } ] },
                        new() { Name = "checkbox", Label = "Checkbox",   Type = FieldType.Checkbox  },
                        new() { Name = "switch",   Label = "Switch",     Type = FieldType.Switch    },
                        new() { Name = "tags",     Label = "Tags",       Type = FieldType.Tags      },
                        new() { Name = "slider",   Label = "Slider",     Type = FieldType.Slider    },
                        new() { Name = "color",    Label = "Color",      Type = FieldType.Color     },
                    ]
                };
            }
            """;

        public const string Validation =
            """
            <DynamicForm Schema="@_schema" @bind-Values="_values" ShowValidationSummary="true" />

            @code {
                private static readonly FormSchema _schema = new()
                {
                    Fields =
                    [
                        new() { Name = "username", Label = "Username", Type = FieldType.Text, Required = true,
                            Validations =
                            [
                                new() { Type = ValidationType.MinLength, Value = 3,  Message = "Min 3 characters." },
                                new() { Type = ValidationType.MaxLength, Value = 20, Message = "Max 20 characters." },
                            ]},
                        new() { Name = "email", Label = "Email", Type = FieldType.Email, Required = true,
                            Validations = [ new() { Type = ValidationType.Email, Message = "Invalid email address." } ] },
                        new() { Name = "age", Label = "Age", Type = FieldType.Number,
                            Validations =
                            [
                                new() { Type = ValidationType.Min, Value = 18.0,  Message = "Must be 18 or older." },
                                new() { Type = ValidationType.Max, Value = 120.0, Message = "Enter a valid age."   },
                            ]},
                    ]
                };
            }
            """;

        public const string Conditional =
            """
            <DynamicForm Schema="@_schema" @bind-Values="_values" ShowSubmitButton="false" />

            @code {
                private static readonly FormSchema _schema = new()
                {
                    Fields =
                    [
                        new() { Name = "contactMethod", Label = "Preferred Contact", Type = FieldType.Select,
                            Options =
                            [
                                new() { Value = "email", Label = "Email" },
                                new() { Value = "phone", Label = "Phone" },
                            ]},
                        new() { Name = "emailAddr",   Label = "Email Address", Type = FieldType.Email, VisibleWhen = "contactMethod == 'email'" },
                        new() { Name = "phoneNumber", Label = "Phone Number",  Type = FieldType.Phone, VisibleWhen = "contactMethod == 'phone'" },
                    ]
                };
            }
            """;
    }
}
