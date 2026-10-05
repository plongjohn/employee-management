using EmployeeManagement.Core.Services;

namespace EmployeeManagement.Core.Tests;

public class OperationResultTests
{
    [Fact]
    public void Failure_SuccessStatus_Throws()
    {
        Assert.Throws<ArgumentException>(() => OperationResult.Failure(OperationStatus.Success));
    }

    [Fact]
    public void Failure_ValidationFailedStatusWithoutErrors_Throws()
    {
        Assert.Throws<ArgumentException>(() => OperationResult<string>.Failure(OperationStatus.ValidationFailed));
    }

    [Fact]
    public void ValidationFailed_NoErrors_Throws()
    {
        Assert.Throws<ArgumentException>(() => OperationResult<string>.ValidationFailed([]));
    }

    [Fact]
    public void Success_Value_IsSuccessWithValue()
    {
        var result = OperationResult<string>.Success("value");

        Assert.True(result.IsSuccess);
        Assert.Equal("value", result.Value);
        Assert.Empty(result.Errors);
    }
}
