namespace EmployeeManagement.Desktop.Forms;

partial class EmployeeForm
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
        mainLayout = new TableLayoutPanel();
        headingLabel = new Label();
        firstNameLabel = new EmployeeManagement.Desktop.Controls.RequiredFieldLabel();
        firstNameTextBox = new TextBox();
        firstNameErrorLabel = new Label();
        lastNameLabel = new EmployeeManagement.Desktop.Controls.RequiredFieldLabel();
        lastNameTextBox = new TextBox();
        lastNameErrorLabel = new Label();
        emailLabel = new EmployeeManagement.Desktop.Controls.RequiredFieldLabel();
        emailTextBox = new TextBox();
        emailErrorLabel = new Label();
        departmentLabel = new EmployeeManagement.Desktop.Controls.RequiredFieldLabel();
        departmentComboBox = new ComboBox();
        departmentErrorLabel = new Label();
        hireDateLabel = new EmployeeManagement.Desktop.Controls.RequiredFieldLabel();
        hireDatePicker = new DateTimePicker();
        hireDateErrorLabel = new Label();
        buttonPanel = new FlowLayoutPanel();
        saveButton = new Button();
        cancelButton = new Button();
        errorProvider = new ErrorProvider(components);
        toolTip = new ToolTip(components);
        mainLayout.SuspendLayout();
        buttonPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
        SuspendLayout();
        //
        // mainLayout
        //
        mainLayout.AutoSize = true;
        mainLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        mainLayout.ColumnCount = 1;
        mainLayout.ColumnStyles.Add(new ColumnStyle());
        mainLayout.Controls.Add(headingLabel, 0, 0);
        mainLayout.Controls.Add(firstNameLabel, 0, 1);
        mainLayout.Controls.Add(firstNameTextBox, 0, 2);
        mainLayout.Controls.Add(firstNameErrorLabel, 0, 3);
        mainLayout.Controls.Add(lastNameLabel, 0, 4);
        mainLayout.Controls.Add(lastNameTextBox, 0, 5);
        mainLayout.Controls.Add(lastNameErrorLabel, 0, 6);
        mainLayout.Controls.Add(emailLabel, 0, 7);
        mainLayout.Controls.Add(emailTextBox, 0, 8);
        mainLayout.Controls.Add(emailErrorLabel, 0, 9);
        mainLayout.Controls.Add(departmentLabel, 0, 10);
        mainLayout.Controls.Add(departmentComboBox, 0, 11);
        mainLayout.Controls.Add(departmentErrorLabel, 0, 12);
        mainLayout.Controls.Add(hireDateLabel, 0, 13);
        mainLayout.Controls.Add(hireDatePicker, 0, 14);
        mainLayout.Controls.Add(hireDateErrorLabel, 0, 15);
        mainLayout.Controls.Add(buttonPanel, 0, 16);
        mainLayout.Location = new Point(0, 0);
        mainLayout.Name = "mainLayout";
        mainLayout.Padding = new Padding(32, 24, 32, 24);
        mainLayout.RowCount = 17;
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.RowStyles.Add(new RowStyle());
        mainLayout.Size = new Size(480, 520);
        mainLayout.TabIndex = 0;
        //
        // headingLabel
        //
        headingLabel.AutoSize = true;
        headingLabel.Location = new Point(32, 24);
        headingLabel.Margin = new Padding(0, 0, 0, 8);
        headingLabel.Name = "headingLabel";
        headingLabel.Size = new Size(0, 17);
        headingLabel.TabIndex = 0;
        //
        // firstNameLabel
        //
        firstNameLabel.Location = new Point(32, 49);
        firstNameLabel.Margin = new Padding(0, 4, 0, 4);
        firstNameLabel.Name = "firstNameLabel";
        firstNameLabel.Size = new Size(0, 17);
        firstNameLabel.TabIndex = 1;
        //
        // firstNameTextBox
        //
        firstNameTextBox.Location = new Point(32, 70);
        firstNameTextBox.Margin = new Padding(0, 0, 24, 0);
        firstNameTextBox.Name = "firstNameTextBox";
        firstNameTextBox.Size = new Size(392, 25);
        firstNameTextBox.TabIndex = 2;
        firstNameTextBox.TextChanged += Field_Changed;
        //
        // firstNameErrorLabel
        //
        firstNameErrorLabel.AutoSize = true;
        firstNameErrorLabel.Location = new Point(32, 97);
        firstNameErrorLabel.Margin = new Padding(0, 2, 0, 4);
        firstNameErrorLabel.MaximumSize = new Size(392, 0);
        firstNameErrorLabel.MinimumSize = new Size(0, 17);
        firstNameErrorLabel.Name = "firstNameErrorLabel";
        firstNameErrorLabel.Size = new Size(0, 17);
        firstNameErrorLabel.TabIndex = 3;
        //
        // lastNameLabel
        //
        lastNameLabel.Location = new Point(32, 122);
        lastNameLabel.Margin = new Padding(0, 4, 0, 4);
        lastNameLabel.Name = "lastNameLabel";
        lastNameLabel.Size = new Size(0, 17);
        lastNameLabel.TabIndex = 4;
        //
        // lastNameTextBox
        //
        lastNameTextBox.Location = new Point(32, 143);
        lastNameTextBox.Margin = new Padding(0, 0, 24, 0);
        lastNameTextBox.Name = "lastNameTextBox";
        lastNameTextBox.Size = new Size(392, 25);
        lastNameTextBox.TabIndex = 5;
        lastNameTextBox.TextChanged += Field_Changed;
        //
        // lastNameErrorLabel
        //
        lastNameErrorLabel.AutoSize = true;
        lastNameErrorLabel.Location = new Point(32, 170);
        lastNameErrorLabel.Margin = new Padding(0, 2, 0, 4);
        lastNameErrorLabel.MaximumSize = new Size(392, 0);
        lastNameErrorLabel.MinimumSize = new Size(0, 17);
        lastNameErrorLabel.Name = "lastNameErrorLabel";
        lastNameErrorLabel.Size = new Size(0, 17);
        lastNameErrorLabel.TabIndex = 6;
        //
        // emailLabel
        //
        emailLabel.Location = new Point(32, 195);
        emailLabel.Margin = new Padding(0, 4, 0, 4);
        emailLabel.Name = "emailLabel";
        emailLabel.Size = new Size(0, 17);
        emailLabel.TabIndex = 7;
        //
        // emailTextBox
        //
        emailTextBox.Location = new Point(32, 216);
        emailTextBox.Margin = new Padding(0, 0, 24, 0);
        emailTextBox.Name = "emailTextBox";
        emailTextBox.Size = new Size(392, 25);
        emailTextBox.TabIndex = 8;
        emailTextBox.TextChanged += Field_Changed;
        //
        // emailErrorLabel
        //
        emailErrorLabel.AutoSize = true;
        emailErrorLabel.Location = new Point(32, 243);
        emailErrorLabel.Margin = new Padding(0, 2, 0, 4);
        emailErrorLabel.MaximumSize = new Size(392, 0);
        emailErrorLabel.MinimumSize = new Size(0, 17);
        emailErrorLabel.Name = "emailErrorLabel";
        emailErrorLabel.Size = new Size(0, 17);
        emailErrorLabel.TabIndex = 9;
        //
        // departmentLabel
        //
        departmentLabel.Location = new Point(32, 268);
        departmentLabel.Margin = new Padding(0, 4, 0, 4);
        departmentLabel.Name = "departmentLabel";
        departmentLabel.Size = new Size(0, 17);
        departmentLabel.TabIndex = 10;
        //
        // departmentComboBox
        //
        departmentComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        departmentComboBox.Location = new Point(32, 289);
        departmentComboBox.Margin = new Padding(0, 0, 24, 0);
        departmentComboBox.Name = "departmentComboBox";
        departmentComboBox.Size = new Size(392, 25);
        departmentComboBox.TabIndex = 11;
        departmentComboBox.SelectedIndexChanged += Field_Changed;
        //
        // departmentErrorLabel
        //
        departmentErrorLabel.AutoSize = true;
        departmentErrorLabel.Location = new Point(32, 316);
        departmentErrorLabel.Margin = new Padding(0, 2, 0, 4);
        departmentErrorLabel.MaximumSize = new Size(392, 0);
        departmentErrorLabel.MinimumSize = new Size(0, 17);
        departmentErrorLabel.Name = "departmentErrorLabel";
        departmentErrorLabel.Size = new Size(0, 17);
        departmentErrorLabel.TabIndex = 12;
        //
        // hireDateLabel
        //
        hireDateLabel.Location = new Point(32, 341);
        hireDateLabel.Margin = new Padding(0, 4, 0, 4);
        hireDateLabel.Name = "hireDateLabel";
        hireDateLabel.Size = new Size(0, 17);
        hireDateLabel.TabIndex = 13;
        //
        // hireDatePicker
        //
        hireDatePicker.Format = DateTimePickerFormat.Custom;
        hireDatePicker.Location = new Point(32, 362);
        hireDatePicker.Margin = new Padding(0, 0, 24, 0);
        hireDatePicker.Name = "hireDatePicker";
        hireDatePicker.Size = new Size(160, 25);
        hireDatePicker.TabIndex = 14;
        hireDatePicker.ValueChanged += Field_Changed;
        //
        // hireDateErrorLabel
        //
        hireDateErrorLabel.AutoSize = true;
        hireDateErrorLabel.Location = new Point(32, 389);
        hireDateErrorLabel.Margin = new Padding(0, 2, 0, 4);
        hireDateErrorLabel.MaximumSize = new Size(392, 0);
        hireDateErrorLabel.MinimumSize = new Size(0, 17);
        hireDateErrorLabel.Name = "hireDateErrorLabel";
        hireDateErrorLabel.Size = new Size(0, 17);
        hireDateErrorLabel.TabIndex = 15;
        //
        // buttonPanel
        //
        buttonPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        buttonPanel.AutoSize = true;
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Location = new Point(32, 434);
        buttonPanel.Margin = new Padding(0, 24, 24, 0);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(392, 38);
        buttonPanel.TabIndex = 16;
        buttonPanel.MouseLeave += ButtonPanel_MouseLeave;
        buttonPanel.MouseMove += ButtonPanel_MouseMove;
        //
        // saveButton
        //
        saveButton.AutoSize = true;
        saveButton.Location = new Point(282, 0);
        saveButton.Margin = new Padding(8, 0, 0, 0);
        saveButton.MinimumSize = new Size(110, 38);
        saveButton.Name = "saveButton";
        saveButton.Padding = new Padding(12, 0, 12, 0);
        saveButton.Size = new Size(110, 38);
        saveButton.TabIndex = 0;
        saveButton.Click += SaveButton_Click;
        //
        // cancelButton
        //
        cancelButton.AutoSize = true;
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(164, 0);
        cancelButton.Margin = new Padding(8, 0, 0, 0);
        cancelButton.MinimumSize = new Size(110, 38);
        cancelButton.Name = "cancelButton";
        cancelButton.Padding = new Padding(12, 0, 12, 0);
        cancelButton.Size = new Size(110, 38);
        cancelButton.TabIndex = 1;
        //
        // errorProvider
        //
        errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        errorProvider.ContainerControl = this;
        //
        // EmployeeForm
        //
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        CancelButton = cancelButton;
        ClientSize = new Size(480, 520);
        Controls.Add(mainLayout);
        Font = new Font("Segoe UI", 10F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "EmployeeForm";
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        mainLayout.ResumeLayout(false);
        mainLayout.PerformLayout();
        buttonPanel.ResumeLayout(false);
        buttonPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel mainLayout;
    private Label headingLabel;
    private EmployeeManagement.Desktop.Controls.RequiredFieldLabel firstNameLabel;
    private TextBox firstNameTextBox;
    private Label firstNameErrorLabel;
    private EmployeeManagement.Desktop.Controls.RequiredFieldLabel lastNameLabel;
    private TextBox lastNameTextBox;
    private Label lastNameErrorLabel;
    private EmployeeManagement.Desktop.Controls.RequiredFieldLabel emailLabel;
    private TextBox emailTextBox;
    private Label emailErrorLabel;
    private EmployeeManagement.Desktop.Controls.RequiredFieldLabel departmentLabel;
    private ComboBox departmentComboBox;
    private Label departmentErrorLabel;
    private EmployeeManagement.Desktop.Controls.RequiredFieldLabel hireDateLabel;
    private DateTimePicker hireDatePicker;
    private Label hireDateErrorLabel;
    private FlowLayoutPanel buttonPanel;
    private Button saveButton;
    private Button cancelButton;
    private ErrorProvider errorProvider;
    private ToolTip toolTip;
}
