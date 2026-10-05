using EmployeeManagement.Core.Models;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Desktop.Resources;
using EmployeeManagement.Desktop.Styling;

namespace EmployeeManagement.Desktop.Forms;

internal partial class MainForm : Form
{
    // Department ids start at 1, so 0 is free to stand for "all departments" in the filter.
    private const int AllDepartmentsId = 0;

    private static readonly int[] PageSizeOptions = [10, EmployeeQuery.DefaultPageSize, 50, EmployeeQuery.MaxPageSize];

    private const int TooltipCursorOffset = 20;

    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly UiExceptionHandler _exceptionHandler;
    private readonly EmployeeValidator _validator;
    private readonly TimeProvider _timeProvider;
    private readonly EmployeeGridRenderer _gridRenderer;
    private readonly Dictionary<DataGridViewColumn, EmployeeSortColumn> _sortColumns;

    private IReadOnlyList<Department> _departments = [];
    private EmployeeQuery _query = new();
    private int _totalPages;
    private bool _isListEmpty;
    private CancellationTokenSource? _loadCancellation;

    public MainForm(
        IEmployeeService employeeService,
        IDepartmentService departmentService,
        UiExceptionHandler exceptionHandler,
        EmployeeValidator validator,
        TimeProvider timeProvider)
    {
        InitializeComponent();

        _employeeService = employeeService;
        _departmentService = departmentService;
        _exceptionHandler = exceptionHandler;
        _validator = validator;
        _timeProvider = timeProvider;
        _gridRenderer = new EmployeeGridRenderer(employeeGrid);
        _sortColumns = new Dictionary<DataGridViewColumn, EmployeeSortColumn>
        {
            [nameColumn] = EmployeeSortColumn.Name,
            [emailColumn] = EmployeeSortColumn.Email,
            [departmentColumn] = EmployeeSortColumn.Department,
            [hireDateColumn] = EmployeeSortColumn.HireDate,
        };

        ApplyTexts();
        ApplyTheme();
        ConfigureGrid();
    }

    private Employee? SelectedEmployee => employeeGrid.CurrentRow?.DataBoundItem as Employee;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ActiveControl = employeeGrid;
        InitializePageSizes();
        Run(ReloadAllAsync);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.F5:
                refreshButton.PerformClick();
                return true;
            case Keys.Control | Keys.N:
                addButton.PerformClick();
                return true;
            case Keys.Enter or Keys.F2 when employeeGrid.Focused && SelectedEmployee is { } employee:
                Run(() => EditEmployeeAsync(employee));
                return true;
            case Keys.Control | Keys.F:
                searchTextBox.Focus();
                searchTextBox.SelectAll();
                return true;
            default:
                return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    private void ApplyTexts()
    {
        Text = Strings.AppTitle;
        titleLabel.Text = Strings.AppTitle;
        searchTextBox.PlaceholderText = Strings.SearchPlaceholder;

        nameColumn.HeaderText = Strings.ColumnName;
        emailColumn.HeaderText = Strings.ColumnEmail;
        departmentColumn.HeaderText = Strings.ColumnDepartment;
        hireDateColumn.HeaderText = Strings.ColumnHireDate;
        actionsColumn.HeaderText = Strings.ColumnActions;
        addButton.Text = Strings.AddEmployee;

        toolTip.SetToolTip(addButton, Strings.AddEmployeeTooltip);
        toolTip.SetToolTip(refreshButton, Strings.Refresh);
        toolTip.SetToolTip(firstPageButton, Strings.FirstPage);
        toolTip.SetToolTip(previousPageButton, Strings.PreviousPage);
        toolTip.SetToolTip(nextPageButton, Strings.NextPage);
        toolTip.SetToolTip(lastPageButton, Strings.LastPage);
    }

    private void ApplyTheme()
    {
        BackColor = Theme.Background;
        headerPanel.BackColor = Theme.Header;
        titleLabel.ForeColor = Theme.HeaderText;
        titleLabel.Font = Theme.TitleFont;

        Theme.ApplyInput(searchTextBox);
        Theme.ApplyInput(departmentFilterComboBox);
        Theme.ApplyInput(pageSizeComboBox);
        Theme.ApplyPrimary(addButton);

        ApplyIconButton(refreshButton, Glyphs.Refresh);
        ApplyIconButton(firstPageButton, Glyphs.FirstPage);
        ApplyIconButton(previousPageButton, Glyphs.PreviousPage);
        ApplyIconButton(nextPageButton, Glyphs.NextPage);
        ApplyIconButton(lastPageButton, Glyphs.LastPage);

        countLabel.Font = Theme.SemiboldFont;
        countLabel.ForeColor = Theme.Text;
        statusLabel.ForeColor = Theme.MutedText;
        pageLabel.ForeColor = Theme.MutedText;
    }

