using System;
using System.Windows;
using System.Windows.Controls;
using oop_coursework.Models;
using oop_coursework.Services;
using System.Linq;

namespace oop_coursework.Views
{
    public partial class AddUserWindow : Window
    {
        private readonly DataService _dataService;
        private readonly User _existingUser;
        private bool _isEditMode;

        public AddUserWindow(DataService dataService, User existingUser = null)
        {
            InitializeComponent();
            _dataService = dataService;
            _existingUser = existingUser;
            _isEditMode = existingUser != null;

            RoleComboBox.SelectionChanged += RoleComboBox_SelectionChanged;

            if (_isEditMode)
            {
                Title = "Edit User";
                LoadExistingUserData();
                RoleComboBox.IsEnabled = false;
            }
            else
            {
                RoleComboBox.SelectedIndex = 0;
            }

            UpdateFieldsVisibility();
        }

        private void LoadExistingUserData()
        {
            FirstNameTextBox.Text = _existingUser.FirstName;
            LastNameTextBox.Text = _existingUser.LastName;
            UsernameTextBox.Text = _existingUser.Username;
            PasswordBox.Password = _existingUser.Password;
            DateOfBirthPicker.SelectedDate = _existingUser.DateOfBirth;

            switch (_existingUser.Role)
            {
                case "Student":
                    RoleComboBox.SelectedIndex = 0;
                    if (_existingUser is Student student)
                        SpecialtyTextBox.Text = student.Specialty;
                    break;
                case "Teacher":
                    RoleComboBox.SelectedIndex = 1;
                    if (_existingUser is Teacher teacher && teacher.Subject != null)
                    {
                        foreach (ComboBoxItem item in SubjectComboBox.Items)
                        {
                            if (item.Content.ToString() == teacher.Subject.Name)
                            {
                                SubjectComboBox.SelectedItem = item;
                                break;
                            }
                        }
                    }
                    break;
                case "Administrator":
                    RoleComboBox.SelectedIndex = 2;
                    break;
            }
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFieldsVisibility();
        }

        private void UpdateFieldsVisibility()
        {
            if (RoleComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string role = selectedItem.Content.ToString();
                bool isStudent = role == "Student";
                bool isTeacher = role == "Teacher";

                SpecialtyLabel.Visibility = isStudent ? Visibility.Visible : Visibility.Collapsed;
                SpecialtyTextBox.Visibility = isStudent ? Visibility.Visible : Visibility.Collapsed;

                SubjectLabel.Visibility = isTeacher ? Visibility.Visible : Visibility.Collapsed;
                SubjectComboBox.Visibility = isTeacher ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(UsernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                !DateOfBirthPicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            User user = _existingUser;

            if (!_isEditMode)
            {
                user = role switch
                {
                    "Student" => new Student(),
                    "Teacher" => new Teacher(),
                    "Administrator" => new Administrator(),
                    _ => throw new InvalidOperationException("Invalid role selected")
                };
            }

            user.FirstName = FirstNameTextBox.Text;
            user.LastName = LastNameTextBox.Text;
            user.Username = UsernameTextBox.Text;
            user.Password = PasswordBox.Password;
            user.DateOfBirth = DateOfBirthPicker.SelectedDate.Value;
            user.Role = role;

            if (user is Student student)
            {
                if (string.IsNullOrWhiteSpace(SpecialtyTextBox.Text))
                {
                    MessageBox.Show("Please enter the student's specialty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                student.Specialty = SpecialtyTextBox.Text;
            }
            else if (user is Teacher teacher)
            {
                if (SubjectComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a subject.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var subjectName = (SubjectComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                Subject subject = subjectName switch
                {
                    "Mathematics" => new MathSubject(),
                    "English" => new EnglishSubject(),
                    "Art" => new ArtSubject(),
                    _ => throw new InvalidOperationException("Invalid subject selected")
                };

                // Check if a subject with the same name already exists
                var existingSubject = _dataService.GetSubjects().FirstOrDefault(s => s.Name == subjectName);
                if (existingSubject != null)
                {
                    teacher.Subject = existingSubject;
                }
                else
                {
                    teacher.Subject = subject;
                    _dataService.AddSubject(subject);
                }
            }

            if (!_isEditMode)
            {
                _dataService.AddUser(user);
            }
            else
            {
                _dataService.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
