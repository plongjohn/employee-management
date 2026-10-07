using EmployeeManagement.Core.Models;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Core.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Core.Tests;

public class EmployeeServiceTests
{
    private readonly InMemoryEmployeeRepository _repository = new();
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        var validator = new EmployeeValidator(TestData.ClockAt(new DateOnly(2026, 10, 5)));
        _service = new EmployeeService(_repository, validator, NullLogger<EmployeeService>.Instance);
    }

    private static CancellationToken Token => TestContext.Current.CancellationToken;

    [Fact]
    public async Task CreateAsync_ValidInput_ReturnsStoredEmployee()
    {
        var result = await _service.CreateAsync(TestData.ValidInput(), Token);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.NotNull(result.Value);
        Assert.Equal("Produktion", result.Value.DepartmentName);
        Assert.Equal(1u, result.Value.Version);
    }

    [Fact]
    public async Task CreateAsync_InputWithSurroundingWhitespace_StoresTrimmedValues()
    {
        var input = TestData.ValidInput() with { FirstName = "  Anna ", Email = " anna.mueller@example.com " };

        var result = await _service.CreateAsync(input, Token);

        Assert.Equal(TestData.ValidInput(), result.Value!.ToInput());
    }

    [Fact]
    public async Task CreateAsync_InvalidInput_ReturnsErrorsWithoutWriting()
    {
        var result = await _service.CreateAsync(TestData.ValidInput() with { LastName = " " }, Token);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.Equal([ValidationError.LastNameRequired], result.Errors);
        Assert.Equal(0, _repository.WriteCount);
    }

    [Fact]
    public async Task CreateAsync_EmailUsedWithDifferentCase_ReturnsDuplicateEmail()
    {
        _repository.Add(TestData.ValidInput());

        var result = await _service.CreateAsync(
            TestData.ValidInput() with { Email = "Anna.Mueller@Example.com" }, Token);

        Assert.Equal(OperationStatus.DuplicateEmail, result.Status);
    }

    [Fact]
    public async Task CreateAsync_UnknownDepartment_ReturnsDepartmentNotFound()
    {
        var result = await _service.CreateAsync(TestData.ValidInput() with { DepartmentId = 99 }, Token);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.Equal([ValidationError.DepartmentNotFound], result.Errors);
    }

    [Fact]
    public async Task UpdateAsync_CurrentVersion_ReturnsEmployeeWithNewVersion()
    {
        var existing = _repository.Add(TestData.ValidInput());

        var result = await _service.UpdateAsync(
            existing.Id, existing.Version, existing.ToInput() with { DepartmentId = 2 }, Token);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal("IT", result.Value!.DepartmentName);
        Assert.Equal(existing.Version + 1, result.Value.Version);
    }

    [Fact]
    public async Task UpdateAsync_ChangedByOtherUser_ReturnsConflict()
    {
        var existing = _repository.Add(TestData.ValidInput());
        _repository.ChangeByOtherUser(existing.Id);

        var result = await _service.UpdateAsync(existing.Id, existing.Version, existing.ToInput(), Token);

        Assert.Equal(OperationStatus.Conflict, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_DeletedByOtherUser_ReturnsNotFound()
    {
        var existing = _repository.Add(TestData.ValidInput());
        _repository.DeleteByOtherUser(existing.Id);

        var result = await _service.UpdateAsync(existing.Id, existing.Version, existing.ToInput(), Token);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_EmailOfOtherEmployee_ReturnsDuplicateEmail()
    {
        _repository.Add(TestData.ValidInput());
        var other = _repository.Add(TestData.ValidInput() with { Email = "lukas.mueller@example.com" });

        var result = await _service.UpdateAsync(
            other.Id, other.Version, other.ToInput() with { Email = "anna.mueller@example.com" }, Token);

        Assert.Equal(OperationStatus.DuplicateEmail, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_UnknownDepartment_ReturnsDepartmentNotFound()
    {
        var existing = _repository.Add(TestData.ValidInput());

        var result = await _service.UpdateAsync(
            existing.Id, existing.Version, existing.ToInput() with { DepartmentId = 99 }, Token);

        Assert.Equal([ValidationError.DepartmentNotFound], result.Errors);
    }

    [Fact]
    public async Task UpdateAsync_InvalidInput_ReturnsErrorsWithoutWriting()
    {
        var existing = _repository.Add(TestData.ValidInput());

        var result = await _service.UpdateAsync(
            existing.Id, existing.Version, existing.ToInput() with { Email = "invalid" }, Token);

        Assert.Equal([ValidationError.EmailInvalid], result.Errors);
        Assert.Equal(0, _repository.WriteCount);
    }

    [Fact]
    public async Task DeleteAsync_CurrentVersion_RemovesEmployee()
    {
        var existing = _repository.Add(TestData.ValidInput());

        var result = await _service.DeleteAsync(existing.Id, existing.Version, Token);

        Assert.True(result.IsSuccess);
        Assert.Null(await _service.GetByIdAsync(existing.Id, Token));
    }

    [Fact]
    public async Task DeleteAsync_ChangedByOtherUser_ReturnsConflictAndKeepsEmployee()
    {
        var existing = _repository.Add(TestData.ValidInput());
        _repository.ChangeByOtherUser(existing.Id);

        var result = await _service.DeleteAsync(existing.Id, existing.Version, Token);

        Assert.Equal(OperationStatus.Conflict, result.Status);
        Assert.NotNull(await _service.GetByIdAsync(existing.Id, Token));
    }

    [Fact]
    public async Task DeleteAsync_DeletedByOtherUser_ReturnsNotFound()
    {
        var existing = _repository.Add(TestData.ValidInput());
        _repository.DeleteByOtherUser(existing.Id);

        var result = await _service.DeleteAsync(existing.Id, existing.Version, Token);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task SearchAsync_ValidQuery_PassesQueryToRepository()
    {
        var query = new EmployeeQuery("mü", DepartmentId: 1, Page: 2, PageSize: EmployeeQuery.MaxPageSize);

        await _service.SearchAsync(query, Token);

        Assert.Equal(query, _repository.LastQuery);
    }

    [Theory]
    [InlineData(0, 25)]
    [InlineData(1, 0)]
    [InlineData(1, EmployeeQuery.MaxPageSize + 1)]
    public async Task SearchAsync_PagingOutOfRange_Throws(int page, int pageSize)
    {
        var query = new EmployeeQuery(Page: page, PageSize: pageSize);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.SearchAsync(query, Token));
    }
}
