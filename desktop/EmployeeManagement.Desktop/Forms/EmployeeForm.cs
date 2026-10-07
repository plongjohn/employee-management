using EmployeeManagement.Core.Models;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Desktop.Resources;
using EmployeeManagement.Desktop.Styling;

namespace EmployeeManagement.Desktop.Forms;

/// <summary>
/// Creates or edits an employee. Closes with <see cref="DialogResult.OK"/> after saving and with
/// <see cref="DialogResult.Abort"/> if the employee was deleted by someone else in the meantime.
/// </summary>
internal partial class EmployeeForm : Form
{
    // Department ids start at 1; 0 marks "nothing chosen yet", which the validator rejects as DepartmentRequired.
    private const int NoDepartmentId = 0;
    private const int TooltipCursorOffset = 20;

    // Only missing values block the save button; format errors are reported when saving.
    private static readonly ValidationError[] MissingValueErrors =
    [
        ValidationError.FirstNameRequired,
        ValidationError.LastNameRequired,
        ValidationError.EmailRequired,
        ValidationError.DepartmentRequired,
    ];

    private readonly IEmployeeService _employeeService;
    private readonly EmployeeValidator _validator;
    private readonly UiExceptionHandler _exceptionHandler;
    private readonly IReadOnlyList<Department> _departments;
    private readonly DateOnly _today;
    private readonly Dictionary<Control, Label> _errorLabels;

    private Employee? _employee;
    private EmployeeInput _original;
    private bool _isSaving;
    private bool _isSaveHintVisible;

    /// <param name="employee">The employee to edit, or null to create a new one.</param>
    public EmployeeForm(
        IEmployeeService employeeService,
        EmployeeValidator validator,
        UiExceptionHandler exceptionHandler,
        IReadOnlyList<Department> departments,
        TimeProvider timeProvider,
        Employee? employee)
    {
        InitializeComponent();

        _employeeService = employeeService;
        _validator = validator;
        _exceptionHandler = exceptionHandler;
        _departments = departments;
        _today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        _errorLabels = new Dictionary<Control, Label>
        {
            [firstNameTextBox] = firstNameErrorLabel,
            [lastNameTextBox] = lastNameErrorLabel,
            [emailTextBox] = emailErrorLabel,
            [departmentComboBox] = departmentErrorLabel,
            [hireDatePicker] = hireDateErrorLabel,
        };
        _employee = employee;
        _original = employee?.ToInput()
            ?? new EmployeeInput(string.Empty, string.Empty, string.Empty, NoDepartmentId, _today);

        ApplyTexts(isNew: employee is null);
        ApplyTheme();
    }

    /// <summary>The employee as stored in the database; set when the form closes with OK.</summary>
    public Employee? SavedEmployee { get; private set; }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ConfigureFields();
        FillFields(_original);
        ClearFieldErrors();
        UpdateSaveButton();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (_isSaving)
        {
            e.Cancel = true;
            return;
        }

        if (DialogResult is DialogResult.OK or DialogResult.Abort || !HasChanges())
        {
            return;
        }

