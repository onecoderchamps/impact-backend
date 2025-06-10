using Microsoft.IdentityModel.Tokens;

public class ValidationUserDto
{
    public List<object> ValidateCreateInput(CreateUserDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Name))
        {
            errors.Add(new { Name = "Name is a required field." });
        }
        return errors;
    }
}