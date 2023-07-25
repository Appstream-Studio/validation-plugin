using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonStructureAlignment;

internal sealed class ValidateJsonStructureAlignmentValidator : AbstractValidator<ValidateJsonStructureAlignment>
{
    public ValidateJsonStructureAlignmentValidator()
    {
        this.RuleFor(r => r.Json1).NotNull().NotEmpty()
            .Must(this.IsValidJson)
            .WithMessage("'{PropertyName}' must be a valid JSON ({ErrorMessage}).");

        this.RuleFor(r => r.Json2).NotNull().NotEmpty()
            .Must(this.IsValidJson)
            .WithMessage("'{PropertyName}' must be a valid JSON ({ErrorMessage}).");
    }

    private bool IsValidJson(ValidateJsonStructureAlignment root, string jsonInstance, ValidationContext<ValidateJsonStructureAlignment> context)
    {
        try
        {
            JsonNode.Parse(jsonInstance);
            return true;
        }
        catch (JsonException e)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", e.Message);
            return false;
        }
    }
}
