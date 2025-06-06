using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using oop_coursework.Models;

namespace oop_coursework.Services
{
    public class DataService
    {
        private readonly string _usersPath = "users.json";
        private readonly string _subjectsPath = "subjects.json";
        private readonly string _gradesPath = "grades.json";

        private List<User> _users = new();
        private List<Subject> _subjects = new();
        private List<Grade> _grades = new();

        private readonly JsonSerializerOptions _jsonOptions;

        public DataService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(),
                    new UserJsonConverter(),
                    new SubjectJsonConverter()
                }
            };

            LoadData();
        }

        private void LoadData()
        {
            _users = LoadFromJson<List<User>>(_usersPath) ?? new List<User>();
            _subjects = LoadFromJson<List<Subject>>(_subjectsPath) ?? new List<Subject>();
            _grades = LoadFromJson<List<Grade>>(_gradesPath) ?? new List<Grade>();
        }

        private T LoadFromJson<T>(string path) where T : new()
        {
            if (!File.Exists(path))
                return new T();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions) ?? new T();
        }

        private void SaveToJson<T>(T data, string path)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(path, json);
        }

        public void SaveChanges()
        {
            var teachers = _users.OfType<Teacher>();
            foreach (var teacher in teachers)
            {
                if (teacher.Subject != null)
                {
                    var subject = _subjects.FirstOrDefault(s => s.Id == teacher.Subject.Id);
                    if (subject != null)
                    {
                        subject.ExamDate = teacher.Subject.ExamDate;
                        subject.RetakeDate = teacher.Subject.RetakeDate;
                        subject.IsExam = teacher.Subject.IsExam;
                    }
                }
            }

            SaveToJson(_users, _usersPath);
            SaveToJson(_subjects, _subjectsPath);
            SaveToJson(_grades, _gradesPath);
        }

        public List<Student> GetStudents() => _users.OfType<Student>().ToList();
        public List<Teacher> GetTeachers() => _users.OfType<Teacher>().ToList();
        public List<Administrator> GetAdministrators() => _users.OfType<Administrator>().ToList();
        public List<Subject> GetSubjects() => _subjects.ToList();
        public List<Grade> GetGrades() => _grades.ToList();

        public void AddUser(User user)
        {
            user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
            SaveChanges();
        }

        public void AddSubject(Subject subject)
        {
            subject.Id = _subjects.Count > 0 ? _subjects.Max(s => s.Id) + 1 : 1;
            _subjects.Add(subject);
            SaveChanges();
        }

        public void AddGrade(Grade grade)
        {
            grade.Id = _grades.Count > 0 ? _grades.Max(g => g.Id) + 1 : 1;
            _grades.Add(grade);
            SaveChanges();
        }

        public void DeleteUser(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return;

            if (user is Student)
            {
                _grades.RemoveAll(g => g.StudentId == userId);
            }

            if (user is Teacher teacher)
            {
                if (teacher.Subject != null)
                {
                    _subjects.RemoveAll(s => s.Id == teacher.Subject.Id);
                    _grades.RemoveAll(g => g.SubjectId == teacher.Subject.Id);
                }
            }

            _users.Remove(user);
            SaveChanges();
        }

        public User? GetUserByCredentials(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }

    public class UserJsonConverter : JsonConverter<User>
    {
        public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var jsonObject = jsonDoc.RootElement;

            var username = jsonObject.GetProperty("Username").GetString() ?? string.Empty;
            var password = jsonObject.GetProperty("Password").GetString() ?? string.Empty;

            User user = jsonObject.GetProperty("Role").GetString() switch
            {
                "Student" => new Student
                {
                    FirstName = jsonObject.GetProperty("FirstName").GetString() ?? string.Empty,
                    LastName = jsonObject.GetProperty("LastName").GetString() ?? string.Empty,
                    Username = username,
                    Password = password,
                    Role = "Student",
                    Specialty = jsonObject.GetProperty("Specialty").GetString() ?? string.Empty
                },
                "Teacher" => new Teacher
                {
                    FirstName = jsonObject.GetProperty("FirstName").GetString() ?? string.Empty,
                    LastName = jsonObject.GetProperty("LastName").GetString() ?? string.Empty,
                    Username = username,
                    Password = password,
                    Role = "Teacher"
                },
                "Administrator" => new Administrator
                {
                    FirstName = jsonObject.GetProperty("FirstName").GetString() ?? string.Empty,
                    LastName = jsonObject.GetProperty("LastName").GetString() ?? string.Empty,
                    Username = username,
                    Password = password,
                    Role = "Administrator"
                },
                _ => throw new JsonException($"Unknown user role: {jsonObject.GetProperty("Role").GetString()}")
            };

            if (jsonObject.TryGetProperty("Id", out var idElement))
                user.Id = idElement.GetInt32();

            if (jsonObject.TryGetProperty("DateOfBirth", out var dateElement) && DateTime.TryParse(dateElement.GetString(), out var parsedDate))
                user.DateOfBirth = parsedDate;

            if (user is Teacher teacher && jsonObject.TryGetProperty("Subject", out var subjectElement))
            {
                var subjectName = subjectElement.GetProperty("Name").GetString();
                Subject subject = (Subject)(subjectName switch
                {
                    "Mathematics" => new MathSubject { Name = "Mathematics" },
                    "English" => new EnglishSubject { Name = "English" },
                    "Art" => new ArtSubject { Name = "Art" },
                    _ => throw new JsonException($"Unknown subject: {subjectName}")
                });
                subject.Id = subjectElement.GetProperty("Id").GetInt32();
                if (jsonObject.TryGetProperty("ExamDate", out var examDateElement) && DateTime.TryParse(examDateElement.GetString(), out var parsedExamDate))
                    subject.ExamDate = parsedExamDate;
                subject.RetakeDate = subjectElement.TryGetProperty("RetakeDate", out var retakeDateElement) ?
                    retakeDateElement.GetDateTime() : null;
                subject.Credits = subjectElement.GetProperty("Credits").GetInt32();
                subject.IsExam = subjectElement.GetProperty("IsExam").GetBoolean();
                teacher.Subject = subject;
            }

            return user;
        }

        public override void Write(Utf8JsonWriter writer, User user, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("Id", user.Id);
            writer.WriteString("FirstName", user.FirstName);
            writer.WriteString("LastName", user.LastName);
            writer.WriteString("Username", user.Username);
            writer.WriteString("Password", user.Password);
            writer.WriteString("Role", user.Role);
            writer.WriteString("DateOfBirth", user.DateOfBirth);

            if (user is Student student)
            {
                writer.WriteString("Specialty", student.Specialty);
            }
            else if (user is Teacher teacher && teacher.Subject != null)
            {
                writer.WriteStartObject("Subject");
                writer.WriteNumber("Id", teacher.Subject.Id);
                writer.WriteString("Name", teacher.Subject.Name);
                writer.WriteString("ExamDate", teacher.Subject.ExamDate?.ToString("o") ?? "");
                if (teacher.Subject.RetakeDate.HasValue)
                {
                    writer.WriteString("RetakeDate", teacher.Subject.RetakeDate.Value);
                }
                writer.WriteNumber("Credits", teacher.Subject.Credits);
                writer.WriteBoolean("IsExam", teacher.Subject.IsExam);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }

    public class SubjectJsonConverter : JsonConverter<Subject>
    {
        public override Subject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var jsonObject = jsonDoc.RootElement;

            var name = jsonObject.GetProperty("Name").GetString() ?? throw new JsonException("Subject name is required");
            Subject subject = CreateSubject(name);

            if (jsonObject.TryGetProperty("Id", out var idElement))
                subject.Id = idElement.GetInt32();

            if (jsonObject.TryGetProperty("ExamDate", out var examDateElement) && DateTime.TryParse(examDateElement.GetString(), out var parsedExamDate))
                subject.ExamDate = parsedExamDate;

            if (jsonObject.TryGetProperty("Credits", out var creditsElement))
                subject.Credits = creditsElement.GetInt32();

            if (jsonObject.TryGetProperty("IsExam", out var isExamElement))
                subject.IsExam = isExamElement.GetBoolean();

            return subject;
        }

        private static Subject CreateSubject(string name) => name switch
        {
            "Mathematics" => new MathSubject { Name = "Mathematics" },
            "English" => new EnglishSubject { Name = "English" },
            "Art" => new ArtSubject { Name = "Art" },
            _ => throw new JsonException($"Unknown subject: {name}")
        };

        public override void Write(Utf8JsonWriter writer, Subject subject, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("Id", subject.Id);
            writer.WriteString("Name", subject.Name);
            writer.WriteString("ExamDate", subject.ExamDate?.ToString("o") ?? "");
            writer.WriteNumber("Credits", subject.Credits);
            writer.WriteBoolean("IsExam", subject.IsExam);

            writer.WriteEndObject();
        }
    }
}