    private static void ApplyIconButton(Button button, string glyph)
    {
        Theme.ApplyIcon(button);
        button.Text = glyph;
    }

    private void ConfigureGrid()
    {
        employeeGrid.AutoGenerateColumns = false;
        employeeGrid.BackgroundColor = Theme.Background;
        employeeGrid.GridColor = Theme.GridLine;
        employeeGrid.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
        employeeGrid.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

        var cellPadding = new Padding(8, 0, 8, 0);

        var headerStyle = employeeGrid.ColumnHeadersDefaultCellStyle;
        headerStyle.BackColor = Theme.Background;
        headerStyle.ForeColor = Theme.MutedText;
        headerStyle.SelectionBackColor = Theme.Background;
        headerStyle.SelectionForeColor = Theme.MutedText;
        headerStyle.Font = Theme.SmallSemiboldFont;
        headerStyle.Padding = cellPadding;

        var cellStyle = employeeGrid.DefaultCellStyle;
        cellStyle.BackColor = Theme.Background;
        cellStyle.ForeColor = Theme.Text;
        cellStyle.SelectionBackColor = Theme.SelectionBackground;
        cellStyle.SelectionForeColor = Theme.Text;
        cellStyle.Font = Theme.BaseFont;
        cellStyle.Padding = cellPadding;

        hireDateColumn.DefaultCellStyle.Format = Strings.DateFormat;
    }

    private void InitializePageSizes()
    {
        pageSizeComboBox.DataSource = PageSizeOptions;
        pageSizeComboBox.SelectedItem = _query.PageSize;
    }

    private async Task ReloadAllAsync()
    {
        var selectedEmployeeId = SelectedEmployee?.Id;
        await LoadDepartmentsAsync();
        await LoadEmployeesAsync(selectedEmployeeId);
    }

    private async Task LoadDepartmentsAsync()
    {
        _departments = await _departmentService.GetAllAsync();

        if (_query.DepartmentId is { } selectedId && _departments.All(department => department.Id != selectedId))
        {
            _query = _query with { DepartmentId = null, Page = 1 };
        }

        departmentFilterComboBox.DisplayMember = nameof(Department.Name);
        departmentFilterComboBox.ValueMember = nameof(Department.Id);
        List<Department> filterOptions = [new Department(AllDepartmentsId, Strings.FilterAllDepartments), .. _departments];
        departmentFilterComboBox.DataSource = filterOptions;
        departmentFilterComboBox.SelectedValue = _query.DepartmentId ?? AllDepartmentsId;
    }

    private async Task ApplyQueryAsync(EmployeeQuery query)
    {
        // While the database is unreachable the shown page and the query can drift apart;
        // the page number must still never drop below the first page.
        _query = query with { Page = Math.Max(1, query.Page) };
        await LoadEmployeesAsync();
    }

    /// <param name="selectEmployeeId">Employee to select after loading, if it is on the loaded page.</param>
    private async Task LoadEmployeesAsync(int? selectEmployeeId = null)
    {
        // A newer search replaces the running one, so a slow old result can never overwrite a newer one.
        _loadCancellation?.Cancel();
        var cancellation = new CancellationTokenSource();
        _loadCancellation = cancellation;
        UseWaitCursor = true;

        try
        {
            var result = await _employeeService.SearchAsync(_query, cancellation.Token);

            // The page can run empty when its last employees were deleted; show the last page that still has data.
            if (result.Items.Count == 0 && result.Page > 1)
            {
                _query = _query with { Page = Math.Max(1, result.TotalPages) };
                result = await _employeeService.SearchAsync(_query, cancellation.Token);
            }

            ShowEmployees(result, selectEmployeeId);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }
        finally
        {
            if (_loadCancellation == cancellation)
            {
                _loadCancellation = null;
                UseWaitCursor = false;
            }

            cancellation.Dispose();
        }
    }

