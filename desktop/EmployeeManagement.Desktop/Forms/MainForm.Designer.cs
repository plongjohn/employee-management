namespace EmployeeManagement.Desktop.Forms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        headerPanel = new Panel();
        titleLabel = new Label();
        toolbarLayout = new TableLayoutPanel();
        searchTextBox = new TextBox();
        departmentFilterComboBox = new ComboBox();
        refreshButton = new Button();
        addButton = new Button();
        contentPanel = new Panel();
        employeeGrid = new DataGridView();
        nameColumn = new DataGridViewTextBoxColumn();
        emailColumn = new DataGridViewTextBoxColumn();
        departmentColumn = new DataGridViewTextBoxColumn();
        hireDateColumn = new DataGridViewTextBoxColumn();
        actionsColumn = new DataGridViewTextBoxColumn();
        footerLayout = new TableLayoutPanel();
        countLabel = new Label();
        statusLabel = new Label();
        pageSizeComboBox = new ComboBox();
        firstPageButton = new Button();
        previousPageButton = new Button();
        pageLabel = new Label();
        nextPageButton = new Button();
        lastPageButton = new Button();
        searchTimer = new System.Windows.Forms.Timer(components);
        statusTimer = new System.Windows.Forms.Timer(components);
        toolTip = new ToolTip(components);
        headerPanel.SuspendLayout();
        toolbarLayout.SuspendLayout();
        contentPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)employeeGrid).BeginInit();
        footerLayout.SuspendLayout();
        SuspendLayout();
        //
        // headerPanel
        //
        headerPanel.Controls.Add(titleLabel);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 0);
        headerPanel.Name = "headerPanel";
        headerPanel.Padding = new Padding(24, 0, 24, 0);
        headerPanel.Size = new Size(1100, 56);
        headerPanel.TabIndex = 0;
        //
        // titleLabel
        //
        titleLabel.Dock = DockStyle.Fill;
        titleLabel.Location = new Point(24, 0);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(1052, 56);
        titleLabel.TabIndex = 0;
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        //
        // toolbarLayout
        //
        toolbarLayout.ColumnCount = 5;
        toolbarLayout.ColumnStyles.Add(new ColumnStyle());
        toolbarLayout.ColumnStyles.Add(new ColumnStyle());
        toolbarLayout.ColumnStyles.Add(new ColumnStyle());
        toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        toolbarLayout.ColumnStyles.Add(new ColumnStyle());
        toolbarLayout.Controls.Add(searchTextBox, 0, 0);
        toolbarLayout.Controls.Add(departmentFilterComboBox, 1, 0);
        toolbarLayout.Controls.Add(refreshButton, 2, 0);
        toolbarLayout.Controls.Add(addButton, 4, 0);
        toolbarLayout.Dock = DockStyle.Top;
        toolbarLayout.Location = new Point(0, 56);
        toolbarLayout.Name = "toolbarLayout";
        toolbarLayout.Padding = new Padding(24, 16, 24, 8);
        toolbarLayout.RowCount = 1;
        toolbarLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        toolbarLayout.Size = new Size(1100, 64);
        toolbarLayout.TabIndex = 1;
        //
        // searchTextBox
        //
        searchTextBox.Anchor = AnchorStyles.Left;
        searchTextBox.Location = new Point(24, 26);
        searchTextBox.Margin = new Padding(0, 0, 8, 0);
        searchTextBox.Name = "searchTextBox";
        searchTextBox.Size = new Size(320, 25);
        searchTextBox.TabIndex = 0;
        searchTextBox.TextChanged += SearchTextBox_TextChanged;
        //
        // departmentFilterComboBox
        //
        departmentFilterComboBox.Anchor = AnchorStyles.Left;
        departmentFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        departmentFilterComboBox.Location = new Point(352, 25);
        departmentFilterComboBox.Margin = new Padding(0, 0, 8, 0);
        departmentFilterComboBox.Name = "departmentFilterComboBox";
        departmentFilterComboBox.Size = new Size(220, 25);
        departmentFilterComboBox.TabIndex = 1;
        departmentFilterComboBox.SelectionChangeCommitted += DepartmentFilterComboBox_SelectionChangeCommitted;
        //
        // refreshButton
        //
        refreshButton.Anchor = AnchorStyles.Left;
        refreshButton.Location = new Point(580, 22);
        refreshButton.Margin = new Padding(0);
        refreshButton.Name = "refreshButton";
        refreshButton.Size = new Size(36, 32);
        refreshButton.TabIndex = 2;
        refreshButton.Click += RefreshButton_Click;
        //
        // addButton
        //
        addButton.Anchor = AnchorStyles.Right;
        addButton.AutoSize = true;
        addButton.Location = new Point(880, 20);
        addButton.Margin = new Padding(0);
        addButton.MinimumSize = new Size(0, 36);
        addButton.Name = "addButton";
        addButton.Padding = new Padding(12, 0, 12, 0);
        addButton.Size = new Size(196, 36);
        addButton.TabIndex = 3;
        addButton.Click += AddButton_Click;
        //
        // contentPanel
        //
        contentPanel.Controls.Add(employeeGrid);
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Location = new Point(0, 120);
        contentPanel.Name = "contentPanel";
        contentPanel.Padding = new Padding(24, 8, 24, 0);
        contentPanel.Size = new Size(1100, 528);
        contentPanel.TabIndex = 2;
        //
        // employeeGrid
        //
        employeeGrid.AllowUserToAddRows = false;
        employeeGrid.AllowUserToDeleteRows = false;
        employeeGrid.AllowUserToResizeRows = false;
        employeeGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        employeeGrid.BorderStyle = BorderStyle.None;
        employeeGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        employeeGrid.ColumnHeadersHeight = 40;
        employeeGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        employeeGrid.Columns.AddRange(new DataGridViewColumn[] { nameColumn, emailColumn, departmentColumn, hireDateColumn, actionsColumn });
        employeeGrid.Dock = DockStyle.Fill;
        employeeGrid.EnableHeadersVisualStyles = false;
        employeeGrid.Location = new Point(24, 8);
        employeeGrid.MultiSelect = false;
        employeeGrid.Name = "employeeGrid";
        employeeGrid.ReadOnly = true;
        employeeGrid.RowHeadersVisible = false;
        employeeGrid.RowTemplate.Height = 44;
        employeeGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        employeeGrid.ShowCellToolTips = false;
        employeeGrid.Size = new Size(1052, 520);
        employeeGrid.StandardTab = true;
        employeeGrid.TabIndex = 0;
        employeeGrid.CellDoubleClick += EmployeeGrid_CellDoubleClick;
        employeeGrid.CellFormatting += EmployeeGrid_CellFormatting;
        employeeGrid.CellMouseClick += EmployeeGrid_CellMouseClick;
        employeeGrid.CellMouseLeave += EmployeeGrid_CellMouseLeave;
        employeeGrid.CellMouseMove += EmployeeGrid_CellMouseMove;
        employeeGrid.CellPainting += EmployeeGrid_CellPainting;
        employeeGrid.ColumnHeaderMouseClick += EmployeeGrid_ColumnHeaderMouseClick;
        employeeGrid.Paint += EmployeeGrid_Paint;
        //
        // nameColumn
        //
        nameColumn.FillWeight = 30F;
        nameColumn.Name = "nameColumn";
        nameColumn.ReadOnly = true;
        nameColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
        //
        // emailColumn
        //
        emailColumn.DataPropertyName = "Email";
        emailColumn.FillWeight = 30F;
        emailColumn.Name = "emailColumn";
        emailColumn.ReadOnly = true;
        emailColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
        //
        // departmentColumn
        //
        departmentColumn.DataPropertyName = "DepartmentName";
        departmentColumn.FillWeight = 20F;
        departmentColumn.Name = "departmentColumn";
        departmentColumn.ReadOnly = true;
        departmentColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
        //
        // hireDateColumn
        //
        hireDateColumn.DataPropertyName = "HireDate";
        hireDateColumn.FillWeight = 14F;
        hireDateColumn.Name = "hireDateColumn";
        hireDateColumn.ReadOnly = true;
        hireDateColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
        //
        // actionsColumn
        //
        actionsColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        actionsColumn.Name = "actionsColumn";
        actionsColumn.ReadOnly = true;
        actionsColumn.Resizable = DataGridViewTriState.False;
        actionsColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        actionsColumn.Width = 96;
        //
        // footerLayout
        //
        footerLayout.ColumnCount = 8;
        footerLayout.ColumnStyles.Add(new ColumnStyle());
        footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        footerLayout.ColumnStyles.Add(new ColumnStyle());
        footerLayout.ColumnStyles.Add(new ColumnStyle());
        footerLayout.ColumnStyles.Add(new ColumnStyle());
        footerLayout.ColumnStyles.Add(new ColumnStyle());
        footerLayout.ColumnStyles.Add(new ColumnStyle());
        footerLayout.ColumnStyles.Add(new ColumnStyle());
        footerLayout.Controls.Add(countLabel, 0, 0);
        footerLayout.Controls.Add(statusLabel, 1, 0);
        footerLayout.Controls.Add(pageSizeComboBox, 2, 0);
        footerLayout.Controls.Add(firstPageButton, 3, 0);
        footerLayout.Controls.Add(previousPageButton, 4, 0);
        footerLayout.Controls.Add(pageLabel, 5, 0);
        footerLayout.Controls.Add(nextPageButton, 6, 0);
        footerLayout.Controls.Add(lastPageButton, 7, 0);
        footerLayout.Dock = DockStyle.Bottom;
        footerLayout.Location = new Point(0, 648);
        footerLayout.Name = "footerLayout";
        footerLayout.Padding = new Padding(24, 8, 24, 8);
        footerLayout.RowCount = 1;
        footerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        footerLayout.Size = new Size(1100, 52);
        footerLayout.TabIndex = 3;
        //
        // countLabel
        //
        countLabel.Anchor = AnchorStyles.Left;
        countLabel.AutoSize = true;
        countLabel.Location = new Point(24, 17);
        countLabel.Margin = new Padding(0, 0, 16, 0);
        countLabel.Name = "countLabel";
        countLabel.Size = new Size(0, 17);
        countLabel.TabIndex = 0;
        //
        // statusLabel
        //
        statusLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        statusLabel.AutoEllipsis = true;
        statusLabel.Location = new Point(40, 17);
        statusLabel.Margin = new Padding(0, 0, 16, 0);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(642, 17);
        statusLabel.TabIndex = 1;
        //
        // pageSizeComboBox
        //
        pageSizeComboBox.Anchor = AnchorStyles.Left;
        pageSizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        pageSizeComboBox.FormattingEnabled = true;
        pageSizeComboBox.Location = new Point(698, 13);
        pageSizeComboBox.Margin = new Padding(0, 0, 16, 0);
        pageSizeComboBox.Name = "pageSizeComboBox";
        pageSizeComboBox.Size = new Size(130, 25);
        pageSizeComboBox.TabIndex = 2;
        pageSizeComboBox.SelectionChangeCommitted += PageSizeComboBox_SelectionChangeCommitted;
        pageSizeComboBox.Format += PageSizeComboBox_Format;
        //
        // firstPageButton
        //
        firstPageButton.Anchor = AnchorStyles.Left;
        firstPageButton.Enabled = false;
        firstPageButton.Location = new Point(844, 10);
        firstPageButton.Margin = new Padding(0, 0, 4, 0);
        firstPageButton.Name = "firstPageButton";
        firstPageButton.Size = new Size(32, 32);
        firstPageButton.TabIndex = 3;
        firstPageButton.Click += FirstPageButton_Click;
        //
        // previousPageButton
        //
        previousPageButton.Anchor = AnchorStyles.Left;
        previousPageButton.Enabled = false;
        previousPageButton.Location = new Point(880, 10);
        previousPageButton.Margin = new Padding(0);
        previousPageButton.Name = "previousPageButton";
        previousPageButton.Size = new Size(32, 32);
        previousPageButton.TabIndex = 4;
        previousPageButton.Click += PreviousPageButton_Click;
        //
        // pageLabel
        //
        pageLabel.Anchor = AnchorStyles.Left;
        pageLabel.AutoSize = true;
        pageLabel.Location = new Point(924, 17);
        pageLabel.Margin = new Padding(12, 0, 12, 0);
        pageLabel.Name = "pageLabel";
        pageLabel.Size = new Size(0, 17);
        pageLabel.TabIndex = 5;
        //
        // nextPageButton
        //
        nextPageButton.Anchor = AnchorStyles.Left;
        nextPageButton.Enabled = false;
        nextPageButton.Location = new Point(1000, 10);
        nextPageButton.Margin = new Padding(0, 0, 4, 0);
        nextPageButton.Name = "nextPageButton";
        nextPageButton.Size = new Size(32, 32);
        nextPageButton.TabIndex = 6;
        nextPageButton.Click += NextPageButton_Click;
        //
        // lastPageButton
        //
        lastPageButton.Anchor = AnchorStyles.Left;
        lastPageButton.Enabled = false;
        lastPageButton.Location = new Point(1036, 10);
        lastPageButton.Margin = new Padding(0);
        lastPageButton.Name = "lastPageButton";
        lastPageButton.Size = new Size(32, 32);
        lastPageButton.TabIndex = 7;
        lastPageButton.Click += LastPageButton_Click;
        //
        // searchTimer
        //
        searchTimer.Interval = 300;
        searchTimer.Tick += SearchTimer_Tick;
        //
        // statusTimer
        //
        statusTimer.Interval = 4000;
        statusTimer.Tick += StatusTimer_Tick;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 700);
        Controls.Add(contentPanel);
        Controls.Add(footerLayout);
        Controls.Add(toolbarLayout);
        Controls.Add(headerPanel);
        Font = new Font("Segoe UI", 10F);
        MinimumSize = new Size(820, 480);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        headerPanel.ResumeLayout(false);
        toolbarLayout.ResumeLayout(false);
        toolbarLayout.PerformLayout();
        contentPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)employeeGrid).EndInit();
        footerLayout.ResumeLayout(false);
        footerLayout.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel headerPanel;
    private Label titleLabel;
    private TableLayoutPanel toolbarLayout;
    private TextBox searchTextBox;
    private ComboBox departmentFilterComboBox;
    private Button refreshButton;
    private Button addButton;
    private Panel contentPanel;
    private DataGridView employeeGrid;
    private DataGridViewTextBoxColumn nameColumn;
    private DataGridViewTextBoxColumn emailColumn;
    private DataGridViewTextBoxColumn departmentColumn;
    private DataGridViewTextBoxColumn hireDateColumn;
    private DataGridViewTextBoxColumn actionsColumn;
    private TableLayoutPanel footerLayout;
    private Label countLabel;
    private Label statusLabel;
    private ComboBox pageSizeComboBox;
    private Button firstPageButton;
    private Button previousPageButton;
    private Label pageLabel;
    private Button nextPageButton;
    private Button lastPageButton;
    private System.Windows.Forms.Timer searchTimer;
    private System.Windows.Forms.Timer statusTimer;
    private ToolTip toolTip;
}
