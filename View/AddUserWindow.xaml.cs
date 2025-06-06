using System;
using System.Windows;
using System.Windows.Controls;
using oop_coursework.Models;
using oop_coursework.Services;
using System.Linq;
using System.Collections.Generic;

namespace oop_coursework.Views
{
    public partial class AddUserWindow : Window
    {
        private readonly DataService _dataService;
        private readonly User? _existingUser;
        private bool _isEditMode;

        public AddUserWindow(DataService dataService, User? existingUser = null)
        {
            InitializeComponent();
            _dataService = dataService;
            _existingUser = existingUser;
            _isEditMode = existingUser != null;

            RoleComboBox.SelectionChanged += RoleComboBox_SelectionChanged;

            if (_isEditMode)
            {
                Title = "Редагувати користувача";
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
            if (_existingUser == null) return;

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
                    if (_existingUser is Teacher teacher)
                    {
                        foreach (ComboBoxItem item in SubjectComboBox.Items)
                        {
                            if (item.Content.ToString() == teacher.Subject?.Name)
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
                bool isStudent = role == "Студент";
                bool isTeacher = role == "Викладач";

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
                !DateOfBirthPicker.SelectedDate.HasValue ||
                RoleComboBox.SelectedItem is not ComboBoxItem selectedRoleItem)
            {
                MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var role = selectedRoleItem.Content.ToString() ?? throw new InvalidOperationException("Роль не може бути порожньою");
            User? user = _existingUser;

            if (!_isEditMode)
            {
                user = role switch
                {
                    "Студент" => new Student
                    {
                        FirstName = FirstNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        Username = UsernameTextBox.Text,
                        Password = PasswordBox.Password,
                        Role = role,
                        Specialty = SpecialtyTextBox.Text
                    },
                    "Викладач" => CreateTeacher(),
                    "Адміністратор" => new Administrator
                    {
                        FirstName = FirstNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        Username = UsernameTextBox.Text,
                        Password = PasswordBox.Password,
                        Role = role
                    },
                    _ => throw new InvalidOperationException("Вибрано неправильну роль")
                };
            }

            if (user == null)
            {
                MessageBox.Show("Не вдалося створити або оновити користувача.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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
                    MessageBox.Show("Будь ласка, введіть спеціальність студента.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                student.Specialty = SpecialtyTextBox.Text;
            }
            else if (user is Teacher teacher)
            {
                if (SubjectComboBox.SelectedItem is not ComboBoxItem selectedSubjectItem)
                {
                    MessageBox.Show("Будь ласка, виберіть предмет.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var subjectName = selectedSubjectItem.Content?.ToString();
                if (string.IsNullOrEmpty(subjectName))
                {
                    MessageBox.Show("Вибрано неправильний предмет.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Subject subject = subjectName switch
                {
                    "Математика" => new MathSubject { Name = "Математика" },
                    "Англійська мова" => new EnglishSubject { Name = "Англійська мова" },
                    "Мистецтво" => new ArtSubject { Name = "Мистецтво" },
                    _ => throw new InvalidOperationException("Вибрано неправильний предмет")
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

        private Teacher CreateTeacher()
        {
            if (SubjectComboBox.SelectedItem is not ComboBoxItem selectedSubjectItem)
            {
                throw new InvalidOperationException("Будь ласка, виберіть предмет.");
            }

            var subjectName = selectedSubjectItem.Content?.ToString();
            if (string.IsNullOrEmpty(subjectName))
            {
                throw new InvalidOperationException("Вибрано неправильний предмет.");
            }

            Subject subject = subjectName switch
            {
                "Математика" => new MathSubject { Name = "Математика" },
                "Англійська мова" => new EnglishSubject { Name = "Англійська мова" },
                "Мистецтво" => new ArtSubject { Name = "Мистецтво" },
                _ => throw new InvalidOperationException("Вибрано неправильний предмет")
            };

            var existingSubject = _dataService.GetSubjects().FirstOrDefault(s => s.Name == subjectName);

            return new Teacher
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Username = UsernameTextBox.Text,
                Password = PasswordBox.Password,
                Role = "Викладач",
                Subject = existingSubject ?? subject
            };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