    private void ShowEmployees(PagedResult<Employee> result, int? selectEmployeeId)
    {
        _totalPages = Math.Max(1, result.TotalPages);
        _isListEmpty = result.TotalCount == 0;
        _gridRenderer.HoveredAction = null;

        employeeGrid.DataSource = result.Items.ToList();
        SelectEmployee(selectEmployeeId);
        UpdateSortGlyphs();
        employeeGrid.Invalidate();

        countLabel.Text = string.Format(Strings.EmployeeCount, result.TotalCount);
        pageLabel.Text = string.Format(Strings.PageOf, result.Page, _totalPages);
        firstPageButton.Enabled = result.Page > 1;
        previousPageButton.Enabled = result.Page > 1;
        nextPageButton.Enabled = result.Page < _totalPages;
        lastPageButton.Enabled = result.Page < _totalPages;
    }

    private void SelectEmployee(int? employeeId)
    {
        if (employeeId is null)
        {
            return;
        }

        var row = employeeGrid.Rows
            .Cast<DataGridViewRow>()
            .FirstOrDefault(candidate => candidate.DataBoundItem is Employee employee && employee.Id == employeeId);

        if (row is not null)
        {
            employeeGrid.ClearSelection();
            employeeGrid.CurrentCell = row.Cells[nameColumn.Index];
            row.Selected = true;
        }
    }

    private void UpdateSortGlyphs()
    {
        foreach (var (column, sortColumn) in _sortColumns)
        {
            column.HeaderCell.SortGlyphDirection = sortColumn != _query.SortBy
                ? SortOrder.None
                : _query.Direction == SortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
        }
    }

    private async Task EditEmployeeAsync(Employee employee)
    {
        var current = await _employeeService.GetByIdAsync(employee.Id);
        if (current is null)
        {
            Dialogs.ShowWarning(this, Strings.EmployeeNotFound);
            await LoadEmployeesAsync();
            return;
        }

        await ShowEmployeeFormAsync(current);
    }

    /// <param name="employee">The employee to edit, or null to create a new one.</param>
    private async Task ShowEmployeeFormAsync(Employee? employee)
    {
        using var form = new EmployeeForm(_employeeService, _validator, _exceptionHandler, _departments, _timeProvider, employee);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            ShowStatus(employee is null ? Strings.StatusCreated : Strings.StatusSaved);
        }