        e.Cancel = !Dialogs.Confirm(
            this, Strings.DiscardHeading, Strings.DiscardText, Strings.DiscardConfirm, Strings.DiscardKeepEditing);
    }

    private void ApplyTexts(bool isNew)
    {
        Text = isNew ? Strings.FormTitleAdd : Strings.FormTitleEdit;
        headingLabel.Text = Text;
        firstNameLabel.Text = Strings.LabelFirstName;
        lastNameLabel.Text = Strings.LabelLastName;
        emailLabel.Text = Strings.LabelEmail;
        departmentLabel.Text = Strings.LabelDepartment;
        hireDateLabel.Text = Strings.LabelHireDate;
        saveButton.Text = Strings.Save;
        cancelButton.Text = Strings.Cancel;
    }

    private void ApplyTheme()
    {
        BackColor = Theme.Background;
        headingLabel.Font = Theme.HeadingFont;
        headingLabel.ForeColor = Theme.Text;

        Theme.ApplyInput(firstNameTextBox);
        Theme.ApplyInput(lastNameTextBox);
        Theme.ApplyInput(emailTextBox);
        Theme.ApplyInput(departmentComboBox);
        Theme.ApplyPrimary(saveButton);
        Theme.ApplySecondary(cancelButton);

        foreach (var errorLabel in _errorLabels.Values)
        {
            errorLabel.Font = Theme.SmallFont;
            errorLabel.ForeColor = Theme.Accent;
        }
    }

    private void ConfigureFields()
    {
        firstNameTextBox.MaxLength = EmployeeValidator.NameMaxLength;
        lastNameTextBox.MaxLength = EmployeeValidator.NameMaxLength;
        emailTextBox.MaxLength = EmployeeValidator.EmailMaxLength;

        List<Department> departmentOptions =
            [new Department(NoDepartmentId, Strings.DepartmentPlaceholder), .. _departments];
        departmentComboBox.DisplayMember = nameof(Department.Name);
        departmentComboBox.ValueMember = nameof(Department.Id);
        departmentComboBox.DataSource = departmentOptions;

        hireDatePicker.CustomFormat = Strings.DateFormat;
        hireDatePicker.MinDate = ToDateTime(EmployeeValidator.EarliestHireDate);
        hireDatePicker.MaxDate = ToDateTime(_today.AddYears(EmployeeValidator.MaxYearsInFuture));
    }

    private void FillFields(EmployeeInput input)
    {
        firstNameTextBox.Text = input.FirstName;
        lastNameTextBox.Text = input.LastName;
        emailTextBox.Text = input.Email;
        departmentComboBox.SelectedValue = input.DepartmentId;
        SetHireDate(input.HireDate);
    }

    private void SetHireDate(DateOnly hireDate)
    {
        // A stored date outside the allowed range must still be shown; the validator reports it on save.
        var value = ToDateTime(hireDate);
        if (value < hireDatePicker.MinDate)
        {
            hireDatePicker.MinDate = value;
        }

        if (value > hireDatePicker.MaxDate)
        {
            hireDatePicker.MaxDate = value;
        }

        hireDatePicker.Value = value;
    }

    private EmployeeInput CurrentInput() => new(
        firstNameTextBox.Text,
        lastNameTextBox.Text,
        emailTextBox.Text,
        departmentComboBox.SelectedValue as int? ?? NoDepartmentId,
        DateOnly.FromDateTime(hireDatePicker.Value));

    // Trimmed, because surrounding spaces are removed on save anyway and are no real change.
    private bool HasChanges() => CurrentInput().Trimmed() != _original;

    private bool AreRequiredFieldsFilled() =>
        !_validator.Validate(CurrentInput()).Any(error => MissingValueErrors.Contains(error));

    private void UpdateSaveButton() => saveButton.Enabled = !_isSaving && HasChanges() && AreRequiredFieldsFilled();

    private string SaveDisabledReason() => HasChanges() ? Strings.SaveRequiredFieldsHint : Strings.SaveNoChangesHint;

    private void SetFieldError(Control field, string message)
    {
        errorProvider.SetError(field, message);
        _errorLabels[field].Text = message;
    }

    private void ClearFieldErrors()
    {
        foreach (var field in _errorLabels.Keys)
        {
            SetFieldError(field, string.Empty);
        }
    }

    private async Task SaveAsync()
    {
        OperationResult<Employee> result;

        SetSaving(true);
        try
        {
            var input = CurrentInput();
            result = _employee is null
                ? await _employeeService.CreateAsync(input)
                : await _employeeService.UpdateAsync(_employee.Id, _employee.Version, input);
        }
        finally
        {
            SetSaving(false);
        }

        await HandleSaveResultAsync(result);
    }

    private async Task HandleSaveResultAsync(OperationResult<Employee> result)
    {
        switch (result.Status)
        {
            case OperationStatus.Success:
                SavedEmployee = result.Value;
                DialogResult = DialogResult.OK;
                break;
            case OperationStatus.ValidationFailed:
                ShowValidationErrors(result.Errors);
                break;
            case OperationStatus.DuplicateEmail:
                SetFieldError(emailTextBox, Strings.DuplicateEmail);
                emailTextBox.Focus();
                break;
            case OperationStatus.Conflict when _employee is not null:
                await HandleConflictAsync(_employee.Id);
                break;
            case OperationStatus.NotFound:
                CloseAsDeleted();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result.Status, null);
        }
    }

    private void ShowValidationErrors(IReadOnlyList<ValidationError> errors)
    {
        ClearFieldErrors();
        foreach (var error in errors)
        {
            SetFieldError(FieldFor(error), ValidationMessages.For(error));
        }

        FieldFor(errors[0]).Focus();
    }

    private Control FieldFor(ValidationError error) => error switch
    {
        ValidationError.FirstNameRequired or ValidationError.FirstNameTooLong => firstNameTextBox,
        ValidationError.LastNameRequired or ValidationError.LastNameTooLong => lastNameTextBox,
        ValidationError.EmailRequired or ValidationError.EmailTooLong or ValidationError.EmailInvalid => emailTextBox,
        ValidationError.DepartmentRequired or ValidationError.DepartmentNotFound => departmentComboBox,
        ValidationError.HireDateTooEarly or ValidationError.HireDateTooFarInFuture => hireDatePicker,
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, null),
    };

    private async Task HandleConflictAsync(int employeeId)
    {
        var reload = Dialogs.Confirm(
            this, Strings.ConflictHeading, Strings.ConflictText, Strings.ConflictReload, Strings.ConflictBack);
        if (!reload)
        {
            return;
        }

        var current = await _employeeService.GetByIdAsync(employeeId);
        if (current is null)
        {
            CloseAsDeleted();
            return;
        }

        ShowEmployee(current);
    }

    private void ShowEmployee(Employee employee)
    {
        _employee = employee;
        _original = employee.ToInput();
        FillFields(_original);
        ClearFieldErrors();
        UpdateSaveButton();
    }

    private void CloseAsDeleted()
    {
        Dialogs.ShowWarning(this, Strings.EmployeeNotFound);
        DialogResult = DialogResult.Abort;
    }

    private void SetSaving(bool isSaving)
    {
        _isSaving = isSaving;
        UseWaitCursor = isSaving;
        mainLayout.Enabled = !isSaving;
        UpdateSaveButton();
    }

    private void Field_Changed(object? sender, EventArgs e)
    {
        if (sender is Control field)
        {
            SetFieldError(field, string.Empty);
        }

        UpdateSaveButton();
    }

    private void SaveButton_Click(object? sender, EventArgs e) => Run(SaveAsync);

    // The only async void method: UI events start their work here, so every failure is reported.
    private async void Run(Func<Task> operation) => await _exceptionHandler.RunAsync(this, operation);

    // Disabled controls get no mouse events and therefore no tooltips, so the panel behind the
    // button shows the hint instead.
    private void ButtonPanel_MouseMove(object? sender, MouseEventArgs e)
    {
        var showHint = !saveButton.Enabled && !_isSaving && saveButton.Bounds.Contains(e.Location);
        if (showHint == _isSaveHintVisible)
        {
            return;
        }

        _isSaveHintVisible = showHint;
        if (showHint)
        {
            toolTip.Show(SaveDisabledReason(), buttonPanel, e.X, e.Y + LogicalToDeviceUnits(TooltipCursorOffset));
        }
        else
        {
            toolTip.Hide(buttonPanel);
        }
    }

    private void ButtonPanel_MouseLeave(object? sender, EventArgs e)
    {
        _isSaveHintVisible = false;
        toolTip.Hide(buttonPanel);
    }

    private static DateTime ToDateTime(DateOnly date) => date.ToDateTime(TimeOnly.MinValue);
}
