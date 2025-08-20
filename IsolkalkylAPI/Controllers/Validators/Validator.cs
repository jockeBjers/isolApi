public class Validator : ControllerBase
{
    public ActionResult? Validate<T>(IValidator<T> validator, T request)
    {
        var result = validator.Validate(request);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors.Select(x => new
            {
                Field = x.PropertyName,
                Message = x.ErrorMessage
            }));
        }

        return null;
    }
}