        // Reloading after every close also shows what the form learned about changes by other users.
        await LoadEmployeesAsync(form.SavedEmployee?.Id ?? employee?.Id);
    }

    private void ShowStatus(string message)
    {
        statusLabel.Text = message;
        statusTimer.Stop();
        statusTimer.Start();
    }

    private GridAction? ActionAt(int columnIndex, int rowIndex, Point locationInCell)
    {
        if (columnIndex != actionsColumn.Index || rowIndex < 0)
        {
            return null;
        }

        var cellSize = employeeGrid.GetCellDisplayRectangle(columnIndex, rowIndex, cutOverflow: false).Size;
        return _gridRenderer.HitTestAction(cellSize, locationInCell);
    }

    private Task ExecuteAsync(GridAction action, Employee employee) => action switch
    {
        GridAction.Edit => EditEmployeeAsync(employee),
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null),
    };

    private void SetHoveredAction((int RowIndex, GridAction Action)? hovered)
    {
        if (_gridRenderer.HoveredAction == hovered)
        {
            return;
        }

        InvalidateActionCell(_gridRenderer.HoveredAction);
        _gridRenderer.HoveredAction = hovered;
        InvalidateActionCell(hovered);

        employeeGrid.Cursor = hovered is null ? Cursors.Default : Cursors.Hand;
        if (hovered is { } target)
        {
            var position = employeeGrid.PointToClient(Cursor.Position);
            toolTip.Show(TooltipFor(target.Action), employeeGrid,
                position.X, position.Y + LogicalToDeviceUnits(TooltipCursorOffset));
        }
        else
        {
            toolTip.Hide(employeeGrid);
        }
    }

    private void InvalidateActionCell((int RowIndex, GridAction Action)? action)
    {
        if (action is { } target && target.RowIndex < employeeGrid.RowCount)
        {
            employeeGrid.InvalidateCell(actionsColumn.Index, target.RowIndex);
        }
    }

    private static string TooltipFor(GridAction action) => action switch
    {
        GridAction.Edit => Strings.EditTooltip,
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null),
    };

    private void AddButton_Click(object? sender, EventArgs e) => Run(() => ShowEmployeeFormAsync(employee: null));

    private void StatusTimer_Tick(object? sender, EventArgs e)
    {
        statusTimer.Stop();
        statusLabel.Text = string.Empty;
    }

    // The only async void method: UI events start their work here, so every failure is reported.
    private async void Run(Func<Task> operation) => await _exceptionHandler.RunAsync(this, operation);

    private void SearchTextBox_TextChanged(object? sender, EventArgs e)
    {
        // Restarting the timer on every keystroke searches once the user pauses typing.
        searchTimer.Stop();
        searchTimer.Start();
    }

    private void SearchTimer_Tick(object? sender, EventArgs e)
    {
        searchTimer.Stop();
        Run(() => ApplyQueryAsync(_query with { SearchText = searchTextBox.Text, Page = 1 }));
    }

    private void DepartmentFilterComboBox_SelectionChangeCommitted(object? sender, EventArgs e)
    {
        var departmentId = (int)departmentFilterComboBox.SelectedValue!;
        int? filter = departmentId == AllDepartmentsId ? null : departmentId;
        Run(() => ApplyQueryAsync(_query with { DepartmentId = filter, Page = 1 }));
    }

    private void PageSizeComboBox_SelectionChangeCommitted(object? sender, EventArgs e) =>
        Run(() => ApplyQueryAsync(_query with { PageSize = (int)pageSizeComboBox.SelectedItem!, Page = 1 }));

    private void PageSizeComboBox_Format(object? sender, ListControlConvertEventArgs e) =>
        e.Value = string.Format(Strings.PageSizeOption, e.ListItem);

    private void RefreshButton_Click(object? sender, EventArgs e) => Run(() => ReloadAllAsync());

    private void FirstPageButton_Click(object? sender, EventArgs e) =>
        Run(() => ApplyQueryAsync(_query with { Page = 1 }));

    private void PreviousPageButton_Click(object? sender, EventArgs e) =>
        Run(() => ApplyQueryAsync(_query with { Page = _query.Page - 1 }));

    private void NextPageButton_Click(object? sender, EventArgs e) =>
        Run(() => ApplyQueryAsync(_query with { Page = _query.Page + 1 }));

    private void LastPageButton_Click(object? sender, EventArgs e) =>
        Run(() => ApplyQueryAsync(_query with { Page = _totalPages }));

    private void EmployeeGrid_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (!_sortColumns.TryGetValue(employeeGrid.Columns[e.ColumnIndex], out var sortBy))
        {
            return;
        }

        var direction = sortBy == _query.SortBy && _query.Direction == SortDirection.Ascending
            ? SortDirection.Descending
            : SortDirection.Ascending;

        Run(() => ApplyQueryAsync(_query with { SortBy = sortBy, Direction = direction, Page = 1 }));
    }

    private void EmployeeGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex == nameColumn.Index && EmployeeAt(e.RowIndex) is { } employee)
        {
            e.Value = $"{employee.LastName}, {employee.FirstName}";
            e.FormattingApplied = true;
        }
    }

    private void EmployeeGrid_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (EmployeeAt(e.RowIndex) is not { } employee)
        {
            return;
        }

        if (e.ColumnIndex == nameColumn.Index)
        {
            _gridRenderer.PaintName(e, employee);
        }
        else if (e.ColumnIndex == departmentColumn.Index)
        {
            _gridRenderer.PaintDepartmentBadge(e, employee);
        }
        else if (e.ColumnIndex == actionsColumn.Index)
        {
            _gridRenderer.PaintActions(e);
        }
    }

    private void EmployeeGrid_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left
            && ActionAt(e.ColumnIndex, e.RowIndex, e.Location) is { } action
            && EmployeeAt(e.RowIndex) is { } employee)
        {
            Run(() => ExecuteAsync(action, employee));
        }
    }

    private void EmployeeGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex != actionsColumn.Index && EmployeeAt(e.RowIndex) is { } employee)
        {
            Run(() => EditEmployeeAsync(employee));
        }
    }

    private void EmployeeGrid_CellMouseMove(object? sender, DataGridViewCellMouseEventArgs e) =>
        SetHoveredAction(ActionAt(e.ColumnIndex, e.RowIndex, e.Location) is { } action ? (e.RowIndex, action) : null);

    private void EmployeeGrid_CellMouseLeave(object? sender, DataGridViewCellEventArgs e) => SetHoveredAction(null);

    private void EmployeeGrid_Paint(object? sender, PaintEventArgs e)
    {
        if (_isListEmpty)
        {
            _gridRenderer.PaintEmptyHint(e.Graphics, Strings.EmptyList);
        }
    }

    private Employee? EmployeeAt(int rowIndex) =>
        rowIndex >= 0 ? employeeGrid.Rows[rowIndex].DataBoundItem as Employee : null;
}
